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
		Grid boardGrid;

		public MainWindow()
		{
			sudokuBoard = new Solver();
			puzzleFileManager = new PuzzleFileManager("C:\\Users\\NoxNo\\Desktop\\Projects\\SudokuSolver\\TestBoards");
			puzzleFileManager.ScanPath();

			sudokuBoard.LoadBoard("testboard.txt");
			sudokuBoard.ConsolePrintBoard();
			sudokuBoard.ConsolePrintPencilBoard();
			//sudokuBoard.Solve();
			sudokuBoard.FillBoardWithPencilMarks();
			sudokuBoard.CleanPencilMarksFromBoard();
			sudokuBoard.ConsolePrintPencilBoard();
			sudokuBoard.ConsolePrintBoard();
			sudokuBoard.ConsolePrintPencilBoard();
			sudokuBoard.ResolveDoublesForBoard();
			sudokuBoard.ConsolePrintPencilBoard();
			
			InitializeComponent();
			LoadList();
			InitBoardGrid(9, 9, 3);
		}

		private void LoadList()
		{
			puzzleList.ItemsSource = puzzleFileManager.GetPuzzleNames();
		}

		private void InitBoardGrid(int width, int hight, int bold)
		{
			boardGrid = new Grid();
			boardGrid.Children.Clear();
			Board_Border.Child = boardGrid;

			for (int i = 0; i < width; i++)
			{
				boardGrid.ColumnDefinitions.Add(new ColumnDefinition());
			}
			for (int i =0; i < hight; i++)
			{
				boardGrid.RowDefinitions.Add(new RowDefinition());
			}

			for (int i = 0; i < hight; i++)
			{
				for (int j = 0; j < width; j++)
				{
					var cell = new Border();
					cell.BorderThickness = new Thickness(1);
					cell.BorderBrush = Brushes.Black;
					boardGrid.Children.Add(cell);
					Grid.SetRow(cell, i);
					Grid.SetColumn(cell, j);
				}
			}

			for (int i =0; i < hight/bold; i++)
			{
				for(int j = 0;j < width/bold; j++)
				{
					var cell = new Border();
					cell.BorderThickness = new Thickness(4);
					cell.BorderBrush = Brushes.Black;
					boardGrid.Children.Add(cell);
					Grid.SetRow(cell, i*bold);
					Grid.SetColumn(cell, j* bold);
					Grid.SetRowSpan(cell, bold);
					Grid.SetColumnSpan(cell, bold);
				}
			}
		}

		private void Button_Click_Open_Puzzle(object sender, RoutedEventArgs e)
		{
			var selectedPuzzle = puzzleList.SelectedItem;
			Console.WriteLine(selectedPuzzle.ToString());
		}

		private void Button_Click_Solve(object sender, RoutedEventArgs e)
		{
			sudokuBoard.Solve();
		}

		private void Button_Click_Fill_Singles(object sender, RoutedEventArgs e)
		{

		}

		private void Button_Click_Check(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("The Board is valid: {0}", sudokuBoard.CheckIsBoardValid());
		}
	}
}