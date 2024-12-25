using Helpers.Structure;

using InputStorageDatabase;

namespace PuzzleDays
{
    public class Day24 : SingleExecutionPuzzle<Day24.ExecState>
    {
        public record ExecState(Dictionary<string, int> InitialWires, Dictionary<string, GateDefinition> Gates);

        public override PuzzleInfo Info => new(2024, 24, "Crossed Wires");

        protected override async Task<ExecState> LoadInputState(string puzzleInput,
                                                                PuzzleInputType inputType)
        {
            var initialWires = new Dictionary<string, int>();
            var readingGates = false;
            var gates = new List<GateDefinition>();

            foreach(var line in puzzleInput.Trim().Split('\n'))
            {
                if (string.IsNullOrEmpty(line))
                {
                    readingGates = true;
                    continue;
                }

                if (!readingGates)
                {
                    var wire = line.Split(": ");
                    var name = wire[0];
                    var value = int.Parse(wire[1]);
                    initialWires[name] = value;
                }
                else
                {
                    var gateSplit = line.Split(" -> ");
                    var outputWire = gateSplit[1];

                    var inputSplit = gateSplit[0].Split(' ');
                    var inputOne = inputSplit[0];
                    var inputTwo = inputSplit[2];
                    var type = inputSplit[1] switch
                    {
                        "AND" => GateType.AND,
                        "OR" => GateType.OR,
                        "XOR" => GateType.XOR,
                        _ => throw new InvalidOperationException("Invalid gate type")
                    };

                    gates.Add(new GateDefinition(inputOne, inputTwo, outputWire, type));
                }
            }

            return new ExecState(initialWires, gates.ToDictionary(x => x.OutputWire, x => x));
        }

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var wireStates = new Dictionary<string, int?>();

            foreach(var startingState in InitialState.InitialWires)
            {
                wireStates[startingState.Key] = startingState.Value;
            }

            foreach(var gate in InitialState.Gates)
            {
                wireStates[gate.Key] = null;
            }

            var zWireNumber = CalculateZ(wireStates);

            return zWireNumber.ToString();
        }

        private long CalculateZ(Dictionary<string, int?> wireStates)
        {
            while(true)
            {
                var unknownWire = wireStates.Where(x => !x.Value.HasValue).Select(x => x.Key).FirstOrDefault();

                if (unknownWire is null)
                {
                    break;
                }

                CalculateWireState(unknownWire, wireStates, this.InitialState.Gates, false, []);
            }

            var zWireNumber = GetValueForWireLetter(wireStates, 'z');
            return zWireNumber;
        }

        private static long GetValueForWireLetter(Dictionary<string, int?> wireStates, char wireStart)
        {
            var zWiresInOrder = wireStates.Where(x => x.Key.StartsWith(wireStart)).OrderByDescending(x => x.Key).Select(x => x.Value);

            var zWireString = string.Join(string.Empty, zWiresInOrder);

            var zWireNumber = Convert.ToInt64(zWireString, 2);
            return zWireNumber;
        }

        private static int CalculateWireState(string targetWire, Dictionary<string, int?> wireStates, Dictionary<string, GateDefinition> gates, bool log, HashSet<string> logged)
        {
            if (wireStates.TryGetValue(targetWire, out var state) && state.HasValue)
            {
                // Console.WriteLine($"Return existing value {state} for {targetWire}");
                return state.Value;
            }

            var gateDefinitionForWire = gates[targetWire];

            if (log && logged.Add(targetWire))
            {
                Console.WriteLine($"{targetWire} relies on {gateDefinitionForWire.WireOne} {gateDefinitionForWire.Type} {gateDefinitionForWire.WireTwo}");
            }

            var inputOneState = CalculateWireState(gateDefinitionForWire.WireOne, wireStates, gates, log, logged);
            var inputTwoState = CalculateWireState(gateDefinitionForWire.WireTwo, wireStates, gates, log, logged);

            var wireState = gateDefinitionForWire.Type switch
            {
                GateType.AND => inputOneState & inputTwoState,
                GateType.OR => inputOneState | inputTwoState,
                GateType.XOR => inputOneState ^ inputTwoState,
                _ => throw new InvalidOperationException("Invalid gate type")
            };

            wireStates[targetWire] = wireState;

            return wireState;
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            Console.Clear();

            var wireStates = new Dictionary<string, int?>();

            foreach(var startingState in InitialState.InitialWires)
            {
                wireStates[startingState.Key] = startingState.Value;
            }

            foreach(var gate in InitialState.Gates)
            {
                wireStates[gate.Key] = null;
            }

            var swapPairs = new[]
            {
                ("z05", "bpf"),
                ("z11", "hcc"),
                ("qcw", "hqc"),
                ("fdw", "z35")
            };

            foreach (var (gateOne, gateTwo) in swapPairs)
            {
                var temp = InitialState.Gates[gateOne];
                InitialState.Gates[gateOne] = InitialState.Gates[gateTwo] with { OutputWire = gateOne };
                InitialState.Gates[gateTwo] = temp with { OutputWire = gateTwo };
            }

            var logged = new HashSet<string>();

            // Check direct addition of a single index
            // For each binary digit of our 44 bit inputs
            for (var i = 0; i < 45; i++)
            {
                // Try all combinations of X and Y
                foreach (var (x, y, expected) in new [] {(0, 0, 0), (0, 1, 1), (1, 0, 1), (1, 1, 0)})
                {
                    // Create the wireState dictionary with all inputs as 0
                    var zeroWireState = wireStates.ToDictionary(x => x.Key, x => x.Key.StartsWith('x') || x.Key.StartsWith('y') ? 0 : x.Value);

                    // Set just our current index
                    zeroWireState[$"x{i:0#}"] = x;
                    zeroWireState[$"y{i:0#}"] = y;

                    var log = x == 0 && y == 0;

                    if (log)
                    {
                        Console.WriteLine("---------------------------------");
                    }

                    // Check if the z at the index matches our expected result
                    CalculateWireState($"z{i:0#}", zeroWireState, InitialState.Gates, x == 0 && y == 0, logged);

                    var resultState = zeroWireState[$"z{i:0#}"];

                    // If it doesn't we can start inspecting the console output
                    // We will have logged what z_i relies upon including any new gates we've seen for the first time (to cut down the logs)
                    // Generally we're expected x_i XOR y_i combined with some other gates that bring in the carry (like 4-5 new ones)
                    // And the z gate should be the XOR of the matching X & Y gates with the carry
                    if (resultState != expected)
                    {
                        Console.WriteLine($"For index {i} X: {x} Y: {y} expected {expected} but got {resultState}");
                    }
                }
            }

            var allWireSwapNames = string.Join(",",
                                               swapPairs.SelectMany(x => new [] {x.Item1, x.Item2})
                                                   .OrderBy(x => x));

            return allWireSwapNames;
        }

        public record GateDefinition(string WireOne, string WireTwo, string OutputWire, GateType Type);
        public enum GateType
        {
            AND,
            OR,
            XOR
        }
    }
}
