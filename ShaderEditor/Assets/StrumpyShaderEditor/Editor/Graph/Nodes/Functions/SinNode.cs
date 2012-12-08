using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Sin", "Function", typeof(SinNode),"Sin trigonmetric function. Returns from [-1,1] when fed from [-PI,PI], Cos is an alternate phase. Used for creating periodic values and rotational math, if using with Time, see SinTime/CosTime instead. When using a single constant with both Sin and Cos, it is optimized to a SinCos operation and far cheaper. To achieve this, Splat a value into both Sin and Cos. See Also Cos")]
	public class SinNode : FunctionOneInput {
		private const string NodeName = "Sin";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "sin"; }
		}
	}
}
