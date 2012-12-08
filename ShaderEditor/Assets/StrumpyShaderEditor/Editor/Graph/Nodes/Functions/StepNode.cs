using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Step", "Function", typeof(StepNode),"Arg1 > Arg2 ? 1 : 0 - That is, if the first value is larger, it returns one, if less, it returns zero. Used for conditional logic and thresholding.")]
	public class StepNode : FunctionTwoInput {
		private const string NodeName = "Step";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "step"; }
		}
	}
}
