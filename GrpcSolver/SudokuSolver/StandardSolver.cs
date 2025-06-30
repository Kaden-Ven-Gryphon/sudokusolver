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
		private List<int> ValidValuesList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

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
			if (box < 0 || box >= BoxPositions.Count) { return indexs; }
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
        /// Gets a list of indexs of the cells that are influenced by this cell
        /// This should be overridden for making varients like chess sudoku
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private List<int> GetListOfIndexsThatInfluenceCell(int row, int col)
        {
            var indexs = new List<int>();

            for (int i = 0; i < Rows; i++)
            {
                if (i != row)
                {
                    indexs.Add(ConvertCordToIndex(i, col));
                }
            }
            for (int i = 0; i < Columns; i++)
            {
                if (i != col)
                {
                    indexs.Add(ConvertCordToIndex(row, i));
                }
            }
            indexs.AddRange(GetListOfIndexsForBox(ConvertCordToBoxIndex(row, col) ?? -1));

            indexs.Remove(ConvertCordToIndex(row, col));

            return indexs;
        }

		/// <summary>
		/// Fills a cell with given value and updates pencil marks
		/// returns false if value in not valid, or if cell already has value
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="vallue"></param>
		/// <returns></returns>
		public bool FillCellValue(int row, int col, int value)
		{
			if (!ValueIsInRange(value)) { return false; }
			if (ValueIsInRange(_board[ConvertCordToIndex(row, col)])) { return false; }
			var indexs = GetListOfIndexsThatInfluenceCell(row, col);
			_board[ConvertCordToIndex(row, col)] = value;

			foreach (var index in indexs)
			{
				_pencilMarks[index].Remove(value);
			}
			_pencilMarks[ConvertCordToIndex(row, col)].Clear();
			return true;
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
		/// Check each cell in list of cells in influence and returns their values
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <returns></returns>
		public List<int> GetValuesInInfluence(int row, int col)
		{
			var values = new List<int>();
			var indexs = GetListOfIndexsThatInfluenceCell(row, col);

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
			return !GetValuesInInfluence(row, col).Contains(value);
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

		/// <summary>
		/// Returns a list of indexs for cells with a single pencil mark
		/// </summary>
		/// <returns></returns>
		private List<int> GetIndexsOfNakedSingles()
		{
			var indexs = new List<int>();

			for (int i = 0; i < Rows*Columns; i++)
			{
				if (_pencilMarks[i].Count == 1)
				{
					indexs.Add(i);
				}
			}

			return indexs;
		}

		/// <summary>
		/// Fills in all cells that have a single pencil mark
		/// </summary>
		/// <returns></returns>
		public bool FillNakedSingles()
		{
			Logs.Add("Start: FillNakedSingles");
            LogsDebug.Add("Start: FillNakedSingles");

			bool madeChange = false;

			var indexs = GetIndexsOfNakedSingles();
			foreach (var i in indexs)
			{
				var cord = ConvertIndexToCord(i);
				FillCellValue(cord.row, cord.col, _pencilMarks[i].First());
			}

            Logs.Add("End: FillNakedSingles");
            LogsDebug.Add("End: FillNakedSingles");
			return madeChange;
        }

		/// <summary>
		/// Finds all naked doubles and returns a list of tuples,
		/// giving the two indexs of the double and a list of their influence
		/// </summary>
		/// <returns></returns>
		private List<(int a, int b, List<int> influence)> GetIndexesOfNakedDoubles()
		{
			var nakedDoubles = new List<(int a, int b, List<int> influence)>();

			// Check each row
			for (int row = 0; row < Rows; row++)
			{
				for (int colA = 0; colA < Columns-1; colA++)
				{
					var pencilsA = GetCellPencilMarks(row, colA);
					if (pencilsA.Count != 2) { continue; }
					for (int colB = colA+1; colB < Columns; colB++)
					{
						var pencilsB = GetCellPencilMarks(row, colB);
						if (pencilsB.Count == 2 && pencilsA.All(pencilsB.Contains))
						{
							var influence = new List<int>();
							for (int i = 0; i < Columns; i++) { influence.Add(i); }
							influence.Remove(colA);
							influence.Remove(colB);
                            for (int i = 0; i < influence.Count; i++) { influence[i] = ConvertCordToIndex(row, influence[i]); }
                            nakedDoubles.Add((ConvertCordToIndex(row, colA), ConvertCordToIndex(row, colB), influence));
						}
					}
				}
			}
			// Check each col
			for (int col = 0; col < Columns; col++)
			{
				for (int rowA = 0; rowA < Columns-1; rowA++)
				{
					var pencilsA = GetCellPencilMarks(rowA, col);
					if (pencilsA.Count != 2) { continue; }
					for (int rowB = rowA+1; rowB < Columns; rowB++)
					{
						var pencilsB = GetCellPencilMarks(rowB, col);
						if (pencilsB.Count == 2 && pencilsA.All(pencilsB.Contains))
						{
                            var influence = new List<int>();
                            for (int i = 0; i < Rows; i++) { influence.Add(i); }
                            influence.Remove(rowA);
                            influence.Remove(rowB);
							for (int i = 0; i < influence.Count; i++) { influence[i] = ConvertCordToIndex(influence[i], col); }
                            nakedDoubles.Add((ConvertCordToIndex(rowA, col), ConvertCordToIndex(rowB, col), influence));
                        }
					}
				}
			}
			// Check each box
			for (int box = 0; box < BoxPositions.Count; box++)
			{
				var indexs = GetListOfIndexsForBox(box);
				for (int i = 0; i < indexs.Count-1; i++)
				{
					var pencilsA = _pencilMarks[i];
					if (pencilsA.Count != 2) { continue; }
					for (int j = i+1; j < indexs.Count; j++)
					{
						var pencilsB = _pencilMarks[j];
						if (pencilsB.Count == 2 && pencilsA.All(pencilsB.Contains))
						{
							var influence = GetListOfIndexsForBox(box);
							influence.Remove(i);
							influence.Remove(j);
							nakedDoubles.Add((i, j, influence));
						}
					}
				}
			}

			return nakedDoubles;
		}

		/// <summary>
		/// Removes any pencil marks that be canceled out due to a naked double
		/// </summary>
		/// <returns></returns>
		public bool EliminateNakedDoubles()
		{
			bool madeChanges = false;

			var doubles = GetIndexesOfNakedDoubles();
			foreach (var pair in doubles)
			{
				var markA = _pencilMarks[pair.a][0];
				var markB = _pencilMarks[pair.a][1];
				foreach (int i in pair.influence)
				{
					if (_pencilMarks[i].Contains(markA) || _pencilMarks[i].Contains(markB))
					{
						madeChanges = true;
						_pencilMarks[i].Remove(markA);
						_pencilMarks[i].Remove(markB);
					}
				}
			}

			return madeChanges;
		}
		#endregion

		/// <summary>
		/// Returns true if the board is in a valid solve state
		/// </summary>
		/// <returns></returns>
		public bool PuzzleIsSolved()
		{
			for (int row = 0; row < Rows; row++)
			{
				var values = GetValuesInRow(row);
                if (!(ValidValuesList.All(values.Contains) && values.Count == ValidValuesList.Count))
				{
					return false;
				}
			}
            for (int col = 0; col < Columns; col++)
            {
                var values = GetValuesInColumn(col);
                if (!(ValidValuesList.All(values.Contains) && values.Count == ValidValuesList.Count))
                {
                    return false;
                }
            }
            for (int box = 0; box < BoxPositions.Count; box++)
            {
                var values = GetValuesInBox(box);
                if (!(ValidValuesList.All(values.Contains) && values.Count == ValidValuesList.Count))
                {
                    return false;
                }
            }
            return true;
		}
	}
}
