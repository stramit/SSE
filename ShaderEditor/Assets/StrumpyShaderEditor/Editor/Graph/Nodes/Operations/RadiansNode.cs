using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Radians", "Operation", typeof(RadiansNode),"Convert Degrees to Radians. When used with constant inputs, it will be compiled out. See also Degrees.")]
	public class RadiansNode : FunctionOneInput {
		private const string NodeName = "Radians";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "radians"; }
		}
	}
}
