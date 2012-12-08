using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Floor", "Operation", typeof(FloorNode),"Returns the integer part of each component. Used for sawtooth repition patterns, and for creating subtiling sections. When used together with Frac the compiler can combine the operations. Potential to cause anti-aliasing problems when used for things like forcing repeats, solutions to those issues are inelegant in code and hard to replicate in graphs. Contrast with Ceil.")]
	public class FloorNode : FunctionOneInput {
		private const string NodeName = "Floor";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "floor"; }
		}
	}
}
