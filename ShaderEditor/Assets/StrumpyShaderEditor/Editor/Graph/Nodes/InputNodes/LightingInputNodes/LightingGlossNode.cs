using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Gloss Input", "Lighting", typeof(LightingGlossNode),"Gloss from pixel shader. Ensured to be atleast of half3, which is contrast to Unity's standard setting of having gloss of half. To be consistent with broken Unity behaviors color is stored in gloss, where specular is the exponent proper.")]
	public class LightingGlossNode : LightingNode{
		private const string NodeName = "LightingGlossNode";
		
		[DataMember] private Float4OutputChannel _glossColor;
		
		public LightingGlossNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_glossColor = _glossColor ?? new Float4OutputChannel( 0, "Color" );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> { _glossColor };
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
			return "float4( s.Gloss.x, s.Gloss.y, s.Gloss.z, 1.0 )";
		}
	}
}
