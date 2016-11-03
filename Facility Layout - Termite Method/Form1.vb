Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports FaciltyLayout.Core
Imports FaciltyLayout.Core.Models

<Assembly: InternalsVisibleTo("FacilityLayout.Core.Tests")>
Public Class Form1
    Private RandomRow As New Random
    Friend myFacilityMatrix(,) As Integer 'Matrix displaying the field of tiles and empty spaces
    Friend myTermiteOwnedTile(,) As Boolean 'Is a termite in the process of moving this tile?
    Friend myTermites As List(Of Termites) 'Agents for moving the tiles
    Private myNumTermites As Integer
    Friend Tile(,) As Windows.Forms.Label
    Private TileRefreshCounter As Integer = 0
    Private myTileColors(,) As Integer
    Private myLoopCounter = 0
    Private myLoopPhase As Integer
    Private myGravStart As Integer
    Private TransformVDC As Integer
    Private facilityEvaluator As New FacilityEvaluator()
    Private contiguityTester As New ContiguityTester()
    Friend FacilityStats As FacilityStats
    Friend WithEvents FacilityLayoutModel As FacilityLayoutModel
    Friend WithEvents TermiteManager As TermiteManager

    Private Sub FreezeDeptMotion(ByVal dept As Integer, ByVal rows As Integer, ByVal columns As Integer)
        For i = 0 To rows - 1
            For j = 0 To columns - 1
                If myFacilityMatrix(i, j) = dept Then
                    myTermiteOwnedTile(i, j) = True
                End If
            Next
        Next
    End Sub
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

        ReleaseTheTermites(Math.Round(FacilityStats.FacilitySize.Rows * FacilityStats.FacilitySize.Columns * 1.5), Rows, Columns)

        Dim ContigIndicator As Boolean
        Dim TotalContig
        Dim VDCP As Integer

        Do
            ReorganizationMethod1(Rows, Columns)

            TileRefresher(Rows, Columns)

            For i = 1 To FacilityStats.DepartmentCount
                ContigIndicator = contiguityTester.DepartmentIsContiguous(i, myFacilityMatrix)

                If ContigIndicator = True Then
                    FreezeDeptMotion(i, Rows, Columns)
                End If
            Next

            TotalContig = contiguityTester.AllDepartmentsAreContiguous(myFacilityMatrix)
        Loop Until TotalContig = True

        VDCP = facilityEvaluator.VolumeDistanceCostProduct(FacilityStats, myFacilityMatrix)
        MessageBox.Show(VDCP.ToString("################.00"))

    End Sub

    Private Sub Form1_TermiteRemovedTile(sender As Object, e As TileEventArgs) Handles TermiteManager.TermiteRemovedTile
        Tile(e.Position.Row, e.Position.Column).BackColor = Color.Black
        Tile(e.Position.Row, e.Position.Column).Text = ""
        Tile(e.Position.Row, e.Position.Column).Refresh()
    End Sub

    Private Sub Form1_TilePlaced(sender As Object, e As TileEventArgs) Handles FacilityLayoutModel.TilePlaced
        Tile(e.Position.Row, e.Position.Column).BackColor = Color.FromArgb(myTileColors(e.DepartmentId, 0),
                                                myTileColors(e.DepartmentId, 1),
                                                myTileColors(e.DepartmentId, 2))
        Tile(e.Position.Row, e.Position.Column).Text = e.DepartmentId.ToString
        myTermiteOwnedTile(e.Position.Row, e.Position.Column) = False
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
        myFacilityMatrix = FacilityLayoutModel.Facility

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
                Tile(i, j).BackColor = Color.FromArgb(myTileColors(myFacilityMatrix(i, j), 0), myTileColors(myFacilityMatrix(i, j), 1),
                                                      myTileColors(myFacilityMatrix(i, j), 2))
                Tile(i, j).Text = myFacilityMatrix(i, j).ToString
                Tile(i, j).Refresh()
            Next
            RowCounter = 10
            ColumnCounter = ColumnCounter + 25
        Next
        TileRefreshCounter = 1
    End Sub
    Private Sub RealisticCollection(ByVal rows As Integer, ByVal columns As Integer)
        Dim i, x, y, upperlimit As Integer
        i = 0
        upperlimit = 5
        For x = 0 To rows - 1
            If FacilityLayoutModel.IsTileAssigned(x, columns - 1) = True Then
                myTermites(i).RowPos = x
                myTermites(i).ColumnPos = columns - 1
                myTermites(i).HasTile = True
                myTermiteOwnedTile(myTermites(i).RowPos, myTermites(i).ColumnPos) = True
                myTermites(i).TileDept = myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos)
                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).BackColor = Color.Black
                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Text = ""
                FacilityLayoutModel.SetTile(myTermites(i).Position, 0)
                i = i + 1
            End If
        Next

        For y = 0 To columns - 2
            If FacilityLayoutModel.IsTileAssigned(rows - 1, y) = True Then
                myTermites(i).RowPos = rows - 1
                myTermites(i).ColumnPos = y
                myTermites(i).HasTile = True
                myTermiteOwnedTile(myTermites(i).RowPos, myTermites(i).ColumnPos) = True
                myTermites(i).TileDept = myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos)
                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).BackColor = Color.Black
                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Text = ""
                FacilityLayoutModel.SetTile(myTermites(i).Position, 0)
                i = i + 1
            End If
        Next

        For x = 0 To myNumTermites - 1
            myTermites(x).Move(FacilityLayoutModel, rows - 1, columns - 1)
            Dim rn = RandomRow.Next(0, 3)
            If rn < 2 Then
                myTermites(x) = myTermites(x).ChangeType(Of GreedyTermite)()
            Else
                myTermites(x) = myTermites(x).ChangeType(Of ScholarTermite)()
            End If
        Next
        y = 0
        Dim refreshcounter As Integer = 0
        Dim TotalContig As Boolean
        Dim ContigIndicator As Boolean
        Dim n As Integer = 0
        Dim Phase1Decay As Integer = 10
        Do
            If rows - 1 = FacilityStats.FacilitySize.Rows AndAlso columns - 1 = FacilityStats.FacilitySize.Columns Then
                Exit Do
            End If
            refreshcounter = refreshcounter + 1
            If Math.IEEERemainder(refreshcounter, 400) = 0 Then
                For a = 0 To rows - 1
                    For b = 0 To columns - 1
                        myTermiteOwnedTile(a, b) = False
                    Next
                Next
                y = 0
                Do
                    ReorganizationMethodScholar(rows - 1, columns - 1, myNumTermites - 1)
                    TileRefresher(rows, columns)
                    y = y + 1
                Loop Until y = 50

            End If
            ReorganizationMethodScholar(rows - 1, columns - 1, myNumTermites - 1)
            myLoopCounter = myLoopCounter + 1
            TotalContig = contiguityTester.AllDepartmentsAreContiguous(myFacilityMatrix)
            If n < myNumTermites Then
                If Math.IEEERemainder(myLoopCounter, Phase1Decay) = 0 AndAlso myLoopCounter >= myGravStart - Math.Round(myGravStart / 4) Then
                    myTermites(n) = myTermites(n).ChangeType(Of GreedyTermite)()
                    n = n + 1
                End If
            End If
            If myLoopCounter >= myGravStart + Math.Round(myGravStart / 2) Then
                For a = 1 To FacilityStats.DepartmentCount
                    ContigIndicator = contiguityTester.DepartmentIsContiguous(a, myFacilityMatrix)
                    If ContigIndicator = False Then
                        TotalContig = False
                    End If
                    If ContigIndicator = True Then
                        FreezeDeptMotion(a, rows, columns)
                    End If
                Next
            End If
            If Math.IEEERemainder(refreshcounter, 100) = 0 Then
                TileRefresher(rows, columns)
            End If
        Loop Until TotalContig = True
    End Sub
    'Sets Initial Position an Direction of Termites
    Friend Sub ReleaseTheTermites(ByVal NumTermites As Integer, ByVal rows As Integer, ByVal columns As Integer)
        TermiteManager = New TermiteManager(FacilityLayoutModel)
        myNumTermites = NumTermites
        Dim TypeRatio As Integer = 3
        If TxtRatio.Text <> "" Then
            Integer.TryParse(TxtRatio.ToString, TypeRatio)
        End If

        myTermites = TermiteManager.ReleaseTheTermites(NumTermites, TypeRatio)
        myTermiteOwnedTile = TermiteManager.OwnedTiles
    End Sub
    'Termites begin to move, collect, and drop tiles
    Private Sub ReorganizationMethod1(ByVal Rows As Integer, ByVal Columns As Integer)
        Dim i As Integer
        Dim counter As Integer = 0
        Dim SimilarAdjTileCount As Integer
        Dim Roulette As Integer
        Dim ChaosStart As Integer = 6000
        Dim ChaosEnd As Integer = 6500

        For i = 0 To myNumTermites - 1
            myTermites(i).Move(FacilityLayoutModel, Rows, Columns)

            'If a termite didn't have a tile before but is now on a space with an un-owned tile, pick it up
            If FacilityLayoutModel.IsTileAssigned(myTermites(i).RowPos, myTermites(i).ColumnPos) = True Then
                If myTermiteOwnedTile(myTermites(i).RowPos, myTermites(i).ColumnPos) = False Then
                    If myTermites(i).HasTile = False Then
                        SimilarAdjTileCount = contiguityTester.CountAdjacentTilesOfSameDepartment(myTermites(i).Position, myFacilityMatrix)
                        Roulette = RandomRow.Next(0, SimilarAdjTileCount ^ 2 + 1)
                        If myLoopCounter > ChaosStart AndAlso myLoopCounter < ChaosEnd Then
                            Roulette = 0
                        End If
                        If Roulette = 0 Then
                            myTermites(i).HasTile = True
                            myTermiteOwnedTile(myTermites(i).RowPos, myTermites(i).ColumnPos) = True
                            myTermites(i).TileDept = myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos)
                            Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).BackColor = Color.Black
                            Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Text = ""
                            'Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Refresh()
                            FacilityLayoutModel.SetTile(myTermites(i).Position, 0)
                        End If
                    End If
                End If
            End If

            'Check to see if any adjacent tile is equivalent to whatever tile termite has
            'If check passes, find nearest empty space and set tile down
            'If check fails, continue moving, loop process until check passes
            Do
                If myTermites(i).HasTile = True Then
                    myTermites(i).FindDropPoint(FacilityLayoutModel, FacilityStats, Rows, Columns)
                    If myTermites(i).HasTile = False Then
                        myTermites(i).HorizDirection = -1 * myTermites(i).HorizDirection
                        myTermites(i).VertDirection = -1 * myTermites(i).VertDirection
                        Exit Do
                    End If
                ElseIf myTermites(i).HasTile = False Then
                    Exit Do
                End If
                myTermites(i).Move(FacilityLayoutModel, Rows, Columns)
                counter = counter + 1
                'If termite continuously fails to find an adjacent equivalent tile, set tile down in nearest empty space
                If counter > 40 Then
                    If FacilityLayoutModel.IsTileAssigned(myTermites(i).RowPos, myTermites(i).ColumnPos) = False Then
                        myTermites(i).DropTile(FacilityLayoutModel)
                    Else
                        Do
                            myTermites(i).Move(FacilityLayoutModel, Rows, Columns)
                        Loop Until FacilityLayoutModel.IsTileAssigned(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
                        myTermites(i).DropTile(FacilityLayoutModel)
                    End If
                    Exit Do
                End If
            Loop Until myTermites(i).HasTile = False
            counter = 0

            'In my new position, check to see if there are any tiles around that are closely related
            'If so, drop my tile in the closest empty space
        Next i
    End Sub

    'This function describes the space immediately around the tile in question. Used for contiguity testing.
    Private Sub ReorganizationMethodScholar(ByVal rows As Integer, ByVal columns As Integer, ByVal numtermites As Integer)
        Dim i As Integer
        Dim counter As Integer = 0
        Dim SimilarAdjTileCount As Integer
        Dim Roulette As Integer
        Dim CurrentTile As Integer = 0
        Dim upperlimit As Integer = 5
        Dim HoardingTermite As Integer
        If myLoopPhase = 1 Then
            HoardingTermite = 0
        Else
            HoardingTermite = 1
        End If

        For i = HoardingTermite To numtermites - 1
            myTermites(i).Move(FacilityLayoutModel, rows, columns)

            'If a termite didn't have a tile before but is now on a space with an un-owned tile, pick it up
            If FacilityLayoutModel.IsTileAssigned(myTermites(i).RowPos, myTermites(i).ColumnPos) = True Then
                If myTermiteOwnedTile(myTermites(i).RowPos, myTermites(i).ColumnPos) = False Then
                    If myTermites(i).HasTile = False Then
                        SimilarAdjTileCount = contiguityTester.CountAdjacentTilesOfSameDepartment(myTermites(i).Position, myFacilityMatrix)
                        Roulette = RandomRow.Next(0, SimilarAdjTileCount ^ 1.75 + 1)
                        If Roulette = 0 Then
                            myTermites(i).HasTile = True
                            myTermiteOwnedTile(myTermites(i).RowPos, myTermites(i).ColumnPos) = True
                            myTermites(i).TileDept = myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos)
                            Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).BackColor = Color.Black
                            Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Text = ""
                            FacilityLayoutModel.SetTile(myTermites(i).Position, 0)
                        End If
                    End If
                End If
            End If

            Do
                If myTermites(i).HasTile = True Then
                    myTermites(i).FindDropPoint(FacilityLayoutModel, FacilityStats, rows, columns)
                    If myTermites(i).HasTile = False Then
                        myTermites(i).HorizDirection = -1 * myTermites(i).HorizDirection
                        myTermites(i).VertDirection = -1 * myTermites(i).VertDirection
                        Exit Do
                    End If
                ElseIf myTermites(i).HasTile = False Then
                    Exit Do
                End If
                myTermites(i).Move(FacilityLayoutModel, rows, columns)
                counter = counter + 1
                'If termite continuously fails to find an adjacent equivalent tile, set tile down in nearest empty space
                If counter > 40 Then
                    If FacilityLayoutModel.IsTileAssigned(myTermites(i).RowPos, myTermites(i).ColumnPos) = False Then
                        myTermites(i).DropTile(FacilityLayoutModel)
                    Else
                        Do
                            myTermites(i).Move(FacilityLayoutModel, rows, columns)
                        Loop Until FacilityLayoutModel.IsTileAssigned(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
                        myTermites(i).DropTile(FacilityLayoutModel)
                    End If
                    Exit Do
                End If
            Loop Until myTermites(i).HasTile = False
            counter = 0
        Next
        Dim f As Integer
        f = 2
    End Sub

    Private Sub ScholarTermiteMethodToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ScholarTermiteMethodToolStripMenuItem.Click
        Dim Rows As Integer = Math.Round(Math.Sqrt(2 * FacilityStats.FacilitySize.Rows ^ 2))
        Dim Columns As Integer = Math.Round(Math.Sqrt(2 * FacilityStats.FacilitySize.Columns ^ 2))
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
            ReleaseTheTermites(NumTermites, Rows, Columns)
            StartTime = Now
            Do
                refreshcounter = refreshcounter + 1

                If Math.IEEERemainder(refreshcounter, 600) = 0 Then
                    'MessageBox.Show("pause")
                    For a = 0 To Rows - 1
                        For b = 0 To Columns - 1
                            myTermiteOwnedTile(a, b) = False
                        Next
                    Next
                    y = 0
                    Do
                        ReorganizationMethodScholar(Rows, Columns, myNumTermites - 1)
                        'TileRefresher(Rows, Columns)
                        y = y + 1
                    Loop Until y = 50

                End If

                ReorganizationMethodScholar(Rows, Columns, myNumTermites)
                myLoopCounter = myLoopCounter + 1
                TotalContig = contiguityTester.AllDepartmentsAreContiguous(myFacilityMatrix)
                If n < myNumTermites Then
                    If Math.IEEERemainder(myLoopCounter, Phase1Decay) = 0 AndAlso myLoopCounter >= myGravStart - Math.Round(myGravStart / 4) Then
                        myTermites(n) = myTermites(n).ChangeType(Of GreedyTermite)()
                        n = n + 1
                    End If
                End If
                If myLoopCounter >= myGravStart + Math.Round(myGravStart / 2) Then
                    For i = 1 To FacilityStats.DepartmentCount
                        ContigIndicator = contiguityTester.DepartmentIsContiguous(i, myFacilityMatrix)
                        If ContigIndicator = False Then
                            TotalContig = False
                        End If
                        If ContigIndicator = True Then
                            FreezeDeptMotion(i, Rows, Columns)
                        End If
                    Next
                End If
                If Math.IEEERemainder(refreshcounter, 100) = 0 Then
                    TileRefresher(Rows, Columns)
                End If
            Loop Until TotalContig = True
            TileRefresher(Rows, Columns)
            'MessageBox.Show("Pause")
            For i = 0 To Rows - 1
                For j = 0 To Columns - 1
                    myTermiteOwnedTile(i, j) = False
                Next
            Next
            'MessageBox.Show("pause")
            Dim rowstore, columnstore As Integer
            rowstore = Rows
            columnstore = Columns
            Do
                For i = 0 To Rows - 1
                    For j = 0 To Columns - 1
                        myTermiteOwnedTile(i, j) = False
                    Next
                Next
                RealisticCollection(Rows, Columns)
                TileRefresher(Rows, Columns)
                Rows = Rows - 1
                Columns = Columns - 1
                If Rows = FacilityStats.FacilitySize.Rows + 1 Then
                    myLoopPhase = 2
                End If
                'MessageBox.Show("pause")
            Loop Until Rows = FacilityStats.FacilitySize.Rows AndAlso Columns = FacilityStats.FacilitySize.Columns
            Rows = rowstore
            Columns = columnstore
            'MessageBox.Show("Pause")
            myLoopCounter = 0
            n = 0
            TileRefresher(Rows, Columns)
            myLoopPhase = 2
            For i = 0 To Rows - 1
                For j = 0 To Columns - 1
                    myTermiteOwnedTile(i, j) = False
                Next
            Next

            '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!STAGE 2!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Do
                ReorganizationMethodScholar(FacilityStats.FacilitySize.Rows, FacilityStats.FacilitySize.Columns, myNumTermites)
                If Math.IEEERemainder(myLoopCounter, 100) = 0 Then
                    TileRefresher(FacilityStats.FacilitySize.Rows, FacilityStats.FacilitySize.Columns)
                End If
                myLoopCounter = myLoopCounter + 1
                TotalContig = contiguityTester.AllDepartmentsAreContiguous(myFacilityMatrix)
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
                                adjTilesContainSameDepartment = contiguityTester.AdjacentTilesContainSameDepartment(myTermites(0).TileDept, i, j, myFacilityMatrix, FacilityStats.DepartmentSizes)

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
            StopTime = Now
            RunTime = StopTime.Subtract(StartTime)
            Dim VDC As Double
            Dim OBJValue As String
            VDC = facilityEvaluator.VolumeDistanceCostProduct(FacilityStats, myFacilityMatrix)
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