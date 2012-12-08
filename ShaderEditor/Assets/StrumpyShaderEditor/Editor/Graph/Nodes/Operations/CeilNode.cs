using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Ceil", "Operation", typeof(CeilNode),"Round each component up to the nearest integer. Not exact due to floating point limitations. Compare to Floor, contrast to Frac")]
	public class CeilNode : FunctionOneInput {
		private const string NodeName = "Ceil";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "ceil"; }
		}
	}
}
