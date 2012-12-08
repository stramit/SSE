using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("ACos", "Function", typeof(ACosNode),"Arc Cosine of input, performed on all channels. Goes between [PI,0] in the range of [-1,1] with sharp ends, smooth center. Compare to Cos and Smoothstep which have smooth ends, and sharp centers")]
	public class ACosNode : FunctionOneInput {
		private const string NodeName = "Acos";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "acos"; }
		}
	}
}
