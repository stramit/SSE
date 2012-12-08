using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Add", "Operation", typeof(AddNode),"Addition, applied per channel. Use multiple adds for combining multiple values. (Adds are about as expensive as any other operation)")]
	public class AddNode : Operation {
		private const string NodeName = "Add";

		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string Operand {
			get {
				return " + ";
			}
		}
	}
}