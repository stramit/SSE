using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class RangeProperty : ShaderProperty
	{
		[DataMember] private EditorRange _value;
		
		public RangeProperty()
		{
		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			_value = _value ?? new EditorRange();
		}
		
		public EditorRange Range
		{
			get{ return _value; }
			set{ _value = value; }
		}
		
		public override void Draw()
		{
			GUILayout.BeginVertical();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label( "Min:");
			GUILayout.Label("Max:");
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			_value.Min = EditorGUILayout.FloatField( _value.Min );
			_value.Max = EditorGUILayout.FloatField( _value.Max );
			GUILayout.EndHorizontal();
			
			_value.Value = EditorGUILayout.Slider( _value.Value, _value.Min, _value.Max );
			GUILayout.EndVertical();
		}
		
		public override InputType GetPropertyType()
		{
			return InputType.Range;
		}
		
		public override string GetPropertyDefinition()
		{
			string result = "";
			result += PropertyName;
			result += "(\""+ PropertyDescription + "\", " + GetPropertyType().PropertyTypeString() + "(" + _value.Min + "," + _value.Max + ") ) = "
						+ _value.Value + "\n";
			return result;
		}
	}
}
