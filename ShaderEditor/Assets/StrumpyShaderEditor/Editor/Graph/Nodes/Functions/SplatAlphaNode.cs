using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("SplatAlpha", "Function", typeof(SplatAlphaNode),"Similar to Splat, only it copies just the Alpha (W) component across four channels.")]
	public class SplatAlphaNode : Node, IResultCacheNode {
		private const string NodeName = "SplatAlpha";
		
		[DataMember] private Float4OutputChannel _result; 
		[DataMember] private Float4InputChannel _vector;
		
		public SplatAlphaNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_result = _result ?? new Float4OutputChannel( 0, "Result" );
			_vector = _vector ?? new Float4InputChannel( 0, "Vector", Vector4.zero );
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
			return new List<InputChannel> {_vector};
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
			result += arg1.QueryResult + ".w;\n";
			return result;
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return UniqueNodeIdentifier;
		}
	}
}