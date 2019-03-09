Imports System
Imports DiscordRPC
Imports DiscordRPC.Logging
Imports DiscordRPC.Message
Imports Test
Public Class HandleDiscord
    ' Handles interaction with the Discord RPC
    ' This allows for the fancy displays in discord,
    ' and should allow for the "Ask to join"
    Private Const ClientId As String = "553650964276576318"
    Private Shared Client As DiscordRpcClient
    Private Shared Logger As New DiscordLogger()

    Public Sub Dispose()
        Client.Dispose()
    End Sub

    Public Sub Invoke()
        If Client IsNot Nothing Then
            Client.Invoke()
        End If
    End Sub

    Public Sub Start()
        If Client IsNot Nothing Then
            Throw New InvalidOperationException("Client already started")
        End If
        Client = New DiscordRpcClient(ClientId, True, -1, Logger)
        Client.SetSubscription(EventType.Join Or EventType.JoinRequest Or EventType.Spectate)
        AddHandler Client.OnReady, AddressOf OnReady

        AddHandler Client.OnJoin, AddressOf OnJoin
        AddHandler Client.OnJoinRequested, AddressOf OnJoinRequest
        Client.Initialize()
    End Sub

    Private Shared Sub OnReady(sender As Object, args As ReadyMessage)
        Logger.Info("Connected, API: " + args.Version.ToString(), args.User)
        Dim pres As New RichPresence()
        pres.State = "Waiting..."
        pres.Details = "Just started"
        Dim secrets As New Secrets
        secrets.JoinSecret = args.User.ID
        pres.Secrets = secrets
        Client.SetPresence(pres)
    End Sub

    Private Shared Sub OnJoin(sender As Object, args As JoinMessage)
        MsgBox("'Join' game with secret of: (doesnt work)" + args.Secret)
    End Sub

    Private Shared Sub OnJoinRequest(sender As Object, args As JoinRequestMessage)
        ' I think the request is handled in Discord itself, but in case its not..
        If Form1.InGame Then
            ' Insta-reject, as we assume they don't want to leave a game to join another..
            Client.Respond(args, False)
        Else

            If MsgBox("Do you want to join a game with:" + vbCrLf + args.User.Username + "#" + args.User.Discriminator + vbCrLf + args.User.ID, MsgBoxStyle.YesNo, "Discord Join Request") Then
                Client.Respond(args, True)
            End If
        End If
    End Sub

End Class

Public Class DiscordLogger
    Implements Logging.ILogger

    Private _level As LogLevel = LogLevel.Info
    Public Property Level As LogLevel Implements ILogger.Level
        Get
            Return _level
        End Get
        Set(value As LogLevel)
            _level = value
        End Set
    End Property

    Private lockObj As New Object()

    Private Sub writeFile(message As String)
        SyncLock lockObj
            System.IO.File.AppendAllText($"log_{DateTime.Now.ToString("yyyy_MM_dd")}.txt", message + vbCrLf)
        End SyncLock
    End Sub

    Private Function getMessage(type As String, message As String) As String
        Return $"{DateTime.Now.ToString("hh:mm:ss.fff")}: [{type}] {message}"
    End Function

    Private Sub handleMessage(message As String)
        Console.WriteLine(message)
        writeFile(message)
    End Sub

    Public Sub Trace(message As String, ParamArray args() As Object) Implements ILogger.Trace
        handleMessage(getMessage("Trace", String.Format(message, args)))
    End Sub

    Public Sub Info(message As String, ParamArray args() As Object) Implements ILogger.Info
        handleMessage(getMessage("Info", String.Format(message, args)))
    End Sub

    Public Sub Warning(message As String, ParamArray args() As Object) Implements ILogger.Warning
        handleMessage(getMessage("Warning", String.Format(message, args)))
    End Sub

    Public Sub [Error](message As String, ParamArray args() As Object) Implements ILogger.Error
        handleMessage(getMessage("ERROR", String.Format(message, args)))
    End Sub
End Class
