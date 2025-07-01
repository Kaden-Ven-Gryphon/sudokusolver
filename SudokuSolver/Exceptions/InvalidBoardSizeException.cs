namespace SudokuSolver.Exceptions
{
	[Serializable]
	public class InvalidBoardSizeException : Exception
	{
		public InvalidBoardSizeException() { }
		public InvalidBoardSizeException(string message) { }
		public InvalidBoardSizeException (string message, Exception innerException) { }
	}
}
