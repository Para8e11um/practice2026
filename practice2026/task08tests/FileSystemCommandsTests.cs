using System.IO;
using Xunit;
using FileSystemCommands;
public class FileSystemCommandsTests
{
    public class FileSystemCommands
    {
        [Fact]
        public void DirectorySizeCommand_ShouldCalculateSize()
        {
            var testDir = Path.Combine(Path.GetTempPath(), "TestDirSize");
            Directory.CreateDirectory(testDir);

            var test1Path = Path.Combine(testDir, "test1.txt");
            var test2Path = Path.Combine(testDir, "test2.txt");
            File.WriteAllText(test1Path, "Hello");
            File.WriteAllText(test2Path, "World");

            long expectedSize = new FileInfo(test1Path).Length + new FileInfo(test2Path).Length;
            var command = new DirectorySizeCommand(testDir);
            command.Execute();
            Assert.Equal(expectedSize, command.Size);
            Directory.Delete(testDir, true);
        }
        [Fact]
        public void FindFilesCommand_ShouldFindMatchingFiles()
        {
            var testDir = Path.Combine(Path.GetTempPath(), "TestDirFind");
            Directory.CreateDirectory(testDir);
            var targetFile = Path.Combine(testDir, "file1.txt");
            var ignoreFile = Path.Combine(testDir, "file2.log");
            File.WriteAllText(targetFile, "Text content");
            File.WriteAllText(targetFile, "Log content");
            var command = new FindFilesCommand(testDir, "*.txt");
            command.Execute();
            Assert.Single(command.FoundFiles);
            Assert.Contains(targetFile, command.FoundFiles);
            Directory.Delete(testDir, true);
        }
    }
}