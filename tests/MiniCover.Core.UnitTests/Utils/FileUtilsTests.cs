using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using MiniCover.Core.Utils;
using MiniCover.TestHelpers;
using Xunit;

namespace MiniCover.UnitTests.Utils
{
    public class FileUtilsTests
    {
        [Fact]
        public void GetFileHash_ShouldReturnHash()
        {
            var fileName = @"/test/ACME.Something.dll".ToOSPath();

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(fileName, new MockFileData(new byte[] { 1, 2, 3, 4, 5 }));

            var fileInfo = mockFileSystem.FileInfo.FromFileName(fileName);
            var hash = FileUtils.GetFileHash(fileInfo);

            hash.Should().Be("7CFDD07889B3295D6A550914AB35E068");
        }

        [Fact]
        public void GetPdbFile_ShouldReturnPDBFile()
        {
            var fileName = @"/test/ACME.Something.dll".ToOSPath();

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(fileName, new MockFileData(new byte[5]));

            var fileInfo = mockFileSystem.FileInfo.FromFileName(fileName);
            var pdbInfo = FileUtils.GetPdbFile(fileInfo);

            pdbInfo.FullName.Should().Be(@"/test/ACME.Something.pdb".ToOSPath());
        }

        [Fact]
        public void GetBackupFile_ShouldReturnBackupFile()
        {
            var fileName = @"/test/ACME.Something.dll".ToOSPath();

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(fileName, new MockFileData(new byte[5]));

            var fileInfo = mockFileSystem.FileInfo.FromFileName(fileName);
            var backupInfo = FileUtils.GetBackupFile(fileInfo);

            backupInfo.FullName.Should().Be(@"/test/ACME.Something.uninstrumented.dll".ToOSPath());
        }

        [InlineData(@"/test/ACME.Something.dll", false)]
        [InlineData(@"/test/ACME.Something.uninstrumented.dll", true)]
        [Theory]
        public void IsBackupFile_ShouldReturnBackupFile(string fileName, bool isBackup)
        {
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(fileName.ToOSPath(), new MockFileData(new byte[5]));

            var fileInfo = mockFileSystem.FileInfo.FromFileName(fileName.ToOSPath());
            var result = FileUtils.IsBackupFile(fileInfo);

            result.Should().Be(isBackup);
        }
    }
}
