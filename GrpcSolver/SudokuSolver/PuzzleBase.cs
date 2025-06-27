using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
	/// <summary>
	/// Puzzle Base class.  This is mostly a struct that is used to store the state of a puzzle
	/// so that it can be passed to and from the grpc back end
	/// </summary>
	internal class PuzzleBase
	{
		public int Id {  get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int Difficulty { get; set; }
		public int BoardWidth { get; set; }
		public int BoardHight { get; set; }

        private int[] _board;
        private int[] _given;
        private List<int>[] _pencilMarks;

        public PuzzleBase(int id, string name, string description, int difficulty, int boardWidth, int boardHight)
		{
			Id = id;
			Name = name;
			Description = description;
			Difficulty = difficulty;
			BoardWidth = boardWidth;
			BoardHight = boardHight;

            _board = new int[BoardHight*BoardWidth];
            _given = new int[BoardHight * BoardWidth];
            _pencilMarks = new List<int>[BoardHight * BoardWidth];
            for (int i = 0; i < _pencilMarks.Length; i++)
            {
                _pencilMarks[i] = new List<int>();
            }
        }

		private int ConvertCordToIndex(int row, int col)
		{
			return col * BoardWidth + row;
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
