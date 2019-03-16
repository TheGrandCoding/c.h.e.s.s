Imports System.ComponentModel
'Imports Newtonsoft.Json
Imports System.IO
Imports MASTERLIST = MasterlistDLL.MasterListClient

Public Class Form1
    Public Shared TestButtons As New List(Of TestButton)
    Private CoordToPlace As New Dictionary(Of Integer, String) From {{1, "A"}, {2, "B"}, {3, "C"}, {4, "D"}, {5, "E"}, {6, "F"}, {7, "G"}, {8, "H"}}
    Public Shared instance As Form1

    Private Shared Discord As HandleDiscord

    Public Shared InGame As Boolean = False
    Public Shared Client As Connection

    Private WithEvents kbHook As New KeyboardHook()

    Dim pPressed As Boolean = False
    Dim ctrlPressed As Boolean = False

    Private Sub kbHook_KeyDown(ByVal key As Keys) Handles kbHook.KeyDown
        If key <> Keys.P AndAlso key.ToString().Contains("Control") = False Then
            ctrlPressed = False
            pPressed = False
        End If
        If key.ToString().Contains("Control") Then
            ctrlPressed = True
        End If
        If key = Keys.P Then
            pPressed = True
        End If
    End Sub

    Private Sub kbHook_KeyUp(ByVal Key As System.Windows.Forms.Keys) Handles kbHook.KeyUp
        If pPressed AndAlso ctrlPressed Then
            Me.Visible = Not Me.Visible
            pPressed = False
            ctrlPressed = False
        End If
    End Sub

    Public Enum PinType
        None
        Vertical ' |
        DiagonalRight ' /
        Horizontal ' --
        DiagonalLeft ' \
        Fully
    End Enum

    Public Shared WeArePlayingFor As String = ""

    Public Shared CurrentGo As String = "W_" 'who's turn it is
    Private ReadOnly Property Opponent As String
        Get
            If CurrentGo = "W_" Then Return "B_"
            Return "W_"
        End Get
    End Property

    Private Shared BlackTimePlayed As New TimeSpan(0, 0, 0, 0, 0)
    Private Shared WhiteTimePlayed As New TimeSpan(0, 0, 0, 0, 0)
    Private Shared BlackPlayer As String
    Private Shared WhitePlayer As String

    Private Sub Switch()
        Dim Bold As Font = New Font(lblPause.Font, FontStyle.Bold)
        Dim Normal As Font = New Font(lblPause.Font, FontStyle.Regular)
        If GameOver Then Return
        If CurrentGo = "B_" Then
            CurrentGo = "W_"
            pb_black.Hide()
            lblBlackName.ForeColor = Color.Black
            lblBlackName.Font = Normal
            pb_whites.Show()
            lblWhiteName.ForeColor = Color.Red
            lblWhiteName.Font = Bold
        Else
            CurrentGo = "B_"
            pb_whites.Hide()
            lblWhiteName.ForeColor = Color.Black
            lblWhiteName.Font = Normal
            pb_black.Show()
            lblBlackName.ForeColor = Color.Red
            lblBlackName.Font = Bold
        End If
        CheckWin()
    End Sub

    Private alreadyGone As Boolean = False
    Private Sub Start()
        If alreadyGone Then Return
        Dim _timerThread As Threading.Thread = New Threading.Thread(AddressOf TimerThread)
        _timerThread.Start()
        alreadyGone = True
    End Sub

    Private lastMili As DateTime = DateTime.Now()
    Private paused As Boolean = False
    Private Sub TimerThread()
        While GameOver = False
            If paused Then Continue While
            Dim now = DateTime.Now()
            Dim diff = now - lastMili
            If diff.TotalMilliseconds < 100 Then
                Continue While
            End If
            lastMili = DateTime.Now()
            If CurrentGo = "B_" Then
                BlackTimePlayed = BlackTimePlayed.Add(New TimeSpan(0, 0, 0, 0, 100))
            Else
                WhiteTimePlayed = WhiteTimePlayed.Add(New TimeSpan(0, 0, 0, 0, 100))
            End If
            UpdateTime()
        End While
    End Sub

    Public Sub UpdateFromDelta(current As JsonGameDelta, last As JsonGameDelta)
        If Me.InvokeRequired Then
            Me.Invoke(Sub() UpdateFromDelta(current, last))
        Else
            If last Is Nothing Then
                ' this is the first game delta
                ' which means we are JUST in a game
                instance.Invoke(Sub()
                                    instance.panel_Hide.Hide()
                                    instance.lblPause.Text = "Game paused" + vbCrLf + "Press P to continue"
                                End Sub)
            Else
            End If
            If current.color = PlayerColour.Black Then
                CurrentGo = "B_"
            ElseIf current.color = PlayerColour.White Then
                CurrentGo = "W_"
            End If
            If Not String.IsNullOrWhiteSpace(current.white) Then
                WhitePlayer = current.white
            End If
            If Not String.IsNullOrWhiteSpace(current.black) Then
                BlackPlayer = current.black
            End If
            If current.whiteTime.TotalSeconds > 0 Then
                WhiteTimePlayed = current.whiteTime
            End If
            If current.blackTime.TotalSeconds > 0 Then
                BlackTimePlayed = current.blackTime
            End If
            If WhitePlayer = Client.Name Then
                ' we are white..
                WeArePlayingFor = "W_"
            Else
                WeArePlayingFor = "B_"
            End If
        End If
    End Sub

    Private Sub UpdateTime()
        If Me.InvokeRequired Then
            Try
                Me.Invoke(Sub() UpdateTime())
                Return
            Catch ex As Exception
                'it keeps crashing when the program closes
                Return
            End Try

        End If
        Dim tlMilBlack As String = BlackTimePlayed.Milliseconds.ToString()
        tlMilBlack = tlMilBlack.Substring(0, 1)
        Dim tlSecBlack As String = BlackTimePlayed.Seconds.ToString()
        If tlSecBlack.Length <> 2 Then
            tlSecBlack = "0" + tlSecBlack
        End If
        Dim tlMinBlack As String = BlackTimePlayed.Minutes.ToString()
        If tlMinBlack.Length <> 2 Then
            tlMinBlack = "0" + tlMinBlack
        End If
        lblBlackTimer.Text = "Black Time: " + tlMinBlack + ":" + tlSecBlack + "." + tlMilBlack
        ' White
        Dim tlMilWhite As String = WhiteTimePlayed.Milliseconds.ToString()
        tlMilWhite = tlMilWhite.Substring(0, 1)
        Dim tlSecWhite As String = WhiteTimePlayed.Seconds.ToString()
        If tlSecWhite.Length <> 2 Then
            tlSecWhite = "0" + tlSecWhite
        End If
        Dim tlMinWhite As String = WhiteTimePlayed.Minutes.ToString()
        If tlMinWhite.Length <> 2 Then
            tlMinWhite = "0" + tlMinWhite
        End If
        lblWhiteTimer.Text = "White Time: " + tlMinWhite + ":" + tlSecWhite + "." + tlMilWhite

        Dim total = WhiteTimePlayed + BlackTimePlayed
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        instance = Me
        panel_Hide.Hide()
        panel_Hide.Location = New Point(0, 0)
        panel_Hide.Dock = DockStyle.Fill
        Dim lastBlack As Boolean = False
        For x As Integer = 1 To 8
            For y As Integer = 1 To 8
                If x = 1 Then
                    Dim lbl As New Label
                    lbl.Text = (y).ToString()
                    lbl.Location = New Point(10, (y * 50) + 5)
                    lbl.Size = New Size(20, lbl.Height)
                    lbl.TextAlign = ContentAlignment.MiddleCenter
                    Me.Controls.Add(lbl)
                End If
                If y = 1 Then
                    Dim lbl As New Label
                    lbl.Text = CoordToPlace(x).ToString()
                    lbl.Location = New Point((x * 50) + 10, 10)
                    lbl.Size = New Size(20, lbl.Height)
                    lbl.TextAlign = ContentAlignment.MiddleCenter
                    Me.Controls.Add(lbl)
                End If
                Dim but As New TestButton
                but.Name = x.ToString() + "," + y.ToString()
                but.Size = New Size(50, 50)
                but.Location = New Point(y * 50, x * 50)
                but.Tag = ""
                If x < 3 Then
                    but.Tag = "W_"
                ElseIf x > 6 Then
                    but.Tag = "B_"
                End If
                If x = 1 Or x = 8 Then
                    If y = 1 Or y = 8 Then
                        but.Tag += "Rook"
                    ElseIf y = 2 Or y = 7 Then
                        but.Tag += "Knight"
                    ElseIf y = 3 Or y = 6 Then
                        but.Tag += "Bishop"
                    ElseIf y = 4 Then
                        If but.Tag.Contains("B") Then
                            but.Tag += "Queen"
                        Else
                            but.Tag += "King"
                        End If
                    ElseIf y = 5 Then
                        If but.Tag.Contains("B") Then
                            but.Tag += "King"
                        Else
                            but.Tag += "Queen"
                        End If
                    End If
                ElseIf x = 2 Or x = 7 Then
                    but.Tag += "Pawn"
                End If
                but.Show()
                Me.Controls.Add(but)
                AddHandler but.Click, AddressOf TestButtonSelect
                AddHandler but.MouseEnter, AddressOf TestMouseEnter
                AddHandler but.MouseLeave, AddressOf TestMouseLeave
                TestButtons.Add(but)
            Next
        Next
        HighlightReset()
        CheckWin()
        pb_black.Hide()
        Discord = New HandleDiscord()
        'Discord.Start()
    End Sub
    Private doneOnce As Boolean = False
    Private Sub Form1_FirstActivate(sender As Object, e As EventArgs) Handles MyBase.Activated
        If doneOnce Then
            Return
        End If
        doneOnce = True
        panel_Hide.Show() ' Hides game screen
        btnSaveGame.Hide() ' Remnants of single player
        btnLoadGame.Hide()
        lblPause.Text = "Contacting Masterlist to find server..."
        Dim nThread As New Threading.Thread(AddressOf RunMLThread)
        nThread.Start()
    End Sub

    Private Sub RunMLThread()
        Dim servers = MASTERLIST.GetServers("chess#4008")
        Dim first = servers.FirstOrDefault()
        If first Is Nothing Then
            Form1.instance.Invoke(Sub()
                                      instance.lblPause.Text = "No servers found." + vbCrLf + "Double click to enter IP address manually"
                                  End Sub)
        Else
            Dim ip As Net.IPAddress
            If Net.IPAddress.TryParse(first.IPAddress, ip) Then
                instance.Invoke(Sub()
                                    instance.lblPause.Text = "Connecting to:" + vbCrLf + first.ServerName + " @ " + first.IPAddress
                                End Sub)
                Connection(ip)
            End If
        End If
    End Sub
    Shared rnd As New Random()
    Private Sub Connection(ip As Net.IPAddress)
        Dim yourName = InputBox("Please enter your name", "Name", rnd.Next(0, 1000).ToString())
        Client = New Connection(Me, ip, yourName)
        instance.Invoke(Sub()
                            ' instance.panel_Hide.Hide() ' Dont show yet, as we still need the other player
                            instance.lblPause.Text = "Waiting for game to start (requires another player)"
                        End Sub)
    End Sub

    Friend Function GetSide(sd As String)
        If sd = "W" Then Return "White"
        Return "Black"
    End Function

    Public Sub HighlightReset()
        For Each but As TestButton In TestButtons
            Dim x As Integer = Convert.ToInt32(but.XPos)
            Dim y As Integer = Convert.ToInt32(but.YPos)
            but.Name = x.ToString() + "," + y.ToString()
            but.BlackThreat = False
            but.WhiteThreat = False
            but.ThreateningAnyKing = False
            but.Pinned = PinType.None
            but.Text = ""
            but.takeable = False
            If x Mod 2 = 0 AndAlso y Mod 2 <> 0 Then
                but.BackColor = Color.Gray
            ElseIf x Mod 2 <> 0 AndAlso y Mod 2 = 0 Then
                but.BackColor = Color.Gray
            Else
                but.BackColor = Color.FromKnownColor(KnownColor.Control)
            End If
            If but.Tag <> "" Then
                but.BackgroundImage = My.Resources.ResourceManager.GetObject(but.Tag)
                but.BackgroundImageLayout = ImageLayout.Stretch
            Else
                but.BackgroundImage = Nothing
            End If
        Next
        CalculatePlacesInContest()
    End Sub

    ''' <summary>
    ''' Highlights or handles the flags for the given square with inforamtion.
    ''' </summary>
    ''' <param name="x">X position of the square</param>
    ''' <param name="y">Y position of the square</param>
    ''' <param name="clr">Color to highlight, Blue = Can mvove there.</param>
    ''' <param name="contest">Used to determine where each piece *could* take</param>
    ''' <param name="fromButton">Button that is moving/could move to this square</param>
    Private Sub Highlight(x As Integer, y As Integer, clr As Color, contest As String, fromButton As TestButton)
        For Each btn As TestButton In TestButtons
            Dim _x As Integer = Convert.ToInt32(btn.XPos)
            Dim _y As Integer = Convert.ToInt32(btn.YPos)
            If x = _x AndAlso y = _y Then
                If contest = "" Or contest.Contains("+") Then
                    If btn.Tag = "" OrElse btn.Tag.Contains(Opponent) Then
                        btn.takeable = True 'ish.. if the colour is red, the piece could be taken regardless of whether or not it is in the piece's path
                        '                   (it's a feature..)
                        btn.BackColor = clr
                    End If
                    If btn.Tag IsNot Nothing AndAlso btn.Tag.contains(Opponent) Then
                        btn.BackColor = Color.Red
                    End If
                End If
                contest = contest.Replace("+", String.Empty)
                If contest = "W" Then
                    btn.WhiteThreat = True
                ElseIf contest = "B" Then
                    btn.BlackThreat = True
                End If
                If btn.Piece = "King" And (fromButton.Side <> btn.Side) Then
                    fromButton.ThreateningAnyKing = True
                End If
            End If
        Next
    End Sub

    Public Function TestButtonAtString(str As String) As TestButton
        Dim y = str.Substring(0, 1)
        Dim x = str.Substring(1)
        For Each btn As TestButton In TestButtons
            Dim yL = CoordToPlace(btn.YPos)
            If x = btn.XPos AndAlso yL = y Then
                Return btn
            End If
        Next
        Return Nothing
    End Function

    Public Function TestButtonAtCoord(x As Integer, y As Integer) As TestButton
        For Each btn As TestButton In TestButtons
            Dim _x As Integer = Convert.ToInt32(btn.XPos)
            Dim _y As Integer = Convert.ToInt32(btn.YPos)
            If x = _x AndAlso y = _y Then
                Return btn
            End If
        Next
        Return Nothing
    End Function
    Public GameOver As Boolean = False
    Private Sub CheckWin()
        ' Will eventually check for checkmate and what not
        ' (lol look forward to that hell)
        ' We are playing as whites
        Me.Text = "Test Form | Current player: " & GetSide(CurrentGo.Substring(0, 1)) + " | Blacks: " & RemainingBlacks.ToString() + " | Whites: " + RemainingWhites.ToString()
        If RemainingWhites <= 0 Or RemainingBlacks <= 0 Then
            For Each btn As TestButton In TestButtons
                btn.Enabled = False
            Next
            btnResign.Enabled = False
            GameOver = True
            Dim totTime As String = ""
            Dim totalTime As TimeSpan = WhiteTimePlayed + BlackTimePlayed
            If totalTime.Hours > 1 Then
                totTime = totalTime.Hours.ToString() + "hours, "
            ElseIf totalTime.Hours = 1 Then
                totTime = " 1 hour, "
            End If
            If totalTime.Minutes > 1 Then
                totTime += totalTime.Minutes.ToString() + " minutes and "
            ElseIf totalTime.Minutes = 1 Then
                totTime += "1 minute and "
            End If
            If totalTime.Seconds > 1 Then
                totTime += totalTime.Seconds.ToString() + " seconds."
            ElseIf totalTime.Seconds = 1 Then
                totTime += " 1 second."
            Else
                ' no seconds
                totTime = totTime.Replace(" and ", String.Empty)
            End If
            Me.Text = "Test Form | Game over. | Total time for this game: " & totTime
        End If
        If RemainingWhites <= 0 Then
            MsgBox("Game over!" + vbCrLf & "Blacks wins.")
        End If
        If RemainingBlacks <= 0 Then
            MsgBox("Game over!" & vbCrLf & "Whites wins.")
        End If
    End Sub

    Public RemainingBlacks As Integer = 16
    Public RemainingWhites As Integer = 16
    Public Sub MovePiece(from As TestButton, _to As TestButton)
        If _to.Tag <> "" Then
            Dim fromSide As String = from.Side
            Dim fromPiece As String = from.Piece
            fromSide = GetSide(fromSide)
            Dim toSide As String = _to.Side
            Dim toPiece As String = _to.Piece
            toSide = GetSide(toSide)
            If toSide = "White" Then
                lblWhiteLost.Text += toPiece + vbCrLf
                RemainingWhites -= 1
            Else
                lblBlackLost.Text += toPiece + vbCrLf
                RemainingBlacks -= 1
            End If
            lblStatus.Text += fromSide + " " + from.Piece + " takes " + toSide + " " + toPiece + vbCrLf
            If toPiece = "King" Then
                If toSide = "White" Then
                    RemainingWhites = 0
                Else
                    RemainingBlacks = 0
                End If
            End If
        End If
        _to.Tag = from.Tag
        from.Tag = ""
        Start()
        'CheckWin()
        Switch()
        CalculatePlacesInContest()
    End Sub

    Private Sub Highlight_Cross(xL As Integer, yL As Integer, contest As String) '(horizontals + verticals)
        Dim actualPiece As TestButton = TestButtonAtCoord(xL, yL) 'the piece selected, clicked on

        Dim lastValidPiece As TestButton
        Dim blocked As Boolean 'if a piece is on the target square, if the path is blocked by a piece
        Dim amountInWay As Integer 'in the way of the selected piece and the king?

        Dim left As Integer() = {0, -1} 'it doesnt like it if i dont declare the variables?
        Dim right As Integer() = {0, 1}
        Dim up As Integer() = {1, 0}
        Dim down As Integer() = {-1, 0}

        For Each direction As Integer() In {left, right, up, down}
            lastValidPiece = Nothing 'resetting
            blocked = False
            amountInWay = 0
            For i As Integer = 1 To 8 Step 1

                Dim targetPos As TestButton = TestButtonAtCoord(xL + (i * direction(0)), yL + (i * direction(1)))
                If targetPos Is Nothing Then Continue For
                If targetPos.PlaceString = actualPiece.PlaceString Then Continue For
                If targetPos.Tag.Contains(CurrentGo) = False Then 'if it is not the player's own colour?
                    If blocked = False And (actualPiece.Pinned = PinType.None Or actualPiece.Pinned = If(direction(0) = 0, 0.3, 0.1)) Then 'if there isnt a piece on the target square AND movement is allowed on that axis
                        Highlight(xL + (i * direction(0)), yL + (i * direction(1)), Color.Blue, contest, actualPiece)
                        lastValidPiece = targetPos
                    End If
                    If targetPos.Side <> actualPiece.Side Then 'if the colour of the piece is different to the selected piece (white or black)
                        Try
                            If targetPos.Piece = "King" Then
                                lastValidPiece.Pinned = If(direction(0) = 0, 0.3, 0.1) 'switch off movement on other axises
                                '                       the above just works out whether the direction is horizontal or vertical
                                Exit For 'if a king is met, there is no point in looking past it because the piece cannot move past it anyway
                            Else
                                If blocked Then
                                    lastValidPiece.Pinned = PinType.None
                                    If targetPos.Tag <> "" Then Exit For
                                End If
                            End If
                        Catch ex As Exception
                            'sometimes lastValidPiece is nothing; 
                        End Try

                    End If
                    If targetPos.Tag <> "" Then 'if there is a piece on the square
                        amountInWay += 1
                        blocked = True
                    End If
                Else
                    Exit For
                End If
            Next
            If lastValidPiece IsNot Nothing AndAlso amountInWay > 2 Then lastValidPiece.Pinned = PinType.None
        Next
    End Sub

    Private Sub Highlight_Diagonals(xL As Integer, yL As Integer, contest As String)
        'same method as Highlight_Cross
        Dim actualPiece As TestButton = TestButtonAtCoord(xL, yL)

        Dim lastValidPiece As TestButton = Nothing
        Dim blocked As Boolean = False
        Dim amountInWay As Integer = 0

        Dim rightUp As Integer() = {1, 1} '/
        Dim rightDown As Integer() = {-1, 1} '\
        Dim leftDown As Integer() = {-1, -1} '/
        Dim leftUp As Integer() = {1, -1} '\

        For Each direction In {rightUp, rightDown, leftDown, leftUp}
            lastValidPiece = Nothing
            blocked = False
            amountInWay = 0

            For i As Integer = 1 To 8 Step 1
                Dim targetPos As TestButton = TestButtonAtCoord(xL + (i * direction(0)), yL + (i * direction(1)))
                If targetPos Is Nothing Then Continue For
                If targetPos.PlaceString = actualPiece.PlaceString Then Continue For
                If targetPos.Tag.Contains(CurrentGo) = False Then
                    If blocked = False And (actualPiece.Pinned = PinType.None Or actualPiece.Pinned = If(direction(0) - direction(1) = 0, 0.2, 0.4)) Then
                        Highlight(xL + (i * direction(0)), yL + (i * direction(1)), Color.Blue, contest, actualPiece)
                        lastValidPiece = targetPos
                    End If
                    If targetPos.Side <> actualPiece.Side Then
                        If targetPos.Piece = "King" Then
                            lastValidPiece.Pinned = If(direction(0) - direction(1) = 0, 0.2, 0.4) 'decides the direction
                            Exit For
                        Else
                            If blocked Then
                                lastValidPiece.Pinned = PinType.None
                            End If
                        End If
                    End If
                    If targetPos.Tag <> "" Then
                        amountInWay += 1
                        blocked = True
                    End If
                Else
                    Exit For
                End If
            Next
            If lastValidPiece IsNot Nothing Then
                If amountInWay > 2 Then lastValidPiece.Pinned = PinType.None
                If lastValidPiece.YPos = actualPiece.YPos AndAlso lastValidPiece.Piece = "Pawn" Then
                    lastValidPiece.Pinned = PinType.None
                End If
            End If
        Next
    End Sub

    Private Sub HighLightPiece(piece As String, xL As Integer, yL As Integer, Optional contest As String = "") 'when a new piece is selected
        Dim actualPiece As TestButton = TestButtonAtCoord(xL, yL)
        If piece = "Pawn" Then
            ' They can only move ahead if space is empty.
            ' They can only capture diagonal if it is not empty
            If actualPiece.Pinned = PinType.Horizontal Then Return 'if it is pinned horizontally (somehow) then there is no way the pawn can move
            Dim toAdd As Integer = If(CurrentGo = "B_", -1, 1) 'towards the opposite side
            Dim ahead As TestButton = TestButtonAtCoord(xL + toAdd, yL)
            If (ahead IsNot Nothing AndAlso ahead.Tag = "") And (actualPiece.Pinned = PinType.None Or actualPiece.Pinned = PinType.Vertical) Then
                Highlight(xL + toAdd, yL, Color.Blue, contest, actualPiece)
            End If
            Dim aheadLeft As TestButton = TestButtonAtCoord(xL + toAdd, yL - toAdd)
            Dim aheadRight As TestButton = TestButtonAtCoord(xL + toAdd, yL + toAdd)
            If aheadLeft IsNot Nothing And (actualPiece.Pinned = PinType.None Or actualPiece.Pinned = PinType.DiagonalLeft) Then
                If aheadLeft.Tag <> "" Then
                    Highlight(xL + toAdd, yL - toAdd, Color.Blue, contest, actualPiece)
                ElseIf aheadLeft.Tag = "" AndAlso contest = actualPiece.Side Then
                    Highlight(xL + toAdd, yL - toAdd, Color.Blue, contest, actualPiece)
                End If
            End If
            If aheadRight IsNot Nothing And (actualPiece.Pinned = PinType.None Or actualPiece.Pinned = PinType.DiagonalRight) Then
                If aheadRight.Tag <> "" Then
                    Highlight(xL + toAdd, yL + toAdd, Color.Blue, contest, actualPiece)
                ElseIf aheadRight.Tag = "" AndAlso contest = actualPiece.Side Then
                    Highlight(xL + toAdd, yL + toAdd, Color.Blue, contest, actualPiece)
                End If
            End If
            Dim twoAhead = TestButtonAtCoord(xL + (toAdd * 2), yL)
            If ((xL = 2 Or xL = 7) AndAlso twoAhead IsNot Nothing AndAlso twoAhead.Tag = "") And (actualPiece.Pinned = PinType.None Or actualPiece.Pinned = PinType.Vertical) Then
                If ahead IsNot Nothing AndAlso ahead.Tag = "" Then
                    Highlight(xL + (toAdd * 2), yL, Color.Blue, contest, actualPiece) ' Can only move ahead if they are both empty
                End If
            End If
        ElseIf piece = "Rook" Then
            Highlight_Cross(xL, yL, contest)
        ElseIf piece = "Bishop" Then
            Highlight_Diagonals(xL, yL, contest)
        ElseIf piece = "Queen" Then
            Highlight_Cross(xL, yL, contest)
            Highlight_Diagonals(xL, yL, contest)
        ElseIf piece = "King" Then

            Dim allPlaces As New List(Of Tuple(Of Integer, Integer)) 'From {topLeft, topCenter, topRight, centerLeft, centerRight, bottomLeft, bottomCenter, bottomRight}
            For x = -1 To 1 'all around the king
                For y = -1 To 1
                    If x = 0 And y = 0 Then Continue For
                    allPlaces.Add(New Tuple(Of Integer, Integer)(xL + x, yL + y))
                Next
            Next

            Dim remPlaces As Integer = allPlaces.Count
            For Each plc As Tuple(Of Integer, Integer) In allPlaces
                Dim btn As TestButton = TestButtonAtCoord(plc.Item1, plc.Item2)
                If btn Is Nothing Then Continue For
                Dim temp As String = contest.Replace("+", String.Empty)
                Dim dontHighlight As Boolean = False
                If temp <> "" Then
                    If btn.Tag <> "" Then
                        ' Something already there, so we cant move.
                        remPlaces -= 1
                        If btn.Side + "_" = Opponent And actualPiece.Side <> btn.Side Then
                            If contest = "" Or contest.Contains("+") Then
                                Highlight(plc.Item1, plc.Item2, Color.Red, "", actualPiece)
                                dontHighlight = True
                                remPlaces += 1 ' allow them to take opponent to save self
                            End If
                        Else
                            Continue For
                        End If
                    End If
                    If temp = "B" Then
                        If btn.WhiteThreat Then
                            remPlaces -= 1
                            Continue For
                        End If
                    Else
                        If btn.BlackThreat Then
                            remPlaces -= 1
                            Continue For
                        End If
                    End If
                End If
                If dontHighlight = False Then
                    Highlight(plc.Item1, plc.Item2, Color.Blue, contest, actualPiece)
                End If
            Next
            If remPlaces = 0 Then
                If CurrentGo = "W_" Then
                    RemainingWhites = 0
                Else
                    RemainingBlacks = 0
                End If
                CheckWin()
                Return
            End If
        ElseIf piece = "Knight" Then
            ' 2 along, 1 up
            If actualPiece.Pinned <> 0 Then Return 'i dont think a knight can move anywhere if it is pinned?
            For Each pos In {-2, -1, 1, 2}
                Highlight(xL + pos, yL + If(pos ^ 2 = 1, 2, 1), Color.Blue, contest, actualPiece)
                Highlight(xL + pos, yL - If(pos ^ 2 = 1, 2, 1), Color.Blue, contest, actualPiece)
            Next

        End If
    End Sub

    Private TestButtonSelected As TestButton = Nothing
    Private Sub TestButtonSelect(sender As Object, e As EventArgs)
        CalculatePlacesInContest()
        Dim but As TestButton = CType(sender, TestButton)
        If but.Pinned = PinType.Fully Then Return
        Dim xL As String = but.XPos
        Dim yL As String = but.YPos
        Dim isBlack As Boolean = but.BackColor = Color.Gray
        Dim fullName As String = but.Tag
        Dim side As String = ""
        Dim piece As String = ""
        Dim whiteContest As Boolean = but.WhiteThreat
        Dim blackContest As Boolean = but.BlackThreat
        If String.IsNullOrWhiteSpace(fullName) Then
        Else
            side = but.Side
            piece = but.Piece
        End If

        If TestButtonSelected Is Nothing Then
            If but.Side + "_" = CurrentGo AndAlso CurrentGo = WeArePlayingFor Then
                If debug Then
                    For Each btn As TestButton In TestButtons
                        btn.BackColor = Color.Blue
                        If btn.Tag <> "" Then btn.BackColor = Color.Red
                    Next
                    but.BackColor = Color.FromKnownColor(KnownColor.Control)
                    TestButtonSelected = but
                    Return
                End If
                HighLightPiece(piece, xL, yL, but.Side + "+")
                TestButtonSelected = but
            End If
        Else
            'Testbuttonselected is the piece selected to move
            'but is the button to move to
            Dim fromSide As String = TestButtonSelected.Side
            If fromSide + "_" <> WeArePlayingFor Then
                Return
            End If
            fromSide = GetSide(fromSide)
            If but.takeable Then
                Dim result = Client.TryMovePiece(CoordToPlace(TestButtonSelected.YPos) + TestButtonSelected.XPos.ToString(), CoordToPlace(yL) + xL.ToString())
                If result = "" Then

                    lblStatus.Text += fromSide + " " + TestButtonSelected.Piece + " to " + CoordToPlace(yL) + xL.ToString() + vbCrLf
                    MovePiece(TestButtonSelected, but)
                Else
                    MsgBox("Unable to move: " + result)
                End If
            End If
            TestButtonSelected = Nothing
            HighlightReset()
            If but.Piece = "Pawn" AndAlso GameOver = False Then
                If but.XPos = 8 Or but.XPos = 1 Then
                    ' Promotion
                    Dim prom As New Promotion
                    waitingForPromotion = True
                    prom.SetPlace(but.PlaceString)
                    prom.ShowDialog()
                    waitingForPromotion = False
                    Dim type = prom.Choice
                    Dim attempt = Client.TryPromotePiece(but.PlaceString, type)
                    If String.IsNullOrWhiteSpace(attempt) Then
                        but.Piece = type.ToString()
                        lblStatus.Text += but.PlaceString + " promoted to " + but.Side + " " + but.Piece
                        HighlightReset()
                    Else
                        MsgBox("Unable to promote: " + attempt, MsgBoxStyle.Critical)
                    End If
                End If
            End If
        End If
        CalculatePlacesInContest()
    End Sub
    Private waitingForPromotion = False

    Private HoveredButton As TestButton = Nothing
    Private Sub TestMouseEnter(sender As Object, e As EventArgs)
        Dim btn As TestButton = CType(sender, TestButton)
        HoveredButton = btn
    End Sub

    Private Sub TestMouseLeave(sender As Object, e As EventArgs)
        HoveredButton = Nothing
    End Sub

    Private Sub CalculatePlacesInContest()
        If GameOver Then Return
        Dim resSide As String = CurrentGo
        For Each btn As TestButton In TestButtons
            CurrentGo = btn.Side + "_"
            If debug Then
                btn.Text = ""
                If btn.BlackThreat Then
                    btn.Text += "Black" + vbCrLf
                    btn.ForeColor = Color.Purple
                End If
                If btn.WhiteThreat Then
                    btn.Text += "White" + vbCrLf
                    btn.ForeColor = Color.Cyan
                End If
                If btn.WhiteThreat And btn.BlackThreat Then
                    btn.ForeColor = Color.Yellow
                End If
            End If
            If btn.Tag = "" Then
                Continue For
            End If
            Highlight(btn.XPos, btn.YPos, Color.Blue, btn.Side, btn)
            HighLightPiece(btn.Piece, btn.XPos, btn.YPos, btn.Side)
            If btn.ThreateningBlackKing And resSide = "B_" Then
                btn.BackColor = Color.DarkRed
            ElseIf btn.ThreateningWhiteKing And resSide = "W_" Then
                btn.BackColor = Color.DarkRed
            End If
            If btn.Pinned <> 0 Then
                If btn.Side + "_" = resSide Then
                    btn.BackColor = Color.Yellow
                End If
            End If
        Next
        CurrentGo = resSide
        'For i As Integer = TestButtons.Count - 1 To 0 Step -1
        'Dim btn As TestButton = TestButtons(i)
        'If btn.Side = "W" Then Exit For
        '    If btn.Tag = "" Then Exit For
        'HighLightPiece(btn.Piece, btn.XPos, btn.YPos, btn.Side)
        'Next
    End Sub

    Private Sub Form1_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        'CalculatePlacesInContest()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnResign.Click
        Dim confirm As Integer = MsgBox("Confirm that you (" & GetSide(CurrentGo.Replace("_", String.Empty)) + ") are going to surrender?", vbOKCancel)
        If confirm = vbOK Then
            Client.Send("RESIGN")
        End If
    End Sub

    Private Sub btnHelp_Click(sender As Object, e As EventArgs) Handles btnHelp.Click
        MsgBox("This is Test. Most rules from original Test do apply; here are some things specific to my game you should understand:" & vbCrLf & vbCrLf _
               & "You may need to click twice on a piece in order to move it correctly" & vbCrLf _
               & "Any blue highlighted squares can be moved to; any red ones will take a piece" & vbCrLf _
               & "If a piece is yellow, that means it is pinned - it is protecting your king from check and you mustn't move it" & vbCrLf _
               & "If a piece is dark red, that means it is currently checking your king" & vbCrLf & vbCrLf _
               & "You may resign at any time by clicking the 'Resign' button." & vbCrLf _
               & "The game ends when either: your king is took, you have no pieces, or the king cannot move - ie checkmate (buggy)")
    End Sub
    Dim debug As Boolean = False
    Private Sub lblBlackTimer_DoubleClick(sender As Object, e As EventArgs) Handles lblBlackTimer.DoubleClick
        debug = Not debug
        If debug Then
            btnResign.Text = "Debug"
        Else
            btnResign.Text = "Resign"
            lblWhiteTimer_DoubleClick(lblWhiteTimer, Nothing)
        End If
        HighlightReset()
    End Sub

    Dim beforeDebugChange As String = ""
    Private Sub lblWhiteTimer_DoubleClick(sender As Object, e As EventArgs) Handles lblWhiteTimer.DoubleClick
        If beforeDebugChange <> "" Then
            CurrentGo = beforeDebugChange
            beforeDebugChange = ""
            btnHelp.Text = "?"
        Else
            If Not debug Then Return
            beforeDebugChange = CurrentGo
            If CurrentGo = "W_" Then
                CurrentGo = "B_"
            Else
                CurrentGo = "W_"
            End If
            btnHelp.Text = CurrentGo.Substring(0, 1)
        End If
    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
        If e.KeyCode = Keys.P Then
            SetPause(Not panel_Hide.Visible)
        End If
        If debug Then
            panel_Hide.Visible = False
            If HoveredButton Is Nothing Then Return
            If e.KeyCode = Keys.Delete Then
                If HoveredButton.Piece = "King" Then Return
                HoveredButton.Tag = ""
            ElseIf e.KeyCode = Keys.Insert Then
                Dim selPiece As TestButton = TestButtonAtCoord(HoveredButton.XPos, HoveredButton.YPos)
                If selPiece.Tag <> "" Then
                    If MsgBox("Are you sure you want to override the " & selPiece.Piece & " at " & selPiece.PlaceString & "?", vbYesNo, "Confirmation") = vbNo Then
                        Return
                    End If
                End If
                waitingForPromotion = True
                Dim addition As New Promotion
                addition.AddDebugPlace(selPiece.PlaceString)
                addition.ShowDialog()
                waitingForPromotion = False
                selPiece.Tag = CurrentGo + addition.Choice
            End If
            HighlightReset()
        End If
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        End 'i dont think the process is ending once the form closes? or maybe my computer is dumb idk
    End Sub

    Private Sub Form1_Deactivate(sender As Object, e As EventArgs) Handles MyBase.Deactivate
        ' This is called whenever focus to the game form is lost
        If waitingForPromotion Then Return ' Promoting shouldnt pause
        If debug Then Return ' We dont hide in debug (could be running debugger or something)
        'SetPause(True)
    End Sub

    Private Sub SetPause(bool As Boolean)
        panel_Hide.Visible = bool
        paused = bool
        lblPause.Text = "GAME PAUSED" & vbCrLf & "Press P to continue" & vbCrLf & $"Current Player: {GetSide(CurrentGo.Substring(0, 1))}"
    End Sub

    Private Sub btnSaveGame_Click(sender As Object, e As EventArgs) Handles btnSaveGame.Click
        Dim save As New GameSave()
        save.BlackTime = BlackTimePlayed
        save.WhiteTime = WhiteTimePlayed
        save.PlayingFor = CurrentGo
        save.BlackLost = lblBlackLost.Text
        save.WhiteLost = lblWhiteLost.Text
        save.RemainingBlacks = RemainingBlacks
        save.RemainingWhites = RemainingWhites
        save.MoveLog = lblStatus.Text
        save.SaveVersion = Application.ProductVersion
        save.Locations = New List(Of ButtonLocation)
        save.BlackName = BlackPlayer
        save.WhiteName = WhitePlayer
        For Each loc As TestButton In TestButtons
            Dim newLoc As New ButtonLocation()
            newLoc.Name = loc.Name
            newLoc.Tag = loc.Tag
            save.Locations.Add(newLoc)
        Next
        Dim output As String = "" '.SerializeObject(save)
        Dim saveFileDialog1 As New SaveFileDialog()
        saveFileDialog1.Filter = "Game files (*.game)|*.game"
        saveFileDialog1.FilterIndex = 2
        saveFileDialog1.RestoreDirectory = True
        saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory
        If saveFileDialog1.ShowDialog() = DialogResult.OK Then
            Try
                Dim filePath As String = saveFileDialog1.FileName
                File.WriteAllText(filePath, output)
                MsgBox("Game Successfully saved, location:" + vbCrLf + filePath)
            Catch ex As Exception
                MsgBox("Error: " + ex.ToString())
            End Try
        End If
    End Sub

    Private Sub btnLoadGame_Click(sender As Object, e As EventArgs) Handles btnLoadGame.Click
        Dim myStream As Stream = Nothing
        Dim file As String = ""
        Dim openFileDialog1 As New OpenFileDialog()
        openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory()
        openFileDialog1.Filter = "Game files (*.game)|*.game"
        openFileDialog1.FilterIndex = 2
        openFileDialog1.RestoreDirectory = True
        If openFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Try
                myStream = openFileDialog1.OpenFile()
                If (myStream IsNot Nothing) Then
                    Using reader = New StreamReader(myStream)
                        file = reader.ReadToEnd()
                    End Using
                End If
            Catch Ex As Exception
                MessageBox.Show("Cannot read file from disk. Original error: " & Ex.Message)
            Finally
                If (myStream IsNot Nothing) Then
                    myStream.Close()
                End If
            End Try
        End If
        If file <> "" Then
            ' Got file successfully.
            Try
                Dim save As GameSave = New GameSave() '"" '.DeserializeObject(Of GameSave)(file)
                ' Check that the save is of correct version
                If save.SaveVersion <> Application.ProductVersion Then
                    MsgBox("Error: that save is of a previous version and is not valid")
                    Return
                End If
                ' Check to see if some things are missing, if they are.. it is corrupt
                If save.Locations Is Nothing Then
                    Throw New ArgumentNullException("Locations")
                End If
                If save.BlackLost Is Nothing Then
                    Throw New ArgumentNullException("BlackLost")
                End If
                If save.MoveLog Is Nothing Then
                    Throw New ArgumentNullException("MoveLog")
                End If
                If save.BlackName Is Nothing Or save.WhiteName Is Nothing Then
                    Throw New ArgumentNullException("Player Names")
                End If
                If MsgBox("Is this the correct game:" + vbCrLf +
                          $"Resuming Player: {If(GetSide(save.PlayingFor(0)) = "White", save.WhiteName, save.BlackName)}" + vbCrLf +
                          $"Other Player: {If(GetSide(save.PlayingFor(0)) = "White", save.BlackName, save.WhiteName)}" + vbCrLf +
                          $"White Play Time: {save.WhiteTime}" + vbCrLf +
                          $"Black Play Time: {save.BlackTime}", vbYesNo) = vbNo Then
                    Return
                End If
                WhiteTimePlayed = save.WhiteTime
                BlackTimePlayed = save.BlackTime
                lblBlackLost.Text = save.BlackLost
                lblWhiteLost.Text = save.WhiteLost
                RemainingWhites = save.RemainingWhites
                RemainingBlacks = save.RemainingBlacks
                BlackPlayer = save.BlackName
                WhitePlayer = save.WhiteName
                lblStatus.Text = save.MoveLog
                If CurrentGo <> save.PlayingFor Then
                    Switch()
                End If
                Dim i As Integer = 0
                For Each loc As ButtonLocation In save.Locations
                    Dim oldBtn As TestButton = TestButtons(i)
                    Me.Controls.Remove(oldBtn) ' We need to remove the old button
                    Dim newC As New TestButton()
                    newC.Name = loc.Name
                    newC.Tag = loc.Tag
                    newC.Location = oldBtn.Location
                    newC.Size = oldBtn.Size
                    oldBtn.Dispose() ' Then add the new one and all its handles
                    AddHandler newC.Click, AddressOf TestButtonSelect
                    AddHandler newC.MouseEnter, AddressOf TestMouseEnter
                    AddHandler newC.MouseLeave, AddressOf TestMouseLeave
                    TestButtons(i) = newC
                    Me.Controls.Add(newC)
                    i += 1
                Next
                HighlightReset()
                Start() ' gets timers/labels ready
                MsgBox("Successfully loaded game." + vbCrLf + "Unpause to continue playing")
            Catch ex As Exception
                MsgBox("Error: unable to load that save, it may be corrupt or incorrectly formatted" + vbCrLf + "We must now quit." + vbCrLf + "Info: " + ex.Message)
                Me.Close()
            End Try
        End If
    End Sub

    Private Sub UpdateNameLabels()
        Me.Invoke(Sub()
                      lblBlackName.Text = BlackPlayer
                      lblWhiteName.Text = WhitePlayer
                  End Sub)
    End Sub

    Private Sub discordTimer_Tick(sender As Object, e As EventArgs) Handles discordTimer.Tick
        If Discord IsNot Nothing Then
            Discord.Invoke()
        End If
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Discord IsNot Nothing Then
            Discord.Dispose()
        End If
    End Sub

    Private Sub uiTimer_Tick(sender As Object, e As EventArgs) Handles uiTimer.Tick
        UpdateNameLabels()
        Me.Text = $"{WhitePlayer} {BlackPlayer} {CurrentGo} {WeArePlayingFor} {Client?.Name}"
    End Sub

    Private Sub lblPause_DoubleClick(sender As Object, e As EventArgs) Handles lblPause.DoubleClick
        Dim connLocalhost = False
        Me.Invoke(Sub()
                      Me.lblPause.Text = "Attempting connection to localhost..."
                  End Sub)
        Try
            Connection(Net.IPAddress.Loopback)
            connLocalhost = True
        Catch ex As Exception
        End Try
        If Not connLocalhost Then
            Me.Invoke(Sub()
                          Me.lblPause.Text = "Local host failed" + vbCrLf + "Enter an IP..."
                      End Sub)
            Dim ip = InputBox("Enter the IP address", "IP Input", "10.249.74.120")
            If Not String.IsNullOrWhiteSpace(ip) Then
                Dim addr As Net.IPAddress
                If Net.IPAddress.TryParse(ip, addr) Then
                    Connection(addr)
                End If
            End If
        End If
    End Sub
End Class