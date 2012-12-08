using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class ShaderInputs
	{
		[DataMember] public List<ShaderProperty> _shaderProperties;
		
		public void Initialize()
		{
			_shaderProperties = _shaderProperties ?? new List<ShaderProperty>();
			
			foreach( var prop in _shaderProperties )
			{
				prop.Initialize();
			}
		}
		
		public bool InputsValid()
		{
			foreach( var property in _shaderProperties )
			{
				if( _shaderProperties.Where( x=> x.PropertyName == property.PropertyName ).Count() > 1 )
				{
					return false;
				}
			}
			
			return _shaderProperties.All( x => x.IsValid() );
		}
		
		public IEnumerable<ShaderProperty> FindValidProperties( InputType type )
		{
			return from property in _shaderProperties
				where property.GetPropertyType() == type
					select property;
		}
		
		public IEnumerable<ShaderProperty> GetProperties( )
		{
			return _shaderProperties;
		}
		
		public string GetInputProperties()
		{
			var result = "";
			foreach( var propery in _shaderProperties )
			{
				result += propery.GetPropertyDefinition();
			}
			return result;
		}
		
		public string GetInputVariables()
		{
			var result = "";
			foreach( var propery in _shaderProperties )
			{
				result += propery.GetVariableDefinition();
			}
			return result;
		}
		
		public int AddProperty( ShaderProperty p )
		{
			var propertyIds = from property in _shaderProperties
								orderby property.PropertyId
								select property.PropertyId;
		
			int nextId = 0;
			while ( propertyIds.Contains (nextId)) {
				++nextId;
			}
			
			p.PropertyId = nextId;
			_shaderProperties.Add( p );
			return p.PropertyId;
		}
		
		GUIContent _upIcon;
		GUIContent _downIcon;
		GUIContent _removeIcon;
		public void Draw()
		{
			if( GUILayout.Button( "Add New Input..." ) )
			{
				var menu = new GenericMenu();
				menu.AddItem( new GUIContent( "Color Property" ), false, AddInputProperty, new ColorProperty() );
				menu.AddItem( new GUIContent( "Float4 Property" ), false, AddInputProperty, new Float4Property() );
				menu.AddItem( new GUIContent( "Float Property" ), false, AddInputProperty, new FloatProperty() );
				menu.AddItem( new GUIContent( "Range Property" ), false, AddInputProperty, new RangeProperty() );
				menu.AddItem( new GUIContent( "Texture 2D Property" ), false, AddInputProperty, new Texture2DProperty() );
				menu.AddItem( new GUIContent( "Texture Cube Property" ), false, AddInputProperty, new TextureCubeProperty() );
				menu.AddItem( new GUIContent( "Matrix Property" ), false, AddInputProperty, new MatrixProperty() );
				
				menu.AddItem( new GUIContent( "Unity/Main Color"), false, AddInputProperty, 
												new ColorProperty(){ 
													PropertyName = "_Color",
													PropertyDescription = "Main Color",
													Color = new Color( 1f,1f,1f,1f ) } );
				menu.AddItem( new GUIContent( "Unity/Main Texture"), false, AddInputProperty, 
												new Texture2DProperty(){ 
													PropertyName = "_MainTex",
													PropertyDescription = "Base (RGB) Gloss (A)",
													_defaultTexture = DefaultTextureType.White } );
				menu.AddItem( new GUIContent( "Unity/Shininess"), false, AddInputProperty, 
												new RangeProperty(){ 
													PropertyName = "_Shininess",
													PropertyDescription = "Shininess",
													Range = new EditorRange(0.01f, 1.0f, 0.078125f ) } );
				menu.AddItem( new GUIContent( "Unity/Bump Map"), false, AddInputProperty, 
												new Texture2DProperty(){ 
													PropertyName = "_BumpMap",
													PropertyDescription = "Normalmap",
													_defaultTexture = DefaultTextureType.Bump } );
				menu.AddItem( new GUIContent( "Unity/Detail"), false, AddInputProperty, 
												new Texture2DProperty(){ 
													PropertyName = "_Detail",
													PropertyDescription = "Detail (RGB)",
													_defaultTexture = DefaultTextureType.Gray } );
				menu.AddItem( new GUIContent( "Unity/Parallax"), false, AddInputProperty, 
												new RangeProperty(){ 
													PropertyName = "_Parallax",
													PropertyDescription = "Height",
													Range = new EditorRange( 0.005f, 0.08f, 0.02f ) } );
				menu.AddItem( new GUIContent( "Unity/Parallax Map"), false, AddInputProperty, 
												new Texture2DProperty(){ 
													PropertyName = "_ParallaxMap",
													PropertyDescription = "Heightmap (A)",
													_defaultTexture = DefaultTextureType.Black } );
				menu.AddItem( new GUIContent( "Unity/Reflection Color"), false, AddInputProperty, 
												new ColorProperty(){ 
													PropertyName = "_ReflectColor",
													PropertyDescription = "Reflection Color",
													Color = new Color(1f,1f,1f,0.5f) } );
				menu.AddItem( new GUIContent( "Unity/Reflection Cubemap"), false, AddInputProperty, 
												new TextureCubeProperty(){ 
													PropertyName = "_Cube",
													PropertyDescription = "Reflection Cubemap",
													_defaultTexture = DefaultTextureType.Black } );
				menu.AddItem( new GUIContent( "Unity/Emission (Lightmapper)"), false, AddInputProperty, 
												new FloatProperty(){ 
													PropertyName = "_EmissionLM",
													PropertyDescription = "Emission (Lightmapper)",
													Float = 0.0f } );
				menu.ShowAsContext();
			}
			
			ShaderProperty moveUpItem = null;
			ShaderProperty moveDownItem = null;
			ShaderProperty deleteItem = null;
			
			
			_upIcon =_upIcon ?? new GUIContent( Resources.Load("Internal/Up", typeof(Texture2D)) as Texture2D );
			_downIcon =_downIcon ?? new GUIContent( Resources.Load("Internal/Down", typeof(Texture2D)) as Texture2D );
			_removeIcon =_removeIcon ?? new GUIContent( Resources.Load("Internal/Delete", typeof(Texture2D)) as Texture2D );
			
			foreach( var property in _shaderProperties )
			{
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal( );
				
				var oldColor = GUI.color;
				if( _shaderProperties.Where( x=> x.PropertyName == property.PropertyName ).Count() > 1 || !property.IsValid() )
				{
					GUI.color = Color.red;
				}
				property.Expanded =  EditorGUILayout.Foldout( property.Expanded, property.GetPropertyType().PropertyTypeString() );
				property.PropertyName = GUILayout.TextField( property.PropertyName, new[] { GUILayout.Width (130)} );
				GUI.color = oldColor;
				
				GUILayout.FlexibleSpace();
				if( GUILayout.Button(_upIcon, GUILayout.Width( 21 ), GUILayout.Height( 21 ) ) )
				{
					moveUpItem = property;
				}
				if( GUILayout.Button(_downIcon, GUILayout.Width( 21 ), GUILayout.Height( 21 ) ) )
				{
					moveDownItem = property;
				}
				if( GUILayout.Button(_removeIcon, GUILayout.Width( 21 ), GUILayout.Height( 21 ) ) )
				{
					deleteItem = property;
				}
				
				GUILayout.EndHorizontal();
				if( property.Expanded )
				{
					GUILayout.Space( 3 );
					GUILayout.BeginHorizontal();
					GUILayout.Label( "Description", new[] { GUILayout.Width (69)} );
					property.PropertyDescriptionDisplay = GUILayout.TextField( property.PropertyDescriptionDisplay );
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					GUILayout.Space( 10 );
					property.Draw();
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
				
				GUILayout.Space( 10 );
			}
			
			if( moveUpItem != null )
			{
				var currentPosition = _shaderProperties.IndexOf( moveUpItem );
				if( currentPosition > 0 )
				{
					_shaderProperties.Remove( moveUpItem );
					_shaderProperties.Insert( currentPosition - 1, moveUpItem );
				}
			}
			else if( moveDownItem != null )
			{
				var currentPosition = _shaderProperties.IndexOf( moveDownItem );
				if( currentPosition < _shaderProperties.Count - 1 )
				{
					_shaderProperties.Remove( moveDownItem );
					_shaderProperties.Insert( currentPosition + 1, moveDownItem );
				}
			}
			else if( deleteItem != null )
			{
				_shaderProperties.Remove( deleteItem );
			}
		}
		
		private void AddInputProperty( object objProperty )
		{
			var inputProperty = objProperty as ShaderProperty;
			
			if( inputProperty != null )
			{
				var propertyIds = from property in _shaderProperties
									orderby property.PropertyId
									select property.PropertyId;
			
				int nextId = 0;
				while ( propertyIds.Contains (nextId)) {
					++nextId;
				}
				
				inputProperty.PropertyId = nextId;
				_shaderProperties.Add( inputProperty );
			}
		}
	}
}
