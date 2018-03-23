using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MiniCover.ApprovalTests
{
    internal static class BinaryConverger
    {
        public static byte[] ReplaceTextInFile(string fileName, params string[] textToRemove)
        {
            byte[] fileBytes = File.ReadAllBytes(fileName);

            foreach (var toRemove in textToRemove)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    fileBytes = fileBytes.ReplaceTextInByteArray("`" + toRemove + Path.DirectorySeparatorChar, "o")
                      .ReplaceTextInByteArray($"{Path.DirectorySeparatorChar}", "/");
                }
                else
                {
                    fileBytes = fileBytes.ReplaceTextInByteArray(toRemove + Path.DirectorySeparatorChar, string.Empty );
                }
            }

            return fileBytes;
        }

        private static byte[] ReplaceTextInByteArray(this byte[] fileBytes, string oldText, string newText)
        {
            byte[] oldBytes = Encoding.UTF8.GetBytes(oldText);
            byte[] newBytes = Encoding.UTF8.GetBytes(newText);
            return fileBytes.ReplaceBytes(oldBytes, newBytes);
        }

        private static byte[] ReplaceBytes(this byte[] fileBytes, byte[] oldBytes, byte[] newBytes)
        {

            byte[] newFileBytes = fileBytes;
            while (true)
            {
                int index = IndexOfBytes(newFileBytes, oldBytes);
                if (index < 0) break;
                var tempileBytes =
                new byte[newFileBytes.Length + newBytes.Length - oldBytes.Length];

                Buffer.BlockCopy(newFileBytes, 0, tempileBytes, 0, index);
                Buffer.BlockCopy(newBytes, 0, tempileBytes, index, newBytes.Length);
                Buffer.BlockCopy(newFileBytes, index + oldBytes.Length,
                    tempileBytes, index + newBytes.Length,
                    newFileBytes.Length - index - oldBytes.Length);
                newFileBytes = tempileBytes;
            }
            return newFileBytes;
        }

        private static int IndexOfBytes(byte[] searchBuffer, byte[] bytesToFind)
        {
            for (int i = 0; i < searchBuffer.Length - bytesToFind.Length; i++)
            {
                bool success = true;

                for (int j = 0; j < bytesToFind.Length; j++)
                {
                    if (searchBuffer[i + j] != bytesToFind[j])
                    {
                        success = false;
                        break;
                    }
                }

                if (success)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
