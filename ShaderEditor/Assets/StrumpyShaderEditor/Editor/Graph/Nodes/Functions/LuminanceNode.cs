using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Luminance", "Function", typeof(LuminanceNode),"Get the luminace of the given vector: dot( input, half3(0.22, 0.707, 0.071) )")]
	public class LuminanceNode : Node, IResultCacheNode {
		private const string NodeName = "Luminance";
		
		[DataMember] private Float4OutputChannel _result;
		[DataMember] private Float4InputChannel _input;
		
		public LuminanceNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_result = _result ?? new Float4OutputChannel( 0, "Result" );
			_input = _input ?? new Float4InputChannel( 0, "Input", Vector4.zero );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_result};
			return ret;
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			var ret = new List<InputChannel> {_input};
			return ret;
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public string GetAdditionalFields()
		{
			return _input.ChannelInput( this ).AdditionalFields;
		}
		
		public string GetUsage()
		{
			var arg1Input = _input.ChannelInput( this );
			
			string result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "= Luminance( ";
			result += arg1Input.QueryResult + ".xyz ).xxxx;\n";
			return result;
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return UniqueNodeIdentifier;
		}
	}
}