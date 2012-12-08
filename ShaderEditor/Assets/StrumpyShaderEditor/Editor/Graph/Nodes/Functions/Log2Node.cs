using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	// Texel - Removed, as you basically never use this for anything honestly ever.
	//[NodeMetaData("Log2", "Function", typeof(Log2Node),"Logarithim on the binary scale. The only times you should use the log are for nonlinear texture storage and ramping")]
	public class Log2Node : FunctionOneInput {
		private const string NodeName = "Log2";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "log2"; }
		}
	}
}
