using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.Debug;

namespace SudokuClient
{
	internal static class GlobalStaticData
	{
		public static ILoggerFactory LogFactory = new LoggerFactory().AddSerilog(
			new LoggerConfiguration()
			.MinimumLevel.Debug()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
			.Enrich.FromLogContext()
			.WriteTo.Debug()
			.CreateLogger());

		public static string PATH = "C:\\Users\\NoxNo\\Desktop\\Projects\\SudokuSolver\\TestBoards"; 
		
		

		public static void InitLog()
		{
			var LogConfig = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
			.Enrich.FromLogContext()
			.WriteTo.Debug()
			.CreateLogger();
			LogFactory = new LoggerFactory();
			LogFactory.AddSerilog(LogConfig);
		}
	}
}
