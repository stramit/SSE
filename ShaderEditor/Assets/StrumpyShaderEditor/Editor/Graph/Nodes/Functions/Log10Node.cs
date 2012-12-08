using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	//[NodeMetaData("Log10", "Function", typeof(Log10Node),"Standard base ten logarithim. Only really should be used for ramping or nonlinear texture storage.")]
	public class Log10Node : FunctionOneInput {
		private const string NodeName = "Log10";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "log10"; }
		}
	}
}
