using System;
using System.Collections.Generic;

namespace MiniCover.Reports.Helpers
{
    public static class ConsoleBox
    {
        private static readonly Dictionary<BoxPart, char> _boxCharacters = new Dictionary<BoxPart, char>
        {
            [BoxPart.All] = '┼',
            [BoxPart.Bottom | BoxPart.Right] = '┌',
            [BoxPart.Bottom | BoxPart.Left] = '┐',
            [BoxPart.Top | BoxPart.Right] = '└',
            [BoxPart.Top | BoxPart.Left] = '┘',
            [BoxPart.Left | BoxPart.Right] = '─',
            [BoxPart.Top | BoxPart.Bottom] = '│',
            [BoxPart.Top | BoxPart.Left | BoxPart.Right] = '┴',
            [BoxPart.Bottom | BoxPart.Left | BoxPart.Right] = '┬',
            [BoxPart.Left | BoxPart.Top | BoxPart.Bottom] = '┤',
            [BoxPart.Right | BoxPart.Top | BoxPart.Bottom] = '├'
        };

        public static char ToChar(this BoxPart parts)
        {
            return _boxCharacters[parts];
        }
    }

    [Flags]
    public enum BoxPart
    {
        None = 0,
        Top = 1,
        Right = 2,
        Bottom = 4,
        Left = 8,
        All = Top | Right | Bottom | Left,
        Vertical = Top | Bottom,
        Horizontal = Left | Right
    }
}
