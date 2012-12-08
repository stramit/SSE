using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Cross", "Operation", typeof(CrossNode),"Geometric Cross product. Returns a vector perpendicular to two inputs, used internally for binormal generation in bumpmapping. Be aware of potential four channel mishaps. This function is primarily for use in vertex shaders, you will likely only use this one if you are already familiar with it.")]
	public class CrossNode : Node, IResultCacheNode {
		private const string NodeName = "Cross";
		
		[DataMember] private Float4OutputChannel _result;
		[DataMember] private Float4InputChannel _vector1;
		[DataMember] private Float4InputChannel _vector2;
		
		public CrossNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_result = _result ?? new Float4OutputChannel( 0, "Result" );
			_vector1 = _vector1 ?? new Float4InputChannel( 0, "Vector1", Vector4.zero );
			_vector2 = _vector2 ?? new Float4InputChannel( 1, "Vector2", Vector4.zero );
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
			var arg1 = _vector1.ChannelInput( this );
			var arg2 = _vector2.ChannelInput( this );
			var ret = arg1.AdditionalFields;
			ret += arg2.AdditionalFields;
			return ret;
		}
		
		public string GetUsage()
		{
			var arg1 = _vector1.ChannelInput( this );
			var arg2 = _vector2.ChannelInput( this );
			var result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=";
			result += "float4( cross( ";
			result += arg1.QueryResult + ".xyz, ";
			result += arg2.QueryResult + ".xyz ), 1.0 );\n";
			return result;
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return UniqueNodeIdentifier;
		}
	}
}