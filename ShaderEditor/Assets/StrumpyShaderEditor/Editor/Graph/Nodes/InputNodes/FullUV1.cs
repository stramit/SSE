using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Full Mesh UV1", "Input", typeof(FullMeshUV1),"Raw UV1 channels from the mesh (Full components). Generally you should use the given UV with a sampler, to allow the user to specify a scale/offset for the texture, but using this is slightly more optimal, and if you recieve a warning about texture interpolators, you should switch to reading from this. Largely used for procedural texture generation.")]
	public class FullMeshUV1 : Node, IStructInput{
		private const string NodeName = "FullMeshUV1";
		[DataMember] private Float4OutputChannel _uv1;
		
		public FullMeshUV1( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_uv1 = _uv1 ?? new Float4OutputChannel(0, "UV1");
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
			return new List<OutputChannel> { _uv1 };
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
			return "(IN." + GetStructFieldName() + ")";
		}
		
		public string GetStructFieldName()
		{
			return "fullMeshUV1";
		}
		
		public string GetStructFieldDefinition()
		{
			return GetStructFieldType().ShaderTypeString() + " " + GetStructFieldName() + ";\n";
		}
		
		public StructTypes GetStructFieldType()
		{
			return StructTypes.Float4;
		}

		public string GetStructVertexShaderString()
		{
			return "o." + GetStructFieldName() + " = v.texcoord;\n";
		}
		
		public bool RequiresStructFieldInclusion()
		{
			return IsOutputChannelConnected( 0 );
		}
	}
}
