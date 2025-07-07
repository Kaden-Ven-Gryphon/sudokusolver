using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SudokuClient
{
    /// <summary>
    /// Interaction logic for ChangePathDialog.xaml
    /// </summary>
    public partial class ImportPuzzleDialog : Window
    {
        //TODO
        // Add import file vs new file
        // Add difficulty
        // Description
        public ImportPuzzleDialog()
        {
            InitializeComponent();
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalStaticData.LAST_PUZZLE_IMPORT_NAME = nameTextBox.Text;
            GlobalStaticData.LAST_PUZZLE_IMPORT_GIVEN = givenTextBox.Text;
            DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
