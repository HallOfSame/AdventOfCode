using System.Text;
using System.Text.RegularExpressions;

using Helpers.FileReaders;
using Helpers.Structure;

using Spectre.Console;

namespace PuzzleDays
{
    public class Day07 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            fileSystem.Draw();

            var result = directories.Select(x => x.Size())
                                    .Where(size => size <= 100_000)
                                    .Sum();

            return result.ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var totalSize = 70_000_000;

            var freeSpaceNeeded = 30_000_000;

            var usedSpace = fileSystem.Root.Size();

            var currentFreeSpace = totalSize - usedSpace;

            var deleteAmountRequired = freeSpaceNeeded - currentFreeSpace;

            var directorySizeToDelete = directories.Select(x => x.Size())
                                                   .Where(size => size >= deleteAmountRequired)
                                                   .OrderBy(x => x)
                                                   .First();

            return directorySizeToDelete.ToString();
        }

        public override async Task ReadInput()
        {
            fileSystem = new FileSystem();

            directories = new HashSet<Directory>
                          {
                              fileSystem.Root
                          };

            Directory currentDirectory = null;

            var lines = await new StringFileReader().ReadInputFromFile();

            var lineIndex = 0;

            var cdRegex = new Regex("\\$ cd (.*)");
            var fileRegex = new Regex("(\\d+) (.*)");

            while(lineIndex < lines.Count)
            {
                var line = lines[lineIndex];

                lineIndex++;

                if (line.StartsWith("$"))
                {
                    if (line.Equals("$ ls"))
                    {
                        var lsLine = lines[lineIndex];

                        while (!lsLine.StartsWith("$"))
                        {
                            if (lsLine.StartsWith("dir"))
                            {
                                var dirName = lsLine.Split(' ')[1];

                                if (!currentDirectory.Directories.ContainsKey(dirName))
                                {
                                    var newDirectory = new Directory
                                                       {
                                                           Name = dirName,
                                                           Parent = currentDirectory
                                                       };

                                    currentDirectory.Directories.Add(dirName, newDirectory);

                                    directories.Add(newDirectory);
                                }
                            }
                            else
                            {
                                var fileData = fileRegex.Match(lsLine);

                                var fileSize = int.Parse(fileData.Groups[1]
                                                                 .Value);
                                var fileName = fileData.Groups[2]
                                                       .Value;

                                if (!currentDirectory.Files.ContainsKey(fileName))
                                {
                                    var newFile = new File
                                                  {
                                                      Name = fileName,
                                                      Size = fileSize
                                                  };

                                    currentDirectory.Files.Add(newFile.Name,
                                                               newFile);
                                }
                            }

                            lineIndex++;

                            if (lineIndex >= lines.Count)
                            {
                                break;
                            }

                            lsLine = lines[lineIndex];
                        }
                    }
                    else
                    {
                        var newDirName = cdRegex.Match(line)
                                                .Groups[1]
                                                .Value;

                        if (newDirName.Equals(".."))
                        {
                            if (currentDirectory.Parent is null)
                            {
                                throw new Exception("");
                            }

                            currentDirectory = currentDirectory.Parent;
                        }
                        else if (newDirName.Equals("/"))
                        {
                            currentDirectory = fileSystem.Root;
                        }
                        else
                        {
                            currentDirectory = currentDirectory.Directories[newDirName];
                        }
                    }
                }
            }
        }

        private FileSystem fileSystem;

        private HashSet<Directory> directories;
    }
}

class FileSystem
{
    public Directory Root { get; } = new()
                                     {
                                         Name = "/",
                                         Parent = null,
                                     };

    public void Draw()
    {
        var stringBuilder = new StringBuilder();
        Root.Draw(0, stringBuilder);
        AnsiConsole.WriteLine(stringBuilder.ToString());
    }
}

class Directory
{
    public Directory? Parent { get; init; }

    public string Name { get; init; }

    public Dictionary<string, File> Files { get; } = new();

    public Dictionary<string, Directory> Directories { get; } = new();

    private int cachedSize = -1;

    public int Size()
    {
        if (cachedSize < 0)
        {
            cachedSize = Files.Values.Sum(x => x.Size) + Directories.Values.Sum(x => x.Size());
        }
        
        return cachedSize;
    }

    public void Draw(int indentLevel, StringBuilder sb)
    {
        sb.AppendWithIndent(indentLevel, $"- {Name} (dir)");
        foreach (var directory in Directories.Values)
        {
            directory.Draw(indentLevel + 1, sb);
        }

        foreach (var file in Files.Values)
        {
            file.Draw(indentLevel + 1, sb);
        }
    }
}

class File
{
    public string Name { get; init; }

    public int Size { get; init; }

    public void Draw(int indentLevel, StringBuilder sb)
    {
        sb.AppendWithIndent(indentLevel,
                            $"- {Name} (file, {Size})");
    }
}

static class Output
{
    public static void AppendWithIndent(this StringBuilder sb, int level, string message)
    {
        var indent = string.Join(string.Empty,
                                 Enumerable.Repeat("\t",
                                                   level));

        sb.AppendLine($"{indent}{message}");
    }
}