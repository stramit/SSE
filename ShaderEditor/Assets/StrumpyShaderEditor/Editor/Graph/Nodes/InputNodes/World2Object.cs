using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("World2Object", "Input", typeof(World2Object),"A matrix for converting from world space to object space, useful for globally aligned effects, like collecting snow. Only available in the vertex shader, World Normal will suffice most of the time in the pixel shader.")]
	public class World2Object : Node{
		private const string NodeName = "World2Object";
		
		[DataMember] private MatrixOutputChannel _matrix;
		
		public World2Object( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_matrix = _matrix ?? new MatrixOutputChannel( 0, "Matrix" );
		}
		
		public override IEnumerable<string> IsValid ( SubGraphType graphType )
		{
			var errors = new List<string> ();
			if( graphType != SubGraphType.Vertex )
			{
				errors.Add( "Node not valid in graph type: " + graphType );
			}
			return errors;
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> {_matrix};
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
			return "_World2Object";
		}
		
	}
}
