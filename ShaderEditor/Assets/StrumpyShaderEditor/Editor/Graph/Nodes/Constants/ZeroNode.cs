using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Zero", "Constant", typeof(ZeroNode),"Constant Value (0,0,0,0)")]
	public class ZeroNode : Node{
		private const string NodeName = "Zero";
		[DataMember] private Float4OutputChannel _constValue;
		
		public ZeroNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_constValue = _constValue ?? new Float4OutputChannel( 0, "Zero" );
			_constValue.DisplayName = "Zero";
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
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
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return "float4( 0.0, 0.0, 0.0, 0.0 )";
		}
	}
}