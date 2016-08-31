using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.IO;

namespace StrumpyShaderEditor
{
	/* High level graph, stores all the subgraphs and shader settings */
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class ShaderGraph
	{
		[DataMember]
		private ShaderSettings shaderSettings;
		
		[DataMember]
		private ShaderInputs shaderInputs;
		
		[DataMember]
		private PixelShaderGraph pixelGraph;
		
		[DataMember]
		private VertexShaderGraph vertexGraph;
		
		[DataMember]
		private SimpleLightingShaderGraph simpleLightingGraph;
		
		[DataMember]
		public SubGraphType CurrentSubGraphType{ get; set; }
		
		//This fetches the sub graph that is currently selected
		public SubGraph CurrentSubGraph
		{
			get{
				switch( CurrentSubGraphType ) 
				{
				case SubGraphType.Pixel:
					return pixelGraph;
				case SubGraphType.Vertex:
					return vertexGraph;
				case SubGraphType.SimpleLighting:
					return simpleLightingGraph;
				default:
					return pixelGraph;
				}
			}
		}
		
		public void Initialize ( Rect screenDimension, bool updateDrawPos )
		{
			shaderInputs = shaderInputs ?? new ShaderInputs(); 
			shaderSettings = shaderSettings ?? new ShaderSettings();
			
			pixelGraph = pixelGraph ?? new PixelShaderGraph();
			vertexGraph = vertexGraph ?? new VertexShaderGraph();
			simpleLightingGraph = simpleLightingGraph ?? new SimpleLightingShaderGraph();
			
			shaderInputs.Initialize();
			shaderSettings.Initialize();
			pixelGraph.Initialize(this, screenDimension, updateDrawPos);
			vertexGraph.Initialize(this, screenDimension, updateDrawPos);
			simpleLightingGraph.Initialize(this, screenDimension, updateDrawPos);
			
			MarkDirty();
		}
		
		public IEnumerable<ShaderProperty> FindValidProperties( InputType type )
		{
			return shaderInputs.FindValidProperties( type );
		}
		
		public int AddProperty( ShaderProperty p )
		{
			var result = shaderInputs.AddProperty( p );
			MarkDirty();
			return result;
		}
		
		public IEnumerable<ShaderProperty> GetProperties( )
		{
			return shaderInputs.GetProperties( );
		}
		
		public IEnumerable<T> GetValidNodes<T> () where T : class
		{
			var foundNodes = new List<T> ();
			foreach( var graph in AllSubGraphs() )
			{
				foundNodes.AddRange( graph.GetValidNodes<T>() );
			}
			return foundNodes;
		}
		
		private IEnumerable<SubGraph> AllSubGraphs()
		{
			return new List<SubGraph> {pixelGraph, vertexGraph, simpleLightingGraph};
		}
		
		public void MarkDirty()
		{
			foreach( var graph in AllSubGraphs() )
			{
				graph.MarkDirty();
			}
		}
		
		public void Draw( NodeEditor editor, bool showComments, Rect drawWindow )
		{
			CurrentSubGraph.Draw( editor, showComments, drawWindow );
		}
		
		public void DrawSettings()
		{
			shaderSettings.Draw();
		}
		
		public void DrawInput()
		{
			shaderInputs.Draw();
		}
		
		public Node FirstSelected
		{
			get{ return CurrentSubGraph.FirstSelected; }
		}
		
		public void Deselect()
		{
			foreach( var graph in AllSubGraphs() )
			{
				graph.Deselect();
			}
		}
		
		public void Deselect( Node n )
		{
			CurrentSubGraph.Deselect( n );
		}
		
		public bool IsSelected( Node n )
		{
			return CurrentSubGraph.IsSelected( n );
		}
		
		public Node NodeAt (Vector2 location )
		{
			return CurrentSubGraph.NodeAt( location );
		}
		
		public bool ButtonAt (Vector2 location )
		{
			return CurrentSubGraph.ButtonAt( location );
		}
		
		public bool Select (Vector2 location, bool addSelect )
		{
			return CurrentSubGraph.Select( location, addSelect );
		}
		
		public bool Select (Rect area, bool addSelect )
		{
			return CurrentSubGraph.Select( area, addSelect );
		}
		
		public void Select ( Node node, bool addSelect )
		{
			CurrentSubGraph.Select( node, addSelect );
		}
		
		public void SelectAll ( )
		{
			CurrentSubGraph.SelectAll( );
		}

		public void MarkSelectedHot()
		{
			CurrentSubGraph.MarkSelectedHot();
		}

		public void UnmarkSelectedHot()
		{
			CurrentSubGraph.UnmarkSelectedHot();
		}

		public bool DragSelectedNodes (Vector2 delta)
		{
			return CurrentSubGraph.DragSelectedNodes( delta );
		}

		public Rect DrawSize {
			get {
				return CurrentSubGraph.DrawSize;
			}
		}

		public RootNode RootNode {
			get {
				return CurrentSubGraph.RootNode;
			}
		}
		
		public bool ContainsCircularReferences ()
		{
			return AllSubGraphs().Any( x => x.ContainsCircularReferences() );
		}

		public bool IsGraphValid ()
		{
			return AllSubGraphs().All( x => x.IsSubGraphValid() )
					&& shaderInputs.InputsValid()
					&& shaderSettings.SettingsValid();
		}
		
		// Texel - Oh the woes to be avoided if you just made settings public.
		
		public bool IsInputsValid () {
			return shaderInputs.InputsValid();
		}
		
		public bool IsSettingsValid () {
			return shaderSettings.SettingsValid();
		}
		
		/* Go through all the graphs and look for any errors in them
		 * if there are errors the shader can not be compiled */
		public void UpdateErrorState()
		{
			//If the graph is nor dirty...
			if( AllSubGraphs().All( x => x.Dirty == false ) )
			{
				return;
			}
			
			foreach( var graph in AllSubGraphs() )
			{
				graph.UpdateErrorState();
			}
			
			foreach( var graph in AllSubGraphs() )
			{
				foreach( var graph2 in AllSubGraphs() )
				{
					if( graph != graph2 )
					{
						var inputNodes = from node in graph.Nodes
											where node is IFieldInput
											select node as IFieldInput;
						
						var inputNodes2 = from node in graph2.Nodes
											where node is IFieldInput
											select node as IFieldInput;
						
						foreach( var input in inputNodes )
						{
							var input1 = input;
							if( inputNodes2.Any( x => x.GetFieldName() == input1.GetFieldName() ) )
							{
								var node = input as Node;
								if (node != null)
								{
									node.AddErrors( new List<string> {"Node with same name exists in " + graph2.GraphTypeName + " graph"} );
									node.CurrentState = NodeState.Error;
								}
							}
						}
					}
				}
			}
		}
		
		/* Built the shader
		 * this mushes the graphs from a tree structure into code, then 
		 * replaces the templated areas from the shader template with the generated
		 * hlsl */
		public string GetShader (string shaderTemplateLocation, bool isPreviewShader)
		{
			var shaderTemplate = File.ReadAllText (shaderTemplateLocation);
			
			bool needsTimeNode = false;
			bool needsSinTimeNode = false;
			bool needsCosTimeNode = false;
			foreach( var graph in AllSubGraphs() )
			{
				graph.SetPreview( isPreviewShader );
				needsTimeNode |= graph.NeedsTimeNode;
				needsSinTimeNode |= graph.NeedsSinTimeNode;
				needsCosTimeNode |= graph.NeedsCosTimeNode;
			}
			
			shaderTemplate = shaderTemplate.Replace ("${ShaderName}", isPreviewShader ? "ShaderEditor/EditorShaderCache" : shaderSettings.ShaderName );
			shaderTemplate = shaderTemplate.Replace ("${Fallback}", shaderSettings.FallBack );
			
			var properties = "";
			var shaderVariableNames = "";
			
			properties += shaderInputs.GetInputProperties();
			shaderVariableNames += shaderInputs.GetInputVariables();
			
			properties += (isPreviewShader && needsTimeNode ) ? "_EditorTime(\"_EditorTime\",Vector) = (0.0,0.0,0.0,0.0)\n" : "";
			properties += (isPreviewShader && needsCosTimeNode ) ? "_EditorCosTime(\"_EditorCosTime\",Vector) = (0.0,0.0,0.0,0.0)\n" : "";
			properties += (isPreviewShader && needsSinTimeNode ) ? "_EditorSinTime(\"_EditorSinTime\",Vector) = (0.0,0.0,0.0,0.0)\n" : "";
			
			shaderVariableNames += (isPreviewShader && needsTimeNode ) ? "float4 _EditorTime;\n" : "";
			shaderVariableNames += (isPreviewShader && needsCosTimeNode ) ? "float4 _EditorCosTime;\n" : "";
			shaderVariableNames += (isPreviewShader && needsSinTimeNode ) ? "float4 _EditorSinTime;\n" : "";
			
			var requiresGrabPass = AllSubGraphs().Any( x=> x.RequiresGrabPass );
			shaderVariableNames += requiresGrabPass ? "sampler2D _GrabTexture;\n" : "" ;
			shaderVariableNames += AllSubGraphs().Any( x => x.RequiresSceneDepth ) ? "sampler2D _CameraDepthTexture;\n" : "";
			
			shaderTemplate = shaderTemplate.Replace ("${ShaderProperties}", properties );
			shaderTemplate = shaderTemplate.Replace ("${ShaderVariableNames}", shaderVariableNames );

			shaderTemplate = shaderTemplate.Replace("${GrabPass}", requiresGrabPass ? "GrabPass { }" : "");

			shaderTemplate = shaderTemplate.Replace ("${Tags}", shaderSettings.Tags);
			shaderTemplate = shaderTemplate.Replace ("${Options}", shaderSettings.Options);
			shaderTemplate = shaderTemplate.Replace ("${SurfaceFlags}", shaderSettings.SurfaceFlags );
			shaderTemplate = shaderTemplate.Replace ("${ShaderPragma}", shaderSettings.Pragma);
			shaderTemplate = shaderTemplate.Replace ("${LightingFunctionPrePass}", simpleLightingGraph.LightingFunctionBody );
			shaderTemplate = shaderTemplate.Replace ("${StructInputs}", pixelGraph.StructInput);
			
			shaderTemplate = shaderTemplate.Replace ("${VertexShader}", vertexGraph.ShaderBody );
			shaderTemplate = shaderTemplate.Replace ("${VertexShaderMods}", pixelGraph.VertexShaderMods);
			
			shaderTemplate = shaderTemplate.Replace ("${SurfaceShader}", pixelGraph.ShaderBody );
			return shaderTemplate;
		}
	}
}
