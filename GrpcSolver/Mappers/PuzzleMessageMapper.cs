using SudokuSolver;


namespace GrpcSolver.Mappers
{
	public static class PuzzleMessageMapper
	{
		/// <summary>
		/// Converts PuzzleBase Class to proto PuzzleReply Class
		/// </summary>
		/// <param name="puzzle"></param>
		/// <returns></returns>
		public static PuzzleReply PuzzleReplyFromPuzzleBase(PuzzleBase puzzle)
		{
			var reply = new PuzzleReply();
			reply.Id = puzzle.Id;
			reply.Name = puzzle.Name;
			reply.Description = puzzle.Description;
			reply.Rows = puzzle.Rows;
			reply.Columns = puzzle.Columns;
			for (int i = 0; i < puzzle.Columns; i++)
			{
				for (int j = 0; j < puzzle.Rows; j++)
				{
					reply.Board.Add(puzzle.GetCellValue(j, i));
					if (puzzle.CellIsGiven(j, i))
					{
						reply.Given.Add(puzzle.GetCellValue(j, i));
					}
					else
					{
						reply.Given.Add(0);
					}
					var pencilMarks = puzzle.GetCellPencilMarks(j, i);
					reply.PencilMarks.Add(pencilMarks.Count);
					foreach (var mark in pencilMarks)
					{
						reply.PencilMarks.Add((int)mark);
					}
				}
			}
			return reply;
		}

		/// <summary>
		/// Converts PuzzleReply to puzzleBase
		/// </summary>
		/// <param name="reply"></param>
		/// <returns></returns>
		public static PuzzleBase PuzzleBaseFromPuzzleReply(PuzzleReply reply)
		{
			var puzzle = new PuzzleBase(
				reply.Id,
				reply.Name,
				reply.Description,
				reply.Difficulty,
				reply.Columns,
				reply.Rows);

			puzzle.LoadBoardFromList(reply.Board.ToList(), reply.Given.ToList(), reply.PencilMarks.ToList());

			return puzzle;

		}

		/// <summary>
		/// Converts puzzleBase into a stateRequest
		/// </summary>
		/// <param name="puzzle"></param>
		/// <returns></returns>
		public static BoardStateRequest BoardStateRequestromPuzzleBase(PuzzleBase puzzle)
		{
			var board = new BoardStateRequest();

			board.Columns = puzzle.Columns;
			board.Rows = puzzle.Rows;
			for (int i = 0; i < puzzle.Columns; i++)
			{
				for (int j = 0; j < puzzle.Rows; j++)
				{
					board.Board.Add(puzzle.GetCellValue(j, i));
					var pencilMarks = puzzle.GetCellPencilMarks(j, i);
					board.PencilMarks.Add(pencilMarks.Count);
					foreach (var mark in pencilMarks)
					{
						board.PencilMarks.Add((int)mark);
					}
				}
			}

			return board;
		}

		/// <summary>
		/// Converts a PuzzleBase into a BoardStateReply adding in the solver logs
		/// </summary>
		/// <param name="puzzle"></param>
		/// <param name="logs"></param>
		/// <returns></returns>
		public static BoardStateReply BoardStateReplyFromPuzzleBase(PuzzleBase puzzle, List<string> logs)
		{
			var board = new BoardStateReply();

			board.Columns = puzzle.Columns;
			board.Rows = puzzle.Rows;
			for (int i = 0; i < puzzle.Columns; i++)
			{
				for (int j = 0; j < puzzle.Rows; j++)
				{
					board.Board.Add(puzzle.GetCellValue(j, i));
					var pencilMarks = puzzle.GetCellPencilMarks(j, i);
					board.PencilMarks.Add(pencilMarks.Count);
					foreach (var mark in pencilMarks)
					{
						board.PencilMarks.Add((int)mark);
					}
				}
			}
			foreach (var log in logs)
			{
				board.Logs.Add(log);
			}

			return board;
		}

		/// <summary>
		/// Converts BoardStateRequest into a puzzleBase
		/// </summary>
		/// <param name="board"></param>
		/// <returns></returns>
		public static PuzzleBase PuzzleBaseFromBoardStateRequest(BoardStateRequest board)
		{
			var puzzle = new PuzzleBase(board.Columns, board.Rows);
			puzzle.LoadBoardFromList(board.Board.ToList(), Enumerable.Repeat(0, board.Columns * board.Rows).ToList(), board.PencilMarks.ToList());
			return puzzle;
		}

		/// <summary>
		/// Converts BoardStateReply into a puzzleBase
		/// </summary>
		/// <param name="board"></param>
		/// <returns></returns>
		public static PuzzleBase? PuzzleBaseFromBoardStateReply(BoardStateReply board)
		{
			var puzzle = new PuzzleBase(board.Columns, board.Rows);
			puzzle.LoadBoardFromList(board.Board.ToList(), Enumerable.Repeat(0, board.Columns * board.Rows).ToList(), board.PencilMarks.ToList());
			return puzzle;
			
		}
	}
}
