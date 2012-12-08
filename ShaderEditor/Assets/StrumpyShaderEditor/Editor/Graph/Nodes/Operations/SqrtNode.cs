using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Sqrt", "Operation", typeof(SqrtNode),"Square Root. Internally more accurate then using Pow with 0.5, aswell as being faster. Has moderate use in ramping, particurally in the [0,1] range it has a distinct ramp with a vertical tangent on approach to zero.")]
	public class SqrtNode : FunctionOneInput {
		private const string NodeName = "Sqrt";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "sqrt"; }
		}
	}
}
