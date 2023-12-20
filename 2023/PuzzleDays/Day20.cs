using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.FileReaders;
using Helpers.MathAndSuch;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day20 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var result = PressButtonMultipleTimes(1000);

            return result.ToString();
        }

        private List<string> inputsConnectedToVR = new()
        {
            "fm", "fg", "dk", "pq"
        };

        private decimal PressButtonMultipleTimes(int times)
        {
            var high = 0m;
            var low = 0m;

            var cycles = new Dictionary<string, int>();

            var i = 0;

            while(true)
            {
                var (h, l, lowHit) = PressButton();

                if (!string.IsNullOrEmpty(lowHit))
                {
                    cycles[lowHit] = i + 1;
                }

                if (i < times)
                {
                    high += h;
                    low += l;
                }

                i++;

                if (i >= times && cycles.Count == 4)
                {
                    break;
                }
            }

            part2 = MathFunctions.LeastCommonMultiple(cycles.Values.Select(x => (decimal)x));

            if (part2 >= 89448022004224314928m)
            {
                Console.WriteLine("Too high");
            }
            else if (part2 <= 932332404000)
            {
                Console.WriteLine("Too low");
            }

            return high * low;
        }

        private decimal part2;

        private (int, int, string) PressButton()
        {
            var pulseCounts = new Dictionary<PulseType, int>
            {
                { PulseType.High, 0 }, { PulseType.Low, 0 }
            };

            var pulsesForThisRound = new List<Pulse>
            {
                new()
                {
                    Type = PulseType.Low,
                    Source = "button",
                    Destination = "broadcaster",
                }
            };

            var lowHitCycle = string.Empty;

            while (pulsesForThisRound.Any())
            {
                pulsesForThisRound.ForEach(x =>
                {
                    if (inputsConnectedToVR.Contains(x.Destination))
                    {
                        if (x.Type == PulseType.Low)
                        {
                            if (!string.IsNullOrWhiteSpace(lowHitCycle))
                            {
                                throw new Exception();
                            }

                            lowHitCycle = x.Destination;

                            Console.WriteLine($"{x.Source} -{(x.Type == PulseType.High ? "high" : "low")}-> {x.Destination}");
                        }
                    }

                    pulseCounts[x.Type] += 1;
                });

                var newPulses = new List<Pulse>();

                foreach (var currPulse in pulsesForThisRound)
                {
                    if (!moduleDictionary.TryGetValue(currPulse.Destination, out var destination))
                    {
                        // Can be expected for untyped modules like 'output' in example 2
                        continue;
                    }

                    var extraPulses = destination.HandlePulse(currPulse);

                    newPulses.AddRange(extraPulses);
                }

                pulsesForThisRound = newPulses;
            }

            return (pulseCounts[PulseType.High], pulseCounts[PulseType.Low], lowHitCycle);
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            return part2.ToString();
        }

        private Dictionary<string, Module> moduleDictionary;

        public override async Task ReadInput()
        {
            var lines = await new StringFileReader().ReadInputFromFile();

            var modules = new List<Module>();

            foreach (var line in lines)
            {
                var lineSplit = line.Split(" -> ");

                var moduleName = lineSplit[0];

                var destinations = lineSplit[1]
                    .Split(", ")
                    .ToList();

                if (moduleName == "broadcaster")
                {
                    modules.Add(new BroadcastModule
                    {
                        Destinations = destinations
                    });

                    continue;
                }

                var destination = lineSplit[1];

                if (moduleName.StartsWith("&"))
                {
                    modules.Add(new ConjunctionModule(moduleName[1..])
                    {
                        Destinations = destinations
                    });
                }
                else if (moduleName.StartsWith("%"))
                {
                    modules.Add(new FlipFlopModule(moduleName[1..])
                    {
                        Destinations = destinations
                    });
                }
                else
                {
                    modules.Add(new OutputModule(moduleName));
                }
            }

            moduleDictionary = modules.ToDictionary(x => x.Name, x => x);

            // Wire up conjuction modules
            var conjuctionModules = modules.Where(x => x is ConjunctionModule)
                .Cast<ConjunctionModule>()
                .ToList();

            foreach (var conjunction in conjuctionModules)
            {
                modules.Where(x => x.Destinations.Contains(conjunction.Name))
                    .ToList()
                    .ForEach(x => conjunction.InputPulseTypes.Add(x.Name, PulseType.Low));
            }
        }

        abstract class Module
        {
            protected Module(string name)
            {
                Name = name;
            }

            public string Name { get; }

            public List<string> Destinations { get; init; }

            public abstract List<Pulse> HandlePulse(Pulse pulse);

            protected List<Pulse> GetResponsePulses(PulseType type)
            {
                return Destinations.Select(x => new Pulse
                    {
                        Source = Name,
                        Destination = x,
                        Type = type,
                    })
                    .ToList();
            }
        }

        class BroadcastModule : Module
        {
            public BroadcastModule() : base("broadcaster")
            {
            }

            public override List<Pulse> HandlePulse(Pulse pulse)
            {
                return GetResponsePulses(pulse.Type);
            }
        }

        class OutputModule : Module
        {
            public OutputModule(string name) : base(name)
            {
                this.Destinations = new List<string>();
            }

            public override List<Pulse> HandlePulse(Pulse pulse)
            {
                return new List<Pulse>();
            }
        }

        class FlipFlopModule : Module
        {
            public FlipFlopModule(string name) : base(name) { }

            private bool isOn = false;

            public override List<Pulse> HandlePulse(Pulse pulse)
            {
                var output = new List<Pulse>();

                if (pulse.Type == PulseType.High)
                {
                    return new List<Pulse>();
                }

                isOn = !isOn;

                return GetResponsePulses(isOn ? PulseType.High : PulseType.Low);
            }
        }

        class ConjunctionModule : Module
        {
            public Dictionary<string, PulseType> InputPulseTypes = new();

            public ConjunctionModule(string name) : base(name)
            {
            }

            public override List<Pulse> HandlePulse(Pulse pulse)
            {
                InputPulseTypes[pulse.Source] = pulse.Type;

                PulseType outputType;

                if (InputPulseTypes.Values.All(x => x == PulseType.High))
                {
                    outputType = PulseType.Low;
                }
                else
                {
                    outputType = PulseType.High;
                }

                return GetResponsePulses(outputType);
            }
        }

        class Pulse
        {
            public string Source { get; set; }
            public PulseType Type { get; set; }

            public string Destination { get; set; }
        }

        enum PulseType
        {
            Low = 0,
            High = 1,
        }
    }
}
