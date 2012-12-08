using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("RSqrt", "Operation", typeof(RSqrt),"Reciprocal Sqaure Root, computed on hardware using the fast inverse sqaureroot approach. Used internally by most hardware for normalize, useful in a few edge cases. Should always be used instead of Pow with -0.5, due to approximation issues with Pow.")]
	public class RSqrt : FunctionOneInput {
		private const string NodeName = "RSqrt";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "rsqrt"; }
		}
	}
}
