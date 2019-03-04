Imports Test.Form1

Partial Public Class TestButton
    Inherits Windows.Forms.Button
    Private CoordToPlace As New Dictionary(Of Integer, String) From {{1, "A"}, {2, "B"}, {3, "C"}, {4, "D"}, {5, "E"}, {6, "F"}, {7, "G"}, {8, "H"}}
    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)

        'Add your custom paint code here
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        'InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Public takeable As Boolean = False

    Public ReadOnly Property XPos As String
        Get
            Return MyBase.Name.Split(",")(0)
        End Get
    End Property
    Public ReadOnly Property YPos As String
        Get
            Return MyBase.Name.Split(",")(1)
        End Get
    End Property
    ''' <summary>
    ''' Colour of the piece
    ''' </summary>
    ''' <returns>One charactar: W = White, B = Black</returns>
    Public ReadOnly Property Side As String
        Get
            If String.IsNullOrWhiteSpace(MyBase.Tag) Then Return ""
            Return MyBase.Tag.Substring(0, 1)
        End Get
    End Property
    Public Property Piece As String
        Get
            If String.IsNullOrWhiteSpace(MyBase.Tag) Then Return ""
            Return MyBase.Tag.substring(2)
        End Get
        Set(value As String)
            Dim sd As String = Me.Side
            MyBase.Tag = sd + "_" + value
        End Set
    End Property
    Private whiteContest As Boolean = False
    Private blackContest As Boolean = False
    ''' <summary>
    ''' Determines whether or not a white piece is capable of taking this square
    ''' </summary>
    Public Property WhiteThreat As Boolean
        Get
            If Side = "W" AndAlso whiteContest = False Then
                whiteContest = True
            End If
            Return whiteContest
        End Get
        Set(value As Boolean)
            whiteContest = value
        End Set
    End Property
    ''' <summary>
    ''' Determines whether or not a black piece is capable of taking this square
    ''' </summary>
    Public Property BlackThreat As Boolean
        Get
            If Side = "B" AndAlso blackContest = False Then
                blackContest = True
            End If
            Return blackContest
        End Get
        Set(value As Boolean)
            blackContest = value
        End Set
    End Property
    Private ThreateningKing As Boolean = False
    ''' <summary>
    ''' Determines if this piece is threatening the black king
    ''' </summary>
    ''' <returns></returns>
    Public Property ThreateningBlackKing As Boolean
        Get
            Return ThreateningKing And Me.Side = "W"
        End Get
        Set(value As Boolean)
            ThreateningKing = value
        End Set
    End Property
    ''' <summary>
    ''' Determines whether this piece is threatening the white king
    ''' </summary>
    ''' <returns></returns>
    Public Property ThreateningWhiteKing As Boolean
        Get
            Return ThreateningKing And Me.Side = "B"
        End Get
        Set(value As Boolean)
            ThreateningKing = value
        End Set
    End Property
    Public Property ThreateningAnyKing As Boolean
        Get
            Return ThreateningKing
        End Get
        Set(value As Boolean)
            ThreateningKing = value
        End Set
    End Property

    Private isPinned As PinType
    Public Property Pinned As PinType
        Get
            If Piece = "King" Then isPinned = 0
            Return isPinned
        End Get
        Set(value As PinType)
            If value = PinType.None Or value = isPinned Then
                isPinned = value
            Else 'if the piece is pinned on more than one axis
                isPinned = 1
            End If
            If Piece = "King" Then isPinned = 0 'if it already changes it in the get, we may not have to set it?
        End Set
    End Property
    ''' <summary>
    ''' Test-proper place string, eg A1; C6; D3 etc.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property PlaceString As String
        Get
            Return CoordToPlace(YPos) + XPos
        End Get
    End Property
End Class

