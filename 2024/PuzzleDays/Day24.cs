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

                CalculateWireState(unknownWire, wireStates, this.InitialState.Gates);
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

        private static int CalculateWireState(string targetWire, Dictionary<string, int?> wireStates, Dictionary<string, GateDefinition> gates)
        {
            if (wireStates.TryGetValue(targetWire, out var state) && state.HasValue)
            {
                // Console.WriteLine($"Return existing value {state} for {targetWire}");
                return state.Value;
            }

            var gateDefinitionForWire = gates[targetWire];

            Console.WriteLine($"{targetWire} relies on {gateDefinitionForWire.WireOne} {gateDefinitionForWire.Type} {gateDefinitionForWire.WireTwo}");

            var inputOneState = CalculateWireState(gateDefinitionForWire.WireOne, wireStates, gates);
            var inputTwoState = CalculateWireState(gateDefinitionForWire.WireTwo, wireStates, gates);

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
            var wireStates = new Dictionary<string, int?>();

            foreach(var startingState in InitialState.InitialWires)
            {
                wireStates[startingState.Key] = startingState.Value;
            }

            foreach(var gate in InitialState.Gates)
            {
                wireStates[gate.Key] = null;
            }

            var xValue = GetValueForWireLetter(wireStates, 'x');
            var yValue = GetValueForWireLetter(wireStates, 'y');

            var expectedValue = xValue + yValue;
            
            var partOneResult = 58367545758258;

            var currentString = Convert.ToString(partOneResult, 2);
            var expectedString = Convert.ToString(expectedValue, 2);

            var incorrectIndices = new List<int>();

            for(var i = 0; i < expectedString.Length; i++)
            {
                if (currentString[i] != expectedString[i])
                {
                    incorrectIndices.Add(i);
                }
            }

            //var gatesToTrySwap = new HashSet<GateDefinition>();

            //foreach(var index in  incorrectIndices)
            //{
            //    var zGate = InitialState.Gates[$"z{index:0#}"];

            //    var gateQueue = new Queue<GateDefinition>();
            //    gateQueue.Enqueue(zGate);

            //    while(gateQueue.Count > 0)
            //    {
            //        var nextGate = gateQueue.Dequeue();

            //        gatesToTrySwap.Add(nextGate);

            //        if (InitialState.Gates.TryGetValue(nextGate.WireOne, out var leftSide))
            //        {
            //            gateQueue.Enqueue(leftSide);
            //        }

            //        if (InitialState.Gates.TryGetValue(nextGate.WireTwo, out var rightSide))
            //        {
            //            gateQueue.Enqueue(rightSide);
            //        }
            //    }               
            //}

            var wireStateCopy = wireStates.ToDictionary(x => x.Key, x => x.Value);

            for(var i = 0; i <= 45; i++)
            {
                CalculateWireState($"z{i:0#}", wireStates, InitialState.Gates);
            }

            SetXAndY(1, 1, wireStateCopy);
            var result = CalculateZ(wireStateCopy);
            

            return "Not Yet";
        }

        private void SetXAndY(int xIndex, int yIndex, Dictionary<string, int?> wireState)
        {
            for(var i = 0; i < 45; i++)
            {
                wireState[$"x{i:0#}"] = i == xIndex ? 1 : 0;
                wireState[$"y{i:0#}"] = i == yIndex ? 1 : 0;
            }
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
