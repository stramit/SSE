using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	//[NodeMetaData("Sinh", "Function", typeof(SinhNode))]
	public class SinhNode : FunctionOneInput {
		private const string NodeName = "Sinh";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "sinh"; }
		}
	}
}
