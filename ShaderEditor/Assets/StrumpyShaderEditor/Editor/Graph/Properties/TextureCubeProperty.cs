using UnityEngine;
using UnityEditor;
using System;
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

        [DataMember]
        string assetPath;   // for DataContract

        public Cubemap Cubemap
		{
			get
            {
                SetTextureFromAssetPath();
                return previewTexture;
            }
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

            var currentTexture = previewTexture;
            previewTexture = (Cubemap)EditorGUILayout.ObjectField(Cubemap, typeof(Cubemap), true, new[] { GUILayout.Width (60), GUILayout.Height (60) });
			GUILayout.EndVertical();

            if (currentTexture != previewTexture) SetAssetPath();

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

        // Save the asset path for DataContract
        void SetAssetPath()
        {
            assetPath = null;
            if (previewTexture != null)
            {
                assetPath = AssetDatabase.GetAssetPath(previewTexture);
            }
        }
        // Load Asset at first time when previewTexture is null and has assert path
        void SetTextureFromAssetPath()
        {
            if (!String.IsNullOrEmpty(assetPath) && previewTexture == null)
            {
                previewTexture = AssetDatabase.LoadAssetAtPath<Cubemap>(assetPath);
            }
        }
    }
}
