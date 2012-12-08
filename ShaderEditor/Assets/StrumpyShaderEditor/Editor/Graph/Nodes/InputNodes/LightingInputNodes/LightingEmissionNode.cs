using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Emission Input", "Lighting", typeof(LightingEmissionNode),"Emission from the surface program. This is already applied and handled seperately by Unity, but it may be useful depending on what your doing. More then likely you can ignore this.")]
	public class LightingEmissionNode : LightingNode{
		private const string NodeName = "LightingEmissionNode";
		
		[DataMember] private Float4OutputChannel _emissionColor;
		
		public LightingEmissionNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_emissionColor = _emissionColor ?? new Float4OutputChannel( 0, "Color" );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> { _emissionColor };
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
			return "float4( s.Emission.x, s.Emission.y, s.Emission.z, 1.0 )";
		}
	}
}
