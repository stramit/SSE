using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public abstract class Operation : Node, IResultCacheNode {
		[DataMember] private Float4OutputChannel _result;
		[DataMember] private Float4InputChannel _arg1;
		[DataMember] private Float4InputChannel _arg2;

	    protected Operation()
	    {
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_result = _result ?? new Float4OutputChannel( 0, "Result" );
			_arg1 = _arg1 ?? new Float4InputChannel( 0, "Arg1", Vector4.zero );
			_arg2 = _arg2 ?? new Float4InputChannel( 1, "Arg2", Vector4.zero );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_result};
		    return ret;
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			var ret = new List<InputChannel> {_arg1, _arg2};
		    return ret;
		}
	
		public abstract string Operand
		{
			get;
		}
		
		public string GetAdditionalFields()
		{
			var arg1Input = _arg1.ChannelInput( this );
			var arg2Input = _arg2.ChannelInput( this );
			
			var result = arg1Input.AdditionalFields;
			result += arg2Input.AdditionalFields;
			return result;
		}
		
		public string GetUsage()
		{
			var arg1Input = _arg1.ChannelInput( this );
			var arg2Input = _arg2.ChannelInput( this );
			
			var result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=";
			result += arg1Input.QueryResult + Operand + arg2Input.QueryResult + ";\n";
			return result;
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return UniqueNodeIdentifier;
		}
	}
}