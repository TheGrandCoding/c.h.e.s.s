Imports System.Net
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

    ''' <summary>
    ''' Milisecond timeout for Moving and Promotion operation before they are considered failed
    ''' </summary>
    Public TimeOut As Integer = 10 * 1000

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

    Private Sub Display(message As String)
        form.Invoke(Sub() form.lblStatus.Text += message + vbCrLf)
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
            ElseIf message.StartsWith("PROMOTE/") Then
                message = message.Substring("PROMOTE/".Length)
                Dim isSuccess As Boolean = False
                If message.StartsWith("SUC:") Then
                    isSuccess = True
                End If
                message = message.Substring("SUC:".Length)
                If promoting IsNot Nothing Then
                    promoting.IsSuccess = isSuccess
                    promoting.ErrorMessage = message
                    promoting.moveConfirmed.Set()
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
                Display($"Moved {from.PlaceString} to {[to].PlaceString}")
            ElseIf message.StartsWith("PROMOTE:") Then
                message = message.Substring("PROMOTE:".Length)
                Dim split() = message.Split(":")
                Dim from = Form1.TestButtonAtString(split(0))
                Dim type = CType(Integer.Parse(split(1)), PieceType)
                from.Piece = type.ToString()
                form.Invoke(Sub() form.HighlightReset())
                Display($"Promoted {from.PlaceString} to {type}")
            End If
        ElseIf message.StartsWith("LOSE") Then
            message = message.Substring("LOSE:".Length)
            Dim reason = "You lost!"
            If message = "RESIGN" Then
                reason = "You resigned"
            End If
            form.Invoke(Sub()
                            form.panel_Hide.Show()
                            form.lblPause.Text = reason
                            form.GameOver = True
                            If Form1.WeArePlayingFor = "W_" Then
                                form.RemainingWhites = 0
                            Else
                                form.RemainingBlacks = 0
                            End If
                        End Sub)
        ElseIf message.StartsWith("WIN") Then
            message = message.Substring("WIN:".Length)
            Dim reason = "You won"
            If message = "RESIGN" Then
                reason = "Your opponent resigned"
            End If
            form.Invoke(Sub()
                            form.panel_Hide.Show()
                            form.lblPause.Text = reason
                            form.GameOver = True
                            If Form1.WeArePlayingFor = "W_" Then
                                form.RemainingBlacks = 0
                            Else
                                form.RemainingWhites = 0
                            End If
                        End Sub)
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
            moving.moveConfirmed.WaitOne(TimeOut) ' waits until confirmed, or a timeout occurs
            Dim suc = moving.IsSuccess
            Dim err = moving.ErrorMessage
            moving = Nothing ' reset
            If suc Then
                Return ""
            Else
                Return err
            End If
        Else
            Return "Move already in progress: " + moving.From + " -> " + moving.[To]
        End If
    End Function
#End Region

#Region "Promoting Piecees"
    Private promoting As PromotePiece

    Private Class PromotePiece
        Public From As String
        Public Piece As PieceType
        Public moveConfirmed As New ManualResetEvent(False)
        Public IsSuccess = False
        Public ErrorMessage = "Operation timed out"
    End Class

    Public Function TryPromotePiece(from As String, type As PieceType) As String
        If promoting Is Nothing Then
            promoting = New PromotePiece()
            promoting.From = from
            promoting.Piece = type
            Send($"PROMOTE:{from};{CType(type, Integer)}")
            promoting.moveConfirmed.WaitOne(TimeOut)
            Dim suc = promoting.IsSuccess
            Dim err = promoting.ErrorMessage
            promoting = Nothing ' reset
            If suc Then
                Return ""
            Else
                Return err
            End If
        Else
            Return "Promotion already in progress: " + promoting.From + " -> " + promoting.Piece.ToString()
        End If
    End Function

#End Region



End Class
Public Enum PlayerColour
    NotSet
    White
    Black
End Enum
