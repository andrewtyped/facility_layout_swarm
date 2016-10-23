<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Public Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SolveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GenerateTileSwarmToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportTileSwarmToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GreedyTermiteMethodToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ScholarTermiteMethodToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ofdLoadSetup = New System.Windows.Forms.OpenFileDialog()
        Me.TxtRows = New System.Windows.Forms.TextBox()
        Me.LblRows = New System.Windows.Forms.Label()
        Me.TxtColumns = New System.Windows.Forms.TextBox()
        Me.LblColumns = New System.Windows.Forms.Label()
        Me.TxtRatio = New System.Windows.Forms.TextBox()
        Me.LblRatio = New System.Windows.Forms.Label()
        Me.TxtTermites = New System.Windows.Forms.TextBox()
        Me.LblNumTermites = New System.Windows.Forms.Label()
        Me.TxtPhase1Decay = New System.Windows.Forms.TextBox()
        Me.TxtPhase2Decay = New System.Windows.Forms.TextBox()
        Me.LblPhase1Decay = New System.Windows.Forms.Label()
        Me.LblPhase2Decay = New System.Windows.Forms.Label()
        Me.TxtGravStart = New System.Windows.Forms.TextBox()
        Me.TxtReps = New System.Windows.Forms.TextBox()
        Me.LblGravStart = New System.Windows.Forms.Label()
        Me.LblReps = New System.Windows.Forms.Label()
        Me.PicVDP = New System.Windows.Forms.PictureBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.MenuStrip1.SuspendLayout()
        CType(Me.PicVDP, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.SolveToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(484, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenToolStripMenuItem, Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'OpenToolStripMenuItem
        '
        Me.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
        Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(103, 22)
        Me.OpenToolStripMenuItem.Text = "Open"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(103, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'SolveToolStripMenuItem
        '
        Me.SolveToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GenerateTileSwarmToolStripMenuItem, Me.ExportTileSwarmToolStripMenuItem, Me.GreedyTermiteMethodToolStripMenuItem, Me.ScholarTermiteMethodToolStripMenuItem})
        Me.SolveToolStripMenuItem.Name = "SolveToolStripMenuItem"
        Me.SolveToolStripMenuItem.Size = New System.Drawing.Size(47, 20)
        Me.SolveToolStripMenuItem.Text = "Solve"
        '
        'GenerateTileSwarmToolStripMenuItem
        '
        Me.GenerateTileSwarmToolStripMenuItem.Name = "GenerateTileSwarmToolStripMenuItem"
        Me.GenerateTileSwarmToolStripMenuItem.Size = New System.Drawing.Size(202, 22)
        Me.GenerateTileSwarmToolStripMenuItem.Text = "Generate Tile Swarm"
        '
        'ExportTileSwarmToolStripMenuItem
        '
        Me.ExportTileSwarmToolStripMenuItem.Name = "ExportTileSwarmToolStripMenuItem"
        Me.ExportTileSwarmToolStripMenuItem.Size = New System.Drawing.Size(202, 22)
        Me.ExportTileSwarmToolStripMenuItem.Text = "Export Tile Swarm"
        '
        'GreedyTermiteMethodToolStripMenuItem
        '
        Me.GreedyTermiteMethodToolStripMenuItem.Name = "GreedyTermiteMethodToolStripMenuItem"
        Me.GreedyTermiteMethodToolStripMenuItem.Size = New System.Drawing.Size(202, 22)
        Me.GreedyTermiteMethodToolStripMenuItem.Text = "Greedy Termite Method"
        '
        'ScholarTermiteMethodToolStripMenuItem
        '
        Me.ScholarTermiteMethodToolStripMenuItem.Name = "ScholarTermiteMethodToolStripMenuItem"
        Me.ScholarTermiteMethodToolStripMenuItem.Size = New System.Drawing.Size(202, 22)
        Me.ScholarTermiteMethodToolStripMenuItem.Text = "Scholar Termite Method"
        '
        'ofdLoadSetup
        '
        Me.ofdLoadSetup.FileName = "OpenFileDialog1"
        '
        'TxtRows
        '
        Me.TxtRows.Location = New System.Drawing.Point(413, 40)
        Me.TxtRows.Name = "TxtRows"
        Me.TxtRows.Size = New System.Drawing.Size(59, 20)
        Me.TxtRows.TabIndex = 1
        '
        'LblRows
        '
        Me.LblRows.AutoSize = True
        Me.LblRows.Location = New System.Drawing.Point(346, 40)
        Me.LblRows.Name = "LblRows"
        Me.LblRows.Size = New System.Drawing.Size(61, 13)
        Me.LblRows.TabIndex = 2
        Me.LblRows.Text = "Total Rows"
        '
        'TxtColumns
        '
        Me.TxtColumns.Location = New System.Drawing.Point(222, 40)
        Me.TxtColumns.Name = "TxtColumns"
        Me.TxtColumns.Size = New System.Drawing.Size(59, 20)
        Me.TxtColumns.TabIndex = 3
        '
        'LblColumns
        '
        Me.LblColumns.AutoSize = True
        Me.LblColumns.Location = New System.Drawing.Point(142, 40)
        Me.LblColumns.Name = "LblColumns"
        Me.LblColumns.Size = New System.Drawing.Size(74, 13)
        Me.LblColumns.TabIndex = 4
        Me.LblColumns.Text = "Total Columns"
        '
        'TxtRatio
        '
        Me.TxtRatio.Location = New System.Drawing.Point(413, 66)
        Me.TxtRatio.Name = "TxtRatio"
        Me.TxtRatio.Size = New System.Drawing.Size(59, 20)
        Me.TxtRatio.TabIndex = 5
        '
        'LblRatio
        '
        Me.LblRatio.AutoSize = True
        Me.LblRatio.Location = New System.Drawing.Point(297, 69)
        Me.LblRatio.Name = "LblRatio"
        Me.LblRatio.Size = New System.Drawing.Size(110, 13)
        Me.LblRatio.TabIndex = 6
        Me.LblRatio.Text = "Greedy/Scholar Ratio"
        '
        'TxtTermites
        '
        Me.TxtTermites.Location = New System.Drawing.Point(222, 66)
        Me.TxtTermites.Name = "TxtTermites"
        Me.TxtTermites.Size = New System.Drawing.Size(59, 20)
        Me.TxtTermites.TabIndex = 7
        '
        'LblNumTermites
        '
        Me.LblNumTermites.AutoSize = True
        Me.LblNumTermites.Location = New System.Drawing.Point(129, 69)
        Me.LblNumTermites.Name = "LblNumTermites"
        Me.LblNumTermites.Size = New System.Drawing.Size(87, 13)
        Me.LblNumTermites.TabIndex = 8
        Me.LblNumTermites.Text = "Number Termites"
        '
        'TxtPhase1Decay
        '
        Me.TxtPhase1Decay.Location = New System.Drawing.Point(222, 92)
        Me.TxtPhase1Decay.Name = "TxtPhase1Decay"
        Me.TxtPhase1Decay.Size = New System.Drawing.Size(59, 20)
        Me.TxtPhase1Decay.TabIndex = 9
        '
        'TxtPhase2Decay
        '
        Me.TxtPhase2Decay.Location = New System.Drawing.Point(413, 92)
        Me.TxtPhase2Decay.Name = "TxtPhase2Decay"
        Me.TxtPhase2Decay.Size = New System.Drawing.Size(59, 20)
        Me.TxtPhase2Decay.TabIndex = 10
        '
        'LblPhase1Decay
        '
        Me.LblPhase1Decay.AutoSize = True
        Me.LblPhase1Decay.Location = New System.Drawing.Point(133, 95)
        Me.LblPhase1Decay.Name = "LblPhase1Decay"
        Me.LblPhase1Decay.Size = New System.Drawing.Size(80, 13)
        Me.LblPhase1Decay.TabIndex = 11
        Me.LblPhase1Decay.Text = "Phase 1 Decay"
        '
        'LblPhase2Decay
        '
        Me.LblPhase2Decay.AutoSize = True
        Me.LblPhase2Decay.Location = New System.Drawing.Point(319, 95)
        Me.LblPhase2Decay.Name = "LblPhase2Decay"
        Me.LblPhase2Decay.Size = New System.Drawing.Size(80, 13)
        Me.LblPhase2Decay.TabIndex = 12
        Me.LblPhase2Decay.Text = "Phase 2 Decay"
        '
        'TxtGravStart
        '
        Me.TxtGravStart.Location = New System.Drawing.Point(222, 118)
        Me.TxtGravStart.Name = "TxtGravStart"
        Me.TxtGravStart.Size = New System.Drawing.Size(59, 20)
        Me.TxtGravStart.TabIndex = 13
        '
        'TxtReps
        '
        Me.TxtReps.Location = New System.Drawing.Point(413, 118)
        Me.TxtReps.Name = "TxtReps"
        Me.TxtReps.Size = New System.Drawing.Size(59, 20)
        Me.TxtReps.TabIndex = 14
        '
        'LblGravStart
        '
        Me.LblGravStart.AutoSize = True
        Me.LblGravStart.Location = New System.Drawing.Point(106, 121)
        Me.LblGravStart.Name = "LblGravStart"
        Me.LblGravStart.Size = New System.Drawing.Size(110, 13)
        Me.LblGravStart.TabIndex = 15
        Me.LblGravStart.Text = "Gravitation Start Point"
        '
        'LblReps
        '
        Me.LblReps.AutoSize = True
        Me.LblReps.Location = New System.Drawing.Point(350, 121)
        Me.LblReps.Name = "LblReps"
        Me.LblReps.Size = New System.Drawing.Size(57, 13)
        Me.LblReps.TabIndex = 16
        Me.LblReps.Text = "Replicates"
        '
        'PicVDP
        '
        Me.PicVDP.Location = New System.Drawing.Point(109, 138)
        Me.PicVDP.Name = "PicVDP"
        Me.PicVDP.Size = New System.Drawing.Size(363, 312)
        Me.PicVDP.TabIndex = 17
        Me.PicVDP.TabStop = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(484, 462)
        Me.Controls.Add(Me.PicVDP)
        Me.Controls.Add(Me.LblReps)
        Me.Controls.Add(Me.LblGravStart)
        Me.Controls.Add(Me.TxtReps)
        Me.Controls.Add(Me.TxtGravStart)
        Me.Controls.Add(Me.LblPhase2Decay)
        Me.Controls.Add(Me.LblPhase1Decay)
        Me.Controls.Add(Me.TxtPhase2Decay)
        Me.Controls.Add(Me.TxtPhase1Decay)
        Me.Controls.Add(Me.LblNumTermites)
        Me.Controls.Add(Me.TxtTermites)
        Me.Controls.Add(Me.LblRatio)
        Me.Controls.Add(Me.TxtRatio)
        Me.Controls.Add(Me.LblColumns)
        Me.Controls.Add(Me.TxtColumns)
        Me.Controls.Add(Me.LblRows)
        Me.Controls.Add(Me.TxtRows)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        CType(Me.PicVDP, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SolveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ofdLoadSetup As System.Windows.Forms.OpenFileDialog
    Friend WithEvents GenerateTileSwarmToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExportTileSwarmToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GreedyTermiteMethodToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ScholarTermiteMethodToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TxtRows As System.Windows.Forms.TextBox
    Friend WithEvents LblRows As System.Windows.Forms.Label
    Friend WithEvents TxtColumns As System.Windows.Forms.TextBox
    Friend WithEvents LblColumns As System.Windows.Forms.Label
    Friend WithEvents TxtRatio As System.Windows.Forms.TextBox
    Friend WithEvents LblRatio As System.Windows.Forms.Label
    Friend WithEvents TxtTermites As System.Windows.Forms.TextBox
    Friend WithEvents LblNumTermites As System.Windows.Forms.Label
    Friend WithEvents TxtPhase1Decay As System.Windows.Forms.TextBox
    Friend WithEvents TxtPhase2Decay As System.Windows.Forms.TextBox
    Friend WithEvents LblPhase1Decay As System.Windows.Forms.Label
    Friend WithEvents LblPhase2Decay As System.Windows.Forms.Label
    Friend WithEvents TxtGravStart As System.Windows.Forms.TextBox
    Friend WithEvents TxtReps As System.Windows.Forms.TextBox
    Friend WithEvents LblGravStart As System.Windows.Forms.Label
    Friend WithEvents LblReps As System.Windows.Forms.Label
    Friend WithEvents PicVDP As System.Windows.Forms.PictureBox
    Friend WithEvents Timer1 As System.Windows.Forms.Timer

End Class
