Public Class GameSave
    Public SaveVersion As String
    Public PlayingFor As String
    Public WhiteTime As TimeSpan
    Public BlackTime As TimeSpan
    Public BlackLost As String
    Public WhiteLost As String
    Public RemainingWhites As Integer
    Public RemainingBlacks As Integer
    Public MoveLog As String
    Public WhiteName As String
    Public BlackName As String
    Public Locations As List(Of ButtonLocation)
End Class
Public Class ButtonLocation
    Public Tag As String
    Public Name As String
End Class
