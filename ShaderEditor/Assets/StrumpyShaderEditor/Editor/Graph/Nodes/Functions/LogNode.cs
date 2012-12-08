using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Log", "Function", typeof(LogNode),"Logarithim of the input, you only really should use this for ramping or nonlinear texture storage.")]
	public class LogNode : FunctionOneInput {
		private const string NodeName = "Log";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "log"; }
		}
	}
}
