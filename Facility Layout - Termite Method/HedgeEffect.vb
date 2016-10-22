Public Class Form1
    Private myDataFile As String
    Private myNumDepartments As Integer
    Private myDeptRowsColumns(1) As Integer
    Private myFixedDeptIndicator() As Boolean 'Is this a fixed department?
    Private myFixedDeptTileArray(,) As Boolean 'Does this space hold a tile belonging to a fixed department?
    Private myFixedDeptLocations(,) As Integer 'Where are the fixed tiles located on the facility field?
    Private myDeptSizes() As Integer
    Private myVolumeMatrix(,) As Integer
    Private myCostMatrix(,) As Double
    Private RandomRow As New Random
    Private myFacilityMatrix(,) As Integer 'Matrix displaying the field of tiles and empty spaces
    Private myAssignedTiles(,) As Boolean 'Does this space have a tile in it?
    Private myTermiteOwnedTile(,) As Boolean 'Is a termite in the process of moving this tile?
    Private myTermites() As Termites 'Agents for moving the tiles
    'Private myTermiteHere(,) As Boolean
    Private myNumTermites As Integer
    Private Tile(,) As Windows.Forms.Label
    Private myTileColors(,) As Integer

    Private Structure Termites
        Dim VertDirection As Integer 'How far up/down should I go each turn?
        Dim HorizDirection As Integer 'How far left/right should I go each turn?
        Dim ColumnPos As Integer 'What column am I in?
        Dim RowPos As Integer 'What row am I in?
        Dim HasTile As Boolean 'Do I have a tile?
        Dim TileDept As Integer 'What kind of tile do I have?
    End Structure

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

        Dim DeptSize(numDepartments - 1) As Integer 'How big is the dept?
        Dim DeptFixed(numDepartments) As Boolean ' Is the Dept fixed in one location?
        Dim FixedDeptLocations(3, 0) As Integer 'If so, where?
        Dim DeptStats(5) As String
        j = 0
        DeptFixed(0) = False

        msg = objDataFile.ReadLine()

        For i = 0 To numDepartments - 1
            msg = objDataFile.ReadLine()
            DeptStats = msg.Split(",")
            DeptSize(i) = Integer.Parse(DeptStats(0))
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
        Dim VolumeMatrix(numDepartments - 1, numDepartments - 1) As Integer
        Dim CostMatrix(numDepartments - 1, numDepartments - 1) As Double

        For i = 0 To numDepartments - 1
            msg = objDataFile.ReadLine()
            msg = msg.Replace("/", ",")
            VolumeMatrixSplitter = msg.Split(",")
            For k = 0 To numDepartments - 1
                VolumeMatrix(i, k) = Integer.Parse(VolumeMatrixSplitter(k))
            Next
        Next

        msg = objDataFile.ReadLine

        For i = 0 To numDepartments - 1
            msg = objDataFile.ReadLine
            msg = msg.Replace("/", ",")
            CostMatrixSplitter = msg.Split(",")
            For k = 0 To numDepartments - 1
                CostMatrix(i, k) = Double.Parse(CostMatrixSplitter(k))
            Next
        Next

        myDeptSizes = DeptSize
        myFixedDeptIndicator = DeptFixed
        myFixedDeptLocations = FixedDeptLocations
        myVolumeMatrix = VolumeMatrix
        myCostMatrix = CostMatrix

        Dim PopUp As String = "Basic Facility Stats" & vbCrLf
        PopUp = PopUp & "No. of Departments: " & numDepartments.ToString() & vbCrLf & _
            "No. of Rows: " & FacilityRows & vbCrLf & "No. of Columns: " & FacilityColumns & vbCrLf & _
            "Department Sizes: " & vbCrLf

        For i = 0 To Math.Round(numDepartments / 2) - 1
            PopUp = PopUp & DeptSize(i).ToString & ", "
        Next
        PopUp = PopUp & vbCrLf
        For i = Math.Round(numDepartments / 2) To numDepartments - 1
            PopUp = PopUp & DeptSize(i).ToString & ", "
        Next
        PopUp = PopUp & vbCrLf & "Fixed Departments: "

        Dim x As Integer = 0

        For i = 0 To numDepartments
            If DeptFixed(i) = True Then
                PopUp = PopUp & x.ToString & ", "
            End If
            x = x + 1
        Next

        MessageBox.Show(PopUp)
    End Sub
    'Creates the facility field, places tiles randomly across the field
    Private Function GenerateFacilitySwarm()
        Dim Rows As Integer = Math.Round(Math.Sqrt(2 * (myDeptRowsColumns(0) ^ 2)))
        Dim Columns As Integer = Math.Round(Math.Sqrt(2 * (myDeptRowsColumns(1) ^ 2)))
        Dim FacilityMatrix(Rows - 1, Columns - 1) As Integer
        Dim TileAssigned(Math.Round(Math.Sqrt(2 * (myDeptRowsColumns(0) ^ 2))) - 1, Math.Round(Math.Sqrt(2 * (myDeptRowsColumns(1) ^ 2))) - 1) As Boolean
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
            Loop Until DeptTilesPlaced = myDeptSizes(i)
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
        myFacilityMatrix = GenerateFacilitySwarm()

        Dim objMatrixFile As New System.IO.StreamWriter("C:\Users\Andrew\Documents\IE 590\FacilitySwarm.txt")
        Dim strMatrix As String = Nothing
        Dim i, j As Integer
        For i = 0 To Math.Round(Math.Sqrt(2 * (myDeptRowsColumns(0) ^ 2))) - 1
            For j = 0 To Math.Round(Math.Sqrt(2 * (myDeptRowsColumns(1) ^ 2))) - 1
                If myFacilityMatrix(i, j).ToString.Length = 1 Then
                    strMatrix = strMatrix & myFacilityMatrix(i, j).ToString() & "  "
                Else
                    strMatrix = strMatrix & myFacilityMatrix(i, j).ToString() & " "
                End If
            Next j
            strMatrix = strMatrix & vbCrLf
        Next i
        objMatrixFile.Write(strMatrix)
        objMatrixFile.Close()

        Dim color As New Color
        Dim Rows As Integer = Math.Round(Math.Sqrt(2 * (myDeptRowsColumns(0) ^ 2)))
        Dim Columns As Integer = Math.Round(Math.Sqrt(2 * (myDeptRowsColumns(1) ^ 2)))

        Me.Height = Rows * 30
        Me.Width = Columns * 28

        Dim RowCounter As Integer = 10
        Dim ColumnCounter As Integer = 30
        ReDim Tile(Rows - 1, Columns - 1)
        ReDim myTileColors(myNumDepartments, 2)
        Dim ColorRangeCount As Integer = 100
        For j = 0 To 2
            myTileColors(0, j) = 255
        Next

        For i = 0 To myNumDepartments - 1
            For j = 0 To 2
                myTileColors(i + 1, j) = (myNumDepartments / 2) * RandomRow.Next(10, 255 / (myNumDepartments / 2))

            Next
            ColorRangeCount = ColorRangeCount + 150 / myNumDepartments
        Next

        For i = 0 To Rows - 1
            For j = 0 To Columns - 1
                Tile(i, j) = New Windows.Forms.Label
                Tile(i, j).Width = 20
                Tile(i, j).Height = 20
                Tile(i, j).Location = New Point(RowCounter, ColumnCounter)
                Tile(i, j).Visible = True
                Me.Controls.Add(Tile(i, j))
                RowCounter = RowCounter + 25
                Tile(i, j).BackColor = color.FromArgb(myTileColors(myFacilityMatrix(i, j), 0), myTileColors(myFacilityMatrix(i, j), 1), _
                                                      myTileColors(myFacilityMatrix(i, j), 2))
                Tile(i, j).Text = myFacilityMatrix(i, j).ToString
            Next
            RowCounter = 10
            ColumnCounter = ColumnCounter + 25
        Next
    End Sub
    'Sets Initial Position an Direction of Termites
    Private Sub ReleaseTheTermites()
        Dim NumTermites = Math.Round(myDeptRowsColumns(0) * myDeptRowsColumns(1) * 1.5)
        myNumTermites = NumTermites
        ReDim myTermites(NumTermites - 1)
        Dim Rows As Integer = Math.Round(Math.Sqrt(2 * myDeptRowsColumns(0) ^ 2))
        Dim Columns As Integer = Math.Round(Math.Sqrt(2 * myDeptRowsColumns(1) ^ 2))
        Dim HorizDirectionCounter As Integer = 0
        Dim VertDirectionCounter As Integer = 0
        'ReDim myTermiteHere(Rows - 1, Columns - 1)
        ReDim myTermiteOwnedTile(Rows - 1, Columns - 1)

        For i = 0 To NumTermites - 1
            myTermites(i).HasTile = False
            myTermites(i).ColumnPos = RandomRow.Next(0, Columns)
            myTermites(i).RowPos = RandomRow.Next(0, Rows)
            If myFixedDeptTileArray(myTermites(i).RowPos, myTermites(i).ColumnPos) = True Then
                'myTermiteHere(myTermites(i).RowPos, myTermites(i).ColumnPos) = True Then
                Do
                    myTermites(i).ColumnPos = RandomRow.Next(0, Columns)
                    myTermites(i).RowPos = RandomRow.Next(0, Rows)
                Loop Until myFixedDeptTileArray(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
                'myTermiteHere(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
            End If
            'myTermiteHere(myTermites(i).RowPos, myTermites(i).ColumnPos) = True
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
                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).BackColor = Color.White
                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Text = "0"
                myAssignedTiles(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
                Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Refresh()
                myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos) = 0
            End If
        Next

    
    End Sub
    'Redirects termite if fixed dept or wall is encountered
    Private Sub MarchMarchMarch(ByVal i As Integer)
        Dim Rows As Integer = Math.Round(Math.Sqrt(2 * (myDeptRowsColumns(0) ^ 2)))
        Dim Columns As Integer = Math.Round(Math.Sqrt(2 * (myDeptRowsColumns(1) ^ 2)))

        'Say what the termites should do in the event they encounter a wall
            If myTermites(i).ColumnPos + myTermites(i).HorizDirection < 0 Or _
                myTermites(i).ColumnPos + myTermites(i).HorizDirection >= Columns Or _
                myTermites(i).RowPos + myTermites(i).VertDirection < 0 Or _
                myTermites(i).RowPos + myTermites(i).VertDirection >= Rows Then
                Do
                myTermites(i).HorizDirection = RandomRow.Next(0, 5) - 2
                myTermites(i).VertDirection = RandomRow.Next(0, 5) - 2
                Loop Until myTermites(i).ColumnPos + myTermites(i).HorizDirection > 0 AndAlso _
                           myTermites(i).ColumnPos + myTermites(i).HorizDirection < Columns AndAlso _
                           myTermites(i).RowPos + myTermites(i).VertDirection > 0 AndAlso _
                           myTermites(i).RowPos + myTermites(i).VertDirection < Rows
            End If

            If myFixedDeptTileArray(myTermites(i).RowPos + myTermites(i).VertDirection, _
                                     myTermites(i).ColumnPos + myTermites(i).HorizDirection) = True Then
                Do
                myTermites(i).HorizDirection = RandomRow.Next(0, 5) - 2
                myTermites(i).VertDirection = RandomRow.Next(0, 5) - 2
                Loop Until myTermites(i).ColumnPos + myTermites(i).HorizDirection > 0 AndAlso _
                           myTermites(i).ColumnPos + myTermites(i).HorizDirection < Columns AndAlso _
                           myTermites(i).RowPos + myTermites(i).VertDirection > 0 AndAlso _
                           myTermites(i).RowPos + myTermites(i).VertDirection < Rows AndAlso _
                           myFixedDeptTileArray(myTermites(i).RowPos + myTermites(i).VertDirection, _
                                     myTermites(i).ColumnPos + myTermites(i).HorizDirection) = False
            End If
    End Sub
    'Termite sets his tile down
    Private Sub DropTile(ByVal X As Integer, ByVal Y As Integer, ByVal TermiteNumber As Integer)
        Tile(X, Y).BackColor = Color.FromArgb(myTileColors(myTermites(TermiteNumber).TileDept, 0), _
                                                                 myTileColors(myTermites(TermiteNumber).TileDept, 1), _
                                                                 myTileColors(myTermites(TermiteNumber).TileDept, 2))
        myTermiteOwnedTile(X, Y) = False
        myFacilityMatrix(X, Y) = myTermites(TermiteNumber).TileDept
        myAssignedTiles(X, Y) = True
        Tile(X, Y).Text = myTermites(TermiteNumber).TileDept.ToString
        Tile(X, Y).Refresh()
        myTermites(TermiteNumber).HasTile = False
        myTermites(TermiteNumber).TileDept = Nothing
    End Sub
    'Termites may not have a horiz,vert direction of 0,0
    Private Sub NoLazinessAllowed(ByVal i As Integer)
        If myTermites(i).HorizDirection = 0 AndAlso myTermites(i).VertDirection = 0 Then
            Do
                myTermites(i).HorizDirection = RandomRow.Next(0, 5) - 2
                myTermites(i).VertDirection = RandomRow.Next(0, 5) - 2
            Loop Until myTermites(i).HorizDirection <> 0 Or myTermites(i).VertDirection <> 0
        End If
    End Sub
    'Termites begin to move, collect, and drop tiles
    Private Sub ReorganizationMethod1()
        Dim Rows As Integer = Math.Round(Math.Sqrt(2 * (myDeptRowsColumns(0) ^ 2)))
        Dim Columns As Integer = Math.Round(Math.Sqrt(2 * (myDeptRowsColumns(1) ^ 2)))
        Dim i As Integer
        Dim counter As Integer = 0
        Dim SimilarAdjTileCount As Integer


        For i = 0 To myNumTermites - 1
            NoLazinessAllowed(i)
            MarchMarchMarch(i)
            myTermites(i).ColumnPos = myTermites(i).ColumnPos + myTermites(i).HorizDirection
            myTermites(i).RowPos = myTermites(i).RowPos + myTermites(i).VertDirection

            'If a termite didn't have a tile before but is now on a space with an un-owned tile, pick it up
            If myAssignedTiles(myTermites(i).RowPos, myTermites(i).ColumnPos) = True Then
                If myTermiteOwnedTile(myTermites(i).RowPos, myTermites(i).ColumnPos) = False Then
                    If myTermites(i).HasTile = False Then
                        SimilarAdjTileCount = SimilarTileCounter(i)
                        If RandomRow.Next(0, 9) + SimilarAdjTileCount <= 3 Then
                            myTermites(i).HasTile = True
                            myTermiteOwnedTile(myTermites(i).RowPos, myTermites(i).ColumnPos) = True
                            myTermites(i).TileDept = myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos)
                            Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).BackColor = Color.White
                            Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Text = "0"
                            myAssignedTiles(myTermites(i).RowPos, myTermites(i).ColumnPos) = False
                            Tile(myTermites(i).RowPos, myTermites(i).ColumnPos).Refresh()
                            myFacilityMatrix(myTermites(i).RowPos, myTermites(i).ColumnPos) = 0
                        End If
                    End If
                End If
            End If

            'Check to see if any adjacent tile is equivalent to whatever tile termite has
            'If check passes, find nearest empty space and set tile down
            'If check fails, continue moving, loop process until check passes
            Do
                If myTermites(i).HasTile = True Then
                    GreedyTermite(i)
                    If myTermites(i).HasTile = False Then
                        Exit Do
                    End If
                End If
                NoLazinessAllowed(i)
                MarchMarchMarch(i)
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
                            MarchMarchMarch(i)
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
    'Termite drops tile in nearest empty space if an equivalent tile is located in an adjacent space
    Private Sub GreedyTermite(ByVal TermiteNumber As Integer)
        Dim i, j, a, b As Integer
        Dim Rows As Integer = Math.Round(Math.Sqrt(2 * myDeptRowsColumns(0) ^ 2))
        Dim Columns As Integer = Math.Round(Math.Sqrt(2 * myDeptRowsColumns(1) ^ 2))
        Dim ClosestFound As Boolean = False
        Dim ClosestEmpty(,) As Integer = LookAround()
        Dim AdjCheck(,) As Integer = LookAround()
        'Check adjacent spaces for equivalent tiles
       
        For i = 0 To 8
            'Termite must not look outside of facility field boundaries
            If 0 <= myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) AndAlso myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) <= Rows - 1 Then
                If 0 <= myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) AndAlso myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) <= Columns - 1 Then
                    If myFacilityMatrix(myTermites(TermiteNumber).RowPos - AdjCheck(i, 0), myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1)) = _
                             myTermites(TermiteNumber).TileDept Then
                        For j = 0 To 8
                            If 0 <= myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) - ClosestEmpty(j, 1) AndAlso _
                                myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) - ClosestEmpty(j, 1) <= Rows - 1 Then
                                If 0 <= myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) - ClosestEmpty(j, 0) AndAlso _
                                    myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) - ClosestEmpty(j, 0) <= Columns - 1 Then
                                    If myAssignedTiles(myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) - ClosestEmpty(j, 0), _
                                                       myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) - ClosestEmpty(j, 1)) = False Then
                                        'TileDropLocation = NearestEmptyTile(myTermites(TermiteNumber).RowPos - i, myTermites(TermiteNumber).ColumnPos - j)
                                        ClosestFound = True
                                        Dim X, Y As Integer
                                        X = myTermites(TermiteNumber).RowPos - AdjCheck(i, 0) - ClosestEmpty(j, 0)
                                        Y = myTermites(TermiteNumber).ColumnPos - AdjCheck(i, 1) - ClosestEmpty(j, 1)
                                        Tile(X, Y).BackColor = Color.FromArgb(myTileColors(myTermites(TermiteNumber).TileDept, 0), _
                                        myTileColors(myTermites(TermiteNumber).TileDept, 1), _
                                        myTileColors(myTermites(TermiteNumber).TileDept, 2))
                                        myTermites(TermiteNumber).ColumnPos = Y
                                        myTermites(TermiteNumber).RowPos = X
                                        myTermiteOwnedTile(X, Y) = False
                                        myFacilityMatrix(X, Y) = myTermites(TermiteNumber).TileDept
                                        myAssignedTiles(X, Y) = True
                                        Tile(X, Y).Text = myTermites(TermiteNumber).TileDept.ToString
                                        Tile(X, Y).Refresh()
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

    Private Function LookAround()
        Dim i, j, a As Integer
        Dim Checked(2, 2) As Boolean
        Dim SearchOrder(8, 1) As Integer

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
        Return SearchOrder

    End Function
    Private Function SimilarTileCounter(ByVal TermiteNumber As Integer)
        Dim i, j As Integer
        Dim NumberSimilarTilesAdj As Integer = 0
        Dim Rows As Integer = Math.Round(Math.Sqrt(2 * myDeptRowsColumns(0) ^ 2))
        Dim Columns As Integer = Math.Round(Math.Sqrt(2 * myDeptRowsColumns(1) ^ 2))

        For i = -1 To 1
            For j = -1 To 1
                If i = 0 AndAlso j = 0 Then
                    j = 1
                End If
                If 0 <= myTermites(TermiteNumber).ColumnPos - j AndAlso myTermites(TermiteNumber).ColumnPos - j <= Rows - 1 Then
                    If 0 <= myTermites(TermiteNumber).RowPos - i AndAlso myTermites(TermiteNumber).RowPos - i <= Columns - 1 Then
                        If myFacilityMatrix(myTermites(TermiteNumber).RowPos - i, myTermites(TermiteNumber).ColumnPos - j) = _
                            myTermites(TermiteNumber).TileDept Then
                            NumberSimilarTilesAdj = NumberSimilarTilesAdj + 1
                        End If
                    End If
                End If
            Next
        Next
        Return NumberSimilarTilesAdj
    End Function
    Private Sub GreedyTermiteMethodToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GreedyTermiteMethodToolStripMenuItem.Click
        ReleaseTheTermites()
        Dim k As Integer = 0
        Do
            ReorganizationMethod1()
            k = k + 1
        Loop Until k = 5000
    End Sub


End Class
