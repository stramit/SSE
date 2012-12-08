using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	//[NodeMetaData("Exp2", "Operation", typeof(Exp2Node))]
	public class Exp2Node : FunctionOneInput {
		private const string NodeName = "Exp2";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "exp2"; }
		}
	}
}
