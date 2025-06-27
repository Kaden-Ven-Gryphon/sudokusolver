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
		private string _address = "localhost";
		private int _port = 7189;

		// board realted values
		private int _boardWidth = 9;
		private int _boardHeight = 9;
		private int _boardBold = 3;
		private BoardWpf _boardWpf = null!;

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
			RefreshBoardState();
			GetPuzzleListAsync("C:\\Users\\NoxNo\\Desktop\\Projects\\SudokuSolver\\TestBoards");
			Console.WriteLine(_puzzleList?.ToString());
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
				Console.WriteLine("Getting Puzzle List Async");
				var reply = await _puzzleManageClient.GetPuzzleListAsync(
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
				MessageBox.Show(ex.Message);
			}
		}
		#endregion

		private async void ButtonClickAsync(object sender, RoutedEventArgs e)
		{
			var reply = await _client.SayHelloAsync(
				new HelloRequest { Name = "GreeterClient" });
		}

        #region Update Functions
        private void RefreshBoardState()
		{
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					//boardWpf.FillCellValue(i, j, sudokuBoard.GetCellValue(j, i));
					//boardWpf.SetCellPencil(i, j, sudokuBoard.GetCellPencil(j, i));
					//if (sudokuBoard.CheckIsCellGiven(j, i))
					//{
						//boardWpf.SetCellForegroundColor(i, j, Colors.Blue);
					//}
					//else
					//{
						//boardWpf.SetCellForegroundColor(i, j, Colors.Black);
					//}
				}
			}
		}

		private void UpdatePuzzleList()
		{
			Console.WriteLine("Update Puzzle List");
			Console.WriteLine(_puzzleList);
			puzzleList.ItemsSource = _puzzleList;
			Console.WriteLine(puzzleList.ItemsSource);
		}
        #endregion

        #region Buttons
        private void Button_Click_Open_Puzzle(object sender, RoutedEventArgs e)
		{
			var selectedPuzzle = puzzleList.SelectedItem ?? "";
			//var puzzlePath = puzzleFileManager.GetPuzzlePath(selectedPuzzle.ToString() ?? "") ?? "NULL";
			//sudokuBoard.LoadBoard(puzzlePath);
			RefreshBoardState();
			//Console.WriteLine(puzzlePath);
		}

		private void Button_Click_Fill_Pencil_Marks(object sender, RoutedEventArgs e)
		{
			//sudokuBoard.FillBoardWithPencilMarks();
			//sudokuBoard.CleanPencilMarksFromBoard();
			RefreshBoardState();
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