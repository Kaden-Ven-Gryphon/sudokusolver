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


namespace SudokuClient
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ChannelBase _channel;
		private Greeter.GreeterClient _client;
		public MainWindow()
		{
			_channel = GrpcChannel.ForAddress("https://localhost:7189");
			_client = new Greeter.GreeterClient(_channel);
			

			InitializeComponent();
		}

		private async void ButtonClickAsync(object sender, RoutedEventArgs e)
		{
			var reply = await _client.SayHelloAsync(
				new HelloRequest { Name = "GreeterClient" });
			Console.WriteLine( reply.Message );
		}
	}
}