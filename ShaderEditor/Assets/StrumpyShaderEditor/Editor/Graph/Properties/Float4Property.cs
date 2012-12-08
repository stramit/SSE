using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class Float4Property : ShaderProperty
	{
		[DataMember] private EditorFloat4 _value;
		
		public Float4Property()
		{
		}
		
		public override void Initialize ()
		{
			base.Initialize();
			_value = _value ?? new EditorFloat4();
		}
		
		public Vector4 Float4
		{
			get{ return _value; }
			set{ _value = value; }
		}
		
		public override void Draw()
		{
			GUILayout.BeginVertical();
			_value = EditorGUILayout.Vector4Field( "Value:", _value );
			GUILayout.EndVertical();
		}

		public override InputType GetPropertyType() 
		{
			return InputType.Vector;
		}
		
		public override string GetPropertyDefinition()
		{
			var result = "";
			result += PropertyName;
			result += "(\"" + PropertyDescription + "\", " + GetPropertyType().PropertyTypeString() + ") = ("
						+ _value.X + ","
						+ _value.Y + ","
						+ _value.Z + ","
						+ _value.W + ")\n";
			return result;
		}
	}
}
