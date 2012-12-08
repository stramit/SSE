using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Vertex Color", "Input", typeof(VertexColorNode),"Color encoded onto the mesh. Interpolated between the nearest vertices, this is very cheap to use, as it does not require reading from textures.")]
	public class VertexColorNode : Node, IStructInput{
		private const string NodeName = "VertexColor";
		
		[DataMember] private Float4OutputChannel _color;
		
		public VertexColorNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_color = _color ?? new Float4OutputChannel( 0, "Color" );
		}
		
		public override IEnumerable<string> IsValid ( SubGraphType graphType )
		{
			var errors = new List<string> ();
			if( graphType != SubGraphType.Pixel )
			{
				errors.Add( "Node not valid in graph type: " + graphType );
			}
			return errors;
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_color};
			return ret;
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			return new List<InputChannel>();
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public StructTypes GetStructFieldType()
		{
			return StructTypes.Float4;
		}
		
		public string GetStructFieldName()
		{
			return "color";
		}
		
		public string GetStructFieldDefinition()
		{
			return GetStructFieldType().ShaderTypeString() + " " + GetStructFieldName() + " : COLOR;\n";
		}

		public string GetStructVertexShaderString()
		{
			return "";
		}

		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return "IN." + GetStructFieldName();
		}
		
		public bool RequiresStructFieldInclusion()
		{
			return IsOutputChannelConnected( 0 );
		}
	}
}
