using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class TextureCubeProperty : ShaderProperty
	{
		[DataMember]
		public DefaultTextureType _defaultTexture;
		
		private Cubemap previewTexture;
		
		public Cubemap Cubemap
		{
			get{ return previewTexture; }
		}
		
		public override void Draw()
		{
			GUILayout.BeginVertical();
			GUILayout.Label( "Default:" );
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			_defaultTexture = (DefaultTextureType)EditorGUILayout.EnumPopup( _defaultTexture );
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			previewTexture = (Cubemap)EditorGUILayout.ObjectField( previewTexture, typeof(Cubemap), new[] { GUILayout.Width (60), GUILayout.Height (60) });
			GUILayout.EndVertical();
		}
		
		public override InputType GetPropertyType()
		{
			return InputType.TextureCube;
		}
		
		public override string GetPropertyDefinition()
		{
			var result = "";
			result += PropertyName;
			result += "(\""+ PropertyDescription + "\", " + GetPropertyType().PropertyTypeString() + ") = \"" + _defaultTexture.ToString().ToLower() +"\" {}\n";
			return result;
		}
		
	}
}
