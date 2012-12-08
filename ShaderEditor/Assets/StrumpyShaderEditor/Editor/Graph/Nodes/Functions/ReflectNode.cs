using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Reflect", "Function", typeof(ReflectNode),"Reflects the first vector off of a normal of the second. Take care with the number of components used, masking alpha may be pertinent. Used for doing some custom reflection effects where WorldReflection does not suffice, like chromatic abrasion." )]
	public class ReflectNode : FunctionTwoInput {
		private const string NodeName = "Reflect";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "reflect"; }
		}
	}
}
