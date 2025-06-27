using Grpc.Core;
using GrpcSolver;
using SudokuSolver;

namespace GrpcSolver.Services
{
	/// <summary>
	/// Service for getting and saving puzzle files
	/// </summary>
	public class PuzzleManagerService : PuzzleManager.PuzzleManagerBase
	{
		private readonly ILogger<PuzzleManagerService> _logger;

		private PuzzleFileManager _fileManager;

		public PuzzleManagerService(ILogger<PuzzleManagerService> logger)
		{
			_logger = logger;
			_fileManager = new PuzzleFileManager();
		}

		/// <summary>
		/// Gets dictionary of puzzle names and their ids
		/// </summary>
		/// <param name="request"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public override Task<ListReply> GetPuzzleList(PathRequest request, ServerCallContext context)
		{
			return Task.Run(() => {
				ListReply reply = new ListReply();

				_fileManager.Path = request.Path;
				_fileManager.ScanPath();
				var dict = _fileManager.GetPuzzleNames();

				
				reply.PuzzleFiles.Add(dict);

				return reply;
			});
		}

		public override Task<PuzzleReply> GetPuzzle(PuzzleRequest request, ServerCallContext context)
		{
			return Task.Run(() =>
			{
				PuzzleReply reply = new PuzzleReply();

				return reply;
			});
		}
	}
}
