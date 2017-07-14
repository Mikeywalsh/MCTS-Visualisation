/// <summary>
/// A special version of a <see cref="C4Board"/>, which allows making of moves in any cell, regardless if any moves have been made beneath it
/// Makes unit testing easier, as not every cell below a required cell has to be played in
/// </summary>
public class C4TestingBoard : C4Board {

    /// <summary>
    /// Makes a move on this Connect 4 board at the specified move position
    /// </summary>
    /// <param name="move">The move to make</param>
    /// <returns>A reference to this testing Connect 4 board</returns>
    public override Board MakeMove(Move move)
    {
        C4Move m = (C4Move)move;

        //Make the move on this board
        boardContents[m.X, m.Y] = 1;

        //Determine if there is a winner
        DetermineWinner(m);

        return this;
    }

    /// <summary>
    /// Factory method used to create a full <see cref="C4TestingBoard"/> with no winner, for testing
    /// </summary>
    /// <returns>A full <see cref="C4TestingBoard"/> with no winner</returns>
    public static C4TestingBoard CreateFullBoardNoWinner()
    {
        //Create an empty board
        C4TestingBoard board = new C4TestingBoard();
        
        //Fill the board up, such that it is full and there are no winners
        for(int y = 0; y < board.Height; y++)
        {
            for(int x = 0; x < board.Width; x++)
            {
                board.boardContents[x, y] = (x % 4 < 2 ? (y % 2 == 0? 1 : 2) : (y % 2 == 0? 2 : 1));
            }
        }

        //Determine if there is a winner for the board and set the Winner flag accordingly
        board.DetermineWinner();

        //Return the board
        return board;
    }
}
