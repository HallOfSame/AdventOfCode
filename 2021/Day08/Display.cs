namespace Day08
{
    internal class DisplayMetadata
    {
        public Display OutputDisplay { get; init; }

        public Segment[] UniqueSignals { get; init; }
    }

    internal class Display
    {
        public Segment[] Segments { get; init; }
    }

    internal class Segment
    {
        public Segment(SignalWire[] signalWires,
                       bool determineDigit)
        {
            EnabledWires = signalWires;

            var attemptedDigit = CorrectSignals.Where(x => x.Value.Length == EnabledWires.Length)
                .ToList();

            if (attemptedDigit.Count == 1)
            {
                DisplayedDigit = attemptedDigit[0].Key;
            }
            else
            {
                if (determineDigit)
                {
                    var correctSignal = CorrectSignals.Where(x => x.Value.ToHashSet().SetEquals(signalWires.ToHashSet()))
                        .ToList();

                    if (correctSignal.Count == 1)
                    {
                        DisplayedDigit = correctSignal[0].Key;
                    }
                    else
                    {
                        DisplayedDigit = Digit.Unknown;
                    }
                }
                else
                {
                    DisplayedDigit = Digit.Unknown;
                }
            }
        }

        public SignalWire[] EnabledWires { get; }

        public Digit DisplayedDigit { get; set; }

        public static Dictionary<Digit, SignalWire[]> CorrectSignals = new Dictionary<Digit, SignalWire[]>
        {
            { Digit.Zero, new [] { SignalWire.a, SignalWire.b, SignalWire.c, SignalWire.e, SignalWire.f, SignalWire.g } },
            { Digit.One, new [] { SignalWire.c, SignalWire.f} },
            { Digit.Two, new [] { SignalWire.a, SignalWire.c, SignalWire.d, SignalWire.e, SignalWire.g } },
            { Digit.Three, new [] { SignalWire.a, SignalWire.c, SignalWire.d, SignalWire.f, SignalWire.g } },
            { Digit.Four, new [] { SignalWire.b, SignalWire.c, SignalWire.d, SignalWire.f } },
            { Digit.Five, new [] { SignalWire.a, SignalWire.b, SignalWire.d, SignalWire.f, SignalWire.g } },
            { Digit.Six, new [] { SignalWire.a, SignalWire.b, SignalWire.d, SignalWire.e, SignalWire.f, SignalWire.g } },
            { Digit.Seven, new [] { SignalWire.a, SignalWire.c, SignalWire.f } },
            { Digit.Eight, new [] { SignalWire.a, SignalWire.b, SignalWire.c, SignalWire.d, SignalWire.e, SignalWire.f, SignalWire.g } },
            { Digit.Nine, new [] { SignalWire.a, SignalWire.b, SignalWire.c, SignalWire.d, SignalWire.f, SignalWire.g } },
        };
    }
    
    enum SignalWire
    {
        a,
        b,
        c,
        d,
        e,
        f,
        g
    }

    enum Digit
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Unknown = 10
    }
}
