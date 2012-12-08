using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Mesh UV", "Input", typeof(MeshUV),"Raw UV1 and UV2 channels from the mesh. Generally you should use the given UV with a sampler, to allow the user to specify a scale/offset for the texture, but using this is slightly more optimal, and if you recieve a warning about texture interpolators, you should switch to reading from this. Largely used for procedural texture generation.")]
	public class MeshUV : Node, IStructInput{
		private const string NodeName = "MeshUV";
		[DataMember] private Float4OutputChannel _uv1;
		[DataMember] private Float4OutputChannel _uv2;
		
		public MeshUV( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_uv1 = _uv1 ?? new Float4OutputChannel(0, "UV1");
			_uv2 = _uv2 ?? new Float4OutputChannel(1, "UV2");
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
			return new List<OutputChannel> { _uv1, _uv2 };
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
			if( channelId == 0 )
			{
				return "(IN." + GetStructFieldName() + ".xyxy)";
			}
			return "(IN." + GetStructFieldName() + ".zwzw)";
		}
		
		public string GetStructFieldName()
		{
			return "meshUV";
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
			var res = "o." + GetStructFieldName() + ".xy = v.texcoord.xy;\n";
			res += "o." + GetStructFieldName() + ".zw = v.texcoord1.xy;\n";
			return res;
		}
		
		public bool RequiresStructFieldInclusion()
		{
			return IsOutputChannelConnected( 0 ) || IsOutputChannelConnected( 1 );
		}
	}
}
