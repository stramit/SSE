using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Sign", "Operation", typeof(SignNode),"For each component, returns -1 if negative, 0 if 0, and 1 if positive. Multiplying a number by it's sign return yields it's absolute value. Useful for detecting backfaces aswell as in conditional logic. Similar in use to Any, All, and Step.")]
	public class SignNode : FunctionOneInput {
		private const string NodeName = "Sign";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "sign"; }
		}
	}
}
