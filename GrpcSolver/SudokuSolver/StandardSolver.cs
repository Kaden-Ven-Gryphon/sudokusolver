using SudokuSolver;

namespace GrpcSolver.SudokuSolver
{
	/// <summary>
	/// Functions that act on the puzzle base that solve sudoku puzzles following
	/// the standard rules.  This also includes many basic functions for getting
	/// info from the board, and is ment to be extended by solvers for other
	/// rule sets.
	/// </summary>
	public class StandardSolver : PuzzleBase
	{
		private static int MIN_VALUE = 1;
		private static int MAX_VALUE = 9;
		public List<String> Logs {  get; private set; }
		public List<String> LogsDebug { get; private set; }

		public int BoxSize = 3;
		public List<(int, int)> BoxPositions = new List<(int, int)> { (0, 0), (0, 3), (0, 6), (3, 0), (3, 3), (3, 6), (6, 0), (6, 3), (6, 6) };

		public StandardSolver(PuzzleBase puzzleBase) : base(puzzleBase)
		{
			Logs = new List<String>();
			LogsDebug = new List<string>();
		}

		/// <summary>
		/// Checks that the values is 1-9
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private bool ValueIsInRange(int value)
		{
			return (value >= MIN_VALUE) && (value <= MAX_VALUE);
		}

		/// <summary>
		/// Gets a list of 1d indexs for each cell of the box
		/// </summary>
		/// <param name="box"></param>
		/// <returns></returns>
		protected List<int> GetListOfIndexsForBox(int box)
		{
			var indexs = new List<int>();
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					indexs.Add(ConvertCordToIndex(
						BoxPositions[box].Item1 + i,
						BoxPositions[box].Item2 + j));
				}
			}
			return indexs;
		}

		/// <summary>
		/// Converts row col cord to box index
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <returns>null if box not found</returns>
		public int? ConvertCordToBoxIndex(int row, int col)
		{
			for (int box = 0; box < BoxPositions.Count; box++)
			{
				if (
					row >= BoxPositions[box].Item1
					&& row < BoxPositions[box].Item1 + BoxSize
					&& col >= BoxPositions[box].Item2
					&& col < BoxPositions[box].Item2 + BoxSize)
				{
					return box;
				}
			}
			return null;
		}

		/// <summary>
		/// Returns a list of values in given row
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public List<int> GetValuesInRow(int row)
		{
			var values = new List<int>();
			for (int i = 0; i < Columns; i++)
			{
				var x = GetCellValue(row, i);
				if (ValueIsInRange(x)) { values.Add(x); }
			}
			return values;
		}

		/// <summary>
		/// Returns a list of values in given column
		/// </summary>
		/// <param name="col"></param>
		/// <returns></returns>
		public List<int> GetValuesInColumn(int col)
		{
			var values = new List<int>();
			for(int i = 0; i < Rows; i++)
			{
				var x = GetCellValue(i, col);
				if (ValueIsInRange(x)) { values.Add(x); }
			}
			return values;
		}

		/// <summary>
		/// Returns a list of balues in given box
		/// </summary>
		/// <param name="box"></param>
		/// <returns></returns>
		public List<int> GetValuesInBox(int box)
		{
			var values = new List<int>();
			if (box >= BoxPositions.Count && box <= 0) { return values; }
			var indexs = GetListOfIndexsForBox(box);
			foreach (var i in indexs)
			{
				if (ValueIsInRange(_board[i])) { values.Add(_board[i]); }
			}
			return values;
		}

		/// <summary>
		/// Returns true if the value is not already in row
		/// </summary>
		/// <param name="row"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool ValueIsValidInRow(int row, int value)
		{
			return !GetValuesInRow(row).Contains(value);
		}

		/// <summary>
		/// Returns true if the value is not already in the column
		/// </summary>
		/// <param name="col"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool ValueIsValidInColumn(int col, int value)
		{
			return !GetValuesInColumn(col).Contains(value);
		}

		/// <summary>
		/// Returns true if the value is not already in the box
		/// </summary>
		/// <param name="box"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool ValueIsValidInBox(int box, int value)
		{
			return !GetValuesInBox(box).Contains(value);
		}

		/// <summary>
		/// Returns true if the values is valid in the cell and is not already in the 
		/// row, column, or box
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool ValueIsValidInCell(int row, int col, int value)
		{
			return ValueIsValidInBox(ConvertCordToBoxIndex(row, col) ?? -1, value) && ValueIsValidInRow(row, value) && ValueIsValidInColumn(col, value);
		}

		/// <summary>
		/// Add a pencil mark to cell returns false if it is not added
		/// either not in range, or already in the list
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public bool AddPencilMarkToCell(int row, int col, int mark)
		{
			bool markAdded = false;

			if(!ValueIsInRange(mark)) { return markAdded; }
			if(GetCellPencilMarks(row, col).Contains(mark)) { return markAdded; }
			_pencilMarks[ConvertCordToIndex(row, col)].Add(mark);
			markAdded = true;

			return markAdded;
		}

		#region Solver Functions
		/// <summary>
		/// Fills the entire board's empty cells with valid pencil marks
		/// </summary>
		public void FillAllValidPencilMarks()
		{
			Logs.Add("Start: FillAllValidPencilMarks");
			LogsDebug.Add("Start: FillAllValidPencilMarks");

			int cellsModified = 0;
			int marksAdded = 0;

			for (int row = 0; row < Rows; row++)
			{
				for (int col = 0; col < Columns;  col++)
				{
					if (ValueIsInRange(GetCellValue(row, col))) { continue; }
					cellsModified++;
					for (int mark = MIN_VALUE; mark <= MAX_VALUE; mark++)
					{
						if(ValueIsValidInCell(row, col, mark))
						{
							marksAdded++;
							_pencilMarks[ConvertCordToIndex(row, col)].Add(mark);
							LogsDebug.Add("\tAdded pencil mark " + mark + " to cell (R" + row + ", C" + col + ")");
						}
					}
				}
			}
			Logs.Add("End: FillAllValidPencilMarks: Added marks " + marksAdded + " to " + cellsModified + " cells");
			LogsDebug.Add("End: FillAllValidPencilMarks: Added marks " + marksAdded + " to " + cellsModified + " cells");
		}
		#endregion
	}
}
