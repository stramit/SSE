using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Divide", "Operation", typeof(DivideNode),"Division. Should be noted that most operations take the same execution time, and dividing once and multiplying is slower then multiple divisions. In the case of 1/x, the compiler will optimize.")]
	public class DivideNode : Operation {
		private const string NodeName = "Divide";

	    public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string Operand {
			get {
				return " / ";
			}
		}
	}
}