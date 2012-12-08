using System;

namespace StrumpyShaderEditor
{
	public enum StructTypes
	{
		Float,
		Float2,
		Float3,
		Float4
	}
	
	public static class InputStructExtensions
	{
		public static string ShaderTypeString(this StructTypes typeEnum)
		{
			switch (typeEnum)
			{
				case StructTypes.Float:
					return "float";
				case StructTypes.Float2:
					return "float2";
				case StructTypes.Float3:
					return "float3";
				case StructTypes.Float4:
					return "float4";
				default:
					throw new Exception("Unsupported Type");
			}
		}
		
		public static string GenerateInputUsageString( this IStructInput structInput )
		{
			return "float4( IN." + structInput.GetStructFieldName() + ".x, " +
							"IN." + structInput.GetStructFieldName() + ".y," +
							"IN." + structInput.GetStructFieldName() + ".z," +
							"1.0 )";
		}
	}

	public interface IStructInput
	{
		StructTypes GetStructFieldType();
		string GetStructFieldName();
		string GetStructFieldDefinition();

		string GetStructVertexShaderString();
		
		bool RequiresStructFieldInclusion();
	}
}
