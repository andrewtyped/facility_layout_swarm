Imports System.Runtime.CompilerServices
Imports FaciltyLayout.Core
Imports FaciltyLayout.Core.Models

<Assembly: InternalsVisibleTo("FacilityLayout.Core.Tests")>
Public Class Form1
    Friend myNumDepartments As Integer
    Friend myDeptRowsColumns(1) As Integer
    Friend myDeptSizes() As Integer
    Friend myVolumeMatrix(,) As Integer
    Friend myCostMatrix(,) As Double
    Private RandomRow As New Random
    Friend myFacilityMatrix(,) As Integer 'Matrix displaying the field of tiles and empty spaces
    Private myTermiteOwnedTile(,) As Boolean 'Is a termite in the process of moving this tile?
    Friend myTermites() As Termites 'Agents for moving the tiles
    Private myNumTermites As Integer
    Friend myFlows() As FlowStats
    Private Tile(,) As Windows.Forms.Label
    Private TileRefreshCounter As Integer = 0
    Private myTileColors(,) As Integer
    Private myLoopCounter = 0
    Private myFrozenDepts() As Boolean
    Private myCountdFrznDepts() As Boolean
    Private myLoopPhase As Integer
    Private myNumFrozenDepts As Integer = 0
    Private myGravStart As Integer
    Private TransformVDC As Integer

    Private facilityEvaluator As New FacilityEvaluator()
    Private contiguityTester As New ContiguityTester()

    Friend FacilityStats As FacilityStats
    Friend FacilityLayoutModel As FacilityLayoutModel

    'Termite sets his tile down
    Private Sub DropTile(ByVal X As Integer, ByVal Y As Integer, ByVal TermiteNumber As Integer)
        Tile(X, Y).BackColor = Color.FromArgb(myTileColors(myTermites(TermiteNumber).TileDept, 0),
                                              myTileColors(myTermites(TermiteNumber).TileDept, 1),
                                              myTileColors(myTermites(TermiteNumber).TileDept, 2))
        myTermiteOwnedTile(X, Y) = False
        FacilityLayoutModel.SetTile(X, Y, myTermites(TermiteNumber).TileDept)
        Tile(X, Y).Text = myTermites(TermiteNumber).TileDept.ToString
        myTermites(TermiteNumber).HasTile = False
        myTermites(TermiteNumber).TileDept = Nothing
    End Sub

    Private Sub FreezeDeptMotion(ByVal dept As Integer, ByVal rows As Integer, ByVal columns As Integer)
        For i = 0 To rows - 1
            For j = 0 To columns - 1
                If myFacilityMatrix(i, j) = dept Then
                    myTermiteOwnedTile(i, j) = True
                End If
            Next
        Next
        If myCountdFrznDepts(dept) = False Then
            myNumFrozenDepts = myNumFrozenDepts + 1
            myCountdFrznDepts(dept) = True
            myFrozenDepts(dept) = True
        End If
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
    'Termite drops tile in nearest empty space if an equivalent tile is located in an adjacent space
    Private Sub GreedyTermite(ByVal TermiteNumber As Integer, ByVal rows As Integer, ByVal columns As Integer)
        Dim i, j As Integer
        Dim ClosestFound As Boolean = False
        Dim ClosestEmpty(,) As Integer = LookAround()
        Dim AdjCheck(,) As Integer = LookAround()
        'Check adjacent spaces for equivalent tiles
        For i = 0 To 8
            'Termite must not look outside of facility field boundaries
            If 0 <= myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) AndAlso myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) <= rows - 1 Then
                If 0 <= myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) AndAlso myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) <= columns - 1 Then
                    If myFacilityMatrix(myTermites(TermiteNumber).RowPos - AdjCheck(i, 0),
                                        myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1)) = myTermites(TermiteNumber).TileDept Then
                        For j = 0 To 8
                            If 0 <= myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) - ClosestEmpty(j, 1) AndAlso
                                myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) - ClosestEmpty(j, 1) <= rows - 1 Then
                                If 0 <= myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) - ClosestEmpty(j, 0) AndAlso
                                    myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) - ClosestEmpty(j, 0) <= columns - 1 Then
                                    If FacilityLayoutModel.IsTileAssigned(myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) - ClosestEmpty(j, 0),
                                                       myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) - ClosestEmpty(j, 1)) = False Then
                                        ClosestFound = True
                                        Dim X, Y As Integer
                                        X = myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) - ClosestEmpty(j, 0)
                                        Y = myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) - ClosestEmpty(j, 1)
                                        Tile(X, Y).BackColor = Color.FromArgb(myTileColors(myTermites(TermiteNumber).TileDept, 0),
                                        myTileColors(myTermites(TermiteNumber).TileDept, 1),
                                        myTileColors(myTermites(TermiteNumber).TileDept, 2))
                                        myTermites(TermiteNumber).ColumnPos = Y
                                        myTermites(TermiteNumber).RowPos = X
                                        myTermiteOwnedTile(X, Y) = False
                                        FacilityLayoutModel.SetTile(X, Y, myTermites(TermiteNumber).TileDept)
                                        Tile(X, Y).Text = myTermites(TermiteNumber).TileDept.ToString
                                        myTermites(TermiteNumber).HasTile = False
                                        myTermites(TermiteNumber).TileDept = Nothing
                                        Exit For
                                    End If
                                End If
                            End If
                        Next
                        If ClosestFound = True Then
                            Exit For
                        End If
                    End If
                End If
            End If
        Next

    End Sub
    Private Sub GreedyTermiteMethodToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GreedyTermiteMethodToolStripMenuItem.Click
        Dim Rows As Integer = Math.Round(Math.Sqrt(2 * myDeptRowsColumns(0) ^ 2))
        Dim Columns As Integer = Math.Round(Math.Sqrt(2 * myDeptRowsColumns(1) ^ 2))

        If TxtRows.Text <> "" Then
            Integer.TryParse(TxtRows.Text, Rows)
        End If

        If TxtColumns.Text <> "" Then
            Integer.TryParse(TxtColumns.Text, Columns)
        End If

        ReleaseTheTermites(Math.Round(myDeptRowsColumns(0) * myDeptRowsColumns(1) * 1.5), Rows, Columns)

        Dim ContigIndicator As Boolean
        Dim TotalContig
        Dim VDCP As Integer
        ReDim myFrozenDepts(myNumDepartments)
        ReDim myCountdFrznDepts(myNumDepartments)

        Do
            ReorganizationMethod1(Rows, Columns)

            For i = 1 To myNumDepartments
                ContigIndicator = contiguityTester.DepartmentIsContiguous(i, myFacilityMatrix)

                If ContigIndicator = True Then
                    FreezeDeptMotion(i, Rows, Columns)
                End If
            Next

            TotalContig = contiguityTester.AllDepartmentsAreContiguous(myFacilityMatrix)
        Loop Until TotalContig = True

        VDCP = facilityEvaluator.VolumeDistanceCostProduct(myNumDepartments, myFacilityMatrix, myVolumeMatrix, myCostMatrix, myDeptSizes)
        MessageBox.Show(VDCP.ToString("################.00"))

    End Sub
    'This funcion defines in what order the termites should look around them for tiles.
    'Depending on the phase of the program, the termites may have a biased direction
    'In their search.
    Private Function LookAround()
        Dim i, j, a As Integer
        Dim Checked(2, 2) As Boolean
        Dim SearchOrder(8, 1) As Integer
        'Dim rand As Integer

        If myLoopPhase = 1 Then
            For i = 0 To 2
                For j = 0 To 2
                    Checked(i, j) = False
                Next
            Next

            For a = 0 To 8
                Do
                    i = RandomRow.Next(0, 3) - 1
                    j = RandomRow.Next(0, 3) - 1
                    SearchOrder(a, 0) = RandomRow.Next(0, 3) - 1
                    SearchOrder(a, 1) = RandomRow.Next(0, 3) - 1
                Loop Until Checked(i + 1, j + 1) = False
                Checked(i + 1, j + 1) = True
            Next
            a = 0
        End If
        If myLoopPhase = 2 Then
            a = 0
            For i = 0 To 2
                For j = 0 To 2
                    Checked(i, j) = False
                Next
            Next

            For a = 0 To 8
                Do
                    i = RandomRow.Next(0, 3) - 1
                    j = RandomRow.Next(0, 3) - 1
                    SearchOrder(a, 0) = RandomRow.Next(0, 3) - 1
                    SearchOrder(a, 1) = RandomRow.Next(0, 3) - 1
                Loop Until Checked(i + 1, j + 1) = False
                Checked(i + 1, j + 1) = True
            Next
        End If
        Return SearchOrder

    End Function
    'Redirects termite if fixed dept or wall is encountered
    Private Sub MarchMarchMarch(ByVal i As Integer, ByVal rows As Integer, ByVal columns As Integer, ByVal upperlimit As Integer)


        'Say what the termites should do in the event they encounter a wall
        If myTermites(i).ColumnPos + myTermites(i).HorizDirection < 0 Or
            myTermites(i).ColumnPos + myTermites(i).HorizDirection >= columns Or
            myTermites(i).RowPos + myTermites(i).VertDirection < 0 Or
            myTermites(i).RowPos + myTermites(i).VertDirection >= rows Then
            Do
                myTermites(i).HorizDirection = RandomRow.Next(0, upperlimit) - 2
                myTermites(i).VertDirection = RandomRow.Next(0, upperlimit) - 2
            Loop Until myTermites(i).ColumnPos + myTermites(i).HorizDirection > 0 AndAlso
                       myTermites(i).ColumnPos + myTermites(i).HorizDirection < columns AndAlso
                       myTermites(i).RowPos + myTermites(i).VertDirection > 0 AndAlso
                       myTermites(i).RowPos + myTermites(i).VertDirection < rows
        End If

        If FacilityLayoutModel.IsTileFixed(myTermites(i).RowPos + myTermites(i).VertDirection,
                                 myTermites(i).ColumnPos + myTermites(i).HorizDirection) = True Then
            Do
                myTermites(i).HorizDirection = RandomRow.Next(0, upperlimit) - 2
                myTermites(i).VertDirection = RandomRow.Next(0, upperlimit) - 2
            Loop Until myTermites(i).ColumnPos + myTermites(i).HorizDirection > 0 AndAlso
                       myTermites(i).ColumnPos + myTermites(i).HorizDirection < columns AndAlso
                       myTermites(i).RowPos + myTermites(i).VertDirection > 0 AndAlso
                       myTermites(i).RowPos + myTermites(i).VertDirection < rows AndAlso
                       FacilityLayoutModel.IsTileFixed(myTermites(i).RowPos + myTermites(i).VertDirection,
                                 myTermites(i).ColumnPos + myTermites(i).HorizDirection) = False
        End If
    End Sub
    Private Function MaxFlowValue()
        Dim i, j, maxflow(myNumDepartments) As Integer

        For i = 0 To myNumDepartments
            maxflow(i) = 0
        Next
        For i = 1 To myNumDepartments
            For j = 1 To myNumDepartments
                If myVolumeMatrix(i, j) > maxflow(i) Then
                    maxflow(i) = myVolumeMatrix(i, j)
                End If
            Next
        Next

        Return maxflow
    End Function
    'Termites may not have a horiz,vert direction of 0,0
    Private Sub NoLazinessAllowed(ByVal i As Integer)
        If myTermites(i).HorizDirection = 0 AndAlso myTermites(i).VertDirection = 0 Then
            Do
                myTermites(i).HorizDirection = RandomRow.Next(0, 5) - 2
                myTermites(i).VertDirection = RandomRow.Next(0, 5) - 2
            Loop Until myTermites(i).HorizDirection <> 0 Or myTermites(i).VertDirection <> 0
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
        Dim myFacilityStats = facilityStatsRepository.Load()

        myNumDepartments = myFacilityStats.DepartmentCount
        myDeptRowsColumns = myFacilityStats.FacilitySize.ToArray()
        myDeptSizes = myFacilityStats.DepartmentSizes
        myVolumeMatrix = myFacilityStats.VolumeMatrix
        myCostMatrix = myFacilityStats.CostMatrix
        myFlows = myFacilityStats.Flows
        FacilityStats = myFacilityStats

        Dim PopUp As String = myFacilityStats.ToString()

        Return PopUp
    End Function

    Private Sub RandomizeTiles(facilityStats As FacilityStats)
        FacilityLayoutModel = GenerateFacilitySwarm(facilityStats)
        myFacilityMatrix = FacilityLayoutModel.Facility

        Dim objMatrixFile As New System.IO.StreamWriter("C:\Users\Andrew\Documents\IE 590\FacilitySwarm.txt")
        Dim strMatrix As String = Nothing
        Dim i, j As Integer
        Dim Rows As Integer = Math.Round(Math.Sqrt(2 * (myDeptRowsColumns(0) ^ 2)))
        Dim Columns As Integer = Math.Round(Math.Sqrt(2 * (myDeptRowsColumns(1) ^ 2)))
        If TxtRows.Text <> "" Then
            Integer.TryParse(TxtRows.Text, Rows)
        End If
        If TxtColumns.Text <> "" Then
            Integer.TryParse(TxtColumns.Text, Columns)
        End If
        For i = 0 To Rows - 1
            For j = 0 To Columns - 1
                If myFacilityMatrix(i, j).ToString.Length = 1 Then
                    strMatrix = strMatrix & myFacilityMatrix(i, j).ToString & "  "
                Else
                    strMatrix = strMatrix & myFacilityMatrix(i, j).ToString & " "
                End If
            Next j
            strMatrix = strMatrix & vbCrLf
        Next i
        objMatrixFile.Write(strMatrix)
        objMatrixFile.Close()

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
            ReDim myTileColors(myNumDepartments, 2)
        End If
        Dim ColorRangeCount As Integer = 100
        For j = 0 To 2
            myTileColors(0, j) = 0
        Next

        For i = 0 To myNumDepartments - 1
            For j = 0 To 2
                myTileColors(i + 1, j) = (myNumDepartments / 2) * RandomRow.Next(10, 255 / (myNumDepartments / 2))

            Next
            ColorRangeCount = ColorRangeCount + 150 / myNumDepartments
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
                'Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Refresh()
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
                'Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Refresh()
                FacilityLayoutModel.SetTile(myTermites(i).Position, 0)
                i = i + 1
            End If
        Next

        For x = 0 To myNumTermites - 1
            NoLazinessAllowed(x)
            MarchMarchMarch(x, rows - 1, columns - 1, upperlimit)
            myTermites(x).ColumnPos = myTermites(x).ColumnPos + myTermites(x).HorizDirection
            myTermites(x).RowPos = myTermites(x).RowPos + myTermites(x).VertDirection
            myTermites(x).TermiteType = RandomRow.Next(0, 3)
            If myTermites(x).TermiteType < 2 Then
                myTermites(x).TermiteType = 0
            Else
                myTermites(x).TermiteType = 1
            End If
        Next
        y = 0
        Dim refreshcounter As Integer = 0
        Dim TotalContig As Boolean
        Dim ContigIndicator As Boolean
        Dim n As Integer = 0
        Dim Phase1Decay As Integer = 10
        Do
            If rows - 1 = myDeptRowsColumns(0) AndAlso columns - 1 = myDeptRowsColumns(1) Then
                Exit Do
            End If
            refreshcounter = refreshcounter + 1
            If Math.IEEERemainder(refreshcounter, 400) = 0 Then
                'MessageBox.Show("pause")
                For a = 0 To rows - 1
                    For b = 0 To columns - 1
                        myTermiteOwnedTile(a, b) = False
                    Next
                Next
                myNumFrozenDepts = 0
                For a = 0 To myNumDepartments
                    myFrozenDepts(a) = False
                    myCountdFrznDepts(a) = False
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
                    myTermites(n).TermiteType = 0
                    n = n + 1
                End If
            End If
            If myLoopCounter >= myGravStart + Math.Round(myGravStart / 2) Then
                For a = 1 To myNumDepartments
                    ContigIndicator = contiguityTester.DepartmentIsContiguous(a, myFacilityMatrix)
                    If ContigIndicator = False Then
                        TotalContig = False
                    End If
                    If ContigIndicator = True Then
                        FreezeDeptMotion(a, rows, columns)
                    End If
                Next
            End If
            'If myNumDepartments - myNumFrozenDepts < 1 AndAlso myLoopCounter > myGravStart * 3 Then
            '    'MessageBox.Show("Departments close to contiguous. Loop exited to avoid infinite loop.")
            '    Exit Do
            'End If
            If Math.IEEERemainder(refreshcounter, 100) = 0 Then
                TileRefresher(rows, columns)
            End If
        Loop Until TotalContig = True
    End Sub
    'Sets Initial Position an Direction of Termites
    Private Sub ReleaseTheTermites(ByVal NumTermites As Integer, ByVal rows As Integer, ByVal columns As Integer)
        myNumTermites = NumTermites
        ReDim myTermites(NumTermites - 1)
        Dim HorizDirectionCounter As Integer = 0
        Dim VertDirectionCounter As Integer = 0
        ReDim myTermiteOwnedTile(rows - 1, columns - 1)
        Dim TypeRatio As Integer = 3
        If TxtRatio.Text <> "" Then
            Integer.TryParse(TxtRatio.ToString, TypeRatio)
        End If

        For i = 0 To NumTermites - 1
            myTermites(i).HasTile = False
            myTermites(i).ColumnPos = RandomRow.Next(0, columns)
            myTermites(i).RowPos = RandomRow.Next(0, rows)
            myTermites(i).TermiteType = RandomRow.Next(0, TypeRatio)
            If myTermites(i).TermiteType < TypeRatio - 1 Then
                myTermites(i).TermiteType = 0
            Else
                myTermites(i).TermiteType = 1
            End If

            If FacilityLayoutModel.IsTileFixed(myTermites(i).RowPos, myTermites(i).ColumnPos) = True Then
                Do
                    myTermites(i).ColumnPos = RandomRow.Next(0, columns)
                    myTermites(i).RowPos = RandomRow.Next(0, rows)
                Loop Until FacilityLayoutModel.IsTileFixed(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
            End If
            myTermites(i).HorizDirection = RandomRow.Next(0, 5) - 2
            myTermites(i).VertDirection = RandomRow.Next(0, 5) - 2
            NoLazinessAllowed(i)
            If myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos) <> 0 Then
                If myTermiteOwnedTile(myTermites(i).RowPos, myTermites(i).ColumnPos) = False Then
                    myTermites(i).HasTile = True
                    myTermiteOwnedTile(myTermites(i).RowPos, myTermites(i).ColumnPos) = True
                    myTermites(i).TileDept = myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos)
                End If
            End If
            If myTermites(i).HasTile = True Then
                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).BackColor = Color.Black
                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Text = ""
                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Refresh()
                FacilityLayoutModel.SetTile(myTermites(i).Position, 0)
            End If
        Next
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
            NoLazinessAllowed(i)
            MarchMarchMarch(i, Rows, Columns, 5)
            myTermites(i).ColumnPos = myTermites(i).ColumnPos + myTermites(i).HorizDirection
            myTermites(i).RowPos = myTermites(i).RowPos + myTermites(i).VertDirection

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
                            If myTermites(i).SpecificTile = True Then
                                If myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos) = myTermites(i).WhatSpecificTile Then
                                    myTermites(i).HasTile = True
                                    myTermiteOwnedTile(myTermites(i).RowPos, myTermites(i).ColumnPos) = True
                                    myTermites(i).TileDept = myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos)
                                    Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).BackColor = Color.Black
                                    Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Text = ""
                                    'Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Refresh()
                                    FacilityLayoutModel.SetTile(myTermites(i).Position, 0)
                                End If
                            ElseIf myTermites(i).SpecificTile = False Then
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
            End If

            'Check to see if any adjacent tile is equivalent to whatever tile termite has
            'If check passes, find nearest empty space and set tile down
            'If check fails, continue moving, loop process until check passes
            Do
                If myTermites(i).HasTile = True Then
                    GreedyTermite(i, Rows, Columns)
                    If myTermites(i).HasTile = False Then
                        myTermites(i).HorizDirection = -1 * myTermites(i).HorizDirection
                        myTermites(i).VertDirection = -1 * myTermites(i).VertDirection
                        Exit Do
                    End If
                ElseIf myTermites(i).HasTile = False Then
                    Exit Do
                End If
                NoLazinessAllowed(i)
                MarchMarchMarch(i, Rows, Columns, 5)
                myTermites(i).ColumnPos = myTermites(i).ColumnPos + myTermites(i).HorizDirection
                myTermites(i).RowPos = myTermites(i).RowPos + myTermites(i).VertDirection
                counter = counter + 1
                'If termite continuously fails to find an adjacent equivalent tile, set tile down in nearest empty space
                If counter > 40 Then
                    If FacilityLayoutModel.IsTileAssigned(myTermites(i).RowPos, myTermites(i).ColumnPos) = False Then
                        DropTile(myTermites(i).RowPos, myTermites(i).ColumnPos, i)
                    Else
                        Do
                            NoLazinessAllowed(i)
                            MarchMarchMarch(i, Rows, Columns, 5)
                            myTermites(i).ColumnPos = myTermites(i).ColumnPos + myTermites(i).HorizDirection
                            myTermites(i).RowPos = myTermites(i).RowPos + myTermites(i).VertDirection
                        Loop Until FacilityLayoutModel.IsTileAssigned(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
                        DropTile(myTermites(i).RowPos, myTermites(i).ColumnPos, i)
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
            'For j = 0 To 9
            NoLazinessAllowed(i)
            MarchMarchMarch(i, rows, columns, upperlimit)
            myTermites(i).ColumnPos = myTermites(i).ColumnPos + myTermites(i).HorizDirection
            myTermites(i).RowPos = myTermites(i).RowPos + myTermites(i).VertDirection

            'If a termite didn't have a tile before but is now on a space with an un-owned tile, pick it up
            If FacilityLayoutModel.IsTileAssigned(myTermites(i).RowPos, myTermites(i).ColumnPos) = True Then
                If myTermiteOwnedTile(myTermites(i).RowPos, myTermites(i).ColumnPos) = False Then
                    If myTermites(i).HasTile = False Then
                        SimilarAdjTileCount = contiguityTester.CountAdjacentTilesOfSameDepartment(myTermites(i).Position, myFacilityMatrix)
                        Roulette = RandomRow.Next(0, SimilarAdjTileCount ^ 1.75 + 1)
                        If Roulette = 0 Then
                            If myTermites(i).SpecificTile = True Then
                                If myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos) = myTermites(i).WhatSpecificTile Then
                                    myTermites(i).HasTile = True
                                    myTermiteOwnedTile(myTermites(i).RowPos, myTermites(i).ColumnPos) = True
                                    myTermites(i).TileDept = myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos)
                                    Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).BackColor = Color.Black
                                    Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Text = ""
                                    FacilityLayoutModel.SetTile(myTermites(i).Position, 0)
                                End If
                            ElseIf myTermites(i).SpecificTile = False Then
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
            End If
            'Next
            'For i = HoardingTermite To myNumTermites - 1
            If myTermites(i).TermiteType = 1 Then
                Do
                    If myTermites(i).HasTile = True Then
                        ScholarTermite(i, rows, columns)
                        If myTermites(i).HasTile = False Then
                            myTermites(i).HorizDirection = -1 * myTermites(i).HorizDirection
                            myTermites(i).VertDirection = -1 * myTermites(i).VertDirection
                            Exit Do
                        End If
                    ElseIf myTermites(i).HasTile = False Then
                        Exit Do
                    End If
                    NoLazinessAllowed(i)
                    MarchMarchMarch(i, rows, columns, upperlimit)
                    myTermites(i).ColumnPos = myTermites(i).ColumnPos + myTermites(i).HorizDirection
                    myTermites(i).RowPos = myTermites(i).RowPos + myTermites(i).VertDirection
                    counter = counter + 1
                    'If termite continuously fails to find an adjacent equivalent tile, set tile down in nearest empty space
                    If counter > 40 Then
                        If FacilityLayoutModel.IsTileAssigned(myTermites(i).RowPos, myTermites(i).ColumnPos) = False Then
                            DropTile(myTermites(i).RowPos, myTermites(i).ColumnPos, i)
                        Else
                            Do
                                NoLazinessAllowed(i)
                                MarchMarchMarch(i, rows, columns, upperlimit)
                                myTermites(i).ColumnPos = myTermites(i).ColumnPos + myTermites(i).HorizDirection
                                myTermites(i).RowPos = myTermites(i).RowPos + myTermites(i).VertDirection
                            Loop Until FacilityLayoutModel.IsTileAssigned(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
                            DropTile(myTermites(i).RowPos, myTermites(i).ColumnPos, i)
                        End If
                        Exit Do
                    End If
                Loop Until myTermites(i).HasTile = False
                counter = 0
            Else
                Do
                    If myTermites(i).HasTile = True Then
                        GreedyTermite(i, rows, columns)
                        If myTermites(i).HasTile = False Then
                            myTermites(i).HorizDirection = -1 * myTermites(i).HorizDirection
                            myTermites(i).VertDirection = -1 * myTermites(i).VertDirection
                            Exit Do
                        End If
                    ElseIf myTermites(i).HasTile = False Then
                        Exit Do
                    End If
                    NoLazinessAllowed(i)
                    MarchMarchMarch(i, rows, columns, upperlimit)
                    myTermites(i).ColumnPos = myTermites(i).ColumnPos + myTermites(i).HorizDirection
                    myTermites(i).RowPos = myTermites(i).RowPos + myTermites(i).VertDirection
                    counter = counter + 1
                    'If termite continuously fails to find an adjacent equivalent tile, set tile down in nearest empty space
                    If counter > 40 Then
                        If FacilityLayoutModel.IsTileAssigned(myTermites(i).RowPos, myTermites(i).ColumnPos) = False Then
                            DropTile(myTermites(i).RowPos, myTermites(i).ColumnPos, i)
                        Else
                            Do
                                NoLazinessAllowed(i)
                                MarchMarchMarch(i, rows, columns, upperlimit)
                                myTermites(i).ColumnPos = myTermites(i).ColumnPos + myTermites(i).HorizDirection
                                myTermites(i).RowPos = myTermites(i).RowPos + myTermites(i).VertDirection
                            Loop Until FacilityLayoutModel.IsTileAssigned(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
                            DropTile(myTermites(i).RowPos, myTermites(i).ColumnPos, i)
                        End If
                        Exit Do
                    End If
                Loop Until myTermites(i).HasTile = False
                counter = 0
                If i = myNumTermites - 1 Then
                    Dim a As Integer
                    a = 0
                End If
            End If

        Next
        Dim f As Integer
        f = 2
    End Sub

    Private Sub ScholarTermite(ByVal TermiteNumber As Integer, ByVal rows As Integer, ByVal columns As Integer)
        Dim i, j As Integer
        Dim ClosestFound As Boolean = False
        Dim ClosestEmpty(,) As Integer = LookAround()
        Dim AdjCheck(,) As Integer = LookAround()
        Dim rand As Integer = RandomRow.Next(0, 100)
        Dim TilePlaceHolder As Integer
        'Check adjacent spaces for equivalent tiles

        For i = 0 To 8
            'Termite must not look outside of facility field boundaries
            If 0 <= myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) AndAlso myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) <= rows - 1 Then
                If 0 <= myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) AndAlso myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) <= columns - 1 Then
                    If FacilityLayoutModel.IsTileAssigned(myTermites(TermiteNumber).RowPos - AdjCheck(i, 0), myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1)) = True Then
                        If rand <= 100 * (myFlows(myTermites(TermiteNumber).TileDept).Flows(myFacilityMatrix(myTermites(TermiteNumber).RowPos - AdjCheck(i, 0),
                                myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1)))) / (myFlows(myTermites(TermiteNumber).TileDept).FlowSum) Then
                            For j = 0 To 8
                                If 0 <= myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) - ClosestEmpty(j, 1) AndAlso
                                    myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) - ClosestEmpty(j, 1) <= rows - 1 Then
                                    If 0 <= myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) - ClosestEmpty(j, 0) AndAlso
                                        myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) - ClosestEmpty(j, 0) <= columns - 1 Then
                                        If FacilityLayoutModel.IsTileAssigned(myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) - ClosestEmpty(j, 0),
                                                           myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) - ClosestEmpty(j, 1)) = False Then
                                            ClosestFound = True
                                            Dim X, Y As Integer
                                            X = myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) - ClosestEmpty(j, 0)
                                            Y = myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) - ClosestEmpty(j, 1)
                                            Tile(X, Y).BackColor = Color.FromArgb(myTileColors(myTermites(TermiteNumber).TileDept, 0),
                                            myTileColors(myTermites(TermiteNumber).TileDept, 1),
                                            myTileColors(myTermites(TermiteNumber).TileDept, 2))
                                            myTermites(TermiteNumber).ColumnPos = Y
                                            myTermites(TermiteNumber).RowPos = X
                                            myTermiteOwnedTile(X, Y) = False
                                            FacilityLayoutModel.SetTile(X, Y, myTermites(TermiteNumber).TileDept)
                                            Tile(X, Y).Text = myTermites(TermiteNumber).TileDept.ToString
                                            myTermites(TermiteNumber).HasTile = False
                                            myTermites(TermiteNumber).TileDept = Nothing
                                            Exit For
                                        End If
                                    End If
                                End If
                            Next
                            If ClosestFound = True Then
                                Exit For
                            End If
                        End If
                    End If
                End If
            End If
        Next
    End Sub
    Private Sub ScholarTermiteMethodToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ScholarTermiteMethodToolStripMenuItem.Click
        Dim Rows As Integer = Math.Round(Math.Sqrt(2 * myDeptRowsColumns(0) ^ 2))
        Dim Columns As Integer = Math.Round(Math.Sqrt(2 * myDeptRowsColumns(1) ^ 2))
        Dim NumTermites As Integer = Math.Round(myDeptRowsColumns(0) * myDeptRowsColumns(1) * 1.5)
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
        ReDim myFrozenDepts(myNumDepartments)
        ReDim myCountdFrznDepts(myNumDepartments)
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
            NumTermites = Math.Round(myDeptRowsColumns(0) * myDeptRowsColumns(1) * 1.5)
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
                    myNumFrozenDepts = 0
                    For a = 0 To myNumDepartments
                        myFrozenDepts(a) = False
                        myCountdFrznDepts(a) = False
                    Next
                    y = 0
                    Do
                        ReorganizationMethodScholar(Rows - 1, Columns - 1, myNumTermites - 1)
                        TileRefresher(Rows, Columns)
                        y = y + 1
                    Loop Until y = 50

                End If

                ReorganizationMethodScholar(Rows, Columns, myNumTermites)
                myLoopCounter = myLoopCounter + 1
                TotalContig = contiguityTester.AllDepartmentsAreContiguous(myFacilityMatrix)
                If n < myNumTermites Then
                    If Math.IEEERemainder(myLoopCounter, Phase1Decay) = 0 AndAlso myLoopCounter >= myGravStart - Math.Round(myGravStart / 4) Then
                        myTermites(n).TermiteType = 0
                        n = n + 1
                    End If
                End If
                If myLoopCounter >= myGravStart + Math.Round(myGravStart / 2) Then
                    For i = 1 To myNumDepartments
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
            For i = 0 To myNumDepartments
                myFrozenDepts(i) = False
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
                myNumFrozenDepts = 0
                For i = 0 To myNumDepartments
                    myFrozenDepts(i) = False
                    myCountdFrznDepts(i) = False
                Next
                RealisticCollection(Rows, Columns)
                TileRefresher(Rows, Columns)
                Rows = Rows - 1
                Columns = Columns - 1
                If Rows = myDeptRowsColumns(0) + 1 Then
                    myLoopPhase = 2
                End If
                'MessageBox.Show("pause")
            Loop Until Rows = myDeptRowsColumns(0) AndAlso Columns = myDeptRowsColumns(1)
            Rows = rowstore
            Columns = columnstore
            'MessageBox.Show("Pause")
            myLoopCounter = 0
            n = 0
            TileRefresher(Rows, Columns)
            myLoopPhase = 2
            myNumFrozenDepts = 0
            For i = 0 To Rows - 1
                For j = 0 To Columns - 1
                    myTermiteOwnedTile(i, j) = False
                Next
            Next
            For i = 0 To myNumDepartments
                myFrozenDepts(i) = False
                myCountdFrznDepts(i) = False
            Next
            For i = 0 To myNumTermites - 1
                myTermites(i).SpecificTile = False
            Next


            '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!STAGE 2!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Do
                ReorganizationMethodScholar(myDeptRowsColumns(0), myDeptRowsColumns(1), myNumTermites)
                If Math.IEEERemainder(myLoopCounter, 100) = 0 Then
                    TileRefresher(myDeptRowsColumns(0), myDeptRowsColumns(1))
                End If
                myLoopCounter = myLoopCounter + 1
                TotalContig = contiguityTester.AllDepartmentsAreContiguous(myFacilityMatrix)
                If n < myNumTermites Then
                    If Math.IEEERemainder(myLoopCounter, Phase2Decay) = 0 AndAlso myLoopCounter >= myGravStart - Math.Round(3 * myGravStart / 4) Then
                        myTermites(n).TermiteType = 0
                        n = n + 1
                    End If
                End If

                If TotalContig = True Then
                    For i = 0 To myDeptRowsColumns(0) - 1
                        For j = 0 To myDeptRowsColumns(1) - 1
                            If FacilityLayoutModel.IsTileAssigned(i, j) = False Then
                                adjTilesContainSameDepartment = contiguityTester.AdjacentTilesContainSameDepartment(myTermites(0).TileDept, i, j, myFacilityMatrix, myDeptSizes)

                                If adjTilesContainSameDepartment Then
                                    DropTile(i, j, 0)
                                    Exit For
                                End If
                            End If
                        Next
                        If adjTilesContainSameDepartment Then
                            Exit For
                        End If
                    Next
                End If

                For i = 0 To myDeptRowsColumns(0) - 1
                    For j = 0 To myDeptRowsColumns(1) - 1
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
            VDC = facilityEvaluator.VolumeDistanceCostProduct(myNumDepartments, myFacilityMatrix, myVolumeMatrix, myCostMatrix, myDeptSizes)
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