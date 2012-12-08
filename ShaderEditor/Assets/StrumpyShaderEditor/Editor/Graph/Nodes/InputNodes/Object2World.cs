using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Object2World", "Input", typeof(Object2World),"Object to World matrix, used for translation into local space. Only valid in the vertex graph, to be used with MxM, VxM, and MxV")]
	public class Object2World : Node{
		private const string NodeName = "Object2World";
		
		[DataMember] private MatrixOutputChannel _matrix;
		
		public Object2World( )
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
			return "_Object2World";
		}
		
	}
}
