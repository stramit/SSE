using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Dot", "Operation", typeof(DotNode),"Geometric Dot product. That is, each component is multiplied by component in the other vector, and then the results are all added. Taking the dotproduct of 0.25 averages the four components, 1/3 averages for only three. Normalized dotproducts are quite common, two normalized vectors will give 1 when aligned, 0 when perpendicular, and -1 when opposite. Used to find angles between things, notably in lighting functions and internally in Frensel.")]
	public class DotNode : Node, IResultCacheNode {
		private const string NodeName = "Dot";
		
		[DataMember] private EditorGroup _channels;
		
		[DataMember] private Float4OutputChannel _result;
		[DataMember] private Float4InputChannel _vector1;
		[DataMember] private Float4InputChannel _vector2;
		
		public DotNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_result = _result ?? new Float4OutputChannel( 0, "Result" );
			_vector1 = _vector1 ?? new Float4InputChannel( 0, "Vector1", Vector4.zero );
			_vector2 = _vector2 ?? new Float4InputChannel( 1, "Vector2", Vector4.zero );
			_channels = _channels ?? new EditorGroup( 1, new[] { "xy", "xyz", "xyzw" }, 3 );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_result};
		    return ret;
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			var ret = new List<InputChannel> {_vector1, _vector2};
		    return ret;
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public string GetAdditionalFields()
		{
			var arg1Input = _vector1.ChannelInput( this );
			var arg2Input = _vector2.ChannelInput( this );
			var ret = arg1Input.AdditionalFields;
			ret += arg2Input.AdditionalFields;
			return ret;
		}
		
		public string GetUsage()
		{
			var arg1Input = _vector1.ChannelInput( this );
			var arg2Input = _vector2.ChannelInput( this );
			
			string result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=";
			result += "dot( ";
			result += arg1Input.QueryResult + "." + _channels.Selected + ", ";
			result += arg2Input.QueryResult + "." + _channels.Selected + " ).xxxx;\n";
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
		
			GUILayout.Label( "Dot Channels" );
			_channels.Value = GUILayout.SelectionGrid( _channels.Value, _channels.GridValues.ToArray(), _channels.GuiRowElementsNum );
		}
	}
}