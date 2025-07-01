using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SudokuSolver.Exceptions;

namespace SudokuSolver.Base
{
	/// <summary>
	/// Puzzle Base class.  This is mostly a struct that is used to store the state of a puzzle
	/// so that it can be passed to and from the grpc back end
	/// </summary>
	public class PuzzleBase
	{
		public int Id {  get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int Difficulty { get; set; }
		public int Rows { get; private set; }
		public int Columns { get; private set; }

        protected int[] _board;
		protected int[] _given;
		protected List<int>[] _pencilMarks;

        public PuzzleBase(int id, string name, string description, int difficulty, int columns, int rows)
		{
			Id = id;
			Name = name;
			Description = description;
			Difficulty = difficulty;
			Columns = columns;
			Rows = rows;

            _board = new int[Rows*Columns];
            _given = new int[Rows * Columns];
            _pencilMarks = new List<int>[Rows * Columns];
            for (int i = 0; i < _pencilMarks.Length; i++)
            {
                _pencilMarks[i] = new List<int>();
            }
        }

		public PuzzleBase() : this(0, "", "", 0, 0, 0) { }
		public PuzzleBase(int columns, int rows) : this(0, "", "", 0, columns, rows) { }

		public PuzzleBase(PuzzleBase toCopy)
		{
			Id = toCopy.Id;
			Name = toCopy.Name;
			Description = toCopy.Description;
			Difficulty = toCopy.Difficulty;
			Columns = toCopy.Columns;
			Rows = toCopy.Rows;


			_board = new int[Rows * Columns];
			_given = new int[Rows * Columns];
			_pencilMarks = new List<int>[Rows * Columns];
			for (int i = 0; i < _pencilMarks.Length; i++)
			{
				_pencilMarks[i] = new List<int>();
				_board[i] = toCopy._board[i];
				_given[i] = toCopy._given[i];
				_pencilMarks[i].AddRange(toCopy._pencilMarks[i]);
			}

		}

		protected int ConvertCordToIndex(int row, int col)
		{
			return row * Columns + col;
		}

		protected (int row, int col) ConvertIndexToCord(int index)
		{
			var row = index / Columns;
			var col = index % Columns;
			return (row, col);
		}
		

		/// <summary>
		/// Resizes the internal board arrays, and copy values
		/// </summary>
		/// <param name="columns"></param>
		/// <param name="rows"></param>
		/// <param name="copy"></param>
		public void ResizeBoard(int columns, int rows, bool copy)
		{
			int oldColumns = Columns;
			int oldRows = Rows;

			Columns = columns;
			Rows = rows;

			var newBoard = new int[Columns * Rows];
			var newGiven = new int[Columns * Rows];
			var newPencilMarks = new List<int>[Columns * Rows];
			for (int i = 0; i < newPencilMarks.Length; i++)
			{
				newPencilMarks[i] = new List<int>();
			}

			if (copy)
			{
				for (int i = 0; i < oldColumns && i < Columns; i++)
				{
					for (int j = 0; j < oldRows && j < Rows; j++)
					{
						newBoard[ConvertCordToIndex(j, i)] = _board[oldColumns * j + i];
						newGiven[ConvertCordToIndex(j, i)] = _given[oldColumns * j + i];
						newPencilMarks[ConvertCordToIndex(j, i)] = _pencilMarks[oldColumns * j + i];
					}
				}
			}

			_board = newBoard;
			_given = newGiven;
			_pencilMarks = newPencilMarks;
		}

		/// <summary>
		/// Sets the value, given, and pencilMarks at once
		/// Should only be used for loading or editing the file
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="value"></param>
		/// <param name="given"></param>
		/// <param name="pencilMarks"></param>
		public void SetCell(int row, int col, int value, int given, List<int> pencilMarks)
		{
			_board[ConvertCordToIndex(row, col)] = value;
			_given[ConvertCordToIndex(row, col)] = given;
			_pencilMarks[ConvertCordToIndex(row, col)].Clear();
			_pencilMarks[ConvertCordToIndex(row, col)].AddRange(pencilMarks);
		}

		/// <summary>
		/// Takes in 1D lists of ints to fill the board's
		/// values, given and pencilMarks.
		/// PencilMarks are a 1D array in format foreach cell (number of marks)(m1,m2...)...
		/// </summary>
		/// <param name="board"></param>
		/// <param name="given"></param>
		/// <param name="pencilMarks"></param>
		public void LoadBoardFromList(List<int> board, List<int> given, List<int> pencilMarks)
		{
			if (board.Count != _board.Length || given.Count != _given.Length || pencilMarks.Count > _pencilMarks.Length*10)
			{
				var message = "Loading board does not fit the dimentions of current board. Current: ("
					+ _board.Length + ", " + _given.Length + ", " + _pencilMarks.Length + ") Loading: ("
					+ board.Count + ", " + given.Count + ", " + pencilMarks.Count + ")";
				throw new InvalidBoardSizeException(message);
			}
			for (int i = 0; i < _board.Length; i++)
			{
				_board[i] = board[0];
				board.RemoveAt(0);
				_given[i] = given[0];
				given.RemoveAt(0);
				var pencilCount = pencilMarks[0];
				pencilMarks.RemoveAt(0);
				_pencilMarks[i].Clear();
				for (int j = 0; j < pencilCount; j++)
				{
					_pencilMarks[i].Add(pencilMarks[0]);
					pencilMarks.RemoveAt(0);
				}
			}
		}

		/// <summary>
		/// Update the board values and pencil marks from anouther board
		/// Mostly for updating the state from the output of the solver
		/// </summary>
		/// <param name="board"></param>
		public void UpdateBoardFromBoard(PuzzleBase board)
		{
			for (int row = 0; row < Rows; row++)
			{
				for (int col = 0; col < Columns; col++)
				{
					_board[ConvertCordToIndex(row, col)] = board.GetCellValue(row, col);
					_pencilMarks[ConvertCordToIndex(row, col)].Clear();
					_pencilMarks[ConvertCordToIndex(row, col)].AddRange(board.GetCellPencilMarks(row, col));
				}
			}
		}

		public int GetCellValue(int row, int col)
		{
			return _board[ConvertCordToIndex(row, col)];
		}

		public List<int> GetCellPencilMarks(int row, int col)
		{
			return _pencilMarks[ConvertCordToIndex(row, col)];
		}

		public bool CellIsGiven(int row, int col)
		{
			return _given[ConvertCordToIndex(row, col)] != 0;
		}
			
		public override string ToString()
		{
			return Name;
		}
	}
}
