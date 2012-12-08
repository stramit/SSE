using UnityEngine;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class EditorFloat4 : IUnityeditorDrawableType {
		private Vector4 _float4Value = Vector4.zero;
	
		public static implicit operator EditorFloat4(Vector4 vector)
		{
			var converted = new EditorFloat4 {Value = vector};
			return converted;
		}
		
		public static implicit operator Vector4(EditorFloat4 value)
		{
			return value == null ? Vector4.zero : value.Value;
		}
		
		public Vector4 Value{ 
			get{ return _float4Value; }
			set{ _float4Value = value; }
		}
		
		//Add serialization to the vector
		[DataMember] public float X
		{
			get{ return _float4Value.x; }
			set{ _float4Value.x = value; }
		}
		
		[DataMember] public float Y
		{
			get{ return _float4Value.y; }
			set{ _float4Value.y = value; }
		}
		
		[DataMember] public float Z
		{
			get{ return _float4Value.z; }
			set{ _float4Value.z = value; }
		}
		
		[DataMember] public float W
		{
			get{ return _float4Value.w; }
			set{ _float4Value.w = value; }
		}
	}
}
