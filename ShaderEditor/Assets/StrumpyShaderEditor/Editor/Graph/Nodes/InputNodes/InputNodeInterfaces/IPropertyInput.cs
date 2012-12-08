using System;
namespace StrumpyShaderEditor
{
	public static class PropertyExtensions
	{
		public static string PropertyTypeString(this InputType typeEnum)
		{
			switch (typeEnum)
			{
				case InputType.Float:
					return "Float";
				case InputType.Color:
					return "Color";
				case InputType.Range:
					return "Range";
				case InputType.Texture2D:
					return "2D";
				case InputType.TextureCube:
					return "Cube";
				case InputType.Vector:
					return "Vector";
				case InputType.Matrix:
					return "Matrix";
				default:
					throw new Exception("Unsupported Type");
			}
		}
	}
}
