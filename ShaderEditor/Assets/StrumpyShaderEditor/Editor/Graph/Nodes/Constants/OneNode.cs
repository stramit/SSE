using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("One", "Constant", typeof(OneNode),"Constant value (1,1,1,1)")]
	public class OneNode : Node{
		private const string NodeName = "One";
		[DataMember] private Float4OutputChannel _constValue;
		
		public OneNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_constValue = _constValue ?? new Float4OutputChannel( 0, "One" );
			_constValue.DisplayName = "One";
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_constValue};
		    return ret;
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			return new List<InputChannel>();
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return "float4( 1.0, 1.0, 1.0, 1.0 )";
		}
	}
}