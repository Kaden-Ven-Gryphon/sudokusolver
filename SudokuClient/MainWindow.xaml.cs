using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcSolverClient;
using SudokuSolver;
using SudokuClient.Mappers;


namespace SudokuClient
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		// grpc connection related values
		private ChannelBase _channel = null!;
		private Greeter.GreeterClient _client = null!;
		private PuzzleManager.PuzzleManagerClient _puzzleManageClient = null!;
		private StandardSolver.StandardSolverClient _standardSolverClient = null!;
		private string _address = "localhost";
		private int _port = 7189;

		// board realted values
		private int _boardWidth = 9;
		private int _boardHeight = 9;
		private int _boardBold = 3;
		private BoardWpf _boardWpf = null!;

		private PuzzleBase _currentBoard = null!;

		// puzzle lists
		private List<PuzzleListEntryWpf> _puzzleList = null!;

		// Entry point of the programs, and function that run on startup
		#region Startup
		/// <summary>
		/// The entry point to the program, opens the main UI starts the connection and initializes the board
		/// </summary>
		public MainWindow()
		{
			// Start grpc connection
			StartConnection(_address, _port);

			InitializeComponent();

			// Create board
			InitializeBoard(_boardHeight, _boardWidth, _boardBold);
			GetPuzzleListAsync("C:\\Users\\NoxNo\\Desktop\\Projects\\SudokuSolver\\TestBoards");
			Console.WriteLine("LOG: " + _puzzleList?.ToString());
		}

		/// <summary>
		/// Starts a connection and client with the grpc backend
		/// </summary>
		/// <param name="address">IP or URL</param>
		/// <param name="port">Port number</param>
		private void StartConnection(string address, int port)
		{
			try
			{
				var fullAddress = "https://" + address + ":" + port.ToString();
				_channel = GrpcChannel.ForAddress(fullAddress);
				_client = new Greeter.GreeterClient(_channel);
				_puzzleManageClient = new PuzzleManager.PuzzleManagerClient(_channel);
				_standardSolverClient = new StandardSolver.StandardSolverClient(_channel);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		/// <summary>
		/// Create the wpf sudoku board element and ataches it as a child of
		/// the view port
		/// </summary>
		/// <param name="width">Number of cells wide</param>
		/// <param name="hight">Number of cells tall</param>
		/// <param name="bold">Number of cells before adding bold line</param>
		private void InitializeBoard(int width, int hight, int bold)
		{
			try
			{
				_boardWpf = new BoardWpf(width, hight, bold);
				Board_Viewbox.Child = _boardWpf.BoardGrid;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//await Task.Run(GetPuzzleListAsync());
		}
		#endregion

		// Function for getting data from the grpc backend
		#region get grpc
		private async void GetPuzzleListAsync(string path)
		{
			try
			{
				Console.WriteLine("LOG: Getting Puzzle List Async");
				var reply = await _puzzleManageClient.GetPuzzleListAndIdsAsync(
					new PathRequest { Path = path });
				if (reply != null)
				{
					_puzzleList = new List<PuzzleListEntryWpf>();
					foreach (var x in reply.PuzzleFiles)
					{
						_puzzleList.Add(new PuzzleListEntryWpf(x.Key, x.Value));
					}
					UpdatePuzzleList();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("CLIENT: " + ex.Message);
			}
		}

		/// <summary>
		/// Get a puzzle base from the grpc backend using it id
		/// </summary>
		/// <param name="id"></param>
		/// <returns>May return null</returns>
		private async Task<PuzzleBase?> GetPuzzleByIdAsync(int id)
		{
			try
			{
				Console.WriteLine("LOG: Getting Puzzle With ID:" + id + " Async");
					var reply = await _puzzleManageClient.GetPuzzleAsync(
						new PuzzleRequest { Id = id });
					return PuzzleMessageMapper.PuzzleBaseFromPuzzleReply(reply);
			}
			catch (Exception ex)
			{
				MessageBox.Show("CLIENT: " + ex.Message);
				return null;
			}
		}

		/// <summary>
		/// Has the grpc backend run the FillValidPencilMarks and return updated board
		/// </summary>
		private async Task<PuzzleBase?> RunFillValidPencilMarksAsync()
		{
			try
			{
				Console.WriteLine("LOG: Running FillValidPencilMarks");
				var reply = await _standardSolverClient.FillPencilMarksAsync(
					PuzzleMessageMapper.BoardStateRequestromPuzzleBase(_currentBoard));
				if (reply != null)
				{
					//TODO MOVE TO UI LOG
					foreach (var log in reply.Logs)
					{
						Console.WriteLine(log);
					}
					var updatedBoard = PuzzleMessageMapper.PuzzleBaseFromBoardStateReply(reply);

					return updatedBoard;
				}
				return null;
			}
			catch (Exception ex)
			{
				MessageBox.Show("CLIENT: " + ex.Message);
				return null;
			}
		}
		#endregion

        #region Update Functions
        private void RefreshBoardState()
		{
			if (_currentBoard == null) {  return; }
			if (_currentBoard.Columns != _boardWpf.Columns && _currentBoard.Rows != _boardWpf.Rows)
			{
				throw new Exception("Invalid board size load");
			}
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					_boardWpf.FillCellValue(i, j, _currentBoard.GetCellValue(j, i));
					_boardWpf.SetCellPencil(i, j, _currentBoard.GetCellPencilMarks(j, i));
					if (_currentBoard.CellIsGiven(j, i))
					{
						_boardWpf.SetCellForegroundColor(i, j, Colors.Blue);
					}
					else
					{
						_boardWpf.SetCellForegroundColor(i, j, Colors.Black);
					}
				}
			}
		}

		private void UpdatePuzzleList()
		{
			Console.WriteLine("LOG: Update Puzzle List");
			puzzleList.ItemsSource = _puzzleList;
		}
        #endregion

        #region Buttons

		/// <summary>
		/// Gets the select puzzle name and id, gets puzzle from grpc backend
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private async void Button_Click_Open_Puzzle(object sender, RoutedEventArgs e)
		{
			var selectedPuzzle = puzzleList.SelectedItem as PuzzleListEntryWpf;
			if (selectedPuzzle != null)
			{
				var puzzle = await GetPuzzleByIdAsync(selectedPuzzle.Id);
				if (puzzle != null)
				{
					_currentBoard = puzzle;
				}
			}
			RefreshBoardState();
		}

		/// <summary>
		/// Button Click to Fill Pencil Marks
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void Button_Click_Fill_Pencil_Marks(object sender, RoutedEventArgs e)
		{
			var newBoard = await RunFillValidPencilMarksAsync();
			if(newBoard != null)
			{
				_currentBoard.UpdateBoardFromBoard(newBoard);
				RefreshBoardState();
			}
		}

		private void Button_Click_Fill_Singles(object sender, RoutedEventArgs e)
		{
			//sudokuBoard.FillSinglePencilMarksAsValue();
			RefreshBoardState();
		}

		private void Button_Click_Doubles(object sender, RoutedEventArgs e)
		{
			//sudokuBoard.ResolveDoublesForBoard();
			RefreshBoardState();
		}

		private void Button_Click_Solve(object sender, RoutedEventArgs e)
		{
			//sudokuBoard.Solve();
			RefreshBoardState();
		}



		private void Button_Click_Check(object sender, RoutedEventArgs e)
		{
			//Console.WriteLine("The Board is valid: {0}", sudokuBoard.CheckIsBoardValid());
		}
        #endregion
    }
}