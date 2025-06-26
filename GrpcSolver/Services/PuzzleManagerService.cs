using Grpc.Core;
using GrpcSolver;
using SudokuSolver;

namespace GrpcSolver.Services
{
	public class PuzzleManagerService : PuzzleManager.PuzzleManagerBase
	{
		private readonly ILogger<PuzzleManagerService> _logger;

		private PuzzleFileManager _fileManager;

		public PuzzleManagerService(ILogger<PuzzleManagerService> logger)
		{
			_logger = logger;
			_fileManager = new PuzzleFileManager();
		}

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
	}
}
