using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class Texture2DProperty : ShaderProperty
	{
		[DataMember]
		public DefaultTextureType _defaultTexture;
		private Texture2D _previewTexture;
		
		public Texture2D Texture
		{
			get{ return _previewTexture;}
		}
		
		public override void Draw()
		{
			GUILayout.BeginVertical( );
			GUILayout.Label( "Default:" );
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			_defaultTexture = (DefaultTextureType)EditorGUILayout.EnumPopup( _defaultTexture );
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			_previewTexture = (Texture2D)EditorGUILayout.ObjectField( _previewTexture, typeof(Texture2D), new[] { GUILayout.Width (60), GUILayout.Height (60) });
			GUILayout.EndVertical();
		}
		
		public override InputType GetPropertyType()
		{
			return InputType.Texture2D;
		}
		
		public override string GetPropertyDefinition()
		{
			string result = "";
			result += PropertyName;
			result += "(\""+ PropertyDescription + "\", " + GetPropertyType().PropertyTypeString() + ") = \"" + _defaultTexture.ToString().ToLower() +"\" {}\n";
			return result;
		}
	}
}
