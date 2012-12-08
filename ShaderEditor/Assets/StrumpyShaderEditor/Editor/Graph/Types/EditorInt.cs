using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class EditorInt : IUnityeditorDrawableType{
		[DataMember] private int _intValue;
		
		public static implicit operator EditorInt(int value)
		{
			var converted = new EditorInt {Value = value};
			return converted;
		}
		
		public static implicit operator int(EditorInt value)
		{
			return value == null ? 0 : value.Value;
		}
		
		public int Value{ 
			get{ return _intValue; }
			set{ _intValue = value; }
		}
	}
}
