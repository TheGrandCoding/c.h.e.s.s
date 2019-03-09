Public Class JsonGameDelta
    Public ID As Integer
    Public color As PlayerColour
    Public white As String
    Public black As String
    Public board As Dictionary(Of String, ChessPosition)
    Public whiteTime As TimeSpan
    Public blackTime As TimeSpan
End Class
