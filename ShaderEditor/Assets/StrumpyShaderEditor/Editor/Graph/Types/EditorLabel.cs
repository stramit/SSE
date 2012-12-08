using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class EditorLabel : IUnityeditorDrawableType {
		[DataMember] private string _label;
		
		public static implicit operator EditorLabel(string label)
		{
			var converted = new EditorLabel {Value = label};
		    return converted;
		}
		
		public static implicit operator string(EditorLabel label)
		{
			return label == null ? "" : label.Value;
		}
		
		public string Value
		{ 
			get{ return _label;}
			set{ _label = value;}
		}
	}
}
