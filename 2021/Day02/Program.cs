using Helpers;

var puzzleInput = await new SubCommandReader().ReadInputFromFile();

// Part 1

var horizontalPos = 0;
var verticalPos = 0;

foreach (var command in puzzleInput)
{
    switch (command.Direction)
    {
        case CommandDirection.Forward:
            horizontalPos += command.Distance;
            break;
        case CommandDirection.Up:
            verticalPos -= command.Distance;
            break;
        case CommandDirection.Down:
            verticalPos += command.Distance;
            break;
    }
}

Console.WriteLine($"Horiz: {horizontalPos} Vert: {verticalPos}. Answer: {horizontalPos * verticalPos}.");

// Part 2

var aim = 0;
horizontalPos = 0;
verticalPos = 0;

foreach (var command in puzzleInput)
{
    switch (command.Direction)
    {
        case CommandDirection.Forward:
            horizontalPos += command.Distance;

            verticalPos += (aim * command.Distance);
            break;
        case CommandDirection.Up:
            aim -= command.Distance;
            break;
        case CommandDirection.Down:
            aim += command.Distance;
            break;
    }
}

Console.WriteLine($"Horiz: {horizontalPos} Vert: {verticalPos}. Answer: {horizontalPos * verticalPos}.");

enum CommandDirection
{
    Forward,
    Down,
    Up
}

class SubCommand
{
    public CommandDirection Direction { get; set; }

    public int Distance { get; set; }
}

class SubCommandReader : FileReader<SubCommand>
{
    protected override SubCommand ProcessLineOfFile(string line)
    {
        var inputSplit = line.Split(' ');

        var direction = inputSplit[0] switch
        {
            "forward" => CommandDirection.Forward,
            "up" => CommandDirection.Up,
            "down" => CommandDirection.Down,
            _ => throw new InvalidDataException($"Unexpected direction {inputSplit[0]}.")
        };

        var distance = int.Parse(inputSplit[1]);

        return new SubCommand
               {
                   Direction = direction,
                   Distance = distance
               };
    }
}