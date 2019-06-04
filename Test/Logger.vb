Public Class Logger
    Public Client As Connection
    Public Sub New(_cl As Connection)
        Client = _cl
        getUserName()
    End Sub
    Public Sub getUserName()
        Dim testLockName = Environment.UserName.ToString()
        Dim count = 0
        While String.IsNullOrEmpty(UserName)
            count += 1
            If System.IO.File.Exists(testLockName + ".lock") Then
                Dim file = New System.IO.FileInfo(testLockName + ".lock")
                If Math.Abs((DateTime.Now - file.LastWriteTime).TotalHours) > 1 Then
                    ' last written to over an hour ago
                    UserName = testLockName + "-"
                Else
                    testLockName = Environment.UserName() + count.ToString()
                End If
            Else
                UserName = testLockName
            End If
        End While
        System.IO.File.WriteAllText(testLockName + ".lock", "wrote")
    End Sub
    Public Sub New()
        getUserName()
    End Sub
    Public UserName As String
    Public FolderPath As String = System.IO.Directory.GetCurrentDirectory() + $"\{UserName}logs\"
    Public FileName As String = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.log"

    Public Sub Log(value As String)
        If System.IO.Directory.Exists(FolderPath) = False Then
            System.IO.Directory.CreateDirectory(FolderPath)
        End If
        System.IO.File.AppendAllText(FolderPath + "\" + FileName, DateTime.Now.ToString("hh:mm:ss.fff") + ": " + value + vbCrLf)
    End Sub
End Class
