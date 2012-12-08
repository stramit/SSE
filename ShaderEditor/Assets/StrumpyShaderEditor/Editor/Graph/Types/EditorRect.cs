using UnityEngine;
using System;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class EditorRect : IUnityeditorDrawableType {
		private Rect _rectValue;
	
		public EditorRect( float left, float top, float width, float height )
		{
			_rectValue = new Rect( left, top, width, height );
		}
		
		public EditorRect( Rect source )
		{
			_rectValue = new Rect( source.x, source.y, source.width, source.height );
		}
		
		public static implicit operator EditorRect(Rect rect)
		{
			return new EditorRect( rect );
		}
		
		public static implicit operator Rect(EditorRect rect)
		{
			return rect == null ? new Rect( 0f, 0f, 10f, 10f ) : rect.Value;
		}
		
		public Rect Value{ 
			get{ return _rectValue; }
			set{ _rectValue = value; }
		}
		
		public bool Contains( Vector2 point )
		{
			return _rectValue.Contains( point );
		}
		
		//Add serialization to the rect
		[DataMember] public float X
		{
			get{ return _rectValue.x; }
			set{ _rectValue.x = value; }
		}
		
		[DataMember] public float Y
		{
			get{ return _rectValue.y; }
			set{ _rectValue.y = value; }
		}
		
		[DataMember] public float Width
		{
			get{ return _rectValue.width; }
			set{ _rectValue.width = value; }
		}
		
		[DataMember] public float Height
		{
			get{ return _rectValue.height; }
			set{ _rectValue.height = value; }
		}
		
		//To be removed next release
		[Obsolete("Refactored to X, will be removed next release")]
		[DataMember] public float x
		{
			get{ return _rectValue.x; }
			set{ _rectValue.x = value; }
		}
		
		[Obsolete("Refactored to Y, will be removed next release")]
		[DataMember] public float y
		{
			get{ return _rectValue.y; }
			set{ _rectValue.y = value; }
		}
		
		[Obsolete("Refactored to width, will be removed next release")]
		[DataMember] public float width
		{
			get{ return _rectValue.width; }
			set{ _rectValue.width = value; }
		}
		[Obsolete("Refactored to height, will be removed next release")]
		[DataMember] public float height
		{
			get{ return _rectValue.height; }
			set{ _rectValue.height = value; }
		}
	}
}
