using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("ATan2", "Function", typeof(ATan2Node),"Variant of ATan that takes two inputs, Y then X, for the angle formed on a 2D Circle. Limited graphical use.")]
	public class ATan2Node : FunctionTwoInput {
		private const string NodeName = "ATan2";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "atan2"; }
		}
	}
}
