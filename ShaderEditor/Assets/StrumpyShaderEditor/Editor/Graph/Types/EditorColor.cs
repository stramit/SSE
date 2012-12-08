using UnityEngine;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class EditorColor : IUnityeditorDrawableType{
		private Color _color = Color.white;
		
		public static implicit operator EditorColor(Color color)
		{
			var converted = new EditorColor {Value = color};
			return converted;
		}
		
		public static implicit operator Color(EditorColor color)
		{
			return color == null ? Color.white : color.Value;
		}
		
		public Color Value{ 
			get{ return _color; }
			set{ _color = value; } 
		}
		
		//Add serialization to the color
		[DataMember] public float R
		{
			get{ return _color.r; }
			set{ _color.r = value; }
		}
		
		[DataMember] public float G
		{
			get{ return _color.g; }
			set{ _color.g = value; }
		}
		
		[DataMember] public float B
		{
			get{ return _color.b; }
			set{ _color.b = value; }
		}
		
		[DataMember] public float A
		{
			get{ return _color.a; }
			set{ _color.a = value; }
		}
	}
}
