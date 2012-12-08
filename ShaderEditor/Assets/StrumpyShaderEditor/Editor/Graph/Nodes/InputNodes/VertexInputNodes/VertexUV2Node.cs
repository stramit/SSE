using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Vertex UV2", "Vertex", typeof(VertexUV2Node))]
	public class VertexUV2Node : VertexNode{
		private const string NodeName = "VertexUV2";
		
		[DataMember] private Float4OutputChannel _vertexUV2;
		
		public VertexUV2Node( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_vertexUV2 = _vertexUV2 ?? new Float4OutputChannel( 0, "UV2" );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> { _vertexUV2 };
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
			return "v.texcoord1";
		}
	}
}
