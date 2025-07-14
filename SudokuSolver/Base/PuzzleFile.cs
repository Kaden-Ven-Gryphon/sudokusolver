using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SudokuSolver.Exceptions;

namespace SudokuSolver.Base
{
	/// <summary>
	/// Adds path and other file specific details to a puzzle
	/// </summary>
	
	public class PuzzleFile : PuzzleBase
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
				else if (path.EndsWith(".json"))
				{
					//Then it is a json
					var jsonSting = sr.ReadToEnd();
					var toCopy = JsonSerializer.Deserialize<PuzzleBase>(jsonSting);
					if (toCopy != null)
					{
						Id = toCopy.Id;
						Name = toCopy.Name;
						Description = toCopy.Description;
						Difficulty = toCopy.Difficulty;
						ResizeBoard(toCopy.Columns, toCopy.Rows, false);


						_board = new int[Rows * Columns];
						_given = new int[Rows * Columns];
						_pencilMarks = new List<int>[Rows * Columns];
						for (int i = 0; i < Rows; i++)
						{
							for (int j = 0; j< Columns; j++)
							{
								_pencilMarks[ConvertCordToIndex(i, j)] = new List<int>();
								_board[ConvertCordToIndex(i, j)] = toCopy.GetCellValue(i, j);
								if (toCopy.CellIsGiven(i, j))
								{
									_given[ConvertCordToIndex(i, j)] = _board[ConvertCordToIndex(i, j)];
								}
								_pencilMarks[ConvertCordToIndex(i, j)].AddRange(toCopy.GetCellPencilMarks(i, j));
							}
							
						}
					}
				}
				sr.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("File Read Error", ex);
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

		/// <summary>
		/// Simple wrtie to disk, just uses name and given and assumes 9x9 needs to be replaced with json
		/// </summary>
		public void WriteToDiskSimple(string path)
		{
			//TODO dont use this function
			StreamWriter sw = null!;
			try
			{
				sw = new StreamWriter(path);

				for (int i = 0; i < 9; i++)
				{
					for(int j = 0;j < 9; j++)
					{
						sw.Write(_given[ConvertCordToIndex(i, j)]);
					}
					sw.Write("\n");
				}
			}
			catch (Exception ex)
			{
				throw new Exception("FileWriting Error", ex);
			}
			finally
			{
				if (sw != null) sw.Close();
			}
		}

		public void WriteToDisk(string path)
		{
			StreamWriter sw = null!;
			try
			{
				sw = new StreamWriter(path+".json");

				string jsonString = JsonSerializer.Serialize<PuzzleBase>(this);

				sw.Write(jsonString);
			}
			catch (Exception ex)
			{
				throw new Exception("FileWriting Error", ex);
			}
			finally
			{
				if (sw != null) sw.Close();
			}
		}

		public void WriteToDisk()
		{
			if (Path == "" || Path == null)
			{
				throw new InvalidPuzzlePathException("No path is set for this puzzle file.");
			}
			WriteToDisk(Path);
		}
	}
}
