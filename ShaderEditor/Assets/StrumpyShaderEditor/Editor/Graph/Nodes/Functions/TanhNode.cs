using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	//[NodeMetaData("Tanh", "Function", typeof(TanhNode))]
	public class TanhNode : FunctionOneInput {
		private const string NodeName = "Tanh";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "tanh"; }
		}
	}
}
