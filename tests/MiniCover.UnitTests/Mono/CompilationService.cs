using System;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;


using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using CS = Microsoft.CodeAnalysis.CSharp;
using VB = Microsoft.CodeAnalysis.VisualBasic;

namespace Mono.Cecil.Tests
{

	struct CompilationResult
	{
		internal DateTime source_write_time;
		internal string result_file;

		public CompilationResult(DateTime write_time, string result_file)
		{
			this.source_write_time = write_time;
			this.result_file = result_file;
		}
	}

	public static class Platform
	{

		public static bool OnMono
		{
			get { return TryGetType("Mono.Runtime") != null; }
		}

		public static bool OnCoreClr
		{
			get { return TryGetType("System.Runtime.Loader.AssemblyLoadContext, System.Runtime.Loader, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a") != null; }
		}

		static Type TryGetType(string assemblyQualifiedName)
		{
			try
			{
				// Note that throwOnError=false only suppresses some exceptions, not all.
				return Type.GetType(assemblyQualifiedName, throwOnError: false);
			}
			catch
			{
				return null;
			}
		}
	}

	abstract class CompilationService
	{

		Dictionary<string, CompilationResult> files = new Dictionary<string, CompilationResult>();

		bool TryGetResult(string name, out string file_result)
		{
			file_result = null;
			CompilationResult result;
			if (!files.TryGetValue(name, out result))
				return false;

			if (result.source_write_time != File.GetLastWriteTime(name))
				return false;

			file_result = result.result_file;
			return true;
		}

		public string Compile(string name)
		{
			string result_file;
			if (TryGetResult(name, out result_file))
				return result_file;

			result_file = CompileFile(name);
			RegisterFile(name, result_file);
			return result_file;
		}

		void RegisterFile(string name, string result_file)
		{
			files[name] = new CompilationResult(File.GetLastWriteTime(name), result_file);
		}

		protected abstract string CompileFile(string name);

		public static string CompileResource(string name)
		{
			var extension = Path.GetExtension(name);
			if (extension == ".il")
				return IlasmCompilationService.Instance.Compile(name);

			if (extension == ".cs" || extension == ".vb")
				return RoslynCompilationService.Instance.Compile (name);

			throw new NotSupportedException(extension);
		}

		protected static string GetCompiledFilePath(string file_name)
		{
			var tmp_cecil = Path.Combine(Path.GetTempPath(), "cecil");
			if (!Directory.Exists(tmp_cecil))
				Directory.CreateDirectory(tmp_cecil);

			return Path.Combine(tmp_cecil, Path.GetFileName(file_name) + ".dll");
		}

		public static void Verify(string name)
		{

		}
	}

	class IlasmCompilationService : CompilationService
	{

		public static readonly IlasmCompilationService Instance = new IlasmCompilationService();

		protected override string CompileFile(string name)
		{
			string file = GetCompiledFilePath(name);

			var output = ShellService.ILAsm(name, file);

			AssertAssemblerResult(output);

			return file;
		}

		static void AssertAssemblerResult(ShellService.ProcessOutput output)
		{
			if (output.ExitCode != 0)
				Assert.True(false, output.ToString());
		}
	}

	class RoslynCompilationService : CompilationService {

		public static readonly RoslynCompilationService Instance = new RoslynCompilationService ();

		protected override string CompileFile (string name)
		{
			var compilation = GetCompilation (name);
			var outputName = GetCompiledFilePath (name);

			var result = compilation.Emit (outputName);
			Assert.True (result.Success, GetErrorMessage (result));

			return outputName;
		}

