
using IntCodeInterpreter;
using IntCodeInterpreter.Input;

var interpreter = new Interpreter();

var program = await new FileInputParser().ReadOperationsFromFile("PuzzleInput.txt");

Console.WriteLine("Running program...");

// Part 1

interpreter.ProcessOperations(program, 1, x =>
{
    Console.WriteLine($"Program output: {x}.");
});

// Part 2

interpreter.ProcessOperations(program, 2, x =>
{
    Console.WriteLine($"Program output: {x}.");
});