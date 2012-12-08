using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Vertex In Color", "Vertex", typeof(VertexInColorNode))]
	public class VertexInColorNode : VertexNode{
		private const string NodeName = "VertexInColorNode";
		
		[DataMember] private Float4OutputChannel _vertexColor;
		
		public VertexInColorNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_vertexColor = _vertexColor ?? new Float4OutputChannel( 0, "Color" );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> { _vertexColor };
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
			return "v.color";
		}
	}
}
