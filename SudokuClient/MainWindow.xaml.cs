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
using GrpcSolver;
using SudokuSolver.Base;
using GrpcSolver.Mappers;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Serilog;
using Serilog.Events;
using Serilog.Expressions;
using Serilog.Templates;
using Serilog.Templates.Themes;
using Serilog.Exceptions;


namespace SudokuClient
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly Serilog.ILogger _logger;
		// grpc connection related values
		private ChannelBase _channel = null!;
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
			//Set up Logger
			Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
			.Enrich.FromLogContext()
			.Enrich.WithExceptionDetails()
			.WriteTo.Console(new ExpressionTemplate(
				"[{@t:HH:mm:ss} {@l:w4}: {SourceContext}:{@p['scope']}]\n\t\t {@m}\n{@x}{#each k, v in @p['ExceptionDetail']} {k}: {v}\n{#end}", theme: TemplateTheme.Code))
			//.WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception} {Properties:j}")
			.CreateLogger();
			_logger = Log.ForContext<MainWindow>();

			// Start grpc connection
			StartConnection(_address, _port);

			InitializeComponent();

			// Create board
			InitializeBoard(_boardHeight, _boardWidth, _boardBold);
			GetPuzzleListAsync(GlobalStaticData.PATH);
		}

		/// <summary>
		/// Starts a connection and client with the grpc backend
		/// </summary>
		/// <param name="address">IP or URL</param>
		/// <param name="port">Port number</param>
		private void StartConnection(string address, int port)
		{
			using var log = LogContext.PushProperty("scope", "StartConnection");
			try
			{
				_logger.Information("Starting Connection to GRPC at {address}:{port}", address, port);
				var fullAddress = "https://" + address + ":" + port.ToString();
				_channel = GrpcChannel.ForAddress(fullAddress);
				_puzzleManageClient = new PuzzleManager.PuzzleManagerClient(_channel);
				_standardSolverClient = new StandardSolver.StandardSolverClient(_channel);
				_logger.Information("Connection made.");
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Failed to make connection.");
				ExceptionMessageBox(ex, "StartConnection");
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
			using var log = LogContext.PushProperty("scope", "InitializeBoard");
			try
			{
				_logger.Information("Initializing board with {rows} rows, {cols} columns", hight, width);

				_boardWpf = new BoardWpf(width, hight, bold);
				Board_Viewbox.Child = _boardWpf.BoardGrid;
				_currentBoard = new PuzzleBase(9, 9);
				RefreshBoardState();
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Failed to create board");
				ExceptionMessageBox(ex, "InitializeBoard");
			}
		}
		#endregion

		// Function for getting data from the grpc backend
		#region get grpc
		/// <summary>
		/// sets _puzzleList to as list of id, string pairs for puzzle names
		/// </summary>
		/// <param name="path"></param>
		private async void GetPuzzleListAsync(string path)
		{
			using var log = LogContext.PushProperty("scope", "GetPuzzleListAsync");
			try
			{
				_logger.Information("Getting Puzzle List Async");
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
				_logger.Information("{count} puzzles in list", _puzzleList.Count);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Failed.");
				ExceptionMessageBox(ex, "GetPuzzleListAsync");
			}
		}

		/// <summary>
		/// Get a puzzle base from the grpc backend using it id
		/// </summary>
		/// <param name="id"></param>
		/// <returns>May return null</returns>
		private async Task<PuzzleBase?> GetPuzzleByIdAsync(int id)
		{
			using var log = LogContext.PushProperty("scope", "GetPuzzleByIdAsync");
			try
			{
				_logger.Information("Getting Puzzle With ID: {id} Async", id);
					var reply = await _puzzleManageClient.GetPuzzleAsync(
						new PuzzleRequest { Id = id });
					return PuzzleMessageMapper.PuzzleBaseFromPuzzleReply(reply);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Failed");
				ExceptionMessageBox(ex, "GetPuzzleByIdAsync");
				return null;
			}
		}

		/// <summary>
		/// Has the grpc backend run the FillValidPencilMarks and return updated board
		/// </summary>
		private async Task<PuzzleBase?> RunFillValidPencilMarksAsync()
		{
			using var log = LogContext.PushProperty("scope", "RunFillValidPencilMarksAsync");
			try
			{
				_logger.Information("Running FillValidPencilMarksAsync");
				var reply = await _standardSolverClient.FillPencilMarksAsync(
					PuzzleMessageMapper.BoardStateRequestromPuzzleBase(_currentBoard));
				if (reply != null)
				{
					//TODO MOVE TO UI LOG
					foreach (var solveLog in reply.Logs)
					{
						Console.WriteLine(solveLog);
					}
					var updatedBoard = PuzzleMessageMapper.PuzzleBaseFromBoardStateReply(reply);

					return updatedBoard;
				}
				return null;
			}
			catch (Exception ex)
			{
				_logger.Warning(ex, "Failed");
				ExceptionMessageBox(ex, "RunFillValidPencilMarksAsync");
				return null;
			}
		}

		/// <summary>
		/// Requests the grpc backend to run FillNakedSingles
		/// </summary>
		/// <returns></returns>
		private async Task<PuzzleBase?> RunFillNakedSingles()
		{
			var log = LogContext.PushProperty("scope", "RunFillNakedSingles");
			try
			{
				_logger.Information("LOG: Running FillValidNakedSingles");
				var reply = await _standardSolverClient.FillNakedSinglesAsync(
					PuzzleMessageMapper.BoardStateRequestromPuzzleBase(_currentBoard));
				if (reply != null)
				{
					//TODO MOVE TO UI LOG
					foreach (var puzzlelog in reply.Logs)
					{
						Console.WriteLine(puzzlelog);
					}
					var updatedBoard = PuzzleMessageMapper.PuzzleBaseFromBoardStateReply(reply);

					return updatedBoard;
				}
				return null;
			}
			catch (Exception ex)
			{
				_logger.Warning(ex, "Failed");
				ExceptionMessageBox(ex, "RunFillValidPencilMarksAsync");
				return null;
			}
		}

		/// <summary>
		/// Requests grpc backend to run eliminatenakeddoubles
		/// </summary>
		/// <returns></returns>
		private async Task<PuzzleBase?> RunEliminateNakedDoubles()
		{
			var log = LogContext.PushProperty("scope", "RunEliminateNakedDoubles");
			try
			{
				_logger.Information("LOG: Running FillValidNakedSingles");
				var reply = await _standardSolverClient.EliminateNakedDoublesAsync(
					PuzzleMessageMapper.BoardStateRequestromPuzzleBase(_currentBoard));
				if (reply != null)
				{
					//TODO MOVE TO UI LOG
					foreach (var puzzleLog in reply.Logs)
					{
                        _logger.Information(puzzleLog);
					}
					var updatedBoard = PuzzleMessageMapper.PuzzleBaseFromBoardStateReply(reply);

					return updatedBoard;
				}
				return null;
			}
			catch (Exception ex)
			{
				_logger.Warning(ex, "Failed");
				ExceptionMessageBox(ex, "RunFillValidPencilMarksAsync");
				return null;
			}
		}

		/// <summary>
		/// Requests the grpc backend to check if the puzzle is solved
		/// </summary>
		/// <returns></returns>
		private async Task<bool?> RunIsPuzzleSolved()
		{
			var log = LogContext.PushProperty("scope", "RunIsPuzzleSolved");
			try
			{
				_logger.Information("LOG: Running IsPuzzleSolved");
				var reply = await _standardSolverClient.BoardIsSolvedAsync(
					PuzzleMessageMapper.BoardStateRequestromPuzzleBase(_currentBoard));
				if (reply != null)
				{
					
					return reply.Solved;
				}
				return null;
			}
			catch (Exception ex)
			{
				_logger.Warning(ex, "Failed");
				ExceptionMessageBox(ex, "RunFillValidPencilMarksAsync");
				return null;
			}
		}

		/// <summary>
		/// Sends info to grpc to backend to create puzzle file
		/// </summary>
		/// <returns></returns>
		private async Task<bool?> SetImportPuzzle()
		{
			var log = LogContext.PushProperty("scope", "SetImportPuzzle");
			try
			{
				_logger.Information("Running SetImportPuzzle");

				var request = new ImportRequest();

				request.Name = GlobalStaticData.LAST_PUZZLE_IMPORT_NAME;

				//TODO fix the int parse tostring chain
				foreach (char c in GlobalStaticData.LAST_PUZZLE_IMPORT_GIVEN)
				{
					if (c >= '0' && c <= '9') {  request.Given.Add(Int32.Parse(c.ToString())); }
				}

				var reply = await _puzzleManageClient.ImportPuzzleAsync(request);
				if (reply != null)
				{
					return reply.Sucsess;
				}

				throw new Exception("Puzzle Failed to be imported");
			}
			catch (Exception ex)
			{
				_logger.Warning(ex, "Failed");
				ExceptionMessageBox(ex, "SetImportPuzzle");
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

		private async void Button_Click_Fill_Naked_Singles(object sender, RoutedEventArgs e)
		{
			var newBoard = await RunFillNakedSingles();
			if (newBoard != null)
			{
				_currentBoard.UpdateBoardFromBoard(newBoard);
				RefreshBoardState();
			}
		}

		private async void Button_Click_Elminate_Naked_Doubles(object sender, RoutedEventArgs e)
		{
			var newBoard = await RunEliminateNakedDoubles();
			if (newBoard != null)
			{
				_currentBoard.UpdateBoardFromBoard(newBoard);
				RefreshBoardState();
			}
		}

		private void Button_Click_Solve(object sender, RoutedEventArgs e)
		{
			//sudokuBoard.Solve();
			RefreshBoardState();
		}



		private async void Button_Click_Check(object sender, RoutedEventArgs e)
		{
			var solved = await RunIsPuzzleSolved();
			if (solved != null)
			{
				Console.WriteLine("Puzzle is Solved = " +  solved);
			}
		}
		#endregion

		#region Menus
		private void Menu_Click_Path(object sender, RoutedEventArgs e)
		{
			var log = LogContext.PushProperty("scope", "Menu_Click_Path");
			try
			{
				var dialog = new ChangePathDialog();

				bool? result = dialog.ShowDialog();

				if (result == true)
				{
					GetPuzzleListAsync(GlobalStaticData.PATH);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Setting Path Error");
				ExceptionMessageBox(ex, "Menu_Click_Path");
			}
		}

		private async void Menu_Click_Import(object sender, RoutedEventArgs e)
		{
			var log = LogContext.PushProperty("scope", "Menu_Click_Import");
			try
			{
				var dialog = new ImportPuzzleDialog();

				bool? result = dialog.ShowDialog();

				if (result == true)
				{
					var succes = await SetImportPuzzle();
					if (succes == true)
					{
						GetPuzzleListAsync(GlobalStaticData.PATH);
                    }
					
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Failed to import puzzle");
				ExceptionMessageBox(ex, "Menu_Click_Path");
			}
			
		}
		#endregion

		private void ExceptionMessageBox(Exception ex, string callingFunction)
		{
			string message = ex.Message;
			var nextEx = ex.InnerException;
			while (nextEx != null)
			{
				message += "\n" + nextEx.Message;
				nextEx = nextEx.InnerException;
			}
			MessageBox.Show("CLIENT " + callingFunction + ": " + message);
		}
	}
}