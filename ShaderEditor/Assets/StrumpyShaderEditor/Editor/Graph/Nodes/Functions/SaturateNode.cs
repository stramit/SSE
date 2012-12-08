using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Saturate", "Function", typeof(SaturateNode),"Clamps to the [0,1] range, per channel. Clamp is for clamping an arbitrary and/or dynamic range. Saturate combines with almost all other instructions, and for all intents and purposes computationally free.")] 
	public class SaturateNode : FunctionOneInput {
		private const string NodeName = "Saturate";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "saturate"; }
		}
	}
}
