using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Albedo Input", "Lighting", typeof(LightingAlbedoNode),"Albedo Light from the pixel shader. If your using Beast lightmaps, this is already adjusted, otherwise it should match what was produced in the pixel shader graph.")]
	public class LightingAlbedoNode : LightingNode{
		private const string NodeName = "LightingAlbedoNode";
		
		[DataMember] private Float4OutputChannel _albedoColor;
		
		public LightingAlbedoNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_albedoColor = _albedoColor ?? new Float4OutputChannel( 0, "Albedo" );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> { _albedoColor };
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
			return "float4( s.Albedo.x, s.Albedo.y, s.Albedo.z, 1.0 )";
		}
	}
}
