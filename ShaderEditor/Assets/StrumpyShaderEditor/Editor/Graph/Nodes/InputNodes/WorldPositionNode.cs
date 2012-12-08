using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("World Position", "Input", typeof(WorldPositionNode),"Position in world space of the pixel. Potential rounding errors for huge scenes. Used for ground fog, or script-directed positional effects.")]
	public class WorldPositionNode : Node, IStructInput{
		private const string NodeName = "WorldPosition";
		
		[DataMember] private Float4OutputChannel _position;
		
		public WorldPositionNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_position = _position ?? new Float4OutputChannel( 0, "Position" );
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
			return StructTypes.Float3;
		}
		
		public string GetStructFieldName()
		{
			return "worldPos";
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
			return this.GenerateInputUsageString();
		}
		
		public bool RequiresStructFieldInclusion()
		{
			return IsOutputChannelConnected( 0 );
		}
	}
}
