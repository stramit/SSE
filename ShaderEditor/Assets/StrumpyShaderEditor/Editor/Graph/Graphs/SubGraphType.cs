using System;
namespace StrumpyShaderEditor
{
	public enum SubGraphType{
		Pixel,
		Vertex,
		SimpleLighting
	}
		
	public static class SubGraphTypeExtensions
	{
		public static string DisplayName(this SubGraphType graphType)
		{
			switch (graphType)
			{
				case SubGraphType.Pixel:
					return "Pixel Graph";
				case SubGraphType.Vertex:
					return "Vertex Graph";
				case SubGraphType.SimpleLighting:
					return "Lighting Graph";
				default:
					throw new Exception("Unsupported Type");
			}
		}
	}
}
