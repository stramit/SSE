using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class FloatProperty : ShaderProperty
	{
		[DataMember] private EditorFloat _value;
		
		public FloatProperty()
		{
		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			_value = _value ?? new EditorFloat();
		}
		
		public float Float
		{
			get{ return _value; }
			set{ _value = value; }
		}
		
		public override void Draw()
		{
			GUILayout.BeginVertical();
			GUILayout.Label( "Value:" );
			_value = EditorGUILayout.FloatField( _value );
			GUILayout.EndVertical();
		}
		
		public override InputType GetPropertyType()
		{
			return InputType.Float;
		}
		
		public override string GetPropertyDefinition()
		{
			string result = "";
			result += PropertyName;
			result += "(\""+ PropertyDescription + "\", " + GetPropertyType().PropertyTypeString() + ") = "
						+ _value.Value + "\n";
			return result;
		}
		
	}
}
