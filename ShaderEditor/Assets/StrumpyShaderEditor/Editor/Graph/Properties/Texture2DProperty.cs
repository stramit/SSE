                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            using UnityEngine;
using UnityEditor;
using System;
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
        [DataMember]
        string assetPath;   // for DataContract

        public Texture2D Texture
		{
			get
            {
                SetTextureFromAssetPath();
                return _previewTexture;
            }
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

            var currentTexture = _previewTexture;
            _previewTexture = (Texture2D)EditorGUILayout.ObjectField( Texture, typeof(Texture2D), true, new[] { GUILayout.Width (60), GUILayout.Height (60) });

            GUILayout.EndVertical();

            if (currentTexture != _previewTexture) SetAssetPath();
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

        // Save the asset path for DataContract
        void SetAssetPath()
        {
            assetPath = null;
            if (_previewTexture != null)
            {
                assetPath = AssetDatabase.GetAssetPath(_previewTexture);
            }
        }
        // Load Asset at first time when _previewTexture is null and has assert path
        void SetTextureFromAssetPath()
        {
            if (!String.IsNullOrEmpty(assetPath) && _previewTexture == null)
            {
                _previewTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            }
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                           