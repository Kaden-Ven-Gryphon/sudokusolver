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
	public partial class ChangePathDialog : Window
	{
		public ChangePathDialog()
		{
			InitializeComponent();
			pathTextBox.Text = GlobalStaticData.PATH;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{

		}

		private void okButton_Click(object sender, RoutedEventArgs e)
		{
			GlobalStaticData.PATH = pathTextBox.Text;
			DialogResult = true;
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
