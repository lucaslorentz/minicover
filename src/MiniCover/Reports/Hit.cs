
namespace MiniCover.Reports
{
    internal abstract class Hit
    {
        public int InstrumentationId { get; }
        

        public Hit(int instrumentationId)
        {
            InstrumentationId = instrumentationId;
        }

        internal static Hit Parse(string line)
        {
            var sections = line.Split(':');
            if(sections.Length == 2)
                return new HitOnly(int.Parse(sections[0]), int.Parse(sections[1]));
            return new HitWithTestsInformation(int.Parse(sections[0]), sections[1], sections[2], sections[3], sections[4]);
        }


        internal class HitOnly : Hit
        {
            public int Counter { get; }

            public HitOnly(int instrumentationId, int counter) : base(instrumentationId)
            {
                Counter = counter;
            }
        }

        internal class HitWithTestsInformation : Hit
        {

            public string AssemblyName { get; }
            public string ClassName { get; }
            public string MethodName { get; }
            public string AssemblyLocation { get; }

            public HitWithTestsInformation(int instrumentationId, string assemblyName, string className, string methodName, string assemblyLocation) : base(instrumentationId)
            {
                AssemblyName = assemblyName;
                ClassName = className;
                MethodName = methodName;
                AssemblyLocation = assemblyLocation;
            }
        }
    }

    
}