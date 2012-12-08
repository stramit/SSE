using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Screen Position", "Input", typeof(ScreenPositionNode),"The position on the screen, most of the time you want Screen Position[0,1] instead, however the Z component is how far away the pixel is from the camera, making it useful for custom fog.")]
	public class ScreenPositionNode : Node, IStructInput{
		private const string NodeName = "ScreenPosition";
		
		[DataMember] private Float4OutputChannel _position;
		public ScreenPositionNode( )
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
			return "IN." + GetStructFieldName();
		}
		
		public bool RequiresStructFieldInclusion()
		{
			return IsOutputChannelConnected( 0 );
		}
	}
}
