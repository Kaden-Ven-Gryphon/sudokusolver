using Grpc.Core;
using SudokuSolver;
using GrpcSolver.Mappers;

namespace GrpcSolver.Services
{
	/// <summary>
	/// Service for solving puzzle using standard sudoku rules
	/// </summary>
	public class StandardSolverService : StandardSolver.StandardSolverBase
	{
		private readonly ILogger<StandardSolverService> _logger;

		public StandardSolverService(ILogger<StandardSolverService> logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Fills the board with pencil marks and cancel out non posible marks
		/// </summary>
		/// <param name="request"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public override Task<BoardStateReply> FillPencilMarks(BoardStateRequest request, ServerCallContext context)
		{
			try
			{
				_logger.LogInformation("Start: FillPencilMarks");
				return Task.Run(() =>
				{
					BoardStateReply reply;
					var solver = new SudokuSolver.StandardSolver(PuzzleMessageMapper.PuzzleBaseFromBoardStateRequest(request));

					solver.FillAllValidPencilMarks();

					reply = PuzzleMessageMapper.BoardStateReplyFromPuzzleBase(solver, solver.Logs);
					_logger.LogInformation("End: FillPencilMarks");
					return reply;
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				//TODO Change to null/error return
				return Task.Run(() => { return new BoardStateReply(); });
			}
			
		}

		/// <summary>
		/// Fill cells that have naked single pencil marks with that value
		/// </summary>
		/// <param name="request"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public override Task<BoardStateReply> FillNakedSingles(BoardStateRequest request, ServerCallContext context)
		{
			return Task.Run(() =>
			{
				BoardStateReply reply = new BoardStateReply();

				return reply;
			});
		}

		/// <summary>
		/// Use naked Doubles to eliminate pencil marks
		/// </summary>
		/// <param name="request"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public override Task<BoardStateReply> EliminateNakedDoubles(BoardStateRequest request, ServerCallContext context)
		{
			return Task.Run(() =>
			{
				BoardStateReply reply = new BoardStateReply();

				return reply;
			});
		}

		/// <summary>
		/// Solve puzzle using any stratagies avalible
		/// </summary>
		/// <param name="request"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public override Task<BoardStateReply> Solve(BoardStateRequest request, ServerCallContext context)
		{
			return Task.Run(() =>
			{
				BoardStateReply reply = new BoardStateReply();

				return reply;
			});
		}
	}
}
