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
		private List<PuzzleFile> puzzleFiles;
		public PuzzleFileManager() {
			Path = "";
			puzzleFiles = new List<PuzzleFile>();
		}
		public PuzzleFileManager(string path)
		{
			Path = path;
			puzzleFiles = new List<PuzzleFile>();
		}

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
					puzzleFiles.Add(puzzle);
				}

				if (subdir != null) { directories.AddRange(subdir); }
			}
		}

		// Returns a list of puzzle names
		public List<string> GetPuzzleNames()
		{
			List<string> names = new List<string>();
			foreach(var puzzle in puzzleFiles)
			{
				names.Add(puzzle.Name);
			}
			return names;
		}

		// Returns string of puzzle file path
		public string? GetPuzzlePath(string puzzleName)
		{
			var puzzle = puzzleFiles.Find(i => i.Name == puzzleName);
			if (puzzle != null && puzzle.Path != null && puzzle.Path != "")
			{
				return puzzle.Path;
			}
			else { return null; }
		}
	}
}
