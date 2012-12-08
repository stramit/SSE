using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Min", "Operation", typeof(MinNode),"Returns the lower of the two components for each input. Useful mostly for combining textures, similiar to a Darken Only blend (where Max is Brighten Only)")]
	public class MinNode : FunctionTwoInput {
		private const string NodeName = "Min";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "min"; }
		}
	}
}
