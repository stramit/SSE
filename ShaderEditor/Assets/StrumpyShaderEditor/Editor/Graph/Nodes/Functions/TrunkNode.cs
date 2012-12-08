using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	//[NodeMetaData("Truncate", "Function", typeof(TruncateNode))]
	public class TruncateNode : FunctionOneInput {
		private const string NodeName = "Truncate";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "trunc"; }
		}
	}
}
