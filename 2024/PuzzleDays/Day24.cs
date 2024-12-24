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

            while(true)
            {
                var unknownWire = wireStates.Where(x => !x.Value.HasValue).Select(x => x.Key).FirstOrDefault();

                if (unknownWire is null)
                {
                    break;
                }

                CalculateWireState(unknownWire, wireStates, InitialState.Gates);
            }

            var zWiresInOrder = wireStates.Where(x => x.Key.StartsWith('z')).OrderByDescending(x => x.Key).Select(x => x.Value);

            var zWireString = string.Join(string.Empty, zWiresInOrder);

            var zWireNumber = Convert.ToInt64(zWireString, 2);

            return zWireNumber.ToString();
        }

        private int CalculateWireState(string targetWire, Dictionary<string, int?> wireStates, Dictionary<string, GateDefinition> gates)
        {
            if (wireStates.TryGetValue(targetWire, out var state) && state.HasValue)
            {
                return state.Value;
            }

            var gateDefinitionForWire = gates[targetWire];

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
            throw new NotImplementedException();
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