		static Compilation GetCompilation (string name)
		{
			var assemblyName = Path.GetFileNameWithoutExtension (name);
			var source = File.ReadAllText (name);

		    Dictionary<string, string> tpa = ((Lazy<Dictionary<string, string>>) typeof(BaseAssemblyResolver)
		        .GetField("TrustedPlatformAssemblies", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null)).Value;
			
			var references = new [] 
			{
				MetadataReference.CreateFromFile (tpa ["netstandard"]),
				MetadataReference.CreateFromFile (tpa ["mscorlib"]),
				MetadataReference.CreateFromFile (tpa ["System.Private.CoreLib"]),
				MetadataReference.CreateFromFile (tpa ["System.Runtime"]),
				MetadataReference.CreateFromFile (tpa ["System.Console"]),
				MetadataReference.CreateFromFile (tpa ["System.Security.AccessControl"]),
			};

			var extension = Path.GetExtension (name);
			switch (extension) {
			case ".cs":
				return CS.CSharpCompilation.Create (
					assemblyName, 
					new [] { CS.SyntaxFactory.ParseSyntaxTree (source) },
					references, 
					new CS.CSharpCompilationOptions (OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

			case ".vb":
				return VB.VisualBasicCompilation.Create (
					assemblyName,
					new [] { VB.SyntaxFactory.ParseSyntaxTree (source) },
					references,
					new VB.VisualBasicCompilationOptions (OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

			default:
				throw new NotSupportedException ();
			}
		}

		static string GetErrorMessage (EmitResult result)
		{
			if (result.Success)
				return string.Empty;

			var builder = new StringBuilder ();
			foreach (var diagnostic in result.Diagnostics)
				builder.AppendLine (diagnostic.ToString ());

			return builder.ToString ();
		}
	}


	class ShellService
	{

		public class ProcessOutput
		{

			public int ExitCode;
			public string StdOut;
			public string StdErr;

			public ProcessOutput(int exitCode, string stdout, string stderr)
			{
				ExitCode = exitCode;
				StdOut = stdout;
				StdErr = stderr;
			}

			public override string ToString()
			{
				return StdOut + StdErr;
			}
		}

		static ProcessOutput RunProcess(string target, params string[] arguments)
		{
			var stdout = new StringWriter();
			var stderr = new StringWriter();

			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = target,
					Arguments = string.Join(" ", arguments),
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
				},
			};

			process.Start();

			process.OutputDataReceived += (_, args) => stdout.Write(args.Data);
			process.ErrorDataReceived += (_, args) => stderr.Write(args.Data);

			process.BeginOutputReadLine();
			process.BeginErrorReadLine();

			process.WaitForExit();

			return new ProcessOutput(process.ExitCode, stdout.ToString(), stderr.ToString());
		}

		public static ProcessOutput ILAsm(string source, string output)
		{
			var ilasm = "ilasm";
			if (!Platform.OnMono)
				ilasm = NetFrameworkTool("ilasm");

			return RunProcess(ilasm, "/nologo", "/dll", "/out:" + Quote(output), Quote(source));
		}

		static string Quote(string file)
		{
			return "\"" + file + "\"";
		}

		public static ProcessOutput PEVerify(string source)
		{
			return RunProcess(WinSdkTool("peverify"), "/nologo", Quote(source));
		}

		public static ProcessOutput PEDump(string source)
		{
			return RunProcess("pedump", "--verify code,metadata", Quote(source));
		}

		static string NetFrameworkTool(string tool)
		{
			return Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Windows), "Microsoft.NET", "Framework", "v4.0.30319", tool + ".exe");
		}

		static string WinSdkTool(string tool)
		{
			var sdks = new[] {
				@"Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7 Tools",
				@"Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.2 Tools",
				@"Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools",
				@"Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools",
				@"Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools",
				@"Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools",
				@"Microsoft SDKs\Windows\v7.0A\Bin",
			};

			foreach (var sdk in sdks)
			{
				var pgf = IntPtr.Size == 8
					? Environment.GetEnvironmentVariable("ProgramFiles(x86)")
					: Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

				var exe = Path.Combine(
					Path.Combine(pgf, sdk),
					tool + ".exe");

				if (File.Exists(exe))
					return exe;
			}

			return tool;
		}
	}
}