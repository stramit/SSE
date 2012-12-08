using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Vertex UV", "Vertex", typeof(VertexUVNode))]
	public class VertexUVNode : VertexNode{
		private const string NodeName = "VertexUV";
		
		[DataMember] private Float4OutputChannel _vertexUV;
		
		public VertexUVNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_vertexUV = _vertexUV ?? new Float4OutputChannel( 0, "UV" );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> { _vertexUV };
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
			return "v.texcoord";
		}
	}
}
