using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	//[NodeMetaData("Cosh", "Function", typeof(CoshNode))]
	public class CoshNode : FunctionOneInput {
		private const string NodeName = "Cosh";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "cosh"; }
		}
	}
}
