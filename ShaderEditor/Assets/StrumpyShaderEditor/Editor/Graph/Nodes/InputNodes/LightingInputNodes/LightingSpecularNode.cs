using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Specular Input", "Lighting", typeof(LightingSpecularNode),"Specularity passed from the pixel shader. To be compliant with Unity's default behaviors, this is actually a roughness value. The basic lighting models use an exponent of viewDot^spec*128, which is twice the exponent most programs use for roughness. By this scale, 1.0 is a mirror, 0.9 is sharp reflective metal and glazed ceramic, 0.6 is non-sharp reflective metals (like Apple's Aluminum), 0.5 is like a waxed apple, 0.4-0.3 is most plastics, and 0.1 is concrete." )]
	public class LightingSpecularNode : LightingNode{
		private const string NodeName = "LightingSpecularNode";
		
		[DataMember] private Float4OutputChannel _specular;
		
		public LightingSpecularNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_specular = _specular ?? new Float4OutputChannel( 0, "Specular" );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> { _specular };
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
			return "s.Specular.xxxx";
		}
	}
}
