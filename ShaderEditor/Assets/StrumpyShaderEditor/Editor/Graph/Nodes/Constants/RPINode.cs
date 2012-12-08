using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("1 over PI", "Constant", typeof(RPINode),"0.3183098862 - Used to scale returns/inputs for the trig functions, extremely useful for putting them to the [0,1] (ACos) and [-.5,.5] ranges (ASin), letting you take advantage of the special ramps.")]
	public class RPINode : Node{
		private const string NodeName = "1/PI";
		[DataMember] private Float4OutputChannel _constValue;
		
		public RPINode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_constValue = _constValue ?? new Float4OutputChannel( 0, "1/PI" );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> {_constValue};
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
			return "float4( 0.3183098862, 0.3183098862, 0.3183098862, 0.3183098862 )";
		}
	}
}