using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Full Mesh UV2", "Input", typeof(FullMeshUV2),"Raw UV2 channels from the mesh (Full components). Generally you should use the given UV with a sampler, to allow the user to specify a scale/offset for the texture, but using this is slightly more optimal, and if you recieve a warning about texture interpolators, you should switch to reading from this. Largely used for procedural texture generation.")]
	public class FullMeshUV2 : Node, IStructInput{
		private const string NodeName = "FullMeshUV2";
		[DataMember] private Float4OutputChannel _uv2;
		
		public FullMeshUV2( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_uv2 = _uv2 ?? new Float4OutputChannel(0, "UV2");
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
			return new List<OutputChannel> { _uv2 };
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
			return "fullMeshUV2";
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
			return "o." + GetStructFieldName() + " = v.texcoord1;\n";
		}
		
		public bool RequiresStructFieldInclusion()
		{
			return IsOutputChannelConnected(0);
		}
	}
}
