using Helpers.Structure;

using PuzzleDays;

var day = DateTime.Now.Date.Day;

var problemTypeName = $"PuzzleDays.Day{day.ToString().PadLeft(2, '0')}, PuzzleDays";

var problemType = Type.GetType(problemTypeName,
                               true);

var problem = (ProblemBase)Activator.CreateInstance(problemType)!;

var solver = new Solver(problem);

await solver.Solve();