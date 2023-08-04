using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MatchGame.SourceGenerator;

[Generator]
public class ContentReferenceGenerator : ISourceGenerator
{
	private readonly Dictionary<string, SortedSet<string>> _fileMappings = new();
	private readonly Dictionary<string, SortedSet<string>> _directoryMappings = new ();

	private bool AddFileMapping(string directoryPath, string filePath)
	{
		if (!_fileMappings.TryGetValue(directoryPath, out var files))
		{
			_fileMappings.Add(directoryPath, files = new SortedSet<string>());
		}

		return files.Add(filePath);
	}

	private bool AddDirectoryMapping(string directoryPath, string parentPath)
	{
		if (!_directoryMappings.TryGetValue(parentPath, out var directories))
		{
			_directoryMappings.Add(parentPath, directories = new SortedSet<string>());
		}

		return directories.Add(directoryPath);
	}

	private void AddDirectoryMappingRecursive(string directoryPath, string rootPath)
	{
		var parentPath = Path.GetDirectoryName(directoryPath) ?? throw new InvalidOperationException();

		while (AddDirectoryMapping(directoryPath, parentPath))
		{
			parentPath = Path.GetDirectoryName(directoryPath) ?? throw new InvalidOperationException();

			if (parentPath.Equals(rootPath, StringComparison.InvariantCultureIgnoreCase))
			{
				break;
			}
		}
	}

	private void GenerateDirectoryReferences(
		TextWriter writer,
		string directoryPath,
		string rootPath,
		int depth)
	{
		var bHasFileMappings = _fileMappings.TryGetValue(directoryPath, out var fileList);
		var bHasDirectoryMappings = _directoryMappings.TryGetValue(directoryPath, out var directoryList);

		if (!bHasFileMappings && !bHasDirectoryMappings)
		{
			return;
		}

		var directoryName = Path.GetFileName(directoryPath);

		writer.WriteLine($"public static class {directoryName}".Indent(depth));
		writer.WriteLine("{".Indent(depth));

		if (bHasFileMappings)
		{
			foreach (var file in fileList!)
			{
				var fileName = Path.GetFileNameWithoutExtension(file);
				var relativePathToFile = Path.GetRelativePath(rootPath, file);
				writer.WriteLine($"public const string {fileName} = @\".\\{relativePathToFile}\";".Indent(depth + 1));
			}
		}

		if (bHasFileMappings && bHasDirectoryMappings)
		{
			writer.WriteLine();
		}

		if (bHasDirectoryMappings)
		{
			foreach (var directory in directoryList!)
			{
				GenerateDirectoryReferences(writer, directory, rootPath, depth + 1);
			}
		}

		writer.WriteLine("}".Indent(depth));
	}

	public void Initialize(GeneratorInitializationContext context)
	{
		Debug.WriteLine(nameof(Initialize));
	}

	public void Execute(GeneratorExecutionContext context)
	{
		Debug.WriteLine(nameof(Execute));

		var rootPath = context.Compilation.SyntaxTrees
			.Where(x => x.HasCompilationUnitRoot)
			.Select(x => Path.GetDirectoryName(x.FilePath))
			.First() ?? throw new InvalidOperationException();

		foreach (var file in context.AdditionalFiles)
		{
			var directoryPath = Path.GetDirectoryName(file.Path)
				?? throw new InvalidOperationException();

			AddFileMapping(directoryPath, file.Path);
			AddDirectoryMappingRecursive(directoryPath, rootPath);
		}

		var writer = new StringWriter();

		writer.WriteLine("using System;");
		writer.WriteLine();
		writer.WriteLine($"namespace {Path.GetFileName(rootPath)};");
		writer.WriteLine();
		writer.WriteLine("public static class ContentReference");
		writer.WriteLine("{");

		if (_directoryMappings.TryGetValue(rootPath, out var directoryList))
		{
			foreach (var directoryPath in directoryList)
			{
				GenerateDirectoryReferences(writer, directoryPath, rootPath, 1);
			}
		}
		else
		{
			writer.WriteLine();
		}

		writer.WriteLine("}");
		writer.WriteLine();
		
		Debug.WriteLine(writer.ToString());

		context.AddSource("ContentReference.Generated.cs", SourceText.From(writer.ToString(), Encoding.Default));
	}
}