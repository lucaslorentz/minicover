using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using MiniCover.Utils;
using Xunit;

namespace MiniCover.UnitTests.Utils
{
    public class FileUtilsTests
    {
        [Fact]
        public void GetFileHash_ShouldReturnHash()
        {
            var fileName = @"/test/ACME.Something.dll";

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(fileName, new MockFileData(new byte[] { 1, 2, 3, 4, 5 }));

            var fileInfo = mockFileSystem.FileInfo.FromFileName(fileName);
            var hash = FileUtils.GetFileHash(fileInfo);

            hash.Should().Be("7CFDD07889B3295D6A550914AB35E068");
        }

        [Fact]
        public void AddEndingDirectorySeparator_ShouldReturnNewDirectory()
        {
            var directoryName = "/test";

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(directoryName);

            var directoryInfo = mockFileSystem.DirectoryInfo.FromDirectoryName(directoryName);
            var newDirectoryInfo = FileUtils.AddEndingDirectorySeparator(directoryInfo);

            newDirectoryInfo.FullName.Should().Be("/test/");
        }

        [Fact]
        public void GetPdbFile_ShouldReturnPDBFile()
        {
            var fileName = @"/test/ACME.Something.dll";

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(fileName, new MockFileData(new byte[5]));

            var fileInfo = mockFileSystem.FileInfo.FromFileName(fileName);
            var pdbInfo = FileUtils.GetPdbFile(fileInfo);

            pdbInfo.FullName.Should().Be(@"/test/ACME.Something.pdb");
        }

        [Fact]
        public void GetBackupFile_ShouldReturnBackupFile()
        {
            var fileName = @"/test/ACME.Something.dll";

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(fileName, new MockFileData(new byte[5]));

            var fileInfo = mockFileSystem.FileInfo.FromFileName(fileName);
            var backupInfo = FileUtils.GetBackupFile(fileInfo);

            backupInfo.FullName.Should().Be(@"/test/ACME.Something.uninstrumented.dll");
        }

        [InlineData(@"/test/ACME.Something.dll", false)]
        [InlineData(@"/test/ACME.Something.uninstrumented.dll", true)]
        [Theory]
        public void IsBackupFile_ShouldReturnBackupFile(string fileName, bool isBackup)
        {
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(fileName, new MockFileData(new byte[5]));

            var fileInfo = mockFileSystem.FileInfo.FromFileName(fileName);
            var result = FileUtils.IsBackupFile(fileInfo);

            result.Should().Be(isBackup);
        }
    }
}
