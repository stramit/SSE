using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Light Color", "Lighting", typeof(LightColorNode),"Light color. For Simple lighting (Which is a prepass shader, to work with deferred), the alpha component contains specular. This is already solved using normals from the pixel shader.")]
	public class LightColorNode : LightingNode{
		private const string NodeName = "LightColorNode";
		
		[DataMember] private Float4OutputChannel _lightColor;
		
		public LightColorNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_lightColor = _lightColor ?? new Float4OutputChannel( 0, "Color" );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> { _lightColor };
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
			return "light";
		}
	}
}
