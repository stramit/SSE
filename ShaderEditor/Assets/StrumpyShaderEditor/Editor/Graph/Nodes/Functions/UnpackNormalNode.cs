using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("UnpackNormal", "Function", typeof(UnpackNormalNode),"Unpacks a compressed normal map. As of Unity3.0, Normal maps can use DXTnm format. These files have much better accuracy/space then storing as full textures, but require this extra node to process. You can freely combine and use them before unpacking via Lerp/Smoothstep, but should Unpack before output or Add/Subtract/Multiply. Builtin to Tex2DNormal")]
	public class UnpackNormalNode : FunctionOneInput {
		private const string NodeName = "UnpackNormal";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "UnpackNormal"; }
		}
		
		public override string GetUsage()
		{
			var arg1Input = _arg1.ChannelInput( this );
			
			string result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=";
			result += "float4(" + FunctionName + "(" + arg1Input.QueryResult + ").xyz, 1.0);\n";
			return result;
		}
	}
}
