using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Max", "Operation", typeof(MaxNode),"Returns the highest of each component for the two inputs. Useful for things like removing negatives, as when mapping zero to one of the inputs. Contrast to Min.")]
	public class MaxNode : FunctionTwoInput {
		private const string NodeName = "Max";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "max"; }
		}
	}
}
