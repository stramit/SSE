using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;

namespace StrumpyShaderEditor
{
	public class PreviewRenderer {
		private readonly Type _internalRenderUtilityType;
		
		public PreviewRenderer()
		{
			foreach (var type in
				AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes().Where(type => type.ToString() == "UnityEditorInternal.InternalEditorUtility")))
			{
				_internalRenderUtilityType = type;
			}
		}

		public void Render( Mesh mesh, 
							Vector3 position, 
							Quaternion rotation, 
							Camera camera, 
							IEnumerable<Light> lights, 
							Color ambient, 
							Material material,
							RenderTexture target,
							bool drawBackground )
		{
			var oldTarget = camera.targetTexture;
			var oldFlags = camera.clearFlags;
			
			camera.targetTexture = target;
			
			_internalRenderUtilityType.InvokeMember( "SetCustomLighting",
													BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static,
													null,
													null,
													new object[] { lights.ToArray(), ambient } );

			var time = (float)EditorApplication.timeSinceStartup;
			if( material.HasProperty( "_EditorTime") )
			{
				var vTime = new Vector4( time / 20, time, time*2, time*3);
				material.SetVector( "_EditorTime", vTime );
			}
			
			if( material.HasProperty( "_EditorSinTime") )
			{
				var sinTime = new Vector4( Mathf.Sin( time / 8f ), Mathf.Sin( time / 4f ), Mathf.Sin( time /2f ), Mathf.Sin( time ) );
				material.SetVector( "_EditorSinTime", sinTime );
			}
			
			if( material.HasProperty( "_EditorCosTime") )
			{
				var cosTime = new Vector4( Mathf.Cos( time / 8f ), Mathf.Cos( time / 4f ), Mathf.Cos( time /2f ), Mathf.Cos( time ) );
				material.SetVector( "_EditorCosTime", cosTime );
			}
			
			if( drawBackground )
			{
				Graphics.DrawMesh(BackPlane, Matrix4x4.identity, CheckerMat, 1, camera, 0);
				camera.Render();
				camera.clearFlags = CameraClearFlags.Nothing;
			}
			
			if( mesh != null )
			{
				for (int i = 0; i < mesh.subMeshCount; i++)
				{
					Graphics.DrawMesh(mesh, position, rotation, material, 1, camera, i);
				}
			}
			
			camera.Render();
			camera.clearFlags = oldFlags;
			
			_internalRenderUtilityType.InvokeMember( "RemoveCustomLighting",
													BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static,
													null,
													null,
													null );
			
			camera.targetTexture = oldTarget;
			return;
		}

		private Mesh _backPlaneCache;
		private Mesh BackPlane {
			get {
				if (_backPlaneCache)
					return _backPlaneCache;
					
				var backplane = new Mesh
				                	{
				                		hideFlags = HideFlags.HideAndDontSave,
				                		name = "Backplane",
				                		vertices = new[]
				                		           	{
				                		           		new Vector3(-1f, -1f, 0),
				                		           		new Vector3(-1f, 1f, 0),
				                		           		new Vector3(1f, 1f, 0),
				                		           		new Vector3(1f, -1f, 0)
				                		           	},
				                		uv = new[]
				                		     	{
				                		     		Vector2.zero,
				                		     		Vector2.up,
				                		     		Vector2.one,
				                		     		Vector2.right
				                		     	},
				                		normals = new[]
				                		          	{
				                		          		Vector3.forward,
				                		          		Vector3.forward,
				                		          		Vector3.forward,
				                		          		Vector3.forward
				                		          	},
				                		triangles = new[]
				                		            	{
				                		            		0, 1, 2,
				                		            		2, 3, 0
				                		            	}
				                	};

				_backPlaneCache = backplane;
				
				return _backPlaneCache;
			}
		}
		
		private static Material CheckerMat {
			get {
				return Resources.Load("Internal/Checker", typeof(Material)) as Material;
			}
		}
	}
}