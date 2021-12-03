using IntCodeInterpreter;
using IntCodeInterpreter.Input;

List<List<T>> Permutate<T>(IEnumerable<T> source)
{
    var xs = source.ToList();
    return
        xs.Count == 1
            ? new List<List<T>> { xs }
            : (
                from n in Enumerable.Range(0, xs.Count)
                let cs = xs.Skip(n).Take(1)
                let dss = Permutate<T>(xs.Take(n).Concat(xs.Skip(n + 1)))
                from ds in dss
                select cs.Concat(ds).ToList()
            ).ToList();
}

var program = await new FileInputParser().ReadOperationsFromFile("PuzzleInput.txt");

var interpreter = new Interpreter();

// Part 1

var maxOutput = 0;

var phases = new List<int> { 0, 1, 2, 3, 4 };

var possiblePhaseSettings = Permutate(phases).ToList();

foreach (var phaseSetting in possiblePhaseSettings)
{
    var currentSignal = 0;

    foreach(var phase in phaseSetting)
    {
        var thruster = phaseSetting.IndexOf(phase);

        //Console.WriteLine($"Running thruster {thruster} with phase setting {phase}. Current signal: {currentSignal}.");

        var inputCount = 0;

        interpreter.ProcessOperations(program, () =>
        {
            switch(inputCount++)
            {
                case 0:
                    return phase;
                case 1:
                    return currentSignal;
                default:
                    throw new InvalidOperationException("Called for input more than twice.");
            }
        }, (output) =>
        {
            currentSignal = output;
        });
    }

    if (currentSignal > maxOutput)
    {
        maxOutput = currentSignal;
    }
}

Console.WriteLine($"Max output: {maxOutput}");