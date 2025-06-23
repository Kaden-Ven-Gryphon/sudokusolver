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

namespace SudokuSolver
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		Solver sudokuBoard;
		PuzzleFileManager puzzleFileManager;
		
		BoardWpf boardWpf;

		public MainWindow()
		{
			sudokuBoard = new Solver();
			puzzleFileManager = new PuzzleFileManager("C:\\Users\\NoxNo\\Desktop\\Projects\\SudokuSolver\\TestBoards");
			puzzleFileManager.ScanPath();

			boardWpf = new BoardWpf();

			sudokuBoard.LoadBoard("testboard.txt");
			sudokuBoard.ConsolePrintBoard();
			sudokuBoard.ConsolePrintPencilBoard();
			
			sudokuBoard.ConsolePrintPencilBoard();
			sudokuBoard.ConsolePrintBoard();
			
			InitializeComponent();
			LoadList();
			Board_Viewbox.Child = boardWpf.BoardGrid;

			RefreshBoardState();
			//InitBoardGrid(9, 9, 3);
		}

		private void RefreshBoardState()
		{
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					boardWpf.FillCellValue(i, j, sudokuBoard.GetCellValue(j, i));
					boardWpf.SetCellPencil(i, j, sudokuBoard.GetCellPencil(j, i));
					if(sudokuBoard.CheckIsCellGiven(j, i))
					{
						boardWpf.SetCellForegroundColor(i, j, Colors.Blue);
					}
					else
					{
						boardWpf.SetCellForegroundColor(i, j, Colors.Black);
					}
				}
			}
		}

		private void LoadList()
		{
			puzzleList.ItemsSource = puzzleFileManager.GetPuzzleNames();
		}

		private void Button_Click_Open_Puzzle(object sender, RoutedEventArgs e)
		{
			var selectedPuzzle = puzzleList.SelectedItem ?? "";
			var puzzlePath = puzzleFileManager.GetPuzzlePath(selectedPuzzle.ToString() ?? "") ?? "NULL";
			sudokuBoard.LoadBoard(puzzlePath);
			RefreshBoardState();
			Console.WriteLine(puzzlePath);
		}
		
		private void Button_Click_Fill_Pencil_Marks(object sender, RoutedEventArgs e)
		{
			sudokuBoard.FillBoardWithPencilMarks();
			sudokuBoard.CleanPencilMarksFromBoard();
			RefreshBoardState();
		}

		private void Button_Click_Fill_Singles(object sender, RoutedEventArgs e)
		{
			sudokuBoard.FillSinglePencilMarksAsValue();
			RefreshBoardState();
		}

		private void Button_Click_Doubles(object sender, RoutedEventArgs e)
		{
			sudokuBoard.ResolveDoublesForBoard();
			RefreshBoardState();
		}

		private void Button_Click_Solve(object sender, RoutedEventArgs e)
		{
			sudokuBoard.Solve();
			RefreshBoardState();
		}

		

		private void Button_Click_Check(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("The Board is valid: {0}", sudokuBoard.CheckIsBoardValid());
		}
	}
}