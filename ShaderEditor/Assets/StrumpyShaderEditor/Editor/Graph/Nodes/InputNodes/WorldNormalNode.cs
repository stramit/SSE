using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("World Normal", "Input", typeof(WorldNormalNode),"Normal of the surface in world space, used for collecting snow, faking light with cubemaps, and numerous other effects. Compution is merged into the vertex shader, no need to compute it on your own.")]
	public class WorldNormalNode : Node, IStructInput{
		private const string NodeName = "WorldNormal";
		
		[DataMember] private Float4OutputChannel _worldNormal;
		
		public WorldNormalNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_worldNormal = _worldNormal ?? new Float4OutputChannel( 0, "WorldNormal" );
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
			return new List<OutputChannel> { _worldNormal };
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			return new List<InputChannel>();
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override bool RequiresInternalData
		{
			get{ return false; }
		}
			
		public StructTypes GetStructFieldType()
		{
			return StructTypes.Float3;
		}
		
		public string GetStructFieldName()
		{
			return "sWorldNormal";
		}
		
		public string GetStructFieldDefinition()
		{
			return GetStructFieldType().ShaderTypeString() + " " + GetStructFieldName() + ";\n";
		}
		
		public string GetStructVertexShaderString()
		{
			return "o.sWorldNormal = mul((float3x3)_Object2World, SCALED_NORMAL);\n";
		}
		
/*		public string GetAdditionalFields( ShaderGraph owner )
		{
			var arg1 = _surfaceNormal.ChannelInput( this, owner );
			return arg1.AdditionalFields;
		}
		
		public string GetUsage( ShaderGraph owner )
		{
			var arg1 = _surfaceNormal.ChannelInput( this, owner );
			
			var result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=";
			result += "float4( normalize( normalize( IN.worldViewDir ) + normalize( WorldReflectionVector (IN, " + arg1.QueryResult + "))), 1.0);\n";
			return result;
		}
		*/
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return this.GenerateInputUsageString();
		}
		
		
		public bool RequiresStructFieldInclusion()
		{
			return IsOutputChannelConnected( 0 );
		}
	}
}
