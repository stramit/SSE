using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Alpha Input", "Lighting", typeof(LightingAlphaNode),"Alpha component passed from the pixel program. This can be useful for passing auxiliary data for opaque shaders, since only the output of the lighting function will contribute to the scene alpha.")]
	public class LightingAlphaNode : LightingNode{
		private const string NodeName = "LightingAlphaNode";
		
		[DataMember] private Float4OutputChannel _alpha;
		
		public LightingAlphaNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_alpha = _alpha ?? new Float4OutputChannel( 0, "Alpha" );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> { _alpha };
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
			return "s.Alpha.xxxx";
		}
	}
}
