using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	//[NodeMetaData("Exp", "Operation", typeof(ExpNode))]
	public class ExpNode : FunctionOneInput {
		private const string NodeName = "Exp";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "exp"; }
		}
	}
}
