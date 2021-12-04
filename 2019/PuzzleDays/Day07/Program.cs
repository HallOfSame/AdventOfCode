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
                    Console.Write("Called for input more than twice in part 1."); // Don't throw so we can run the examples
                    return 0;
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

// Part 2

maxOutput = 0;

var feedbackPhases = new List<int> { 5, 6, 7, 8, 9 };

var possibleFeedbackSettings = Permutate(feedbackPhases).ToList();

foreach (var phaseSetting in possibleFeedbackSettings)
{
    var thrusters = new (Interpreter, ManualResetEvent)[phaseSetting.Count];

    for(var i = 0; i < phaseSetting.Count; i++)
    {
        var thruster = new Interpreter();
        thrusters[i] = (thruster, new ManualResetEvent(false));
    }

    var output = new int[phaseSetting.Count];

    var tasks = new List<Task>();

    for (var i = 0; i < phaseSetting.Count; i++)
    {
        // J is the index of the thruster we are receiving from
        var j = i - 1;

        // Closures are mean
        var x = i;

        if (j < 0)
        {
            j = phaseSetting.Count - 1;
        }

        var firstCall = true;
        var secondCall = false;

        var thrusterMethod = () =>
        {
            thrusters[x].Item1.ProcessOperations(program, () =>
            {
                if (firstCall)
                {
                    firstCall = false;
                    secondCall = true;
                    return phaseSetting[x];
                }

                if (secondCall && x == 0)
                {
                    // Send 0 to first thruster once
                    secondCall = false;
                    return 0;
                }

                thrusters[j].Item2.WaitOne();
                thrusters[j].Item2.Reset();

                return output[j];
            }, (val) =>
            {
                output[x] = val;
                thrusters[x].Item2.Set();
            });
        };

        var thrusterTask = Task.Run(thrusterMethod);

        tasks.Add(thrusterTask);
    }

    Task.WaitAll(tasks.ToArray());

    var endOutput = output[phaseSetting.Count - 1];

    if (endOutput > maxOutput)
    {
        maxOutput = endOutput;
    }
}

Console.WriteLine($"Max output: {maxOutput}");