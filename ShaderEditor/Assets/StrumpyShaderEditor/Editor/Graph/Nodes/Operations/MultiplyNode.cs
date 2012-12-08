using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Multiply", "Operation", typeof(MultiplyNode),"Multiplies two values together. Unlike most other operators, when followed by Add, they will combine internally. See also divide.")]
	public class MultiplyNode : Operation {
		private const string NodeName = "Multiply";

		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string Operand {
			get {
				return " * ";
			}
		}
	}
}