using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class EditorGroup : IUnityeditorDrawableType {
		[DataMember] private int _selected;
		[DataMember] private List<string> _gridValues = new List<string>();
		[DataMember] private int _guiRowElementsNum;
		
		public int Value
		{
			get{ return _selected; }
			set{ _selected = value; }
		}
		
		public int GuiRowElementsNum
		{
			get{ return _guiRowElementsNum; }
			set{ _guiRowElementsNum = value; }
		}
		
		public IEnumerable<string> GridValues
		{
			get{ return _gridValues; }
		}
		
		public IEnumerable<int> GridInts
		{
			get{ 
				var ints = new List<int>();
				for( var i = 0; i < _gridValues.Count; i++ )
				{
					ints.Add( i );
				}
				return ints;
			}
		}
		
		public string Selected
		{
			get{ return _gridValues[Value]; }
		}
		
		public EditorGroup( int defaultSelection, IEnumerable<string> values, int guiRowElementsNum )
		{
			Value = defaultSelection;
			_gridValues.AddRange( values ); 
			GuiRowElementsNum = guiRowElementsNum;
		}
	}
}
