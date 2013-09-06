using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using StrumpyShaderEditor;
using Object = UnityEngine.Object;

namespace StrumpyShaderEditor
{
	public class PreviewWindowInternal : EditorWindow{
		private NodeEditor _parent;
	
		private readonly PreviewRenderer _renderer = new PreviewRenderer ();
		private RenderTexture _previewTexture;
		private GUIContent _previewTextureContent;
		
		private Vector2 _cameraRotation;
		
		private Mesh _previewMesh;
		
		private Camera _previewCamera;
		private IEnumerable<Light> _previewLights;
	
		private GUIContent _hotControlHash;
		private bool _initialized = false;
	
		public bool ShouldUpdate = false;
	
		public void Initialize( NodeEditor parent )
		{
			_parent = parent;
			_parent.UpdateShader();
		
			if( _initialized )
				return;
		
			_previewCamera = EditorUtility.CreateGameObjectWithHideFlags( "RenderPreviewCamera", HideFlags.HideAndDontSave, new[] { typeof(Camera) }).camera;
			_previewCamera.enabled = false;
			_previewCamera.clearFlags = CameraClearFlags.Color;
			_previewCamera.backgroundColor = new Color( 0.15f, 0.15f, 0.15f, 1.0f);
			_previewCamera.fieldOfView = 30.0f;
			_previewCamera.renderingPath = RenderingPath.DeferredLighting;
			
			var lights = new List<Light> ();
			for (var i = 0; i < 2; i++) 
			{
				var l = EditorUtility.CreateGameObjectWithHideFlags ("PreRenderLight", HideFlags.HideAndDontSave, new[] { typeof(Light) }).light;
				l.type = LightType.Directional;
				l.intensity = 0.5f;
				l.enabled = false;
				lights.Add (l);
			}
			
			lights[0].color = Color.grey;
			lights[0].intensity = 0.8f;
			lights[0].transform.rotation = Quaternion.Euler (50f, 50f, 0f);
			lights[1].color = new Color (0.4f, 0.4f, 0.45f, 0f) * 0.7f;
			lights[1].intensity = 0.8f;
			lights[1].transform.rotation = Quaternion.Euler (340f, 218f, 177f);
			
			_previewLights = lights;
			
			Object.DontDestroyOnLoad( _previewCamera );
			Object.DontDestroyOnLoad( lights[0] );
			Object.DontDestroyOnLoad( lights[1] );
			
			_hotControlHash = new GUIContent("GUIPREVIEWTEXTURE");
			_previewMaterial = null;
			
			var go = GameObject.CreatePrimitive( PrimitiveType.Sphere );
			_previewMesh = go.GetComponent<MeshFilter> ().sharedMesh;
			GameObject.DestroyImmediate( go );
		
			_initialized = true;
			UpdatePreviewTexture();
		}
	
		private void UpdatePreviewTexture()
		{
			if( _previewTexture != null )
			{
				RenderTexture.ReleaseTemporary( _previewTexture );
			}
			_previewTexture = RenderTexture.GetTemporary( (int)position.width, (int)position.height - 40, 24, RenderTextureFormat.ARGB32 );
			_previewTextureContent = new GUIContent( _previewTexture );
		}
	
		private Material _previewMaterial;
		public Material PreviewMaterial {
			get { 
				if( _previewMaterial  == null )
				{
					_previewMaterial = new Material(Shader.Find("Diffuse"));
					Object.DontDestroyOnLoad( _previewMaterial );
				}
				return _previewMaterial;
			}
			set {
				_previewMaterial = value;
				Object.DontDestroyOnLoad( _previewMaterial );
			}
		}
	
		private void UpdateMaterial()
		{
			var material = PreviewMaterial;
			
			var shaderProperties = _parent.CurrentGraph.GetProperties();
			
			foreach( var property in shaderProperties )
			{
				if( !material.HasProperty( property.PropertyName ) )
				{
					continue;
				}
			
				var colorProperty = property as ColorProperty;
				if( colorProperty != null )
				{
					material.SetColor( colorProperty.PropertyName, colorProperty.Color );
				}
			
				var textureProperty = property as Texture2DProperty;
				if( textureProperty != null )
				{
					material.SetTexture( textureProperty.PropertyName, textureProperty.Texture );
				}
			
				var textureCubeProperty = property as TextureCubeProperty;
				if( textureCubeProperty != null )
				{
					material.SetTexture( textureCubeProperty.PropertyName, textureCubeProperty.Cubemap );
				}
			
				var floatProperty = property as FloatProperty;
				if( floatProperty != null )
				{
					material.SetFloat( floatProperty.PropertyName, floatProperty.Float );
				}
			
				var float4Property = property as Float4Property;
				if( float4Property != null )
				{
					material.SetVector( float4Property.PropertyName, float4Property.Float4 );
				}
			
				var rangeProperty = property as RangeProperty;
				if( rangeProperty != null )
				{
					material.SetFloat( rangeProperty.PropertyName, rangeProperty.Range.Value );
				}
			}
		}
		
