using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("ScreenPos[0,1]", "Input", typeof(NormalizedScreenPositionNode),"Normalized Screen position, used as UV input for GrabSampler and a Tex2D node, aswell as for screenspace effects. Contains the UV's for your screen, computing the dotproduct with (0.5,0.5) yields lense vignetting.")]
	public class NormalizedScreenPositionNode : Node, IStructInput{
		private const string NodeName = "NormalizedScreenPosition";
		
		[DataMember] private Float4OutputChannel _position;
		public NormalizedScreenPositionNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_position = _position ?? new Float4OutputChannel(0, "Position");
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
			var ret = new List<OutputChannel> {_position};
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

		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return "((IN." + GetStructFieldName() + ".xy/IN." + GetStructFieldName() + ".w).xyxy)";
		}
		
		
		public bool RequiresStructFieldInclusion()
		{
			return IsOutputChannelConnected( 0 );
		}
	}
}
