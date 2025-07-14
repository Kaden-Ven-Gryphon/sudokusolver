namespace SudokuSolver.Base
{
	public class PuzzleFileManager
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
			_puzzleFiles.Clear();
			var directories = new List<string> { Path };
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

		/// <summary>
		/// Clears out list of puzzle files
		/// </summary>
		public void Clear()
		{
			_puzzleFiles.Clear();
		}

		public void ImportNewPuzzleSimple(string puzzleName, int[] given)
		{
			var newPuzzle = new PuzzleFile();
			newPuzzle.ResizeBoard(9, 9, false);
			

			newPuzzle.Name = puzzleName;
			
			for (int i = 0; i < 9;  i++)
			{
				for (int j = 0; j < 9; j++)
				{
					newPuzzle.SetCell(i, j, given[i*9+j], given[i*9+j], new List<int>());
				}
			}

			newPuzzle.Path = Path + "\\" + puzzleName;

			newPuzzle.WriteToDisk(newPuzzle.Path);
		}
	}
}
