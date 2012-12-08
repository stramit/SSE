using UnityEngine;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class EditorMatrix : IUnityeditorDrawableType {
		private Matrix4x4 _value = Matrix4x4.identity;
	
		public static implicit operator EditorMatrix(Matrix4x4 matrix)
		{
			return new EditorMatrix {_value = matrix};
		}
		
		public static implicit operator Matrix4x4(EditorMatrix value)
		{
			return value == null ? Matrix4x4.identity : value.Value;
		}
		
		public Matrix4x4 Value{ 
			get{ return _value; }
			set{ _value = value; }
		}
		
		//Add serialization to the matrix
		[DataMember] public float m00
		{
			get{ return _value.m00; }
			set{ _value.m00 = value; }
		}
		
		[DataMember] public float m01
		{
			get{ return _value.m01; }
			set{ _value.m01 = value; }
		}
		
		[DataMember] public float m02
		{
			get{ return _value.m02; }
			set{ _value.m02 = value; }
		}
		
		[DataMember] public float m03
		{
			get{ return _value.m03; }
			set{ _value.m03 = value; }
		}
		
		[DataMember] public float m10
		{
			get{ return _value.m10; }
			set{ _value.m10 = value; }
		}
		[DataMember] public float m11
		{
			get{ return _value.m11; }
			set{ _value.m11 = value; }
		}
		[DataMember] public float m12
		{
			get{ return _value.m12; }
			set{ _value.m12 = value; }
		}
		[DataMember] public float m13
		{
			get{ return _value.m13; }
			set{ _value.m13 = value; }
		}
		[DataMember] public float m20
		{
			get{ return _value.m20; }
			set{ _value.m20 = value; }
		}
		[DataMember] public float m21
		{
			get{ return _value.m21; }
			set{ _value.m21 = value; }
		}
		[DataMember] public float m22
		{
			get{ return _value.m22; }
			set{ _value.m22 = value; }
		}
		[DataMember] public float m23
		{
			get{ return _value.m23; }
			set{ _value.m23 = value; }
		}
		[DataMember] public float m30
		{
			get{ return _value.m30; }
			set{ _value.m30 = value; }
		}
		[DataMember] public float m31
		{
			get{ return _value.m31; }
			set{ _value.m31 = value; }
		}
		[DataMember] public float m32
		{
			get{ return _value.m32; }
			set{ _value.m32 = value; }
		}
		[DataMember] public float m33
		{
			get{ return _value.m33; }
			set{ _value.m33 = value; }
		}
	}
}
