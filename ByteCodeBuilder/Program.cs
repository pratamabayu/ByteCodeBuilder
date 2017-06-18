using System;
using System.IO;

namespace ByteCodeBuilder
{
	class Program
	{
		public static void Main(string[] args)
		{
			if (args.Length == 0 || args.Length == 1 && args[0].Equals("-help"))
			{
				ShowHelper();
				return;
			}

			bool isInternal = false;
			bool isPartial = false;

			string namespaceName = null;
			string className = null;
			string fieldName = null;

			string inputPath = null;
			string outputPath = null;

			for (int i = 0; i < args.Length; i++)
			{
				switch (args[i])
				{
					case "-i":
						{
							if (i + 1 < args.Length)
								inputPath = args[i + 1];
						}
						break;
					case "-o":
						{
							if (i + 1 < args.Length)
								outputPath = args[i + 1];
						}
						break;
					case "-n":
						{
							if (i + 1 < args.Length)
								namespaceName = args[i + 1];
						}
						break;
					case "-c":
						{
							if (i + 1 < args.Length)
								className = args[i + 1];
						}
						break;
					case "-f":
						{
							if (i + 1 < args.Length)
								fieldName = args[i + 1];
						}
						break;
					case "--internal":
						{
							isInternal = true;
						}
						break;
					case "--partial":
						{
							isPartial = true;
						}
						break;
				}
			}

			if (string.IsNullOrEmpty(namespaceName) || 
			    string.IsNullOrEmpty(className) || 
			    string.IsNullOrEmpty(fieldName))
			{
				ShowHelper();
				return;
			}

			if (!File.Exists(inputPath))
			{
				ShowHelper();
				return;
			}

			Console.WriteLine("Building ...");

			using(StreamWriter writer = new StreamWriter(outputPath, false))
			{
				writer.WriteLine("using System;");
				writer.WriteLine();
				writer.WriteLine("// Generated using ByteCodeBuilder by Pratama Bayu Widagdo");
				writer.WriteLine("// At {0}", DateTime.UtcNow);
				writer.WriteLine();
				writer.WriteLine("namespace {0}", namespaceName);
				writer.WriteLine("{");
				if (isInternal)
				{
					if (isPartial)
						writer.WriteLine("\tinternal static partial class {0}", className);
					else
						writer.WriteLine("\tinternal static class {0}", className);
				}
				else
				{
					if (isPartial)
						writer.WriteLine("\tpublic static partial class {0}", className);
					else
						writer.WriteLine("\tpublic static class {0}", className);
				}
				writer.WriteLine("\t{");

				if (isInternal)
					writer.WriteLine("\t\tinternal static byte[] {0} = new byte[]", fieldName);
				else
					writer.WriteLine("\t\tpublic static byte[] {0} = new byte[]", fieldName);
				
				writer.WriteLine("\t\t{");

				writer.Write("\t\t\t");

				byte[] bytes = File.ReadAllBytes(inputPath);
				for (int j = 0; j < bytes.Length; j++)
				{
					writer.Write(bytes[j].ToString("D3"));

					bool isNextLineNeeded = (j + 1) % 15 == 0;
					if (j < bytes.Length - 1)
					{
						writer.Write("," + (isNextLineNeeded ? "" : " "));
					}

					if (isNextLineNeeded)
					{
						writer.WriteLine();
						writer.Write("\t\t\t");
					}
				}

				writer.WriteLine();
				writer.WriteLine("\t\t}");
				writer.WriteLine("\t}");
				writer.WriteLine("}");
			}

			Console.WriteLine("Build finished");
		}

		static void ShowHelper()
		{
			Console.WriteLine("Format:");
			Console.WriteLine("        -i {input} -o {output} -n {namespace} -c {class} -f {field} --internal --partial");
			Console.WriteLine("Where :");
			Console.WriteLine("        {input}    : input file");
			Console.WriteLine("        {output}   : output file (.cs)");
			Console.WriteLine("        {namespace}: namespace name in C# file");
			Console.WriteLine("        {class}    : class name in C# file");
			Console.WriteLine("        {field}    : field name in C# file");
			Console.WriteLine("        --internal : use internal class and field in C# file");
			Console.WriteLine("        --partial  : use partial class in C# file");
		}
	}
}
