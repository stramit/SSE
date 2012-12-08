using UnityEngine;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class EditorFloat2 : IUnityeditorDrawableType {
		private Vector2 _float2Value = Vector2.zero;
	
		public static implicit operator EditorFloat2(Vector2 vector)
		{
			var converted = new EditorFloat2 {Value = vector};
			return converted;
		}
		
		public static implicit operator Vector2(EditorFloat2 value)
		{
			return value == null ? Vector2.zero : value.Value;
		}
		
		public Vector2 Value{ 
			get{ return _float2Value; }
			set{ _float2Value = value; }
		}
		
		//Add serialization to the vector
		[DataMember] public float X
		{
			get{ return _float2Value.x; }
			set{ _float2Value.x = value; }
		}
		
		[DataMember] public float Y
		{
			get{ return _float2Value.y; }
			set{ _float2Value.y = value; }
		}
	}
}
