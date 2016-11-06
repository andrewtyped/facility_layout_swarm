Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports FaciltyLayout.Core
Imports FaciltyLayout.Core.Models

<Assembly: InternalsVisibleTo("FacilityLayout.Core.Tests")>
Public Class Form1
    Private RandomRow As New Random
    Friend Tile(,) As Windows.Forms.Label
    Private TileRefreshCounter As Integer = 0
    Private myTileColors(,) As Integer
    Friend FacilityStats As FacilityStats
    Friend WithEvents TileOrganizer As TileOrganizer

    'Animates the facility field with a series of randomly colored labels
    Private Sub GenerateTileSwarmToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GenerateTileSwarmToolStripMenuItem.Click

        'RandomizeTiles(FacilityStats)
    End Sub

    Private Sub GreedyTermiteMethodToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GreedyTermiteMethodToolStripMenuItem.Click
        Dim NumTermites As Integer? = Nothing
        Dim Phase1Decay As Integer? = Nothing
        Dim Phase2Decay As Integer? = Nothing
        Dim gravStart As Integer? = Nothing
        Dim numcycles As Integer? = Nothing
        Dim ratioOfGreedyTermitesToScholarTermites As Integer? = Nothing
        Dim uiUpdateFrequency = 20

        If TxtTermites.Text <> "" Then
            Integer.TryParse(TxtTermites.Text, NumTermites)
        End If
        If TxtRatio.Text <> "" Then
            Integer.TryParse(TxtRatio.ToString, ratioOfGreedyTermitesToScholarTermites)
        End If
        If TxtReps.Text <> "" Then
            Integer.TryParse(TxtReps.Text, numcycles)
        End If

        Dim TileOrganizerOptions = New TileOrganizerOptions(NumTermites, Phase1Decay, Phase2Decay, numcycles, gravStart, ratioOfGreedyTermitesToScholarTermites, uiUpdateFrequency)
        TileOrganizer = New TileOrganizer(TileOrganizerOptions)

        For Each solution As FacilityLayoutSolution In TileOrganizer.GreedyMethod(FacilityStats)
            Dim OBJValue = solution.VolumeDistanceCostProduct.ToString & vbCrLf & solution.RunTime.ToString
            MessageBox.Show(OBJValue)
        Next
    End Sub

    Private Sub Form1_TermiteRemovedTile(sender As Object, e As TileEventArgs) Handles TileOrganizer.TileRemoved
        Tile(e.Position.Row, e.Position.Column).BackColor = Color.Black
        Tile(e.Position.Row, e.Position.Column).Text = ""
    End Sub

    Private Sub Form1_TilePlaced(sender As Object, e As TileEventArgs) Handles TileOrganizer.TilePlaced
        Tile(e.Position.Row, e.Position.Column).BackColor = Color.FromArgb(myTileColors(e.DepartmentId, 0),
                                                myTileColors(e.DepartmentId, 1),
                                                myTileColors(e.DepartmentId, 2))
        Tile(e.Position.Row, e.Position.Column).Text = e.DepartmentId.ToString
    End Sub

    Private Sub Form1_OrganizerMileStoneReached(sender As Object, e As GridEventArgs) Handles TileOrganizer.OrganizerMilestoneReached
        For i = 0 To e.GridSize.Rows - 1
            For j = 0 To e.GridSize.Columns - 1
                Tile(i, j).Refresh()
            Next
        Next
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

    Private Sub Form1_FacilityInitialized(sender As Object, e As FacilityInitializedEventArgs) Handles TileOrganizer.FacilityInitialized

        Dim Rows As Integer = e.LayoutArea.Rows
        Dim Columns As Integer = e.LayoutArea.Columns
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
            ReDim myTileColors(FacilityStats.DepartmentCount, 2)
        End If
        Dim ColorRangeCount As Integer = 100
        For j = 0 To 2
            myTileColors(0, j) = 0
        Next

        For i = 0 To e.DepartmentCount - 1
            For j = 0 To 2
                myTileColors(i + 1, j) = (e.DepartmentCount / 2) * RandomRow.Next(10, 255 / (e.DepartmentCount / 2))

            Next
            ColorRangeCount = ColorRangeCount + 150 / e.DepartmentCount
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

                Dim department = e.Facility(i, j)
                Tile(i, j).BackColor = Color.FromArgb(myTileColors(department, 0), myTileColors(department, 1),
                                                      myTileColors(department, 2))
                Tile(i, j).Text = department.ToString
                Tile(i, j).Refresh()
            Next
            RowCounter = 10
            ColumnCounter = ColumnCounter + 25
        Next
        TileRefreshCounter = 1
    End Sub

    Private Sub ScholarTermiteMethodToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ScholarTermiteMethodToolStripMenuItem.Click
        Dim NumTermites As Integer? = Nothing
        Dim Phase1Decay As Integer? = Nothing
        Dim Phase2Decay As Integer? = Nothing
        Dim gravStart As Integer? = Nothing
        Dim numcycles As Integer? = Nothing
        Dim ratioOfGreedyTermitesToScholarTermites As Integer? = Nothing
        Dim uiUpdateFrequency = 20

        If TxtTermites.Text <> "" Then
            Integer.TryParse(TxtTermites.Text, NumTermites)
        End If
        If TxtPhase1Decay.Text <> "" Then
            Integer.TryParse(TxtPhase1Decay.Text, Phase1Decay)
        End If
        If TxtPhase2Decay.Text <> "" Then
            Integer.TryParse(TxtPhase2Decay.Text, Phase2Decay)
        End If
        If TxtGravStart.Text <> "" Then
            Integer.TryParse(TxtGravStart.Text, gravStart)
        End If
        If TxtRatio.Text <> "" Then
            Integer.TryParse(TxtRatio.ToString, ratioOfGreedyTermitesToScholarTermites)
        End If
        If TxtReps.Text <> "" Then
            Integer.TryParse(TxtReps.Text, numcycles)
        End If

        Dim TileOrganizerOptions = New TileOrganizerOptions(NumTermites, Phase1Decay, Phase2Decay, numcycles, gravStart, ratioOfGreedyTermitesToScholarTermites, uiUpdateFrequency)
        TileOrganizer = New TileOrganizer(TileOrganizerOptions)

        For Each solution As FacilityLayoutSolution In TileOrganizer.ScholarMethod(FacilityStats)
            Dim OBJValue = solution.VolumeDistanceCostProduct.ToString & vbCrLf & solution.RunTime.ToString
            MessageBox.Show(OBJValue)
        Next
    End Sub
End Class