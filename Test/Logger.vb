Public Class Logger
    Public Client As Connection
    Public Sub New(_cl As Connection)
        Client = _cl
    End Sub
    Public Sub New()
    End Sub
    Public FolderPath As String = System.IO.Directory.GetCurrentDirectory() + "\logs\" +
        If(Client Is Nothing, Environment.UserName, Client.Name)
    Public FileName As String = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.log"

    Public Sub Log(value As String)
        Return ' logging disabled
        If System.IO.Directory.Exists(FolderPath) = False Then
            System.IO.Directory.CreateDirectory(FolderPath)
        End If
        System.IO.File.AppendAllText(FolderPath + "\" + FileName, DateTime.Now.ToString("hh:mm:ss.fff") + ": " + value + vbCrLf)
    End Sub
End Class
