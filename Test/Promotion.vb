Public Class Promotion
    Public Choice As String = ""
    Private Sub rb_Rook_CheckedChanged(sender As Object, e As EventArgs) Handles rb_Rook.CheckedChanged, rb_Bishop.CheckedChanged, rb_Queen.CheckedChanged, rb_Knight.CheckedChanged, rb_pawn.CheckedChanged
        Dim rb As RadioButton = CType(sender, RadioButton)
        Choice = rb.Text
    End Sub

    Public Sub SetPlace(xy As String)
        rb_pawn.Hide()
        Label1.Text = "What rank do you want to promote your Pawn at " + xy + " to"
        Me.Text = "Promotion"
    End Sub

    Public Sub AddDebugPlace(xy As String)
        Label1.Text = "What piece do you want to add at " & xy
        rb_pawn.Show()
        Me.Text = "Debug add piece"
    End Sub

    Private Sub Promotion_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Choice = "" Then
            e.Cancel = True
            MsgBox("You must select a piece to promote to!")
        End If
    End Sub

    Private initSize As New Size(335, 99)
End Class