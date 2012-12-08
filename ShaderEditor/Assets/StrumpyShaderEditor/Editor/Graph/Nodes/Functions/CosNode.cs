using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Cos", "Function", typeof(CosNode),"Cosine function, indentical to Sin with a phase offset. Periodic in the range of [-PI,PI] to the range of [-1,1], returns 1, instead of 0, at Sin. Used largely with ramping and texture rotation, this function is performed per channel, solving four values simultaneously. An optimal version is used when the input is only one component (Splat, for instance). See also Sin" )]
	public class CosNode : FunctionOneInput {
		private const string NodeName = "Cos";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "cos"; }
		}
	}
}
