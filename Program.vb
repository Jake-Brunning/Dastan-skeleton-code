'Skeleton Program code for the AQA A Level Paper 1 Summer 2023 examination
'this code should be used in conjunction with the Preliminary Material
'written by the AQA Programmer Team
'developed in the Visual Studio Community Edition programming environment

Imports System

Module Module1
    Sub Main()
        Dim ThisGame As Dastan = New Dastan(6, 6, 4)
        ThisGame.PlayGame()
        Console.WriteLine("Goodbye!")
        Console.ReadLine()
    End Sub

    Class Dastan
        'Ive got no clue why these need to be protected, as nothing inherits this class.
        Protected Board As List(Of Square) 'the board itself
        Protected NoOfRows, NoOfColumns, MoveOptionOfferPosition As Integer 'moveOptionOfferPosition is the index offer being displayed (based on an index of a fixed list defined in createMoveOptionOffer)
        Protected Players As New List(Of Player) 'Currently holds 2, program will (essentially) ignore more than 2 players
        Protected MoveOptionOffer As New List(Of String) 'the list of offer for moves. Seperate from each player's move option list
        Protected CurrentPlayer As Player 'the player doing his turn. Changed at the end of the play game while loop
        Protected RGen As New Random()

        Sub New(ByVal R As Integer, ByVal C As Integer, ByVal NoOfPieces As Integer) 'Initilses attributes (just a normal constructor)
            Players.Add(New Player("Player One", 1))
            Players.Add(New Player("Player Two", -1))
            CreateMoveOptions()
            NoOfRows = R
            NoOfColumns = C
            MoveOptionOfferPosition = 0
            CreateMoveOptionOffer()
            CreateBoard()
            CreatePieces(NoOfPieces)
            CurrentPlayer = Players(0)
        End Sub

        Private Sub DisplayBoard() 'self explanatory
            Console.Write(Environment.NewLine & "   ")
            'Write column numbers
            For Column = 1 To NoOfColumns
                Console.Write(Column.ToString() & "  ")
            Next
            Console.Write(Environment.NewLine & "  ")
            'Line gap between column and actual board
            For Count = 1 To NoOfColumns
                Console.Write("---")
            Next
            Console.WriteLine("-")
            For Row = 1 To NoOfRows
                Console.Write(Row.ToString() & " ") 'row number
                For Column = 1 To NoOfColumns 'spaces
                    Dim Index As Integer = GetIndexOfSquare(Row * 10 + Column)
                    Console.Write("|" & Board(Index).GetSymbol())
                    'Display pieces
                    Dim PieceInSquare As Piece = Board(Index).GetPieceInSquare()
                    If PieceInSquare Is Nothing Then
                        Console.Write(" ")
                    Else
                        Console.Write(PieceInSquare.GetSymbol())
                    End If
                Next
                Console.WriteLine("|")
            Next
            Console.Write("  -")
            For Column = 1 To NoOfColumns
                Console.Write("---")
            Next
            Console.WriteLine()
            Console.WriteLine()
        End Sub

        Private Sub DisplayState() 'the text under the board stating the move option offer, and which player's turn
            DisplayBoard()
            Console.WriteLine("Move option offer: " & MoveOptionOffer(MoveOptionOfferPosition))
            Console.WriteLine()
            Console.WriteLine(CurrentPlayer.GetPlayerStateAsString()) 'displays the info seen about the player, so score and thier move queue
            Console.WriteLine("Turn: " & CurrentPlayer.GetName())
            Console.WriteLine()
        End Sub

        Private Function GetIndexOfSquare(ByVal SquareReference As Integer) As Integer 'turns 2 digit integer number to 2 seperate numbers , which indicates an index in the board array
            Dim Row As Integer = SquareReference \ 10
            Dim Col As Integer = SquareReference Mod 10
            Return (Row - 1) * NoOfColumns + (Col - 1) '(row - 1) and (col - 1) to convert to array indexes (as array starts at 0 and user input for rows and columns start at 1)
        End Function

        Private Function CheckSquareInBounds(ByVal SquareReference As Integer) As Boolean 'checks if a square is on the board
            Dim Row As Integer = SquareReference \ 10
            Dim Col As Integer = SquareReference Mod 10
            If Row < 1 Or Row > NoOfRows Then
                Return False
            ElseIf Col < 1 Or Col > NoOfColumns Then
                Return False
            Else
                Return True
            End If
        End Function

        Private Function CheckSquareIsValid(ByVal SquareReference As Integer, ByVal StartSquare As Boolean) As Boolean 'Checks if THE SQUARE BEING MOVED TO is valid (not the move itself)
            'start square shows if the function call is for selecting a piece (true) or moving a piece (false)
            'squareReference is the square the user has selected
            If Not CheckSquareInBounds(SquareReference) Then 'if square is of the board
                Return False
            End If
            Dim PieceInSquare As Piece = Board(GetIndexOfSquare(SquareReference)).GetPieceInSquare() 'Gets the piece in the square the user has selected
            If PieceInSquare Is Nothing Then 'if theres no piece in the square then
                If StartSquare Then 'if selecting a piece
                    Return False
                Else 'if moving a piece
                    Return True
                End If
            ElseIf CurrentPlayer.SameAs(PieceInSquare.GetBelongsTo()) Then 'if the piece in the square is the same team as the current player
                If StartSquare Then 'if selecting a piece
                    Return True
                Else 'if moving a piece
                    Return False
                End If
            Else
                If StartSquare Then
                    Return False
                Else
                    Return True
                End If
            End If
        End Function

        Private Function CheckIfGameOver() As Boolean 'Checks if both players have a mirza, and returns true if any of the mirzas are missing. Also returns true when a mirza is in a opposing players fortress
            Dim Player1HasMirza As Boolean = False
            Dim Player2HasMirza As Boolean = False
            For Each S In Board
                Dim PieceInSquare As Piece = S.GetPieceInSquare()
                If PieceInSquare IsNot Nothing Then 'if there is a piece in the square
                    If S.ContainsKotla() And PieceInSquare.GetTypeOfPiece() = "mirza" And Not PieceInSquare.GetBelongsTo().SameAs(S.GetBelongsTo()) Then 'If a mirza is in a opposing players fortress
                        Return True
                    ElseIf PieceInSquare.GetTypeOfPiece() = "mirza" And PieceInSquare.GetBelongsTo().SameAs(Players(0)) Then 'If there is a mirza in the square being looked ata
                        Player1HasMirza = True
                    ElseIf PieceInSquare.GetTypeOfPiece() = "mirza" And PieceInSquare.GetBelongsTo().SameAs(Players(1)) Then 'If there is a mirza in the square being looked ata
                        Player2HasMirza = True
                    End If
                End If
            Next
            Return Not (Player1HasMirza And Player2HasMirza)
        End Function

        Private Function GetSquareReference(ByVal Description As String) As Integer 'gets a user input for entering a square to move to or to select a piece from the board (Any user input really)
            Dim SelectedSquare As Integer
            Console.Write("Enter the square " & Description & " (row number followed by column number): ")
            SelectedSquare = Console.ReadLine()
            Return SelectedSquare
        End Function

        Private Sub UseMoveOptionOffer() 'self explanatory (when the player chooses the offer displayed to them)
            Dim ReplaceChoice As Integer
            Console.Write("Choose the move option from your queue to replace (1 to 5): ")
            ReplaceChoice = Console.ReadLine()
            CurrentPlayer.UpdateMoveOptionQueueWithOffer(ReplaceChoice - 1, CreateMoveOption(MoveOptionOffer(MoveOptionOfferPosition), CurrentPlayer.GetDirection()))
            CurrentPlayer.ChangeScore(-(10 - (ReplaceChoice * 2)))
            MoveOptionOfferPosition = RGen.Next(0, 5) 'randomly choose the next move offer to display
        End Sub

        Private Function GetPointsForOccupancyByPlayer(ByVal CurrentPlayer As Player) As Integer 'Loops through board and adds points for occupance -> the only points for occupance which is possible to get is occupying kotlas. There is only 2 instances of this on the board (which are always a fixed position)
            Dim ScoreAdjustment As Integer = 0
            For Each S In Board
                ScoreAdjustment += (S.GetPointsForOccupancy(CurrentPlayer)) 'returns 0 if square, if kotla then the (return 0) function is overrided and returns the correct amount of score for occupying a kotla
            Next
            Return ScoreAdjustment
        End Function

        Private Sub UpdatePlayerScore(ByVal PointsForPieceCapture As Integer) 'Changes player score, but only the adding options ( so occupancy of kotla + capturing pieces). should be called at the end of the turn ( so not getting points for occupancy twice)
            CurrentPlayer.ChangeScore(GetPointsForOccupancyByPlayer(CurrentPlayer) + PointsForPieceCapture)
        End Sub

        Private Function CalculatePieceCapturePoints(ByVal FinishSquareReference As Integer) As Integer
            If Board(GetIndexOfSquare(FinishSquareReference)).GetPieceInSquare() IsNot Nothing Then 'If there is a piece in the square
                Return Board(GetIndexOfSquare(FinishSquareReference)).GetPieceInSquare().GetPointsIfCaptured() 'gets the square being moved to uses that to get the piece in the square, and then calls GetPointsIfCapturd (function in class piece)
            End If
            Return 0
        End Function

        Public Sub PlayGame()
            Dim GameOver As Boolean = False
            While Not GameOver
                DisplayState()
                Dim SquareIsValid As Boolean = False
                Dim Choice As Integer
                Dim scoreBeforeMove As Integer
                Do 'loop unitl a correct move option from queue is picked
                    Console.Write("Choose move option to use from queue (1 to 3) or 9 to take the offer: ")
                    Choice = Console.ReadLine()
                    If Choice = 9 Then 'using move offer
                        UseMoveOptionOffer()
                        DisplayState()
                    End If
                Loop Until Choice >= 1 And Choice <= 3
                Dim StartSquareReference As Integer
                While Not SquareIsValid 'loop unitl a piece has been selected
                    StartSquareReference = GetSquareReference("containing the piece to move")
                    SquareIsValid = CheckSquareIsValid(StartSquareReference, True)
                End While
                Dim FinishSquareReference As Integer
                SquareIsValid = False
                While Not SquareIsValid
                    FinishSquareReference = GetSquareReference("to move to")
                    SquareIsValid = CheckSquareIsValid(FinishSquareReference, False) 'Will return true as long as the square is valid, not the move
                End While
                Dim MoveLegal As Boolean = CurrentPlayer.CheckPlayerMove(Choice, StartSquareReference, FinishSquareReference) 'checks if the move is in the possible move options for that move
                If MoveLegal Then 'Perform the move (if its legal)
                    scoreBeforeMove = CurrentPlayer.GetScore()
                    Dim PointsForPieceCapture As Integer = CalculatePieceCapturePoints(FinishSquareReference)
                    CurrentPlayer.ChangeScore(-(Choice + (2 * (Choice - 1))))
                    CurrentPlayer.UpdateQueueAfterMove(Choice)
                    UpdateBoard(StartSquareReference, FinishSquareReference)
                    UpdatePlayerScore(PointsForPieceCapture)
                    Console.WriteLine("New score: " & CurrentPlayer.GetScore() & Environment.NewLine)
                End If

                DisplayBoard()
                Console.WriteLine("Do you want to undo your move (1 to accept, 2 to decline)")
                Dim undoornot As Integer = Console.ReadLine()
                If undoornot = 1 Then
                    'reset score
                    CurrentPlayer.ChangeScore(-CurrentPlayer.GetScore())
                    CurrentPlayer.ChangeScore(scoreBeforeMove - 5)
                    'reset queue
                    CurrentPlayer.ResetQueueBackAfterUndo(Choice - 1)
                    'reset board
                    Dim temp As Square = Board(GetIndexOfSquare(StartSquareReference))
                    Board(GetIndexOfSquare(StartSquareReference)) = Board(GetIndexOfSquare(FinishSquareReference))
                    Board(GetIndexOfSquare(FinishSquareReference)) = temp
                    Continue While
                End If

                If CurrentPlayer.SameAs(Players(0)) Then 'swap players
                    CurrentPlayer = Players(1)
                Else
                    CurrentPlayer = Players(0)
                End If
                GameOver = CheckIfGameOver()
            End While
            DisplayState()
            DisplayFinalResult()
        End Sub

        Private Sub UpdateBoard(ByVal StartSquareReference As Integer, ByVal FinishSquareReference As Integer) 'moves the piece from the start square to the finish square
            Board(GetIndexOfSquare(FinishSquareReference)).SetPiece(Board(GetIndexOfSquare(StartSquareReference)).RemovePiece())
        End Sub

        Private Sub DisplayFinalResult()
            If Players(0).GetScore() = Players(1).GetScore() Then
                Console.WriteLine("Draw!")
            ElseIf Players(0).GetScore() > Players(1).GetScore() Then
                Console.WriteLine(Players(0).GetName() & " is the winner!")
            Else
                Console.WriteLine(Players(1).GetName() & " is the winner!")
            End If
        End Sub

        Private Sub CreateBoard() 'Initilises the board
            Dim S As Square
            Board = New List(Of Square)
            For Row = 1 To NoOfRows
                For Column = 1 To NoOfColumns
                    If Row = 1 And Column = NoOfColumns \ 2 Then
                        S = New Kotla(Players(0), "K")
                    ElseIf Row = NoOfRows And Column = NoOfColumns \ 2 + 1 Then
                        S = New Kotla(Players(1), "k")
                    Else
                        S = New Square()
                    End If
                    Board.Add(S)
                Next
            Next
        End Sub

        Private Sub CreatePieces(ByVal NoOfPieces As Integer) 'initiles player 1 and 2 pieces and mirzas
            Dim CurrentPiece As Piece
            For Count = 1 To NoOfPieces
                CurrentPiece = New Piece("piece", Players(0), 1, "!")
                Board(GetIndexOfSquare(2 * 10 + Count + 1)).SetPiece(CurrentPiece)
            Next
            CurrentPiece = New Piece("mirza", Players(0), 5, "1")
            Board(GetIndexOfSquare(10 + NoOfColumns \ 2)).SetPiece(CurrentPiece)
            For Count = 1 To NoOfPieces
                CurrentPiece = New Piece("piece", Players(1), 1, """")
                Board(GetIndexOfSquare((NoOfRows - 1) * 10 + Count + 1)).SetPiece(CurrentPiece)
            Next
            CurrentPiece = New Piece("mirza", Players(1), 5, "2")
            Board(GetIndexOfSquare(NoOfRows * 10 + (NoOfColumns \ 2 + 1))).SetPiece(CurrentPiece)
        End Sub

        Private Sub CreateMoveOptionOffer() 'the move option offers for both players to use. seperate from the move queue
            MoveOptionOffer.Add("jazair")
            MoveOptionOffer.Add("chowkidar")
            MoveOptionOffer.Add("cuirassier")
            MoveOptionOffer.Add("ryott")
            MoveOptionOffer.Add("faujdar")
        End Sub
        '-----Following functions make the list of possible move options to each Move option----------
        Private Function CreateRyottMoveOption(ByVal Direction As Integer) As MoveOption
            Dim NewMoveOption As MoveOption = New MoveOption("ryott")
            Dim NewMove As Move = New Move(0, 1 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(0, -1 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(1 * Direction, 0)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(-1 * Direction, 0)
            NewMoveOption.AddToPossibleMoves(NewMove)
            Return NewMoveOption
        End Function

        Private Function CreateFaujdarMoveOption(ByVal Direction As Integer) As MoveOption
            Dim NewMoveOption As MoveOption = New MoveOption("faujdar")
            Dim NewMove As Move = New Move(0, -1 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(0, 1 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(0, 2 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(0, -2 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            Return NewMoveOption
        End Function

        Private Function CreateJazairMoveOption(ByVal Direction As Integer) As MoveOption
            Dim NewMoveOption As MoveOption = New MoveOption("jazair")
            Dim NewMove As Move = New Move(2 * Direction, 0)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(2 * Direction, -2 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(2 * Direction, 2 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(0, 2 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(0, -2 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(-1 * Direction, -1 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(-1 * Direction, 1 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            Return NewMoveOption
        End Function

        Private Function CreateCuirassierMoveOption(ByVal Direction As Integer) As MoveOption
            Dim NewMoveOption As MoveOption = New MoveOption("cuirassier")
            Dim NewMove As Move = New Move(1 * Direction, 0)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(2 * Direction, 0)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(1 * Direction, -2 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(1 * Direction, 2 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            Return NewMoveOption
        End Function

        Private Function CreateChowkidarMoveOption(ByVal Direction As Integer) As MoveOption
            Dim NewMoveOption As MoveOption = New MoveOption("chowkidar")
            Dim NewMove As Move = New Move(1 * Direction, 1 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(1 * Direction, -1 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(-1 * Direction, 1 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(-1 * Direction, -1 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(0, 2 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            NewMove = New Move(0, -2 * Direction)
            NewMoveOption.AddToPossibleMoves(NewMove)
            Return NewMoveOption
        End Function
        '------------------------------------------------------------------------------------------

        Private Function CreateMoveOption(ByVal Name As String, ByVal Direction As Integer) As MoveOption 'translates a name into new move options
            If Name = "chowkidar" Then
                Return CreateChowkidarMoveOption(Direction)
            ElseIf Name = "ryott" Then
                Return CreateRyottMoveOption(Direction)
            ElseIf Name = "faujdar" Then
                Return CreateFaujdarMoveOption(Direction)
            ElseIf Name = "jazair" Then
                Return CreateJazairMoveOption(Direction)
            Else
                Return CreateCuirassierMoveOption(Direction)
            End If
        End Function

        Private Sub CreateMoveOptions() 'initilses the starting move option queue
            Players(0).AddToMoveOptionQueue(CreateMoveOption("ryott", 1))
            Players(0).AddToMoveOptionQueue(CreateMoveOption("chowkidar", 1))
            Players(0).AddToMoveOptionQueue(CreateMoveOption("cuirassier", 1))
            Players(0).AddToMoveOptionQueue(CreateMoveOption("faujdar", 1))
            Players(0).AddToMoveOptionQueue(CreateMoveOption("jazair", 1))

            Players(1).AddToMoveOptionQueue(CreateMoveOption("ryott", -1))
            Players(1).AddToMoveOptionQueue(CreateMoveOption("chowkidar", -1))
            Players(1).AddToMoveOptionQueue(CreateMoveOption("jazair", -1))
            Players(1).AddToMoveOptionQueue(CreateMoveOption("faujdar", -1))
            Players(1).AddToMoveOptionQueue(CreateMoveOption("cuirassier", -1))

            'int after in createmoveoption specifies direction of the piece
        End Sub
    End Class

    Class Piece
        Protected TypeOfPiece, Symbol As String 'Symbol and name of poiece
        Protected PointsIfCaptured As Integer 'the amount of points for capturing said piece -> One for all piece apart from mirza (which gives 5 for being captured)
        Protected BelongsTo As Player 'Who the piece belongs to

        Sub New(ByVal T As String, ByVal B As Player, ByVal P As Integer, ByVal S As String)
            TypeOfPiece = T
            BelongsTo = B
            PointsIfCaptured = P
            Symbol = S
        End Sub

        Public Function GetSymbol() As String
            Return Symbol
        End Function

        Public Function GetTypeOfPiece() As String
            Return TypeOfPiece
        End Function

        Public Function GetBelongsTo() As Player
            Return BelongsTo
        End Function

        Public Function GetPointsIfCaptured() As Integer
            Return PointsIfCaptured
        End Function
    End Class

    Class Square
        Protected Symbol As String 'the text to display in a square
        Protected PieceInSquare As Piece 'indicates what piece is saved in a square
        Protected BelongsTo As Player 'indicates if a square has a piece which belongs to a certain player

        'belongsTo seems to always be equal to nothing for this class
        'apart from the inherited class
        Sub New()
            PieceInSquare = Nothing
            BelongsTo = Nothing
            Symbol = " "
        End Sub

        Public Overridable Sub SetPiece(ByVal P As Piece) 'putting a piece in a square : 'No function which overrides in the only class which inherits square
            PieceInSquare = P
        End Sub

        Public Overridable Function RemovePiece() As Piece 'taking a piece out of the square : 'No function which overrides in the only class which inherits square
            Dim PieceToReturn As Piece = PieceInSquare
            PieceInSquare = Nothing
            Return PieceToReturn
        End Function

        Public Overridable Function GetPieceInSquare() As Piece 'No function which overrides in the only class which inherits square
            Return PieceInSquare
        End Function

        Public Overridable Function GetSymbol() As String 'No function which overrides in the only class which inherits square
            Return Symbol
        End Function

        Public Overridable Function GetPointsForOccupancy(ByVal CurrentPlayer As Player) As Integer
            Return 0
        End Function

        Public Overridable Function GetBelongsTo() As Player 'No function which overrides in the only class which inherits square
            Return BelongsTo
        End Function

        Public Overridable Function ContainsKotla() As Boolean 'No function which overrides in the only class which inherits square
            If Symbol = "K" Or Symbol = "k" Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class

    Class Kotla
        Inherits Square

        Sub New(ByVal P As Player, ByVal S As String)
            MyBase.New()
            BelongsTo = P
            Symbol = S
        End Sub

        Public Overrides Function GetPointsForOccupancy(ByVal CurrentPlayer As Player) As Integer 'returns the amount of points a player should get for occuping a kotla
            If PieceInSquare Is Nothing Then 'if nothing in kotla
                Return 0
            ElseIf BelongsTo.SameAs(CurrentPlayer) Then 'same as current player + piece in kotla is mirza or piece then
                If CurrentPlayer.SameAs(PieceInSquare.GetBelongsTo()) And (PieceInSquare.GetTypeOfPiece() = "piece" Or PieceInSquare.GetTypeOfPiece() = "mirza") Then
                    Return 5
                Else
                    Return 0
                End If
            Else 'if opposing kotla
                If CurrentPlayer.SameAs(PieceInSquare.GetBelongsTo()) And (PieceInSquare.GetTypeOfPiece() = "piece" Or PieceInSquare.GetTypeOfPiece() = "mirza") Then
                    Return 1
                Else
                    Return 0
                End If
            End If
        End Function
    End Class

    Class MoveOption 'contains all possible moves of A SINGULAR piece
        Protected Name As String 'the name of the piece
        Protected PossibleMoves As List(Of Move) 'All possible moves avaliable for the piece

        Sub New(ByVal N As String)
            Name = N
            PossibleMoves = New List(Of Move)
        End Sub

        Public Sub AddToPossibleMoves(ByVal M As Move)
            PossibleMoves.Add(M)
        End Sub

        Public Function GetName() As String
            Return Name
        End Function

        Public Function CheckIfThereIsAMoveToSquare(ByVal StartSquareReference As Integer, ByVal FinishSquareReference As Integer) As Boolean
            'Iterates through all possible moves for a piece
            'If input = possible move then return true
            Dim StartRow As Integer = StartSquareReference \ 10
            Dim StartColumn As Integer = StartSquareReference Mod 10
            Dim FinishRow As Integer = FinishSquareReference \ 10
            Dim FinishColumn As Integer = FinishSquareReference Mod 10
            For Each M In PossibleMoves
                If StartRow + M.GetRowChange() = FinishRow And StartColumn + M.GetColumnChange() = FinishColumn Then
                    Return True
                End If
            Next
            Return False
        End Function
    End Class

    Class Move 'A given move for a given piece. One piece will have many moves
        Protected RowChange, ColumnChange As Integer
        'Row change is the change in rows from the piece to the destination move
        'Column change is the change in columns from the piece to the destination move
        Sub New(ByVal R As Integer, ByVal C As Integer)
            RowChange = R
            ColumnChange = C
        End Sub

        Public Function GetRowChange() As Integer
            Return RowChange
        End Function

        Public Function GetColumnChange() As Integer
            Return ColumnChange
        End Function
    End Class

    Class MoveOptionQueue 'The queue of available moves to choose from
        Private Queue As New List(Of MoveOption) 'contains the 5 possible moves the player can see.


        Public Sub ResetQueueBack(ByVal pos As Integer)
            Dim temp As MoveOption = Queue(Queue.Count - 1)
            Queue(Queue.Count - 1) = Queue(pos)
            Queue(pos) = temp
        End Sub

        Public Function GetQueueAsString() 'for displaying the moves possible to the player
            Dim QueueAsString As String = ""
            Dim Count As Integer = 1
            For Each M In Queue
                QueueAsString &= Count.ToString() & ". " & M.GetName() & "   "
                Count += 1
            Next
            Return QueueAsString
        End Function

        Public Sub Add(ByVal NewMoveOption As MoveOption) 'adding a move to the queue
            Queue.Add(NewMoveOption)
        End Sub

        Public Sub Replace(ByVal Position As Integer, ByVal NewMoveOption As MoveOption) 'used when offer is taken, and the player decides a move in the queue to be replaced
            Queue(Position) = NewMoveOption
        End Sub

        Public Sub MoveItemToBack(ByVal Position As Integer) 'used when a move is made and needs to be changed to the back of the queue
            Dim Temp As MoveOption = Queue(Position)
            Queue.RemoveAt(Position)
            Queue.Add(Temp)
        End Sub

        Public Function GetMoveOptionInPosition(ByVal Pos As Integer) As MoveOption 'self explanatory
            Return Queue(Pos)
        End Function
    End Class

    Class Player
        Private Name As String
        Private Direction, Score As Integer '-1 for moving pieces down, 1 for moving pieces up
        Private Queue As New MoveOptionQueue() 'The moves the player can select (the options available to them)


        Public Sub ResetQueueBackAfterUndo(ByVal pos As Integer)
            Queue.ResetQueueBack(pos)
        End Sub

        Sub New(ByVal N As String, ByVal D As Integer)
            Score = 100
            Name = N
            Direction = D
        End Sub

        Public Function SameAs(ByVal APlayer As Player) As Boolean 'Identifies a player based on name. Used for checking who won, checking if a player owns a square, etc
            If APlayer Is Nothing Then
                Return False
            ElseIf APlayer.GetName() = Name Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetPlayerStateAsString() As String 'displays info about score and move option queue + name
            Return Name & Environment.NewLine & "Score: " & Score.ToString() & Environment.NewLine & "Move option queue: " & Queue.GetQueueAsString() & Environment.NewLine()
        End Function

        Public Sub AddToMoveOptionQueue(ByVal NewMoveOption As MoveOption)
            Queue.Add(NewMoveOption)
        End Sub

        Public Sub UpdateQueueAfterMove(ByVal Position As Integer)
            Queue.MoveItemToBack(Position - 1)
        End Sub

        Public Sub UpdateMoveOptionQueueWithOffer(ByVal Position As Integer, ByVal NewMoveOption As MoveOption)
            Queue.Replace(Position, NewMoveOption)
        End Sub

        Public Function GetScore() As Integer
            Return Score
        End Function

        Public Function GetName() As String
            Return Name
        End Function

        Public Function GetDirection() As Integer
            Return Direction
        End Function

        Public Sub ChangeScore(ByVal Amount As Integer)
            Score += Amount
        End Sub

        Public Function CheckPlayerMove(ByVal Pos As Integer, ByVal StartSquareReference As Integer, FinishSquareReference As Integer) As Boolean 'checks if a move is possible or not
            Dim Temp As MoveOption = Queue.GetMoveOptionInPosition(Pos - 1) ' Temp = Move the player has selected
            Return Temp.CheckIfThereIsAMoveToSquare(StartSquareReference, FinishSquareReference) 'loops through all possible moves to see if there is a possible move
        End Function
    End Class
End Module