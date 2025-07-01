using Grpc.Core;
using SudokuSolver.Base;
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
					var solver = new SudokuSolver.Solvers.StandardSolver(PuzzleMessageMapper.PuzzleBaseFromBoardStateRequest(request));

					solver.FillAllValidPencilMarks();

					reply = PuzzleMessageMapper.BoardStateReplyFromPuzzleBase(solver, solver.Logs);
					_logger.LogInformation("End: FillPencilMarks");
					return reply;
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed");
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
            try
            {
                _logger.LogInformation("Start: FillNakedSingles");
                return Task.Run(() =>
                {
                    BoardStateReply reply;
                    var solver = new SudokuSolver.Solvers.StandardSolver(PuzzleMessageMapper.PuzzleBaseFromBoardStateRequest(request));

                    solver.FillNakedSingles();

                    reply = PuzzleMessageMapper.BoardStateReplyFromPuzzleBase(solver, solver.Logs);
                    _logger.LogInformation("End: FillNakedSingles");
                    return reply;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                //TODO Change to null/error return
                return Task.Run(() => { return new BoardStateReply(); });
            }
        }

		/// <summary>
		/// Use naked Doubles to eliminate pencil marks
		/// </summary>
		/// <param name="request"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public override Task<BoardStateReply> EliminateNakedDoubles(BoardStateRequest request, ServerCallContext context)
		{
            try
            {
                _logger.LogInformation("Start: EliminateNakedDoubles");
                return Task.Run(() =>
                {
                    BoardStateReply reply;
                    var solver = new SudokuSolver.Solvers.StandardSolver(PuzzleMessageMapper.PuzzleBaseFromBoardStateRequest(request));

                    solver.EliminateNakedDoubles();

                    reply = PuzzleMessageMapper.BoardStateReplyFromPuzzleBase(solver, solver.Logs);
                    _logger.LogInformation("End: EliminateNakedDoubles");
                    return reply;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                //TODO Change to null/error return
                return Task.Run(() => { return new BoardStateReply(); });
            }
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

		public override Task<SolvedReply> BoardIsSolved(BoardStateRequest request, ServerCallContext context)
		{
            try
            {
                _logger.LogInformation("Start: BoardIsSolved");
                return Task.Run(() =>
                {
                    SolvedReply reply = new SolvedReply();
                    var solver = new SudokuSolver.Solvers.StandardSolver(PuzzleMessageMapper.PuzzleBaseFromBoardStateRequest(request));

                    var isSolved = solver.PuzzleIsSolved();

					reply.Solved = isSolved;

                    _logger.LogInformation("End: BoardIsSolved");
                    return reply;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                //TODO Change to null/error return
                return Task.Run(() => { return new SolvedReply(); });
            }
        }
    }
}
