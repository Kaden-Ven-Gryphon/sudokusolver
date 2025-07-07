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
		

		public static string PATH = "C:\\Users\\NoxNo\\Desktop\\Projects\\SudokuSolver\\TestBoards";

		public static string LAST_PUZZLE_IMPORT_NAME = "";

		public static string LAST_PUZZLE_IMPORT_GIVEN = "";

		
	}
}
