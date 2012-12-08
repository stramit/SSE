using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Tan", "Function", typeof(TanNode),"Trigonometric Tangent function. Periodic, from [-PI/2,PI/2] it goes from [-inf,inf] with smooth easein/out towards zero. Repeats every PI. Useful as an alternative to [0,1] powers for ramping towards infinity.")]
	public class TanNode : FunctionOneInput {
		private const string NodeName = "tan";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "tan"; }
		}
	}
}
