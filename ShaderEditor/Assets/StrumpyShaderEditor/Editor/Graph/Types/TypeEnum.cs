namespace StrumpyShaderEditor
{
	public enum TypeEnum {
		Float,
		Float2,
		Float3,
		Float4,
		Sampler2D,
		SamplerCube,
		Matrix
	}
	
	public static class TypeExtensions{
		public static string ShaderString(this TypeEnum typeEnum)
		{
			switch (typeEnum)
			{
				case TypeEnum.Float:
					return "float";
				case TypeEnum.Float2:
					return "float2";
				case TypeEnum.Float3:
					return "float3";
				case TypeEnum.Float4:
					return "float4";
				case TypeEnum.Sampler2D:
					return "Sampler2d";
				case TypeEnum.Matrix:
					return "float4x4";
				case TypeEnum.SamplerCube:
					return "SamplerCube";
				default:
					return "Invalid Type";
			}
		}
	}
}