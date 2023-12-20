using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.FileReaders;
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

        private decimal PressButtonMultipleTimes(int times)
        {
            var high = 0m;
            var low = 0m;

            for (var i = 0; i < times; i++)
            {
                var (h, l) = PressButton();
                high += h;
                low += l;
            }

            return high * low;
        }

        private (int, int) PressButton()
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

            while (pulsesForThisRound.Any())
            {
                pulsesForThisRound.ForEach(x =>
                {
                    // Console.WriteLine($"{x.Source} -{(x.Type == PulseType.High ? "high" : "low")}-> {x.Destination}");
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

            return (pulseCounts[PulseType.High], pulseCounts[PulseType.Low]);
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
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
            Low,
            High,
        }
    }
}
