using UnityEngine;
using System.Linq;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class VertexShaderGraph : SubGraph
	{
		private const string GraphName = "Vertex";
		
		public VertexShaderGraph ()
		{
			AddNode (new VertexOutputMasterNode());
			MarkDirty();
		}
		
		public override string GraphTypeName { 
			get{ return GraphName; }
		}
		
		public override SubGraphType GraphType {
			get { return SubGraphType.Vertex; }
		}
		
		public override RootNode RootNode {
			get {
				var master = Nodes.FirstOrDefault (x => x is RootNode) as RootNode;
				if (master == null) {
					Debug.LogError ("No master shader node in graph");
				}
				return master;
			}
		}

		private VertexOutputMasterNode MasterNode {
			get {
				var master = Nodes.FirstOrDefault (x => x.GetType () == typeof(VertexOutputMasterNode)) as VertexOutputMasterNode;
				if (master == null) {
					Debug.LogError ("No master shader node in graph");
				}
				return master;
			}
		}
		
		public string ShaderBody {
			get{
				var shaderBody = "";
				var resultCacheNodes = GetValidNodes<IResultCacheNode> ();
				foreach (var node in resultCacheNodes) {
					shaderBody += node.GetAdditionalFields ();
					shaderBody += node.GetUsage ();
				}
				
				shaderBody += MasterNode.GetAdditionalFields ();
				
				shaderBody += MasterNode.PositionConnected() ? "v.vertex = " + MasterNode.GetPositionExpression() + ";\n" : "";
				shaderBody += MasterNode.ColorConnected() ? "v.color = " + MasterNode.GetColorExpression() + ";\n" : "";
				shaderBody += MasterNode.NormalConnected() ? "v.normal = (" + MasterNode.GetNormalExpression() + ").xyz;\n" : "";
				shaderBody += MasterNode.TangentConnected() ? "v.tangent = " + MasterNode.GetTangentExpression() + ";\n" : "";
				return shaderBody;
			}
		}
	}
}
