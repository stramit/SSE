using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	//[NodeMetaData("IsInf", "Function", typeof(IsInfNode))]
	public class IsInfNode : FunctionOneInput {
		private const string NodeName = "IsInf";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "isinf"; }
		}
	}
}
