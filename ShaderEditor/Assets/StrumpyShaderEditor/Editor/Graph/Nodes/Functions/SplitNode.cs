using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Split", "Function", typeof(SplitNode),"Seperate a vector into four vectors, one for each component. Shorthand for splatting each vector, while this is the opposite of Assemble, it is a free operation. Used for splatmaps, see also Splat and SplatAlpha.")]
	public class SplitNode : Node, IResultCacheNode{
		private const string NodeName = "Split";
		
		[DataMember] private Float4OutputChannel _x;
		[DataMember] private Float4OutputChannel _y;
		[DataMember] private Float4OutputChannel _z;
		[DataMember] private Float4OutputChannel _w;
		
		[DataMember] private Float4InputChannel _vector;
		
		public SplitNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			//_rgba = _rgba ?? new Float4OutputChannel( 0, "RGBA" );
			_x = _x ?? new Float4OutputChannel( 0, "X" );
			_y = _y ?? new Float4OutputChannel( 1, "Y" );
			_z = _z ?? new Float4OutputChannel( 2, "Z" );
			_w = _w ?? new Float4OutputChannel( 3, "W" );
		
			_vector = _vector ?? new Float4InputChannel( 0, "Vector", Vector4.zero );
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> {_x, _y, _z, _w};
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			return new List<InputChannel> {_vector};
		}
		
		public string GetAdditionalFields()
		{
			string result = "";
			if( IsInputChannelConnected( 0 ) )
			{
				var v = _vector.ChannelInput( this );
				
				result += v.AdditionalFields;
			}
			return result;
		}
		
		public string GetUsage()
		{
			var vector = _vector.ChannelInput( this );
			
			var result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=" + vector.QueryResult + ";\n";
			return result;
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			
			if( channelId == 0 )
				return "float4( " + UniqueNodeIdentifier + ".x, " + UniqueNodeIdentifier + ".x, "+ UniqueNodeIdentifier + ".x, " + UniqueNodeIdentifier + ".x)";
			if( channelId == 1 )
				return "float4( " + UniqueNodeIdentifier + ".y, " + UniqueNodeIdentifier + ".y, "+ UniqueNodeIdentifier + ".y, " + UniqueNodeIdentifier + ".y)";
			if( channelId == 2 )
				return "float4( " + UniqueNodeIdentifier + ".z, " + UniqueNodeIdentifier + ".z, "+ UniqueNodeIdentifier + ".z, " + UniqueNodeIdentifier + ".z)";
			//if( channelId == 3 )
				return "float4( " + UniqueNodeIdentifier + ".w, " + UniqueNodeIdentifier + ".w, "+ UniqueNodeIdentifier + ".w, " + UniqueNodeIdentifier + ".w)";
		}
	}
}