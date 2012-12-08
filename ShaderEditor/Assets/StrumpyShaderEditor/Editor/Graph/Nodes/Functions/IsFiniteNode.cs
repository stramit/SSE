using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	//[NodeMetaData("IsFinite", "Function", typeof(IsFiniteNode))]
	public class IsFiniteNode : FunctionOneInput {
		private const string NodeName = "IsFinite";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "isfinite"; }
		}
	}
}
