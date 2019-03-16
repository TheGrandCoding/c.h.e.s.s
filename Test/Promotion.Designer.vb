<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Promotion
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.rb_Rook = New System.Windows.Forms.RadioButton()
        Me.rb_Bishop = New System.Windows.Forms.RadioButton()
        Me.rb_Queen = New System.Windows.Forms.RadioButton()
        Me.rb_Knight = New System.Windows.Forms.RadioButton()
        Me.rb_pawn = New System.Windows.Forms.RadioButton()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(355, 17)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "What rank do you want to promote your Pawn at  0,0 to"
        '
        'rb_Rook
        '
        Me.rb_Rook.AutoSize = True
        Me.rb_Rook.Location = New System.Drawing.Point(15, 47)
        Me.rb_Rook.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.rb_Rook.Name = "rb_Rook"
        Me.rb_Rook.Size = New System.Drawing.Size(62, 21)
        Me.rb_Rook.TabIndex = 1
        Me.rb_Rook.TabStop = True
        Me.rb_Rook.Text = "Rook"
        Me.rb_Rook.UseVisualStyleBackColor = True
        '
        'rb_Bishop
        '
        Me.rb_Bishop.AutoSize = True
        Me.rb_Bishop.Location = New System.Drawing.Point(117, 47)
        Me.rb_Bishop.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.rb_Bishop.Name = "rb_Bishop"
        Me.rb_Bishop.Size = New System.Drawing.Size(72, 21)
        Me.rb_Bishop.TabIndex = 2
        Me.rb_Bishop.TabStop = True
        Me.rb_Bishop.Text = "Bishop"
        Me.rb_Bishop.UseVisualStyleBackColor = True
        '
        'rb_Queen
        '
        Me.rb_Queen.AutoSize = True
        Me.rb_Queen.Location = New System.Drawing.Point(341, 47)
        Me.rb_Queen.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.rb_Queen.Name = "rb_Queen"
        Me.rb_Queen.Size = New System.Drawing.Size(72, 21)
        Me.rb_Queen.TabIndex = 3
        Me.rb_Queen.TabStop = True
        Me.rb_Queen.Tag = ""
        Me.rb_Queen.Text = "Queen"
        Me.rb_Queen.UseVisualStyleBackColor = True
        '
        'rb_Knight
        '
        Me.rb_Knight.AutoSize = True
        Me.rb_Knight.Location = New System.Drawing.Point(225, 47)
        Me.rb_Knight.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.rb_Knight.Name = "rb_Knight"
        Me.rb_Knight.Size = New System.Drawing.Size(69, 21)
        Me.rb_Knight.TabIndex = 4
        Me.rb_Knight.TabStop = True
        Me.rb_Knight.Text = "Knight"
        Me.rb_Knight.UseVisualStyleBackColor = True
        '
        'rb_pawn
        '
        Me.rb_pawn.AutoSize = True
        Me.rb_pawn.Location = New System.Drawing.Point(15, 73)
        Me.rb_pawn.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.rb_pawn.Name = "rb_pawn"
        Me.rb_pawn.Size = New System.Drawing.Size(63, 21)
        Me.rb_pawn.TabIndex = 3
        Me.rb_pawn.TabStop = True
        Me.rb_pawn.Text = "Pawn"
        Me.rb_pawn.UseVisualStyleBackColor = True
        '
        'Promotion
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(425, 156)
        Me.Controls.Add(Me.rb_pawn)
        Me.Controls.Add(Me.rb_Knight)
        Me.Controls.Add(Me.rb_Queen)
        Me.Controls.Add(Me.rb_Bishop)
        Me.Controls.Add(Me.rb_Rook)
        Me.Controls.Add(Me.Label1)
        Me.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Name = "Promotion"
        Me.Text = "Promotion"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents rb_Rook As RadioButton
    Friend WithEvents rb_Bishop As RadioButton
    Friend WithEvents rb_Queen As RadioButton
    Friend WithEvents rb_Knight As RadioButton
    Friend WithEvents rb_pawn As RadioButton
End Class
