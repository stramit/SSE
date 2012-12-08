using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.IO;

namespace StrumpyShaderEditor
{
	/* This is the pixel shader graph, this stores all the nodes that 
	 * are used for the pixel graph selection */
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class PixelShaderGraph : SubGraph
	{
		private const string GraphName = "Pixel";
		
		public PixelShaderGraph ()
		{
			AddNode (new ShaderMasterNode ());
			MarkDirty();
		}
		
		public override string GraphTypeName { 
			get{ return GraphName; }
		}
		
		public override SubGraphType GraphType {
			get { return SubGraphType.Pixel; }
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

		public ShaderMasterNode MasterNode {
			get {
				var master = Nodes.FirstOrDefault (x => x.GetType () == typeof(ShaderMasterNode)) as ShaderMasterNode;
				if (master == null) {
					Debug.LogError ("No master shader node in graph");
				}
				return master;
			}
		}
		
		/*
		 * Get the struct input for this shader graph... that is the 
		 * fields that effect the stuct that are required in the .shader file*/
		public string StructInput
		{
			get{
				var structInput = "";
				var sortedStuctInputs = GetValidStructNodes ();
			
				bool addedInput = false;
				foreach (var nodeList in sortedStuctInputs.Values) {
					foreach( var structNode in nodeList )
					{
						if( structNode.RequiresStructFieldInclusion() )
						{
							structInput += structNode.GetStructFieldDefinition ();
							addedInput |= true;
							break;
						}
					}
				}
				if (GetConnectedNodesDepthFirst ().Any (x => x.RequiresInternalData)) {
					structInput += "INTERNAL_DATA\n";
					addedInput |= true;
				}
				if (!addedInput) {
					structInput += "float4 color : COLOR;\n";
				}
				
				return structInput;
			}
		}
		
		/* Any vertex shader code that is required by the pixel graph gets injected here*/
		public string VertexShaderMods
		{
			get{
				var sortedStuctInputs = GetValidStructNodes ();
				var vertexShader = "";
				foreach (var nodeList in sortedStuctInputs.Values) {
					foreach( var structNode in nodeList )
					{
						if( structNode.RequiresStructFieldInclusion() )
						{
							vertexShader += structNode.GetStructVertexShaderString ();
							break;
						}
					}
				}
				return vertexShader;
			}
		}
		
		/*Build the pixel shader body*/
		public string ShaderBody
		{
			get{
				var shaderBody = "";
				var resultCacheNodes = GetValidNodes<IResultCacheNode> ();
				foreach (var node in resultCacheNodes) {
					shaderBody += node.GetAdditionalFields ();
					shaderBody += node.GetUsage ();
				}
				
				shaderBody += MasterNode.GetAdditionalFields ();
				
				shaderBody += MasterNode.ClipConnected () ? "clip( " + MasterNode.GetClipExpression () + " );\n" : "";
				shaderBody += MasterNode.AlbedoConnected () ? "o.Albedo = " + MasterNode.GetAlbedoExpression () + ";\n" : "";
				if( MasterNode.NormalConnected () )
				{
					shaderBody += "o.Normal = " + MasterNode.GetNormalExpression () + ";\n";
				}
				shaderBody += MasterNode.EmissionConnected () ? "o.Emission = " + MasterNode.GetEmissionExpression () + ";\n" : "";
				shaderBody += MasterNode.SpecularConnected () ? "o.Specular = " + MasterNode.GetSpecularExpression () + ";\n" : "";
				shaderBody += MasterNode.GlossConnected () ? "o.Gloss = " + MasterNode.GetGlossExpression () + ";\n" : "";
				shaderBody += MasterNode.CustomConnected () ? "o.Custom = " + MasterNode.GetCustomExpression () + ";\n" : "";
				
				if( MasterNode.AlphaConnected () )
				{
					shaderBody += "o.Alpha = " + MasterNode.GetAlphaExpression () + ";\n";
				}
				
				return shaderBody;
			}
		}
		
	}
}
