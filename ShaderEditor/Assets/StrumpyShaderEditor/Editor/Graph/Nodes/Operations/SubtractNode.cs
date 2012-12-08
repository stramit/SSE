using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Subtract", "Operation", typeof(SubtractNode),"Subtaction, per component. For compiler combination purposes, it is generally better to add negative numbers then subtract, especially with multiplication, but it generally is not worth the operation to flip it if it isn't already.")]
	public class SubtractNode : Operation {
		private const string NodeName = "Subtract";

	    public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string Operand {
			get {
				return " - ";
			}
		}
	}
}