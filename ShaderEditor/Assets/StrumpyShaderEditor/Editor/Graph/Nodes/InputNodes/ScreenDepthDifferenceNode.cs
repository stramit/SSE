using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Depth Diff", "Input", typeof(ScreenDepthDifferenceNode),"Distance, in world units, between the front of this surface and the world behind it. Only works in Pro, when using deferred, or with the enable depth effects script. Used for fog and advanced refraction, primarily.")]
		public class ScreenDepthDifferenceNode : Node, IResultCacheNode, IStructInput {
		private const string NodeName = "ScreenDepthDiff";
		
		[DataMember] private Float4OutputChannel _depth;
		public ScreenDepthDifferenceNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_depth = _depth ?? new Float4OutputChannel(0, "Difference");
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
			var ret = new List<OutputChannel> {_depth };
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

		public string GetAdditionalFields()
		{
			return "";
		}

		public string GetUsage()
		{
			var result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=";
			result += " LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos)).r) - IN.screenPos.z;\n";
			return result;
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return UniqueNodeIdentifier;
		}
		
		public override bool RequiresSceneDepth {
			get { return true; }
		}
		
		public StructTypes GetStructFieldType()
		{
			return StructTypes.Float4;
		}
		
		public string GetStructFieldName()
		{
			return "screenPos";
		}
		
		public string GetStructFieldDefinition()
		{
			return GetStructFieldType().ShaderTypeString() + " " + GetStructFieldName() + ";\n";
		}

		public string GetStructVertexShaderString()
		{
			return "";
		}
		
		public bool RequiresStructFieldInclusion()
		{
			return IsOutputChannelConnected( 0 );
		}
	}
}
