using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuSolver.Exceptions;
using System.IO;

namespace SudokuSolver
{
	/// <summary>
	/// Adds path and other file specific details to a puzzle
	/// </summary>
	internal class PuzzleFile : PuzzleBase
	{
		
		public string Path { get; set; }
		public bool Stale = true;
		
		public PuzzleFile()
		{
			Path = string.Empty;
		}
		public PuzzleFile(int id, string name, string description, int difficulty, int width, int hight, string path) : base(id, name, description, difficulty, width, hight)
		{
			Path = path;
		}

		/// <summary>
		/// LoadFile from a given path and saves the data to the class properties
		/// </summary>
		/// <param name="path"></param>
		/// <returns>false if file fails to load</returns>
		public void LoadFromDisk(string path)
		{
			StreamReader sr = null!;
			try
			{
				sr = new StreamReader(path);

				// Read old non json format
				if (!path.EndsWith(".json"))
				{
					ResizeBoard(9, 9, false);

					var line = sr.ReadLine();
					int i = 0;
					while (line != null && i < 81)
					{
						for (int j = 0; j < line.Length; j++)
						{
							_board[i] = line[j] - '0';
							_given[i] = _board[i];
							_pencilMarks[i].Clear();
							i++;
						}
						line = sr.ReadLine();
					}
					Difficulty = 0;
					Description = "From Old Format";
					Stale = false;
				}
				sr.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			finally
			{
				if (sr != null) sr.Close();
			}
		}

		/// <summary>
		/// Loads file using the object current path value
		/// </summary>
		public void LoadFromDisk()
		{
			if (Path == "" || Path == null)
			{
				throw new InvalidPuzzlePathException("No path is set for this puzzle file.");
			}
			LoadFromDisk(Path);

		}
	}
}
