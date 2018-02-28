namespace MiniCover.Reports
{
    internal class Hit
    {
        public int InstrumentationId { get; }
        public string AssemblyName { get; }
        public string ClassName { get; }
        public string MethodName { get; }
        public string AssemblyLocation { get; }

        public Hit(int instrumentationId, string assemblyName, string className, string methodName, string assemblyLocation)
        {
            InstrumentationId = instrumentationId;
            AssemblyName = assemblyName;
            ClassName = className;
            MethodName = methodName;
            AssemblyLocation = assemblyLocation;
        }

        internal static Hit Parse(string line)
        {
            var sections = line.Split(':');
            return new Hit(int.Parse(sections[0]), sections[1], sections[2], sections[3], sections[4]);
        }
    }
}