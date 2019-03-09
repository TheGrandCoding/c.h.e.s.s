Imports Newtonsoft.Json

Public Class ChessPosition
    <JsonProperty("Pos")>
    Public Position As String
    Public PieceHere As ChessPiece
End Class
