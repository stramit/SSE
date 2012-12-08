using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	//[NodeMetaData("IsNan", "Function", typeof(IsNanNode))]
	public class IsNanNode : FunctionOneInput {
		private const string NodeName = "IsNan";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "isnan"; }
		}
	}
}
