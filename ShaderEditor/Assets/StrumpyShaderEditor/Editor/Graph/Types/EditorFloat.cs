using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class EditorFloat : IUnityeditorDrawableType{
		[DataMember] private float _floatValue;
		
		public static implicit operator EditorFloat(float value)
		{
			var converted = new EditorFloat {Value = value};
			return converted;
		}
		
		public static implicit operator float(EditorFloat value)
		{
			return value == null ? 0.0f : value.Value;
		}
		
		public float Value{ 
			get{ return _floatValue; }
			set{ _floatValue = value; }
		}
	}
}
