using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Length", "Function", typeof(LengthNode),"Takes the magnitude of the input vector, over it's four channels (so mask out the fourth component for three)- Faster then doing Distance, but negligibly.")]
	public class LengthNode : FunctionOneInput {
		private const string NodeName = "Length";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "length"; }
		}
	}
}
