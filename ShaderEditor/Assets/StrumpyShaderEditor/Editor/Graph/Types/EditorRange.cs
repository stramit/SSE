using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class EditorRange : IUnityeditorDrawableType{
		[DataMember] private float _min;
		[DataMember] private float _max;
		[DataMember] private float _floatValue;
		
		public EditorRange()
		{
			_min = 0f;
			_max = 1f;
			_floatValue = 0.5f;
		}
		
		public EditorRange (float _min, float _max, float _floatValue)
		{
			this._min = _min;
			this._max = _max;
			this._floatValue = _floatValue;
		}
		
		public float Value{ 
			get{ return _floatValue; }
			set{ _floatValue = value; }
		}
		
		public float Min{ 
			get{ return _min; }
			set{ _min = value; }
		}
		
		public float Max{ 
			get{ return _max; }
			set{ _max = value; }
		}
	}
}
