using Helpers.FileReaders;
using Helpers.Structure;

var solver = new Solver(new Day16Problem());

await solver.Solve();

class Day16Problem : ProblemBase
{
    protected override Task<string> SolvePartOneInternal()
    {
        var packet = new PacketParser().ParseHexString(originalHexString);

        return Task.FromResult(packet.GetVersionSum()
                                     .ToString());
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        throw new NotImplementedException();
    }

    public override async Task ReadInput()
    {
        //originalHexString = "A0016C880162017C3686B18A3D4780";
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
                return ParseOperatorPacket(version);
        }
    }

    private Packet ParseOperatorPacket(int version)
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
                   InternalPackets = innerPackets.ToArray()
               };
    }
}


abstract class Packet
{
    public int Version { get; set; }

    public abstract int TypeId { get; set; }

    public abstract int GetVersionSum();
}

class LiteralValuePacket : Packet
{
    public override int TypeId { get; set; } = PacketTypes.Literal;

    public long Value { get; set; }

    public override int GetVersionSum()
    {
        return this.Version;
    }
}

class OperatorPacket : Packet
{
    public override int TypeId { get; set; } = PacketTypes.Operator;

    public Packet[] InternalPackets { get; set; }

    public override int GetVersionSum()
    {
        return InternalPackets.Select(x => x.GetVersionSum())
                              .Sum()
               + this.Version;
    }
}

static class PacketTypes
{
    public const int Literal = 4;

    public const int Operator = 0;
}