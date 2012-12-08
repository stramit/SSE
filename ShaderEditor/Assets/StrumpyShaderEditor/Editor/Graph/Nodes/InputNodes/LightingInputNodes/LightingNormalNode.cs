using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Normal Input", "Lighting", typeof(LightingNormalNode),"Normals from the pixel shader. In deferred mode these are normals from the normal buffer, which may have be compressed depending on the platform, so accuracy is not ensured. Since light color already considers this, there isn't any real need to solve it yourself." )]
	public class LightingNormalNode : LightingNode{
		private const string NodeName = "LightingNormalNode";
		
		[DataMember] private Float4OutputChannel _normal;
		
		public LightingNormalNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_normal = _normal ?? new Float4OutputChannel( 0, "Normal" );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> { _normal };
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
			return "float4( s.Normal.x, s.Normal.y, s.Normal.z, 1.0 )";
		}
	}
}
