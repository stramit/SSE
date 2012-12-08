using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Vertex Position", "Vertex", typeof(VertexPositionNode))]
	public class VertexPositionNode : VertexNode{
		private const string NodeName = "VertexPosition";
		
		[DataMember] private Float4OutputChannel _vertexPosition;
		
		public VertexPositionNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_vertexPosition = _vertexPosition ?? new Float4OutputChannel( 0, "Position" );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> { _vertexPosition };
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
			return "v.vertex";
		}
	}
}
