using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("ATan", "Function", typeof(ATanNode),"Trigonometric Arctangent. For calcuating an Angle, use ATan2. This function is more for taking a value up to infinity and compressing to the [-PI/PI] range, making it very useful in conjunction with divisions and powers in the [0,1] range.")]
	public class ATanNode : FunctionOneInput {
		private const string NodeName = "ATan";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "atan"; }
		}
	}
}
