using Grpc.Core;
using GrpcSolver;
using SudokuSolver;
using GrpcSolver.Mappers;

namespace GrpcSolver.Services
{
	/// <summary>
	/// Service for getting and saving puzzle files
	/// </summary>
	public class PuzzleManagerService : PuzzleManager.PuzzleManagerBase
	{
		private readonly ILogger<PuzzleManagerService> _logger;

		private static PuzzleFileManager _fileManager = new PuzzleFileManager();

		public PuzzleManagerService(ILogger<PuzzleManagerService> logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Gets dictionary of puzzle names and their ids
		/// </summary>
		/// <param name="request"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public override Task<ListReply> GetPuzzleListAndIds(PathRequest request, ServerCallContext context)
		{
			return Task.Run(() => {
				ListReply reply = new ListReply();

				_fileManager.Path = request.Path;
				_fileManager.ScanPath();
				var dict = _fileManager.GetPuzzleNamesAndIds();

				
				reply.PuzzleFiles.Add(dict);

				return reply;
			});
		}

		/// <summary>
		/// Gets a puzzleBase for the given id
		/// </summary>
		/// <param name="request">The id of the puzzle.</param>
		/// <param name="context">Puzzle base info.</param>
		/// <returns></returns>
		public override Task<PuzzleReply> GetPuzzle(PuzzleRequest request, ServerCallContext context)
		{
			return Task.Run(() =>
			{
				var puzzle = _fileManager.GetPuzzleById(request.Id);
				if (puzzle == null) { puzzle = new PuzzleFile(); }
				PuzzleReply reply = PuzzleMessageMapper.PuzzleReplyFromPuzzleBase(puzzle);

				return reply;
			});
		}
	}
}
