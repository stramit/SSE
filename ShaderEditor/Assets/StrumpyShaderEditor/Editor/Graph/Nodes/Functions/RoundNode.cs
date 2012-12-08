using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	//[NodeMetaData("Round", "Function", typeof(RoundNode))]
	public class RoundNode : FunctionOneInput {
		private const string NodeName = "Round";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "round"; }
		}
	}
}
