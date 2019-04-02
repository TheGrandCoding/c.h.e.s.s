<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
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
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.lblBlackLost = New System.Windows.Forms.RichTextBox()
        Me.lblWhiteLost = New System.Windows.Forms.RichTextBox()
        Me.lblWhiteTimer = New System.Windows.Forms.Label()
        Me.lblBlackTimer = New System.Windows.Forms.Label()
        Me.btnHelp = New System.Windows.Forms.Button()
        Me.btnResign = New System.Windows.Forms.Button()
        Me.pb_whites = New System.Windows.Forms.PictureBox()
        Me.pb_black = New System.Windows.Forms.PictureBox()
        Me.panel_Hide = New System.Windows.Forms.Panel()
        Me.lblPause = New System.Windows.Forms.Label()
        Me.btnSaveGame = New System.Windows.Forms.Button()
        Me.btnLoadGame = New System.Windows.Forms.Button()
        Me.lblWhiteName = New System.Windows.Forms.Label()
        Me.lblBlackName = New System.Windows.Forms.Label()
        Me.discordTimer = New System.Windows.Forms.Timer(Me.components)
        Me.uiTimer = New System.Windows.Forms.Timer(Me.components)
        Me.btnUndo = New System.Windows.Forms.Button()
        CType(Me.pb_whites, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pb_black, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panel_Hide.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(455, 5)
        Me.lblStatus.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(160, 375)
        Me.lblStatus.TabIndex = 1
        Me.lblStatus.Text = "Log:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'lblBlackLost
        '
        Me.lblBlackLost.Location = New System.Drawing.Point(620, 64)
        Me.lblBlackLost.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.lblBlackLost.Name = "lblBlackLost"
        Me.lblBlackLost.ReadOnly = True
        Me.lblBlackLost.Size = New System.Drawing.Size(155, 164)
        Me.lblBlackLost.TabIndex = 2
        Me.lblBlackLost.Text = "Black Lost:" & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'lblWhiteLost
        '
        Me.lblWhiteLost.Location = New System.Drawing.Point(620, 292)
        Me.lblWhiteLost.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.lblWhiteLost.Name = "lblWhiteLost"
        Me.lblWhiteLost.ReadOnly = True
        Me.lblWhiteLost.Size = New System.Drawing.Size(155, 163)
        Me.lblWhiteLost.TabIndex = 3
        Me.lblWhiteLost.Text = "White lost:" & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'lblWhiteTimer
        '
        Me.lblWhiteTimer.Location = New System.Drawing.Point(505, 428)
        Me.lblWhiteTimer.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblWhiteTimer.Name = "lblWhiteTimer"
        Me.lblWhiteTimer.Size = New System.Drawing.Size(110, 19)
        Me.lblWhiteTimer.TabIndex = 4
        Me.lblWhiteTimer.Text = "White Time:"
        '
        'lblBlackTimer
        '
        Me.lblBlackTimer.Location = New System.Drawing.Point(505, 402)
        Me.lblBlackTimer.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblBlackTimer.Name = "lblBlackTimer"
        Me.lblBlackTimer.Size = New System.Drawing.Size(110, 19)
        Me.lblBlackTimer.TabIndex = 5
        Me.lblBlackTimer.Text = "Black Time:"
        '
        'btnHelp
        '
        Me.btnHelp.Location = New System.Drawing.Point(9, 10)
        Me.btnHelp.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(30, 32)
        Me.btnHelp.TabIndex = 6
        Me.btnHelp.Text = "?"
        Me.btnHelp.UseVisualStyleBackColor = True
        '
        'btnResign
        '
        Me.btnResign.Location = New System.Drawing.Point(451, 402)
        Me.btnResign.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnResign.Name = "btnResign"
        Me.btnResign.Size = New System.Drawing.Size(50, 45)
        Me.btnResign.TabIndex = 7
        Me.btnResign.Text = "Resign"
        Me.btnResign.UseVisualStyleBackColor = True
        '
        'pb_whites
        '
        Me.pb_whites.Image = Global.Test.My.Resources.Resources.W_Pawn
        Me.pb_whites.Location = New System.Drawing.Point(718, 233)
        Me.pb_whites.Name = "pb_whites"
        Me.pb_whites.Size = New System.Drawing.Size(54, 54)
        Me.pb_whites.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pb_whites.TabIndex = 8
        Me.pb_whites.TabStop = False
        '
        'pb_black
        '
        Me.pb_black.Image = Global.Test.My.Resources.Resources.B_Pawn
        Me.pb_black.InitialImage = Global.Test.My.Resources.Resources.B_Pawn
        Me.pb_black.Location = New System.Drawing.Point(718, 6)
        Me.pb_black.Name = "pb_black"
        Me.pb_black.Size = New System.Drawing.Size(54, 54)
        Me.pb_black.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pb_black.TabIndex = 9
        Me.pb_black.TabStop = False
        '
        'panel_Hide
        '
        Me.panel_Hide.Controls.Add(Me.lblPause)
        Me.panel_Hide.Controls.Add(Me.btnSaveGame)
        Me.panel_Hide.Controls.Add(Me.btnLoadGame)
        Me.panel_Hide.Location = New System.Drawing.Point(9, 46)
        Me.panel_Hide.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.panel_Hide.Name = "panel_Hide"
        Me.panel_Hide.Size = New System.Drawing.Size(222, 182)
        Me.panel_Hide.TabIndex = 10
        '
        'lblPause
        '
        Me.lblPause.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblPause.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPause.ForeColor = System.Drawing.Color.Red
        Me.lblPause.Location = New System.Drawing.Point(0, 0)
        Me.lblPause.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblPause.Name = "lblPause"
        Me.lblPause.Size = New System.Drawing.Size(222, 134)
        Me.lblPause.TabIndex = 0
        Me.lblPause.Text = "GAME PAUSED" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Press P to continue" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.lblPause.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnSaveGame
        '
        Me.btnSaveGame.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.btnSaveGame.Location = New System.Drawing.Point(0, 134)
        Me.btnSaveGame.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnSaveGame.Name = "btnSaveGame"
        Me.btnSaveGame.Size = New System.Drawing.Size(222, 24)
        Me.btnSaveGame.TabIndex = 2
        Me.btnSaveGame.Text = "Save"
        Me.btnSaveGame.UseVisualStyleBackColor = True
        '
        'btnLoadGame
        '
        Me.btnLoadGame.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.btnLoadGame.Location = New System.Drawing.Point(0, 158)
        Me.btnLoadGame.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnLoadGame.Name = "btnLoadGame"
        Me.btnLoadGame.Size = New System.Drawing.Size(222, 24)
        Me.btnLoadGame.TabIndex = 1
        Me.btnLoadGame.Text = "Load"
        Me.btnLoadGame.UseVisualStyleBackColor = True
        '
        'lblWhiteName
        '
        Me.lblWhiteName.Location = New System.Drawing.Point(620, 233)
        Me.lblWhiteName.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblWhiteName.Name = "lblWhiteName"
        Me.lblWhiteName.Size = New System.Drawing.Size(93, 54)
        Me.lblWhiteName.TabIndex = 11
        Me.lblWhiteName.Text = "Black"
        Me.lblWhiteName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblBlackName
        '
        Me.lblBlackName.Location = New System.Drawing.Point(620, 6)
        Me.lblBlackName.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblBlackName.Name = "lblBlackName"
        Me.lblBlackName.Size = New System.Drawing.Size(93, 54)
        Me.lblBlackName.TabIndex = 12
        Me.lblBlackName.Text = "White"
        Me.lblBlackName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'discordTimer
        '
        Me.discordTimer.Enabled = True
        Me.discordTimer.Interval = 250
        '
        'uiTimer
        '
        Me.uiTimer.Enabled = True
        Me.uiTimer.Interval = 250
        '
        'btnUndo
        '
        Me.btnUndo.Location = New System.Drawing.Point(451, 373)
        Me.btnUndo.Margin = New System.Windows.Forms.Padding(2)
        Me.btnUndo.Name = "btnUndo"
        Me.btnUndo.Size = New System.Drawing.Size(164, 25)
        Me.btnUndo.TabIndex = 13
        Me.btnUndo.Text = "Undo last action"
        Me.btnUndo.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(782, 466)
        Me.Controls.Add(Me.btnUndo)
        Me.Controls.Add(Me.panel_Hide)
        Me.Controls.Add(Me.pb_black)
        Me.Controls.Add(Me.pb_whites)
        Me.Controls.Add(Me.btnResign)
        Me.Controls.Add(Me.btnHelp)
        Me.Controls.Add(Me.lblBlackTimer)
        Me.Controls.Add(Me.lblWhiteTimer)
        Me.Controls.Add(Me.lblWhiteLost)
        Me.Controls.Add(Me.lblBlackLost)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.lblBlackName)
        Me.Controls.Add(Me.lblWhiteName)
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.Name = "Form1"
        Me.Text = "Test Form"
        CType(Me.pb_whites, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pb_black, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panel_Hide.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lblStatus As Label
    Friend WithEvents lblBlackLost As RichTextBox
    Friend WithEvents lblWhiteLost As RichTextBox
    Friend WithEvents lblWhiteTimer As Label
    Friend WithEvents lblBlackTimer As Label
    Friend WithEvents btnHelp As Button
    Friend WithEvents btnResign As Button
    Friend WithEvents pb_whites As PictureBox
    Friend WithEvents pb_black As PictureBox
    Friend WithEvents panel_Hide As Panel
    Friend WithEvents lblPause As Label
    Friend WithEvents btnSaveGame As Button
    Friend WithEvents btnLoadGame As Button
    Friend WithEvents lblWhiteName As Label
    Friend WithEvents lblBlackName As Label
    Friend WithEvents discordTimer As Timer
    Friend WithEvents uiTimer As Timer
    Friend WithEvents btnUndo As Button
End Class
