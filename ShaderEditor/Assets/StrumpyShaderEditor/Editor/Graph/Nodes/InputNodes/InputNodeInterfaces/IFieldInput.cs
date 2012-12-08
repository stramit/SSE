using System;

namespace StrumpyShaderEditor
{
	public enum InputType
	{
		Float,
		Color,
		Range,
		Texture2D,
		TextureCube,
		Vector,
		Matrix
	}

	public static class InputTypeExtensions
	{
		public static string ShaderTypeString(this InputType typeEnum)
		{
			switch (typeEnum)
			{
				case InputType.Float:
					return "float";
				case InputType.Color:
					return "float4";
				case InputType.Range:
					return "float";
				case InputType.Texture2D:
					return "sampler2D";
				case InputType.TextureCube:
					return "samplerCUBE";
				case InputType.Vector:
					return "float4";
				case InputType.Matrix:
					return "float4x4";
				default:
					throw new Exception("Unsupported Type");
			}
		}
	}

	public interface IFieldInput
	{
		InputType GetFieldType();
		string GetFieldName();
		string GetFieldDefinition();
	}
}
