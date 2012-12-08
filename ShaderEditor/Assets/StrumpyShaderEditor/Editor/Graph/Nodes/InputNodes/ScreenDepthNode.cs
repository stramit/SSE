using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Screen Depth", "Input", typeof(ScreenDepthNode),"Depth of Scene behind object. Distance from camera, use the Z component of Screen Position")]
	public class ScreenDepthNode : Node, IResultCacheNode, IStructInput{
		private const string NodeName = "ScreenDepth";
		
		[DataMember] private Float4OutputChannel _depth;
		public ScreenDepthNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_depth = _depth ?? new Float4OutputChannel(0, "Depth");
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
			string result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=";
			result += " LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD( IN." + GetStructFieldName() + ")).r);\n";
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
