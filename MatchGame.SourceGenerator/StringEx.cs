namespace MatchGame.SourceGenerator;

public static class StringEx
{
	public static string Indent(this string s, int depth)
	{
		return s.Insert(0, new string('\t', depth));
	}
}