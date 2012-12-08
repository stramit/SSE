using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Degrees", "Operation", typeof(DegreesNode),"Radians to Degrees conversion. When used with constants, it will be compiled out.")]
	public class DegreesNode : FunctionOneInput {
		private const string NodeName = "Degrees";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "degrees"; }
		}
	}
}
