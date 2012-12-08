using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Vertex Normal", "Vertex", typeof(VertexNormalNode))]
	public class VertexNormalNode : VertexNode{
		private const string NodeName = "VertexNormal";
		
		[DataMember] private Float4OutputChannel _vertexNormal;
		
		public VertexNormalNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_vertexNormal = _vertexNormal ?? new Float4OutputChannel( 0, "Normal" );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> { _vertexNormal };
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
			return "float4( v.normal.x, v.normal.y, v.normal.z, 1.0 )";
		}
	}
}
