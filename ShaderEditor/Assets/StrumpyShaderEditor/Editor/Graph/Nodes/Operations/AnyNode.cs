using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Any", "Operation", typeof(AnyNode),"Returns (1,1,1,1) if any of the inputs are not equal to zero. Used for conditional logic and some mild optimizations. Contrast to All")]
	public class AnyNode : FunctionOneInput {
		private const string NodeName = "Any";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "any"; }
		}
	}
}
