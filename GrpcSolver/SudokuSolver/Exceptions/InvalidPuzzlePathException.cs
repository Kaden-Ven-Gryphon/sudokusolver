namespace SudokuSolver.Exceptions
{
	[Serializable]
	public class InvalidPuzzlePathException : Exception
	{
		public InvalidPuzzlePathException() : base() { }
		public InvalidPuzzlePathException(string message) : base(message) { }
		public InvalidPuzzlePathException(string message, Exception inner) : base(message, inner) { }
	}
}
