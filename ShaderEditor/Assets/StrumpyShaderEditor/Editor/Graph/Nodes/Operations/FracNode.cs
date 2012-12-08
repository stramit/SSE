using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Frac", "Operation", typeof(FracNode),"Returns the fractional part of each component. Frac + Floor are the two components of the starting value. Used to force repeition similiar to how described for Floor, though be aware that it may cause anti-aliasing issues that are hard to correct.")]
	public class FracNode : FunctionOneInput {
		private const string NodeName = "Frac";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "frac"; }
		}
	}
}
