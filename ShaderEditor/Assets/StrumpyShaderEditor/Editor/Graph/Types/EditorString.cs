using System;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class EditorString : IUnityeditorDrawableType {
		[DataMember] private string _stringValue = "";
		
		public string Value
		{ 
			get{ return _stringValue;}
			set{ _stringValue = value;}
		}
		
		public static implicit operator EditorString(string value)
		{
			var converted = new EditorString {Value = value};
			return converted;
		}
		
		public static implicit operator string(EditorString value)
		{
			return value == null ? "" : value.Value;
		}
		
		[Obsolete("Refactored to _enabled, will be removed next release")]
		[DataMember] private string stringValue
		{
			get{ return _stringValue; }
			set{ _stringValue = value; }
		}
	}
}
