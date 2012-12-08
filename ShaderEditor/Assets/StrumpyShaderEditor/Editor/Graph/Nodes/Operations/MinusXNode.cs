using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("-v", "Operation", typeof(MinusX),"Return the negative of the input. Shorthand for multiplying by minus one, however the compiler will combine this operation where applicable.")]
	public class MinusX : Node, IResultCacheNode {
		private const string NodeName = "Negative";
		
		[DataMember] private Float4OutputChannel _result;
		[DataMember] private Float4InputChannel _in;
		
		public MinusX()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_result = _result ?? new Float4OutputChannel( 0, "-arg" );
			_result.DisplayName = "-arg";
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
			result += " -" + arg1.QueryResult + "; \n ";
			return result;
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return UniqueNodeIdentifier;
		}
	}
}