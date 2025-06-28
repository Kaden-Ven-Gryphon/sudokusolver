using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
	internal class PuzzleFileManager
	{
		public string Path { get; set; }
		public int Count { get; set; }

		private int _nextId = 0;
		private List<PuzzleFile> _puzzleFiles;

		public PuzzleFileManager(string path)
		{
			Path = path;
			_puzzleFiles = new List<PuzzleFile>();
		}
		public PuzzleFileManager() : this("") { }

		#region Private methods
		
		#endregion

		// Seaches the path for txt files and loads the names of files into list
		// Does not varify that they are valid puzzle files
		public void ScanPath()
		{
			var directories = Directory.GetDirectories(Path).ToList();
			if (directories == null) { return; }

			for (int i = 0; i < directories.Count; i++)
			{
				var subdir = Directory.GetDirectories(directories[i]).ToList();
				var files = Directory.GetFiles(directories[i]).ToList();


				foreach (var file in files)
				{
					var puzzle = new PuzzleFile();
					puzzle.Path = file;
					puzzle.Name = file.Split('\\').Last();
					puzzle.Id = _nextId++;
					_puzzleFiles.Add(puzzle);
				}

				if (subdir != null) { directories.AddRange(subdir); }
			}
		}

		// Returns a dictionary of puzzle names and ids
		public Dictionary<int, string> GetPuzzleNamesAndIds()
		{
			Dictionary<int, string> names = new Dictionary<int, string>();
			foreach(var puzzle in _puzzleFiles)
			{
				names.Add(puzzle.Id, puzzle.Name);
			}
			return names;
		}

		// Returns string of puzzle file path
		public string? GetPuzzlePath(string puzzleName)
		{
			var puzzle = _puzzleFiles.Find(i => i.Name == puzzleName);
			if (puzzle != null && puzzle.Path != null && puzzle.Path != "")
			{
				return puzzle.Path;
			}
			else { return null; }
		}

		/// <summary>
		/// Finds puzzle by its id
		/// </summary>
		/// <param name="id"></param>
		/// <returns>A puzzle file, or null</returns>
		public PuzzleFile? GetPuzzleById(int id)
		{
			var puzzle = _puzzleFiles.Find(i => i.Id == id);
			if (puzzle != null)
			{ 
				if (puzzle.Stale) { puzzle.LoadFromDisk(); }
				return puzzle;
			}
			else { return null;}
		}
	}
}
