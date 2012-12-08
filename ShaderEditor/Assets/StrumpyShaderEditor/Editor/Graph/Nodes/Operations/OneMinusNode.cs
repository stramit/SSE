using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("1-v", "Operation", typeof(OneMinus),"Invert via the one minus approach, Used to find the inverse for numbers in the [0,1] range, in particular textures. Used often with blending. To sanitize input, combine with saturate. Shorthand for subtract, no performance difference over simply subtracting.")]
	public class OneMinus : Node, IResultCacheNode {
		private const string NodeName = "Invert";
		
		[DataMember] private Float4OutputChannel _result;
		[DataMember] private Float4InputChannel _in;
		
		public OneMinus()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_result = _result ?? new Float4OutputChannel( 0, "1-v" );
			_result.DisplayName = "1-v";
			_in = _in ?? new Float4InputChannel( 0, "arg", Vector4.zero );
			_in.DisplayName = "arg";
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
			var ret = new List<InputChannel> {_in };
			return ret;
		}
		
		public string GetAdditionalFields()
		{
			var arg1 = _in.ChannelInput( this );
			var ret = arg1.AdditionalFields;
			//ret += arg2.AdditionalFields;
			return ret;
		}
		
		public string GetUsage()
		{
			var arg1 = _in.ChannelInput( this );
			var result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=";
			result += " float4(1.0, 1.0, 1.0, 1.0) - " + arg1.QueryResult + ";\n";
			return result;
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return UniqueNodeIdentifier;
		}
	}
}