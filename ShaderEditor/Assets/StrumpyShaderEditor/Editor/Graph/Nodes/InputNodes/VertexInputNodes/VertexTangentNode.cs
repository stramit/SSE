using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Vertex Tangent", "Vertex", typeof(VertexTangentNode))]
	public class VertexTangentNode : VertexNode{
		private const string NodeName = "VertexTangent";
		
		[DataMember] private Float4OutputChannel _vertexTangent;
		
		public VertexTangentNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_vertexTangent = _vertexTangent ?? new Float4OutputChannel( 0, "Tangent" );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> { _vertexTangent };
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
			return "v.tangent";
		}
	}
}
