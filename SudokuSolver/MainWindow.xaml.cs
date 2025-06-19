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
        public MainWindow()
        {
            sudokuBoard = new Solver();
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