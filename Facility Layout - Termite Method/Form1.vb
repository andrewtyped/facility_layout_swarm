Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports FaciltyLayout.Core
Imports FaciltyLayout.Core.Models

<Assembly: InternalsVisibleTo("FacilityLayout.Core.Tests")>
Public Class Form1
    Private RandomRow As New Random
    Friend myTermites As List(Of Termites) 'Agents for moving the tiles
    Private myNumTermites As Integer
    Friend Tile(,) As Windows.Forms.Label
    Private TileRefreshCounter As Integer = 0
    Private TileChangedCounter As Integer = 0
    Private myTileColors(,) As Integer
    Private myLoopCounter = 0
    Private myLoopPhase As Integer
    Private myGravStart As Integer
    Private TransformVDC As Integer
    Private facilityEvaluator As New FacilityEvaluator()
    Private contiguityTester As New ContiguityTester()
    Friend FacilityStats As FacilityStats
    Friend WithEvents FacilityLayoutModel As FacilityLayoutModel
    Friend TermiteManager As TermiteManager

    'Creates the facility field, places tiles randomly across the field
    Friend Function GenerateFacilitySwarm(facilityStats As FacilityStats) As FacilityLayoutModel
        Dim facilityLayout As FacilityLayoutModel = New FacilityLayoutModel(facilityStats)
        facilityLayout.InitializeDepartmentTiles()

        Return facilityLayout
    End Function
    'Animates the facility field with a series of randomly colored labels
    Private Sub GenerateTileSwarmToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GenerateTileSwarmToolStripMenuItem.Click
        RandomizeTiles(FacilityStats)
    End Sub

    Private Sub GreedyTermiteMethodToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GreedyTermiteMethodToolStripMenuItem.Click
        Dim Rows As Integer = FacilityLayoutModel.LayoutArea.Rows
        Dim Columns As Integer = FacilityLayoutModel.LayoutArea.Columns

        If TxtRows.Text <> "" Then
            Integer.TryParse(TxtRows.Text, Rows)
        End If

        If TxtColumns.Text <> "" Then
            Integer.TryParse(TxtColumns.Text, Columns)
        End If

        ReleaseTheTermites(Math.Round(FacilityStats.FacilitySize.Rows * FacilityStats.FacilitySize.Columns * 1.5))

        Dim ContigIndicator As Boolean
        Dim TotalContig
        Dim VDCP As Integer

        Do
            ReorganizationMethodScholar(Rows, Columns, myNumTermites)

            For i = 1 To FacilityStats.DepartmentCount
                ContigIndicator = contiguityTester.DepartmentIsContiguous(i, FacilityLayoutModel.Facility)

                If ContigIndicator = True Then
                    FacilityLayoutModel.LockDeptTiles(i)
                End If
            Next

            TotalContig = contiguityTester.AllDepartmentsAreContiguous(FacilityLayoutModel.Facility)
        Loop Until TotalContig = True

        VDCP = facilityEvaluator.VolumeDistanceCostProduct(FacilityStats, FacilityLayoutModel.Facility)
        MessageBox.Show(VDCP.ToString("################.00"))

    End Sub

    Private Sub Form1_TermiteRemovedTile(sender As Object, e As TileEventArgs) Handles FacilityLayoutModel.TileRemoved
        Tile(e.Position.Row, e.Position.Column).BackColor = Color.Black
        Tile(e.Position.Row, e.Position.Column).Text = ""

        TileChangedCounter = TileChangedCounter + 1

        If TileChangedCounter = 20000 Then
            TileRefresher(FacilityLayoutModel.LayoutArea.Rows, FacilityLayoutModel.LayoutArea.Columns)
            TileChangedCounter = 0
        End If
    End Sub

    Private Sub Form1_TilePlaced(sender As Object, e As TileEventArgs) Handles FacilityLayoutModel.TilePlaced
        Tile(e.Position.Row, e.Position.Column).BackColor = Color.FromArgb(myTileColors(e.DepartmentId, 0),
                                                myTileColors(e.DepartmentId, 1),
                                                myTileColors(e.DepartmentId, 2))
        Tile(e.Position.Row, e.Position.Column).Text = e.DepartmentId.ToString

        TileChangedCounter = TileChangedCounter + 1

        If TileChangedCounter = 20000 Then
            TileRefresher(FacilityLayoutModel.LayoutArea.Rows, FacilityLayoutModel.LayoutArea.Columns)
            TileChangedCounter = 0
        End If
    End Sub

    Friend Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        ofdLoadSetup.InitialDirectory = "C:/"
        ofdLoadSetup.Title = "Load Facility Data"
        ofdLoadSetup.FileName = ""
        ofdLoadSetup.Filter = "Text Files (*.txt)|*.txt"
        ofdLoadSetup.FilterIndex = 1

        Dim datafile As String
        If ofdLoadSetup.ShowDialog() = DialogResult.OK Then
            datafile = ofdLoadSetup.FileName
            Dim result As String = Configure_App(datafile)
            MessageBox.Show(result)
        End If
    End Sub

    Friend Function Configure_App(pathToConfigFile As String) As String
        Dim facilityStatsRepository = New FacilityStatsRepository(pathToConfigFile)
        FacilityStats = facilityStatsRepository.Load()

        Return FacilityStats.ToString()
    End Function

    Private Sub RandomizeTiles(facilityStats As FacilityStats)
        FacilityLayoutModel = GenerateFacilitySwarm(facilityStats)

        Dim Rows As Integer = FacilityLayoutModel.LayoutArea.Rows
        Dim Columns As Integer = FacilityLayoutModel.LayoutArea.Columns

        Dim color As New Color

        Dim txtboxspacing As Integer = 200
        Me.Height = Rows * 29 + 500
        Me.Width = Columns * 25 + 500
        TxtColumns.Left = Columns * 25 + 150
        LblColumns.Left = TxtColumns.Left - LblColumns.Width - 3
        TxtRows.Left = TxtColumns.Left + txtboxspacing
        LblRows.Left = TxtRows.Left - LblRows.Width - 3
        TxtTermites.Left = TxtColumns.Left
        LblNumTermites.Left = TxtColumns.Left - LblNumTermites.Width - 3
        TxtRatio.Left = TxtRows.Left
        LblRatio.Left = TxtRows.Left - LblRatio.Width - 3
        TxtPhase1Decay.Left = TxtColumns.Left
        LblPhase1Decay.Left = TxtColumns.Left - LblPhase1Decay.Width - 3
        TxtPhase2Decay.Left = TxtRows.Left
        LblPhase2Decay.Left = TxtRows.Left - LblPhase2Decay.Width - 3
        TxtGravStart.Left = TxtColumns.Left
        LblGravStart.Left = TxtColumns.Left - LblGravStart.Width - 3
        TxtReps.Left = TxtRows.Left
        LblReps.Left = TxtRows.Left - LblReps.Width - 3

        Dim RowCounter As Integer = 10
        Dim ColumnCounter As Integer = 30
        If TileRefreshCounter = 0 Then
            ReDim Tile(Rows - 1, Columns - 1)
            ReDim myTileColors(facilityStats.DepartmentCount, 2)
        End If
        Dim ColorRangeCount As Integer = 100
        For j = 0 To 2
            myTileColors(0, j) = 0
        Next

        For i = 0 To facilityStats.DepartmentCount - 1
            For j = 0 To 2
                myTileColors(i + 1, j) = (facilityStats.DepartmentCount / 2) * RandomRow.Next(10, 255 / (facilityStats.DepartmentCount / 2))

            Next
            ColorRangeCount = ColorRangeCount + 150 / facilityStats.DepartmentCount
        Next

        For i = 0 To Rows - 1
            For j = 0 To Columns - 1
                If TileRefreshCounter = 0 Then
                    Tile(i, j) = New Windows.Forms.Label
                    Tile(i, j).Width = 20
                    Tile(i, j).Height = 20
                    Tile(i, j).Location = New Point(RowCounter, ColumnCounter)
                    Tile(i, j).Visible = True
                    Me.Controls.Add(Tile(i, j))
                    RowCounter = RowCounter + 25
                End If
                Tile(i, j).BackColor = Color.FromArgb(myTileColors(FacilityLayoutModel.GetTile(i, j), 0), myTileColors(FacilityLayoutModel.GetTile(i, j), 1),
                                                      myTileColors(FacilityLayoutModel.GetTile(i, j), 2))
                Tile(i, j).Text = FacilityLayoutModel.GetTile(i, j).ToString
                Tile(i, j).Refresh()
            Next
            RowCounter = 10
            ColumnCounter = ColumnCounter + 25
        Next
        TileRefreshCounter = 1
    End Sub

    'Sets Initial Position an Direction of Termites
    Friend Sub ReleaseTheTermites(ByVal NumTermites As Integer)
        TermiteManager = New TermiteManager(FacilityLayoutModel)
        myNumTermites = NumTermites
        Dim TypeRatio As Integer = 3
        If TxtRatio.Text <> "" Then
            Integer.TryParse(TxtRatio.ToString, TypeRatio)
        End If

        myTermites = TermiteManager.ReleaseTheTermites(NumTermites, TypeRatio)
    End Sub
    'Termites begin to move, collect, and drop tiles
    'This function describes the space immediately around the tile in question. Used for contiguity testing.
    Private Sub ReorganizationMethodScholar(ByVal rows As Integer, ByVal columns As Integer, ByVal numtermites As Integer)
        Dim i As Integer
        Dim HoardingTermite As Integer
        If myLoopPhase = 1 Then
            HoardingTermite = 0
        Else
            HoardingTermite = 1
        End If

        For i = HoardingTermite To numtermites - 1
            myTermites(i).MoveTile(FacilityLayoutModel, FacilityStats, contiguityTester, rows, columns)
        Next
    End Sub

    Private Sub ScholarTermiteMethodToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ScholarTermiteMethodToolStripMenuItem.Click
        Dim Rows As Integer = FacilityLayoutModel.LayoutArea.Rows
        Dim Columns As Integer = FacilityLayoutModel.LayoutArea.Columns
        Dim NumTermites As Integer = Math.Round(FacilityStats.FacilitySize.Rows * FacilityStats.FacilitySize.Columns * 1.5)
        If TxtTermites.Text <> "" Then
            Integer.TryParse(TxtTermites.Text, NumTermites)
        End If
        If TxtRows.Text <> "" Then
            Integer.TryParse(TxtRows.Text, Rows)
        End If
        If TxtColumns.Text <> "" Then
            Integer.TryParse(TxtColumns.Text, Columns)
        End If

        Dim ContigIndicator As Boolean
        Dim TotalContig As Boolean
        Dim n As Integer = 0
        Dim StartTime, StopTime As DateTime
        Dim RunTime As TimeSpan
        Dim i, j, y As Integer

        Dim adjTilesContainSameDepartment As Boolean = False
        Dim Phase1Decay As Integer = 10
        Dim Phase2Decay As Integer = 10
        If TxtPhase1Decay.Text <> "" Then
            Integer.TryParse(TxtPhase1Decay.Text, Phase1Decay)
        End If
        If TxtPhase2Decay.Text <> "" Then
            Integer.TryParse(TxtPhase2Decay.Text, Phase2Decay)
        End If

        TextReader()
        myLoopPhase = 1
        myLoopCounter = 0
        Dim cycle As Integer = 0
        Dim numcycles As Integer = 10
        If TxtReps.Text <> "" Then
            Integer.TryParse(TxtReps.Text, numcycles)
        End If
        Dim VDC2 As Integer = 30000
        Dim Solutions(9) As Double
        Dim x As Integer = 0
        Dim refreshcounter As Integer = 0
        Do
            cycle = cycle + 1
            If cycle > 1 Then
                RandomizeTiles(FacilityStats)
            End If
            myLoopPhase = 1
            myLoopCounter = 0
            NumTermites = Math.Round(FacilityStats.FacilitySize.Rows * FacilityStats.FacilitySize.Columns * 1.5)
            If TxtTermites.Text <> "" Then
                Integer.TryParse(TxtTermites.Text, NumTermites)
            End If
            ReleaseTheTermites(NumTermites)
            StartTime = Now
            Do
                refreshcounter = refreshcounter + 1

                If Math.IEEERemainder(refreshcounter, 600) = 0 Then
                    FacilityLayoutModel.UnlockTiles(Rows, Columns)
                    y = 0
                    Do
                        ReorganizationMethodScholar(Rows, Columns, myNumTermites - 1)
                        y = y + 1
                    Loop Until y = 50

                End If

                ReorganizationMethodScholar(Rows, Columns, myNumTermites)
                myLoopCounter = myLoopCounter + 1
                TotalContig = contiguityTester.AllDepartmentsAreContiguous(FacilityLayoutModel.Facility)
                If n < myNumTermites Then
                    If Math.IEEERemainder(myLoopCounter, Phase1Decay) = 0 AndAlso myLoopCounter >= myGravStart - Math.Round(myGravStart / 4) Then
                        myTermites(n) = myTermites(n).ChangeType(Of GreedyTermite)()
                        n = n + 1
                    End If
                End If
                If myLoopCounter >= myGravStart + Math.Round(myGravStart / 2) Then
                    For i = 1 To FacilityStats.DepartmentCount
                        ContigIndicator = contiguityTester.DepartmentIsContiguous(i, FacilityLayoutModel.Facility)
                        If ContigIndicator = False Then
                            TotalContig = False
                        End If
                        If ContigIndicator = True Then
                            FacilityLayoutModel.LockDeptTiles(i)
                        End If
                    Next
                End If
            Loop Until TotalContig = True
            FacilityLayoutModel.UnlockTiles(Rows, Columns)
            Dim rowstore, columnstore As Integer
            rowstore = Rows
            columnstore = Columns
            Do
                FacilityLayoutModel.UnlockTiles(Rows, Columns)
                FacilityLayoutModel.ReduceLayoutArea(myTermites, Rows, Columns, myGravStart, myLoopCounter)
                Rows = Rows - 1
                Columns = Columns - 1
                If Rows = FacilityStats.FacilitySize.Rows + 1 Then
                    myLoopPhase = 2
                End If
            Loop Until Rows = FacilityStats.FacilitySize.Rows AndAlso Columns = FacilityStats.FacilitySize.Columns
            Rows = rowstore
            Columns = columnstore
            myLoopCounter = 0
            n = 0
            myLoopPhase = 2
            FacilityLayoutModel.UnlockTiles(Rows, Columns)

            '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!STAGE 2!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Do
                ReorganizationMethodScholar(FacilityStats.FacilitySize.Rows, FacilityStats.FacilitySize.Columns, myNumTermites)
                myLoopCounter = myLoopCounter + 1
                TotalContig = contiguityTester.AllDepartmentsAreContiguous(FacilityLayoutModel.Facility)
                If n < myNumTermites Then
                    If Math.IEEERemainder(myLoopCounter, Phase2Decay) = 0 AndAlso myLoopCounter >= myGravStart - Math.Round(3 * myGravStart / 4) Then
                        myTermites(n) = myTermites(n).ChangeType(Of GreedyTermite)()
                        n = n + 1
                    End If
                End If

                If TotalContig = True Then
                    For i = 0 To FacilityStats.FacilitySize.Rows - 1
                        For j = 0 To FacilityStats.FacilitySize.Columns - 1
                            If FacilityLayoutModel.IsTileAssigned(i, j) = False Then
                                adjTilesContainSameDepartment = contiguityTester.AdjacentTilesContainSameDepartment(myTermites(0).TileDept, i, j, FacilityLayoutModel.Facility, FacilityStats.DepartmentSizes)

                                If adjTilesContainSameDepartment Then
                                    myTermites(0).DropTile(FacilityLayoutModel, i, j)
                                    Exit For
                                End If
                            End If
                        Next
                        If adjTilesContainSameDepartment Then
                            Exit For
                        End If
                    Next
                End If

                For i = 0 To FacilityStats.FacilitySize.Rows - 1
                    For j = 0 To FacilityStats.FacilitySize.Columns - 1
                        If FacilityLayoutModel.IsTileAssigned(i, j) = False Then
                            TotalContig = False
                            Exit For
                        End If
                    Next
                Next
            Loop Until TotalContig = True AndAlso adjTilesContainSameDepartment

            TileRefresher(FacilityLayoutModel.LayoutArea.Rows, FacilityLayoutModel.LayoutArea.Columns)

            StopTime = Now
            RunTime = StopTime.Subtract(StartTime)
            Dim VDC As Double
            Dim OBJValue As String
            VDC = facilityEvaluator.VolumeDistanceCostProduct(FacilityStats, FacilityLayoutModel.Facility)
            OBJValue = VDC.ToString & vbCrLf & RunTime.ToString
            MessageBox.Show(OBJValue)
            Solutions(x) = VDC
            x = x + 1
        Loop Until cycle = numcycles
    End Sub

    Private Sub TextReader()
        myGravStart = 500
        If TxtGravStart.Text <> "" Then
            Integer.TryParse(TxtGravStart.Text, myGravStart)
        End If
    End Sub

    'Refreshes the animation of tiles
    Private Sub TileRefresher(ByVal rows As Integer, ByVal columns As Integer)
        For i = 0 To rows - 1
            For j = 0 To columns - 1
                Tile(i, j).Refresh()
            Next
        Next
    End Sub
End Class