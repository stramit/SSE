using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Abs", "Function", typeof(AbsNode),"Absolute Value of an input (|x|,|y|,|z|,|w|)")]
	public class AbsNode : FunctionOneInput {
		private const string NodeName = "Abs";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "abs"; }
		}
		
	}
}