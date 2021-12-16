using Helpers.FileReaders;
using Helpers.Structure;

var solver = new Solver(new Day16Problem());

await solver.Solve();

class Day16Problem : ProblemBase
{
    Packet parsedPacket;

    protected override Task<string> SolvePartOneInternal()
    {
        parsedPacket = new PacketParser().ParseHexString(originalHexString);

        return Task.FromResult(parsedPacket.GetVersionSum()
                                           .ToString());
    }

    protected override Task<string> SolvePartTwoInternal()
    {
        return Task.FromResult(parsedPacket.GetValue()
                                           .ToString());
    }

    public override async Task ReadInput()
    {
        //originalHexString = "9C0141080250320F1802104A08";
        originalHexString = (await new StringFileReader().ReadInputFromFile()).First();
    }

    private string originalHexString;
}

class PacketParser
{
    private string binaryString;

    private int currentIndex;

    private string GetBinary(int bitCount)
    {
        var subString = binaryString.Substring(currentIndex,
                                               bitCount);

        currentIndex += bitCount;

        return subString;
    }

    private long GetLiteralValue()
    {
        var binaryValue = string.Empty;

        while (true)
        {
            var nextDigit = GetBinary(5);

            var isLast = nextDigit.StartsWith("0");

            binaryValue += nextDigit[1..5];

            if (isLast)
            {
                break;
            }
        }

        return Convert.ToInt64(binaryValue,
                               2);
    }

    public Packet ParseHexString(string hexString)
    {
        this.binaryString = string.Join(string.Empty,
                                        hexString.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(),
                                                                                               16),
                                                                               2)
                                                                     .PadLeft(4,
                                                                              '0')));
        currentIndex = 0;

        return ParsePacket();
    }

    private Packet ParsePacket()
    {
        var first6Bits = GetBinary(6);

        var version = Convert.ToInt32(first6Bits[..3], 2);
        var type = Convert.ToInt32(first6Bits[3..6], 2);

        switch (type)
        {
            case PacketTypes.Literal:
                var value = GetLiteralValue();

                return new LiteralValuePacket
                       {
                           Version = version,
                           Value = value
                       };
            default:
                // Operator
                return ParseOperatorPacket(version,
                                           type);
        }
    }

    private Packet ParseOperatorPacket(int version,
                                       int type)
    {
        var lengthType = GetBinary(1);

        Func<bool> isFinished;

        var bitsReadCount = 0;

        var innerPackets = new List<Packet>();

        switch (lengthType)
        {
            case "0":
                var lengthInBits = Convert.ToInt32(GetBinary(15), 2);
                isFinished = () => bitsReadCount >= lengthInBits;
                break;
            case "1":
                var numberOfPackets = Convert.ToInt32(GetBinary(11), 2);
                isFinished = () => innerPackets.Count == numberOfPackets;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(lengthType),
                                                      "Unexpected value for length type ID.");
        }

        var startIndex = currentIndex;

        while (!isFinished())
        {
            innerPackets.Add(ParsePacket());

            bitsReadCount = currentIndex - startIndex;
        }

        return new OperatorPacket
               {
                   Version = version,
                   TypeId = type,
                   InternalPackets = innerPackets.ToArray()
               };
    }
}


abstract class Packet
{
    public int Version { get; set; }

    public abstract int TypeId { get; set; }

    public abstract int GetVersionSum();

    public abstract long GetValue();
}

class LiteralValuePacket : Packet
{
    public override int TypeId { get; set; } = PacketTypes.Literal;

    public long Value { get; set; }

    public override int GetVersionSum()
    {
        return this.Version;
    }

    public override long GetValue()
    {
        return this.Value;
    }
}

class OperatorPacket : Packet
{
    public override int TypeId { get; set; }

    public Packet[] InternalPackets { get; set; }

    public override int GetVersionSum()
    {
        return InternalPackets.Select(x => x.GetVersionSum())
                              .Sum()
               + this.Version;
    }

    public override long GetValue()
    {
        long GetComparison()
        {
            var valueOne = this.InternalPackets[0]
                               .GetValue();
            var valueTwo = this.InternalPackets[1]
                               .GetValue();

            switch (this.TypeId)
            {
                case PacketTypes.Gt:
                    return valueOne > valueTwo
                               ? 1L
                               : 0L;
                case PacketTypes.Lt:
                    return valueOne < valueTwo
                               ? 1L
                               : 0L;
                case PacketTypes.Eq:
                    return valueOne == valueTwo
                               ? 1L
                               : 0L;
            }

            throw new InvalidOperationException();
        }

        switch (this.TypeId)
        {
            case PacketTypes.Sum:
                return this.InternalPackets.Sum(x => x.GetValue());
            case PacketTypes.Product:
                return this.InternalPackets.Aggregate(1L,
                                                      (curr,
                                                       next) => curr * next.GetValue());
            case PacketTypes.Min:
                return this.InternalPackets.Min(x => x.GetValue());
            case PacketTypes.Max:
                return this.InternalPackets.Max(x => x.GetValue());
            case PacketTypes.Gt:
            case PacketTypes.Lt:
            case PacketTypes.Eq:
                return GetComparison();
            default:
                throw new ArgumentOutOfRangeException(nameof(TypeId),
                                                      "Type ID value not supported.");
        }
    }
}

static class PacketTypes
{
    public const int Literal = 4;

    public const int Sum = 0;

    public const int Product = 1;

    public const int Min = 2;

    public const int Max = 3;

    public const int Gt = 5;

    public const int Lt = 6;

    public const int Eq = 7;
}