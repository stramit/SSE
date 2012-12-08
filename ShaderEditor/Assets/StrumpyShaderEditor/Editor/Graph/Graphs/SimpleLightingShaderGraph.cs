using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.IO;
using System.Text;

namespace StrumpyShaderEditor
{
	/*Lighting graph... this allows overriding the default 
	 * unity lighting model with a custom model */
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class SimpleLightingShaderGraph : SubGraph
	{
		private const string GraphName = "SimpleLighting";
		
		public SimpleLightingShaderGraph ()
		{
			AddNode (new LightingOutputMasterNode());
			MarkDirty();
		}
		
		public override string GraphTypeName { 
			get{ return GraphName; }
		}
		
		public override SubGraphType GraphType {
			get { return SubGraphType.SimpleLighting; }
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

		public LightingOutputMasterNode MasterNode {
			get {
				var master = Nodes.FirstOrDefault (x => x.GetType () == typeof(LightingOutputMasterNode)) as LightingOutputMasterNode;
				if (master == null) {
					Debug.LogError ("No master shader node in graph");
				}
				return master;
			}
		}
		
		
		public string LightingFunctionBody
		{
			get{
				if( MasterNode.LightingFunctionPresent )
				{
					var lightingFunction = "";
					var resultCacheNodes = GetValidNodes<IResultCacheNode> ();
					foreach (var node in resultCacheNodes) {
						lightingFunction += node.GetAdditionalFields ();
						lightingFunction += node.GetUsage ();
					}
					
					lightingFunction += MasterNode.GetAdditionalFields ();
					
					lightingFunction += "return " + MasterNode.GetLightingExpression() + ";\n";
					return lightingFunction;
				}
				else
				{
					var lightingFunction = "";
					lightingFunction += "half3 spec = light.a * s.Gloss;\n";
					lightingFunction += "half4 c;\n";
					lightingFunction += "c.rgb = (s.Albedo * light.rgb + light.rgb * spec) * s.Alpha;\n";
					lightingFunction += "c.a = s.Alpha;\n";
					lightingFunction += "return c;\n";
					return lightingFunction;
				}
			}
		}
	}
}
