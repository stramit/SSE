using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Clamp", "Function", typeof(ClampNode),"Clamps the first value to be within the range of the second and third, again another per channel operation, but the compiler will optimize this. If you mean to Clamp to [0,1], use Saturate instead.")]
	public class ClampNode : FunctionThreeInput {
		private const string NodeName = "Clamp";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "clamp"; }
		}
	}
}
