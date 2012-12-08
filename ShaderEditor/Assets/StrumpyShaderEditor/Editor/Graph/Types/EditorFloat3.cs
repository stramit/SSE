using UnityEngine;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class EditorFloat3 : IUnityeditorDrawableType {
		private Vector3 _float3Value = Vector3.zero;
	
		public static implicit operator EditorFloat3(Vector3 vector)
		{
			var converted = new EditorFloat3 {Value = vector};
			return converted;
		}
		
		public static implicit operator Vector3(EditorFloat3 value)
		{
			return value == null ? Vector3.zero : value.Value;
		}
		
		public Vector3 Value{ 
			get{ return _float3Value; }
			set{ _float3Value = value; }
		}
		
		//Add serialization to the vector
		[DataMember] public float X
		{
			get{ return _float3Value.x; }
			set{ _float3Value.x = value; }
		}
		
		[DataMember] public float Y
		{
			get{ return _float3Value.y; }
			set{ _float3Value.y = value; }
		}
		
		[DataMember] public float Z
		{
			get{ return _float3Value.z; }
			set{ _float3Value.z = value; }
		}
	}
}
