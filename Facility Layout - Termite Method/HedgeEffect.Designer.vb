<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SolveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GenerateTileSwarmToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportTileSwarmToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ofdLoadSetup = New System.Windows.Forms.OpenFileDialog()
        Me.GreedyTermiteMethodToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.SolveToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(284, 24)
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
        Me.SolveToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GenerateTileSwarmToolStripMenuItem, Me.ExportTileSwarmToolStripMenuItem, Me.GreedyTermiteMethodToolStripMenuItem})
        Me.SolveToolStripMenuItem.Name = "SolveToolStripMenuItem"
        Me.SolveToolStripMenuItem.Size = New System.Drawing.Size(47, 20)
        Me.SolveToolStripMenuItem.Text = "Solve"
        '
        'GenerateTileSwarmToolStripMenuItem
        '
        Me.GenerateTileSwarmToolStripMenuItem.Name = "GenerateTileSwarmToolStripMenuItem"
        Me.GenerateTileSwarmToolStripMenuItem.Size = New System.Drawing.Size(200, 22)
        Me.GenerateTileSwarmToolStripMenuItem.Text = "Generate Tile Swarm"
        '
        'ExportTileSwarmToolStripMenuItem
        '
        Me.ExportTileSwarmToolStripMenuItem.Name = "ExportTileSwarmToolStripMenuItem"
        Me.ExportTileSwarmToolStripMenuItem.Size = New System.Drawing.Size(200, 22)
        Me.ExportTileSwarmToolStripMenuItem.Text = "Export Tile Swarm"
        '
        'ofdLoadSetup
        '
        Me.ofdLoadSetup.FileName = "OpenFileDialog1"
        '
        'GreedyTermiteMethodToolStripMenuItem
        '
        Me.GreedyTermiteMethodToolStripMenuItem.Name = "GreedyTermiteMethodToolStripMenuItem"
        Me.GreedyTermiteMethodToolStripMenuItem.Size = New System.Drawing.Size(200, 22)
        Me.GreedyTermiteMethodToolStripMenuItem.Text = "Greedy Termite Method"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(284, 262)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
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

End Class
