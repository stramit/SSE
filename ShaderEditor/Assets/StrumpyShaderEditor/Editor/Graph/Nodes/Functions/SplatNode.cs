using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Splat", "Function", typeof(SplatNode),"Copies the desired channel across the entire output, so if you wanted to create blend mask (For Lerp) using the red component of an image, Splat it's X channel. Splat Alpha is a specialized version equivalent to Swizzle WWWW or Splat W")]
	public class SplatNode : Node, IResultCacheNode {
		private const string NodeName = "Splat";
		[DataMember] private EditorGroup _splatSelection;
		[DataMember] private Float4OutputChannel _result;
		[DataMember] private Float4InputChannel _vector;
		
		public SplatNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_splatSelection = _splatSelection ?? new EditorGroup( 0, new[] { "x", "y", "z", "w" }, 4 );
			_result = _result ?? new Float4OutputChannel( 0, "Result" );
			_vector = _vector ?? new Float4InputChannel( 1, "Vector", Vector4.zero );
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_result};
		    return ret;
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			var ret = new List<InputChannel> {_vector};
		    return ret;
		}
		
		public string GetAdditionalFields()
		{
			var arg1 = _vector.ChannelInput( this );
			return arg1.AdditionalFields;
		}
		
		public string GetUsage()
		{
			var arg1 = _vector.ChannelInput( this );
			var result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=";
			result += arg1.QueryResult + "." + _splatSelection.Selected + ";\n";
			return result;
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return UniqueNodeIdentifier;
		}
		
		public override void DrawProperties()
		{
			base.DrawProperties();
		
			GUILayout.Label( "Splat Channel" );
			_splatSelection.Value = GUILayout.SelectionGrid( _splatSelection.Value, _splatSelection.GridValues.ToArray(), _splatSelection.GuiRowElementsNum );
		}
	}
}