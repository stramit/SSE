using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("ASin", "Function", typeof(ASinNode),"Arc Sine function, remaps [-1,1] to [-PI/2,PI/2], similar uses for ACos in ramping, only in this case it's signed. See also ACos")]
	public class ASinNode : FunctionOneInput {
		private const string NodeName = "ASin";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "asin"; }
		}
		
	}
}
