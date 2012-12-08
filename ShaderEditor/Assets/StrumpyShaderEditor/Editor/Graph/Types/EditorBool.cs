using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class EditorBool : IUnityeditorDrawableType {
		[DataMember] private bool _enabled;
		
		public static implicit operator EditorBool(bool value)
		{
			var converted = new EditorBool {Value = value};
			return converted;
		}
		
		public static implicit operator bool(EditorBool value)
		{
			return value == null ? false : value.Value;
		}
		
		public bool Value{ 
			get{ return _enabled; }
			set{ _enabled = value; } 
		}
		
		public EditorBool( )
		{
			Value = false;
		}
	}
}
