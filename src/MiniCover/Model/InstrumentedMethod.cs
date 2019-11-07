using System;
using System.Collections.Generic;

namespace MiniCover.Model
{
    public class InstrumentedMethod : IEquatable<InstrumentedMethod>
    {
        public string Class { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as InstrumentedMethod);
        }

        public bool Equals(InstrumentedMethod other)
        {
            return other != null &&
                   Class == other.Class &&
                   Name == other.Name &&
                   FullName == other.FullName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Class, Name, FullName);
        }

        public static bool operator ==(InstrumentedMethod left, InstrumentedMethod right)
        {
            return EqualityComparer<InstrumentedMethod>.Default.Equals(left, right);
        }

        public static bool operator !=(InstrumentedMethod left, InstrumentedMethod right)
        {
            return !(left == right);
        }
    }
}
