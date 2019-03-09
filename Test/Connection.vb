﻿Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports Newtonsoft.Json

Public Class Connection
    Public Name As String ' Our name
    Public Color As PlayerColour
    Public IsOurGo As Boolean
    Private Client As TcpClient
    Private Const Port = 9993
    Private listenThread As Thread

    Private form As Form1

    Private Deltas As New Dictionary(Of Integer, JsonGameDelta)

    ''' <summary>
    ''' Some other message needs to be handleds
    ''' </summary>
    Public Event RecievedMessage As EventHandler(Of String)
    ''' <summary>
    ''' The game was updated
    ''' </summary>
    Public Event GameUpdated As EventHandler(Of String)
    ''' <summary>
    ''' All messages raise this
    ''' </summary>
    Private Event internalMessage As EventHandler(Of String)


    Public Sub Send(message As String)
        Try
            message = $"%{message}`"
            Dim stream = Client.GetStream()
            Dim bytes = System.Text.Encoding.UTF8.GetBytes(message)
            stream.Write(bytes, 0, bytes.Length)
        Catch x As Exception
            Client = Nothing
            Try
                System.IO.File.WriteAllText($"errlog_{DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")}.txt", x.ToString())
                MsgBox("Server has disconnected, the error has been logged")
            Catch
                MsgBox("Server has disconnected, error cannot be logged" + vbCrLf + x.ToString())
            End Try
            Try
                listenThread.Abort()
            Catch ex As Exception
            End Try
        End Try
    End Sub

    Public Sub New(f As Form1, conn As IPAddress, _name As String)
        form = f
        Client = New TcpClient(conn.AddressFamily)
        Dim cTask = Client.ConnectAsync(conn, Port)
        Dim tTask = Task.Delay(30000)
        Task.WaitAny(cTask, tTask) ' waits for timeout or connnection
        If Client.Connected Then
            Send(_name)
            listenThread = New Thread(AddressOf Listener)
            listenThread.Start()
            Name = _name
            AddHandler Me.internalMessage, AddressOf HandleMessage
        Else
            Throw New Exception("Failed to connect to " + conn.ToString() + ":" + Port.ToString())
        End If
    End Sub

    Private Sub HandleMessage(sender As Object, message As String)
        If message.StartsWith("GAME:") Then
            Dim delta = JsonConvert.DeserializeObject(Of JsonGameDelta)(message.Replace("GAME:", ""))
            Deltas.Add(delta.ID, delta)
            Dim lastDelta As JsonGameDelta
            Deltas.TryGetValue(delta.ID - 1, lastDelta)
            Form1.UpdateFromDelta(delta, lastDelta)
        ElseIf message.StartsWith("RES/") Then
            ' Result/response messages
            message = message.Substring("RES/".Length)
            If message.StartsWith("MOVE/") Then
                ' a move error
                message = message.Substring("MOVE/".Length)
                Dim isSuccess As Boolean = False
                If message.StartsWith("SUC:") Then
                    isSuccess = True
                End If
                message = message.Substring("SUC:".Length)
                If moving IsNot Nothing Then
                    moving.IsSuccess = isSuccess
                    moving.ErrorMessage = message
                    moving.moveConfirmed.Set()
                End If
            End If
        ElseIf message.StartsWith("OTH/") Then
            ' message due to the other player's actions
            message = message.Substring("OTH/".Length)
            If message.StartsWith("MOVE:") Then
                message = message.Substring("MOVE:".Length)
                Dim split() = message.Split(":")
                Dim from = Form1.TestButtonAtString(split(0))
                Dim [to] = Form1.TestButtonAtString(split(1))
                form.Invoke(Sub() form.MovePiece(from, [to]))
                form.Invoke(Sub() form.HighlightReset())
            End If
        Else
            RaiseEvent RecievedMessage(Me, message)
        End If
    End Sub

    Private Sub Listener()
        ' should be on a different thread.
        While Client IsNot Nothing AndAlso Client.Connected
            Dim stream = Client.GetStream()
            Dim bytes(Client.ReceiveBufferSize) As Byte
            stream.Read(bytes, 0, Client.ReceiveBufferSize)
            Dim msg = System.Text.Encoding.UTF8.GetString(bytes).Replace(vbNullChar, String.Empty)
            For Each message As String In msg.Split("%")
                If String.IsNullOrWhiteSpace(message) Then
                    Continue For
                End If
                RaiseEvent internalMessage(Me, message.Substring(0, message.LastIndexOf("`")))
            Next
        End While
    End Sub


#Region "Moving Pieces"
    Private Class MovePiece
        Public From As String
        Public [To] As String
        Public DelayTask As Task
        Public moveConfirmed As New ManualResetEvent(False)
        Public IsSuccess As Boolean = False
        Public ErrorMessage As String = "Operation timed out"
    End Class
    Private moving As MovePiece
    Public Function TryMovePiece(from As String, [to] As String) As String
        If moving Is Nothing Then
            moving = New MovePiece()
            moving.From = from
            moving.To = [to]
            Send("MOVE:" + from + ":" + [to])
            moving.moveConfirmed.WaitOne(1500000) ' waits until confirmed, or a timeout occurs
            Dim suc = moving.IsSuccess
            Dim err = moving.ErrorMessage
            moving = Nothing ' reset
            If suc Then
                Return ""
            Else
                Return err
            End If
        Else
            Return "Move already in progress: " + from + " -> " + [to]
        End If
    End Function
#End Region
End Class
Public Enum PlayerColour
    NotSet
    White
    Black
End Enum
