using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Normalize", "Function", typeof(NormalizeNode),"Sets the length of the input to one. Most of the time you only want this to occur for the first three channels, so it is likely pertinent to mask out alpha. A normalized vector multiplied by it's length yields the original, and this function is primarily used for combining directions, though can be used to take advantage of certain compression schemes.")]
	public class NormalizeNode : FunctionOneInput {
		private const string NodeName = "Normalize";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "normalize"; }
		}
	}
}
