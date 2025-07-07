using Grpc.Core;
using GrpcSolver;
using SudokuSolver.Base;
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
				_fileManager.Clear();
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

		/// <summary>
		/// Imports a puzzle / creates new puzzle and writes to file
		/// </summary>
		/// <param name="request"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public override Task<ImportReply> ImportPuzzle(ImportRequest request, ServerCallContext context)
		{
			_logger.LogInformation("Starting ImportPuzzle");
			try
			{
                return Task.Run(() =>
                {
                    _fileManager.ImportNewPuzzleSimple(request.Name, request.Given.ToArray());
                    ImportReply reply = new ImportReply();

                    return reply;
                });
            }
			catch (Exception ex)
			{
				_logger.LogError(ex, "Puzzle Import Failed");
				return Task.FromResult(new ImportReply { Sucsess = false});
			}
			
		}
	}
}
