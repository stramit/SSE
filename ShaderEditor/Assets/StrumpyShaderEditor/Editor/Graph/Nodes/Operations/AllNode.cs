using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("All", "Operation", typeof(AllNode),"Returns (1,1,1,1) if and only if none of the input components is zero. Used for conditionals, contrast to Any.")]
	public class AllNode : FunctionOneInput {
		private const string NodeName = "All";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "all"; }
		}
	}
}
