using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Pow", "Operation", typeof(PowNode),"Raises the first value to the power of the second, per channel. Hardware uses an approximation which does not neccessarily hold up for negative powers, you should reciprocate yourself. Used extensively due to it's properties to values [0,1] range with creating variable falloffs.")]
	public class PowNode : FunctionTwoInput {
		private const string NodeName = "Pow";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "pow"; }
		}
	}
}