		public void OnLostFocus()
		{
			GUIUtility.hotControl = 0;
		}
		
		void OnGUI ()
		{
			var drawPos = new Rect(0, 0, (int)position.width, (int)position.height-40);
			lock( _previewTexture )
			{
				GUI.Box( drawPos, _previewTextureContent );
			}
		
			if ( _parent.ShaderNeedsUpdate() )
			{
				GUILayout.BeginArea( new Rect(0, 0, (int)position.width, (int)position.height-40) );
				GUILayout.BeginHorizontal();
				var oColor = GUI.color; // Cache old content color
				GUI.color = Color.red; // Make the warning red
				var oldFontSize = GUI.skin.label.fontSize;
				GUI.skin.label.fontSize = 20;
				GUILayout.FlexibleSpace(); // Flexible space (ensures buttohns don't move)
				GUILayout.Label("Shader Needs Updating"); // Draw the warning
				GUILayout.FlexibleSpace();
				GUI.skin.label.fontSize = oldFontSize;
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
				GUI.color = oColor; // Revert the color
			}
			
			int dragControl = GUIUtility.GetControlID( _hotControlHash, FocusType.Passive );
			switch ( Event.current.type )
			{
			case EventType.MouseDown:
				if( drawPos.Contains(Event.current.mousePosition) )
				{
					GUIUtility.hotControl = dragControl;
				}
				break;
			case EventType.MouseDrag:
				if( GUIUtility.hotControl == dragControl )
				{
					_cameraRotation -= Event.current.delta;
				}
				break;
			case EventType.MouseUp:
				if( GUIUtility.hotControl == dragControl )
				{
					GUIUtility.hotControl = 0;
				}
				break;
			}
		
			GUILayout.FlexibleSpace();
			_previewMesh = (Mesh) EditorGUILayout.ObjectField( "Mesh:", _previewMesh, typeof(Mesh) );
			
			GUILayout.BeginHorizontal( );
			_drawBackground = EditorExtensions.ToggleButton( _drawBackground, "Draw Background" );
			if( GUILayout.Button( "Update Preview" ) )
			{
				_parent.UpdateShader();
			}
			GUILayout.EndHorizontal();
		}

		private bool _drawBackground;
		private DateTime _lastRenderTime = DateTime.Now;
		
		public void Update()
		{
			if( !ShouldUpdate )
				return;
		
			Bounds bounds = _previewMesh ? _previewMesh.bounds : new Bounds(Vector3.zero, Vector3.one);
			float magnitude = bounds.extents.magnitude;
			float distance = magnitude * 3.5f;
			
			_previewCamera.DrawMeshOnly();
			
			//Set up the camera location...
			_cameraRotation.y = ClampAngle(_cameraRotation.y);
			_cameraRotation.x = ClampAngle(_cameraRotation.x);
			
			_previewCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, 0.0f, 0f) * Quaternion.Euler( 0.0f, _cameraRotation.x, 0.0f );
			_previewCamera.transform.position = _previewCamera.transform.rotation * new Vector3(0.0f, 0.0f, -distance);
			
			_previewCamera.nearClipPlane = distance - (magnitude * 1.1f);
			_previewCamera.farClipPlane = distance + (magnitude * 1.1f);
			
			//Lock rendering to 33hz (30fps).
			if (DateTime.Now > _lastRenderTime + new TimeSpan(0, 0, 0, 0, 33))
			{
				if( position.width != _previewTexture.width || position.height != _previewTexture.height )
				{
					UpdatePreviewTexture();
				}
			
				_lastRenderTime = DateTime.Now;
				UpdateMaterial();
				_renderer.Render ( _previewMesh, Vector3.zero, Quaternion.identity, _previewCamera, _previewLights, new Color (0.1f, 0.1f, 0.1f, 0f), PreviewMaterial, _previewTexture, _drawBackground);
				Repaint();
			}
		}
	
		private static float ClampAngle( float angle )
		{
			while (angle < -360)
				angle += 360;
			while (angle > 360)
				angle -= 360;
			return angle;
		}
	}
}