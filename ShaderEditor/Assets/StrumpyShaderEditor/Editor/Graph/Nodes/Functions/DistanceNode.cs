using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Distance", "Function", typeof(DistanceNode),"Distance between two vectors, includes the fourth component as 4D if they do not match. Not really any more expensive to get distance versus SqrDistance")]
	public class DistanceNode : FunctionTwoInput {
		private const string NodeName = "Distance";
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string FunctionName
		{
			get{ return "distance"; }
		}
	}
}
