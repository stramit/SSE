using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Custom Input", "Lighting", typeof(LightingCustomNode),"Custom Input from the pixel graph")]
	public class LightingCustomNode : LightingNode{
		private const string NodeName = "LightingCustomNode";
		
		[DataMember] private Float4OutputChannel _customColor;
		
		public LightingCustomNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_customColor = _customColor ?? new Float4OutputChannel( 0, "Custom" );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> { _customColor };
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
			return "s.Custom";
		}
	}
}
