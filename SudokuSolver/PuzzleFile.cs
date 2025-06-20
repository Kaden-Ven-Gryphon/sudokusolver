﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
	internal class PuzzleFile
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public int Difficulty { get; set; }
		public PuzzleFile()
		{
			Name = string.Empty;
			Path = string.Empty;
			Difficulty = 0;
		}
		public PuzzleFile(string name, string path, int difficulty)
		{
			Name = name;
			Path = path;
			Difficulty = difficulty;
		}
	}
}
