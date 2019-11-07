using System;
using System.Collections.Generic;

namespace MiniCover.Extensions
{
    public static class CodeExtensions
    {
        public static string ExtractCode(this string[] fileLines, int startLine, int endLine, int startColumn, int endColumn)
        {
            if (startLine == endLine)
            {
                var lineIndex = startLine - 1;
                if (lineIndex < 0 || lineIndex >= fileLines.Length)
                    return null;

                var startIndex = startColumn - 1;
                if (startIndex < 0 || startIndex >= fileLines[lineIndex].Length)
                    return null;

                var length = endColumn - startColumn;
                if (length <= 0 || startIndex + length > fileLines[lineIndex].Length)
                    return null;

                return fileLines[lineIndex].Substring(startIndex, length);
            }
            else
            {
                var result = new List<string>();

                var firstLineIndex = startLine - 1;
                if (firstLineIndex < 0 || firstLineIndex >= fileLines.Length)
                    return null;

                var startColumnIndex = startColumn - 1;
                if (startColumnIndex < 0 || startColumnIndex >= fileLines[firstLineIndex].Length)
                    return null;

                var lastLineIndex = endLine - 1;
                if (lastLineIndex < firstLineIndex || lastLineIndex >= fileLines.Length)
                    return null;

                var endLineLength = endColumn - 1;
                if (endLineLength <= 0 || endLineLength > fileLines[lastLineIndex].Length)
                    return null;

                result.Add(fileLines[firstLineIndex].Substring(startColumnIndex));

                for (var l = firstLineIndex + 1; l < lastLineIndex; l++)
                    result.Add(fileLines[l]);

                result.Add(fileLines[lastLineIndex].Substring(0, endLineLength));

                return string.Join(Environment.NewLine, result);
            }
        }
    }
}
