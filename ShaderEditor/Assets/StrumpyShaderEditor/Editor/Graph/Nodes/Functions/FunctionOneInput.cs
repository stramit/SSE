using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public abstract class FunctionOneInput : Node, IFunction {
		[DataMember] protected Float4OutputChannel _result;
		[DataMember] protected Float4InputChannel _arg1;

		protected FunctionOneInput()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_result = _result ?? new Float4OutputChannel( 0, "Result" );
			_arg1 = _arg1 ?? new Float4InputChannel( 0, "Arg1", Vector4.zero );
		}
		
		public abstract string FunctionName
		{
			get;
		}
		
		
		public string GetFunctionDefinition()
		{
			return "";
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_result};
			return ret;
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			var ret = new List<InputChannel> {_arg1};
			return ret;
		}
		
		public string GetAdditionalFields()
		{
			var arg1Input = _arg1.ChannelInput( this );
			return arg1Input.AdditionalFields;
		}
		
		public virtual string GetUsage()
		{
			var arg1Input = _arg1.ChannelInput( this );
			
			string result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=";
			result += FunctionName + "(" + arg1Input.QueryResult + ");\n";
			return result;
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return UniqueNodeIdentifier;
		}
	}
}