Imports System.Runtime.CompilerServices
Imports FaciltyLayout.Core
Imports FaciltyLayout.Core.Models

<Assembly: InternalsVisibleTo("FacilityLayout.Core.Tests")>
Public Class Form1
    Private myDataFile As String
    Friend myNumDepartments As Integer
    Private myDeptRowsColumns(1) As Integer
    Private myFixedDeptIndicator() As Boolean 'Is this a fixed department?
    Private myFixedDeptTileArray(,) As Boolean 'Does this space hold a tile belonging to a fixed department?
    Private myFixedDeptLocations(,) As Integer 'Where are the fixed tiles located on the facility field?
    Friend myDeptSizes() As Integer
    Friend myVolumeMatrix(,) As Integer
    Private myTransformedVolumeMatrix(,) As Integer
    Private myAverageFlow As Integer
    Friend myCostMatrix(,) As Double
    Private RandomRow As New Random
    Friend myFacilityMatrix(,) As Integer 'Matrix displaying the field of tiles and empty spaces
    Private myAssignedTiles(,) As Boolean 'Does this space have a tile in it?
    Private myTermiteOwnedTile(,) As Boolean 'Is a termite in the process of moving this tile?
    Friend myTermites() As Termites 'Agents for moving the tiles
    Private myNumTermites As Integer
    Private myFlows() As FlowStats
    Private Tile(,) As Windows.Forms.Label
    Private TileRefreshCounter As Integer = 0
    Private myTileColors(,) As Integer
    Private myVacancyCounter As Integer = 0
    Private myLoopCounter = 0
    Private myNumContigDeptCounter = 0
    Private myFrozenDepts() As Boolean
    Private myCountdFrznDepts() As Boolean
    Private myLoopPhase As Integer
    Private myNumFrozenDepts As Integer = 0
    Private myGravStart As Integer
    Private TransformVDC As Integer

    Private facilityEvaluator As New FacilityEvaluator()
    Private contiguityTester As New ContiguityTester()

    'Termite sets his tile down
    Private Sub DropTile(ByVal X As Integer, ByVal Y As Integer, ByVal TermiteNumber As Integer)
        Tile(X, Y).BackColor = Color.FromArgb(myTileColors(myTermites(TermiteNumber).TileDept, 0),
                                              myTileColors(myTermites(TermiteNumber).TileDept, 1),
                                              myTileColors(myTermites(TermiteNumber).TileDept, 2))
        myTermiteOwnedTile(X, Y) = False
        myFacilityMatrix(X, Y) = myTermites(TermiteNumber).TileDept
        myAssignedTiles(X, Y) = True
        Tile(X, Y).Text = myTermites(TermiteNumber).TileDept.ToString
        myTermites(TermiteNumber).HasTile = False
        myTermites(TermiteNumber).TileDept = Nothing
    End Sub

    Private Function FlowFinder()
        Dim i, j, k As Integer
        Dim FlowRelation(myNumDepartments) As FlowStats
        For i = 1 To myNumDepartments
            ReDim FlowRelation(i).Flows(myNumDepartments)
            FlowRelation(i).NumRelations = 0
            FlowRelation(i).FlowSum = 0
        Next

        For j = 1 To myNumDepartments
            For i = 1 To myNumDepartments
                If myTransformedVolumeMatrix(j, i) > 0 Then
                    FlowRelation(j).Flows(i) = FlowRelation(j).Flows(i) + myTransformedVolumeMatrix(j, i) / myCostMatrix(j, i)
                End If
                If myTransformedVolumeMatrix(i, j) > 0 Then
                    FlowRelation(j).Flows(i) = FlowRelation(j).Flows(i) + myTransformedVolumeMatrix(i, j) / myCostMatrix(i, j)
                End If
                If myTransformedVolumeMatrix(j, i) > 0 Or myTransformedVolumeMatrix(i, j) > 0 Then
                    FlowRelation(j).NumRelations = FlowRelation(j).NumRelations + 1
                End If
            Next
        Next
        For j = 1 To myNumDepartments
            ReDim FlowRelation(j).CondensedFlows(FlowRelation(j).NumRelations - 1)
            For i = 0 To FlowRelation(j).NumRelations - 1
                For k = 0 To myNumDepartments
                    If FlowRelation(j).Flows(k) <> 0 Then
                        FlowRelation(j).CondensedFlows(i) = FlowRelation(j).Flows(k)
                        i = i + 1
                    End If
                Next
            Next
            For i = 1 To myNumDepartments
                FlowRelation(j).FlowSum = FlowRelation(j).FlowSum + FlowRelation(j).Flows(i)
            Next
        Next

        Return FlowRelation
    End Function
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
    Private Function GenerateFacilitySwarm()
        'Generates rows and columns proportionally larger than the
        'acutal size of the facility to allow empty space for
        'the execution of the algorithm
        Dim Rows As Integer = Math.Round(Math.Sqrt(2 * (myDeptRowsColumns(0) ^ 2)))
        Dim Columns As Integer = Math.Round(Math.Sqrt(2 * (myDeptRowsColumns(1) ^ 2)))

        If TxtRows.Text <> "" Then
            Integer.TryParse(TxtRows.Text, Rows)
        End If
        If TxtColumns.Text <> "" Then
            Integer.TryParse(TxtColumns.Text, Columns)
        End If
        Dim FacilityMatrix(Rows - 1, Columns - 1) As Integer
        Dim TileAssigned(Rows - 1, Columns - 1) As Boolean
        Dim TileSelectRow As Integer
        Dim TileSelectColumn As Integer
        Dim i, j, k, m, FixedDept As Integer
        Dim DeptTilesPlaced As Integer
        FixedDept = 0

        'Initialize the matrix with all 0's, indicate all as unassigned
        For i = 0 To Rows - 1
            For j = 0 To Columns - 1
                FacilityMatrix(i, j) = 0
                TileAssigned(i, j) = False
            Next
        Next

        'Place fixed departments first and mark their tiles as taken
        For i = 0 To myNumDepartments - 1
            If myFixedDeptIndicator(i + 1) = True Then
                For k = myFixedDeptLocations(0, FixedDept) To myFixedDeptLocations(2, FixedDept)
                    For m = myFixedDeptLocations(1, FixedDept) To myFixedDeptLocations(3, FixedDept)
                        FacilityMatrix(k - 1, m - 1) = i + 1
                        TileAssigned(k - 1, m - 1) = True
                    Next
                Next
                FixedDept = FixedDept + 1
            End If
        Next

        'Starting at department 1, select a random tile and assign it to dept 1. If this process
        'would be done for a dept that is already fixed, skip that department
        For i = 0 To myNumDepartments - 1
            If myFixedDeptIndicator(i + 1) = True Then
                i = i + 1
                If myNumDepartments - 1 < i Then
                    Exit For
                End If
            End If
            DeptTilesPlaced = 0
            Do
                TileSelectRow = RandomRow.Next(0, Rows)
                TileSelectColumn = RandomRow.Next(0, Columns)
                If TileAssigned(TileSelectRow, TileSelectColumn) = True Then
                    Do
                        TileSelectRow = RandomRow.Next(0, Rows)
                        TileSelectColumn = RandomRow.Next(0, Columns)
                    Loop Until TileAssigned(TileSelectRow, TileSelectColumn) = False
                End If
                FacilityMatrix(TileSelectRow, TileSelectColumn) = i + 1
                DeptTilesPlaced = DeptTilesPlaced + 1
                TileAssigned(TileSelectRow, TileSelectColumn) = True
            Loop Until DeptTilesPlaced = myDeptSizes(i + 1)
        Next

        Dim temp(Rows - 1, Columns - 1) As Boolean
        myAssignedTiles = temp
        'ReDim myAssignedTiles(Rows - 1, Columns - 1)
        For i = 0 To Rows - 1
            For j = 0 To Columns - 1
                myAssignedTiles(i, j) = TileAssigned(i, j)
            Next
        Next

        ReDim myFixedDeptTileArray(Rows - 1, Columns - 1)
        For i = 0 To Rows - 1
            For j = 0 To Columns - 1
                If myFixedDeptIndicator(FacilityMatrix(i, j)) = True Then
                    myFixedDeptTileArray(i, j) = True
                Else
                    myFixedDeptTileArray(i, j) = False
                End If
            Next
        Next
        'Output is a matrix representing position of all tiles and empty spaces on the field
        Return FacilityMatrix
    End Function
    'Animates the facility field with a series of randomly colored labels
    Private Sub GenerateTileSwarmToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GenerateTileSwarmToolStripMenuItem.Click
        RandomizeTiles()
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
                                    If myAssignedTiles(myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) - ClosestEmpty(j, 0),
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
                                        myFacilityMatrix(X, Y) = myTermites(TermiteNumber).TileDept
                                        myAssignedTiles(X, Y) = True
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

        If myFixedDeptTileArray(myTermites(i).RowPos + myTermites(i).VertDirection,
                                 myTermites(i).ColumnPos + myTermites(i).HorizDirection) = True Then
            Do
                myTermites(i).HorizDirection = RandomRow.Next(0, upperlimit) - 2
                myTermites(i).VertDirection = RandomRow.Next(0, upperlimit) - 2
            Loop Until myTermites(i).ColumnPos + myTermites(i).HorizDirection > 0 AndAlso
                       myTermites(i).ColumnPos + myTermites(i).HorizDirection < columns AndAlso
                       myTermites(i).RowPos + myTermites(i).VertDirection > 0 AndAlso
                       myTermites(i).RowPos + myTermites(i).VertDirection < rows AndAlso
                       myFixedDeptTileArray(myTermites(i).RowPos + myTermites(i).VertDirection,
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
    Private Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        ofdLoadSetup.InitialDirectory = "C:/"
        ofdLoadSetup.Title = "Load Facility Data"
        ofdLoadSetup.FileName = ""
        ofdLoadSetup.Filter = "Text Files (*.txt)|*.txt"
        ofdLoadSetup.FilterIndex = 1

        Dim datafile As String
        If ofdLoadSetup.ShowDialog() = DialogResult.OK Then
            datafile = ofdLoadSetup.FileName
        End If

        Dim msg As String = ""
        myDataFile = datafile
        Dim objDataFile As New System.IO.StreamReader(myDataFile)
        Dim numDepartments As Integer
        Dim RowsColumns(1) As String
        Dim FacilityRows As Integer
        Dim FacilityColumns As Integer
        Dim DeptSplitter() As String
        Dim i, j, k As Integer

        msg = objDataFile.ReadLine()
        msg = objDataFile.ReadLine()
        msg = objDataFile.ReadLine()
        msg = msg.Replace("/", ",")
        DeptSplitter = msg.Split(",") 'the replace and split methods allow ignorance of txt file comments
        numDepartments = Integer.Parse(DeptSplitter(0))
        myNumDepartments = numDepartments
        msg = objDataFile.ReadLine
        msg = msg.Replace("/", ",")
        RowsColumns = msg.Split(",")
        FacilityRows = Integer.Parse(RowsColumns(0))
        FacilityColumns = Integer.Parse(RowsColumns(1))
        myDeptRowsColumns(0) = FacilityRows
        myDeptRowsColumns(1) = FacilityColumns

        Dim DeptSize(numDepartments) As Integer 'How big is the dept?
        Dim DeptFixed(numDepartments) As Boolean ' Is the Dept fixed in one location?
        Dim FixedDeptLocations(3, 0) As Integer 'If so, where?
        Dim DeptStats(5) As String
        j = 0
        DeptFixed(0) = False

        msg = objDataFile.ReadLine()

        For i = 0 To numDepartments - 1
            msg = objDataFile.ReadLine()
            DeptStats = msg.Split(",")
            DeptSize(i + 1) = Integer.Parse(DeptStats(0))
            DeptFixed(i + 1) = False
            If DeptStats(1) = 1 Then
                DeptFixed(i + 1) = True
                j = j + 1
                ReDim Preserve FixedDeptLocations(3, j - 1)
                For k = 0 To 3
                    FixedDeptLocations(k, j - 1) = Integer.Parse(DeptStats(k + 2))
                Next
            End If
        Next

        msg = objDataFile.ReadLine
        Dim VolumeMatrixSplitter(numDepartments - 1) As String
        Dim CostMatrixSplitter(numDepartments - 1) As String
        Dim VolumeMatrix(numDepartments, numDepartments) As Integer
        Dim CostMatrix(numDepartments, numDepartments) As Double

        For i = 1 To numDepartments
            msg = objDataFile.ReadLine()
            msg = msg.Replace("/", ",")
            VolumeMatrixSplitter = msg.Split(",")
            For k = 0 To numDepartments - 1
                VolumeMatrix(i, k + 1) = Integer.Parse(VolumeMatrixSplitter(k))
            Next
        Next


        msg = objDataFile.ReadLine
        For i = 0 To numDepartments
            CostMatrix(i, 0) = Integer.MaxValue
            CostMatrix(0, i) = Integer.MaxValue
        Next

        For i = 1 To numDepartments
            msg = objDataFile.ReadLine
            msg = msg.Replace("/", ",")
            CostMatrixSplitter = msg.Split(",")
            For k = 0 To numDepartments - 1
                CostMatrix(i, k + 1) = Double.Parse(CostMatrixSplitter(k))
            Next
        Next

        i = 1
        j = 1

        myDeptSizes = DeptSize
        myFixedDeptIndicator = DeptFixed
        myFixedDeptLocations = FixedDeptLocations
        myVolumeMatrix = VolumeMatrix
        myCostMatrix = CostMatrix

        ReDim myTransformedVolumeMatrix(myNumDepartments, myNumDepartments)
        For i = 1 To numDepartments
            For j = i To numDepartments
                myTransformedVolumeMatrix(i, j) = (VolumeMatrix(i, j) + VolumeMatrix(j, i)) / 2
                myTransformedVolumeMatrix(j, i) = myTransformedVolumeMatrix(i, j)
            Next
        Next

        Dim PopUp As String = "Basic Facility Stats" & vbCrLf
        PopUp = PopUp & "No. of Departments: " & numDepartments.ToString & vbCrLf &
            "No. of Rows: " & FacilityRows & vbCrLf & "No. of Columns: " & FacilityColumns & vbCrLf &
            "Department Sizes: " & vbCrLf

        For i = 0 To Math.Round(numDepartments / 2) - 1
            PopUp = PopUp & DeptSize(i + 1).ToString & ", "
        Next
        PopUp = PopUp & vbCrLf
        For i = Math.Round(numDepartments / 2) To numDepartments - 1
            PopUp = PopUp & DeptSize(i + 1).ToString & ", "
        Next
        PopUp = PopUp & vbCrLf & "Fixed Departments: "

        Dim x As Integer = 0

        For i = 0 To numDepartments
            If DeptFixed(i) = True Then
                PopUp = PopUp & x.ToString & ", "
            End If
            x = x + 1
        Next
        For j = 1 To myNumDepartments
            For i = 1 To myNumDepartments
                myTransformedVolumeMatrix(j, i) = (myVolumeMatrix(j, i) ^ 2) / ((myDeptSizes(j) + myDeptSizes(i)) / 2)
            Next
        Next
        ReDim myFlows(myNumDepartments)
        myFlows = FlowFinder()
        MessageBox.Show(PopUp)
    End Sub
    Private Sub RandomizeTiles()
        myFacilityMatrix = GenerateFacilitySwarm()

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
        PicVDP.Left = LblGravStart.Left

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
            If myAssignedTiles(x, columns - 1) = True Then
                myTermites(i).RowPos = x
                myTermites(i).ColumnPos = columns - 1
                myTermites(i).HasTile = True
                myTermiteOwnedTile(myTermites(i).RowPos, myTermites(i).ColumnPos) = True
                myTermites(i).TileDept = myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos)
                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).BackColor = Color.Black
                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Text = ""
                myAssignedTiles(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
                'Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Refresh()
                myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos) = 0
                i = i + 1
            End If
        Next

        For y = 0 To columns - 2
            If myAssignedTiles(rows - 1, y) = True Then
                myTermites(i).RowPos = rows - 1
                myTermites(i).ColumnPos = y
                myTermites(i).HasTile = True
                myTermiteOwnedTile(myTermites(i).RowPos, myTermites(i).ColumnPos) = True
                myTermites(i).TileDept = myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos)
                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).BackColor = Color.Black
                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Text = ""
                myAssignedTiles(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
                'Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Refresh()
                myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos) = 0
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

            If myFixedDeptTileArray(myTermites(i).RowPos, myTermites(i).ColumnPos) = True Then
                Do
                    myTermites(i).ColumnPos = RandomRow.Next(0, columns)
                    myTermites(i).RowPos = RandomRow.Next(0, rows)
                Loop Until myFixedDeptTileArray(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
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
                myAssignedTiles(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Refresh()
                myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos) = 0
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
            If myAssignedTiles(myTermites(i).RowPos, myTermites(i).ColumnPos) = True Then
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
                                    myAssignedTiles(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
                                    'Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Refresh()
                                    myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos) = 0
                                End If
                            ElseIf myTermites(i).SpecificTile = False Then
                                myTermites(i).HasTile = True
                                myTermiteOwnedTile(myTermites(i).RowPos, myTermites(i).ColumnPos) = True
                                myTermites(i).TileDept = myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos)
                                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).BackColor = Color.Black
                                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Text = ""
                                myAssignedTiles(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
                                'Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Refresh()
                                myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos) = 0
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
                    If myAssignedTiles(myTermites(i).RowPos, myTermites(i).ColumnPos) = False Then
                        DropTile(myTermites(i).RowPos, myTermites(i).ColumnPos, i)
                    Else
                        Do
                            NoLazinessAllowed(i)
                            MarchMarchMarch(i, Rows, Columns, 5)
                            myTermites(i).ColumnPos = myTermites(i).ColumnPos + myTermites(i).HorizDirection
                            myTermites(i).RowPos = myTermites(i).RowPos + myTermites(i).VertDirection
                        Loop Until myAssignedTiles(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
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
            If myAssignedTiles(myTermites(i).RowPos, myTermites(i).ColumnPos) = True Then
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
                                    myAssignedTiles(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
                                    myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos) = 0
                                End If
                            ElseIf myTermites(i).SpecificTile = False Then
                                myTermites(i).HasTile = True
                                myTermiteOwnedTile(myTermites(i).RowPos, myTermites(i).ColumnPos) = True
                                myTermites(i).TileDept = myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos)
                                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).BackColor = Color.Black
                                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Text = ""
                                myAssignedTiles(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
                                myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos) = 0
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
                        If myAssignedTiles(myTermites(i).RowPos, myTermites(i).ColumnPos) = False Then
                            DropTile(myTermites(i).RowPos, myTermites(i).ColumnPos, i)
                        Else
                            Do
                                NoLazinessAllowed(i)
                                MarchMarchMarch(i, rows, columns, upperlimit)
                                myTermites(i).ColumnPos = myTermites(i).ColumnPos + myTermites(i).HorizDirection
                                myTermites(i).RowPos = myTermites(i).RowPos + myTermites(i).VertDirection
                            Loop Until myAssignedTiles(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
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
                        If myAssignedTiles(myTermites(i).RowPos, myTermites(i).ColumnPos) = False Then
                            DropTile(myTermites(i).RowPos, myTermites(i).ColumnPos, i)
                        Else
                            Do
                                NoLazinessAllowed(i)
                                MarchMarchMarch(i, rows, columns, upperlimit)
                                myTermites(i).ColumnPos = myTermites(i).ColumnPos + myTermites(i).HorizDirection
                                myTermites(i).RowPos = myTermites(i).RowPos + myTermites(i).VertDirection
                            Loop Until myAssignedTiles(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
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
                    If myAssignedTiles(myTermites(TermiteNumber).RowPos - AdjCheck(i, 0), myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1)) = True Then
                        If rand <= 100 * (myFlows(myTermites(TermiteNumber).TileDept).Flows(myFacilityMatrix(myTermites(TermiteNumber).RowPos - AdjCheck(i, 0),
                                myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1)))) / (myFlows(myTermites(TermiteNumber).TileDept).FlowSum) Then
                            For j = 0 To 8
                                If 0 <= myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) - ClosestEmpty(j, 1) AndAlso
                                    myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) - ClosestEmpty(j, 1) <= rows - 1 Then
                                    If 0 <= myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) - ClosestEmpty(j, 0) AndAlso
                                        myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) - ClosestEmpty(j, 0) <= columns - 1 Then
                                        If myAssignedTiles(myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) - ClosestEmpty(j, 0),
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
                                            myFacilityMatrix(X, Y) = myTermites(TermiteNumber).TileDept
                                            myAssignedTiles(X, Y) = True
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

        Dim myPic As Bitmap
        myPic = New Bitmap(PicVDP.Width, PicVDP.Height)
        VDPGraph(myPic)
        PicVDP.Image = myPic
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
                RandomizeTiles()
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
                            If myAssignedTiles(i, j) = False Then
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
                        If myAssignedTiles(i, j) = False Then
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
            Dim g As Graphics = Graphics.FromImage(myPic)
            VDC = facilityEvaluator.VolumeDistanceCostProduct(myNumDepartments, myFacilityMatrix, myVolumeMatrix, myCostMatrix, myDeptSizes)
            TransformVDC = PicVDP.Height - (40 + Math.Round(240 * ((VDC - 30000) / 60000)))
            OBJValue = VDC.ToString & vbCrLf & RunTime.ToString
            g.FillRectangle(Brushes.Red, cycle * 30 + 30, TransformVDC - 2, 4, 4)
            PicVDP.Refresh()
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

    Private Sub VDPGraph(ByVal Pic As Bitmap)
        Dim i, k, n, yspace, tickcounter, ytickcounter As Integer
        Dim scaler, fontcounter As Double
        Dim fontobj As Font
        fontobj = New System.Drawing.Font("Arial", 8, FontStyle.Regular)
        Dim JobFont As Font
        JobFont = New System.Drawing.Font("Arial", 7, FontStyle.Regular)
        Dim HeadFont As Font
        HeadFont = New System.Drawing.Font("Arial", 12, FontStyle.Bold)
        yspace = 30
        tickcounter = 30
        ytickcounter = 40
        n = tickcounter
        k = ytickcounter
        'Set Scaling Constant
        scaler = 1

        'Define Picture Box Size
        'PicVDP.Left = LblGravStart.Left
        PicVDP.Visible = False

        'Create Drawing Surface
        For row As Integer = 0 To PicVDP.Width - 1
            For col As Integer = 0 To PicVDP.Height - 1
                Pic.SetPixel(row, col, Color.White)
            Next
        Next

        PicVDP.Image = Pic
        Dim g As Graphics = Graphics.FromImage(Pic)

        'Draw Background Lines
        For i = 0 To 9
            g.DrawLine(Pens.LightGray, yspace + n, ytickcounter - 15, yspace + n, PicVDP.Height - 43)
            n = n + tickcounter
        Next
        For i = 0 To 6
            g.DrawLine(Pens.LightGray, yspace + 3, PicVDP.Height - 40 - k, PicVDP.Width - 15, PicVDP.Height - 40 - k)
            k = k + ytickcounter
        Next
        n = tickcounter
        k = ytickcounter

        'Draw axes and ticks
        g.DrawLine(Pens.Black, yspace, 10, yspace, PicVDP.Height - 40) 'y axis
        g.DrawLine(Pens.Black, yspace, PicVDP.Height - 40, PicVDP.Width - 15, PicVDP.Height - 40) 'x axis

        For i = 0 To 9
            g.DrawLine(Pens.Black, yspace + n, PicVDP.Height - 37, yspace + n, PicVDP.Height - 43)
            n = n + tickcounter
        Next
        For i = 0 To 7
            g.DrawLine(Pens.Black, yspace - 3, PicVDP.Height - 40 - k, yspace + 3, PicVDP.Height - 40 - k)
            k = k + ytickcounter
        Next

        'Draw Time Units on ticks
        Dim fontstring As String
        fontcounter = 0
        n = 0

        For i = 0 To 10
            fontstring = ""
            fontstring = fontcounter.ToString(fontstring)
            g.DrawString(fontstring, fontobj, Brushes.Black, yspace + n - 3, PicVDP.Height - 35)
            fontcounter = fontcounter + 1
            n = n + tickcounter
        Next
        fontcounter = 30
        k = 0
        For i = 0 To 7
            fontstring = ""
            fontstring = fontcounter.ToString(fontstring) & "k"
            g.DrawString(fontstring, fontobj, Brushes.Black, yspace - 25, PicVDP.Height - 50 - k)
            fontcounter = fontcounter + 10
            k = k + ytickcounter
        Next
        g.DrawString("Cycle Number", fontobj, Brushes.Black, (PicVDP.Width / 2) - 65, PicVDP.Height - 15)

        'Draw Legend
        g.DrawString("VDP Scores", HeadFont, Brushes.Black, (PicVDP.Width / 2) - 60, 5)
        'Display Chart
        PicVDP.Visible = True
        g.Dispose()
    End Sub
End Class
