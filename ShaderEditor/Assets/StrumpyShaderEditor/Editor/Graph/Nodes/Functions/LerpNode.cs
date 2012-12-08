using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Lerp", "Function", typeof(LerpNode),"Linearlly interpolate between two values using a third. That is, if the third value is 0, the first input is returned, if it's 1, the second. This occurs per channel, and intermediary results are linear. If you want smooth interpolation, use Smoothstep.")]
	public class LerpNode : FunctionThreeInput {
		private const string NodeName = "Lerp";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "lerp"; }
		}
	}
}
