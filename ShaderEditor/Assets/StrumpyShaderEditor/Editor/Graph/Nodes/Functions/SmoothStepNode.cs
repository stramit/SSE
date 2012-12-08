using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("SmoothStep", "Function", typeof(SmoothStepNode),"Smoothly interpolate between the first and second values by the third. The interpolation in the range verymuch resembles a Sin-ramp, with smoothed ins. It is more efficient then manually ramping to a Sin curve. Same input structure and other general notes as Lerp, negligably slower")]
	public class SmoothStepNode : FunctionThreeInput {
		private const string NodeName = "SmoothStep";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "smoothstep"; }
		}
	}
}
