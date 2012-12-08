using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class ColorProperty : ShaderProperty
	{
		[DataMember] private EditorColor _color;
		
		public ColorProperty()
		{
			Initialize();
		}
		
		public override void Initialize()
		{
			base.Initialize();
			_color = _color ?? new EditorColor();
		}
		
		public Color Color
		{
			get{ return _color; }
			set{ _color = value; }
		}
		
		public override void Draw()
		{
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.Label( "Color:", GUILayout.Width( 63 ) );
			_color = EditorGUILayout.ColorField( _color );
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
		
		public override string GetPropertyDefinition()
		{
			string result = "";
			result += PropertyName;
			result += "(\""+ PropertyDescription + "\", " + GetPropertyType().PropertyTypeString() + ") = (" 
						+ _color.R + ","
						+ _color.G + ","
						+ _color.B + ","
						+ _color.A + ")\n";
			return result;
		}
		
		public override InputType GetPropertyType()
		{
			return InputType.Color;
		}
	}
}
