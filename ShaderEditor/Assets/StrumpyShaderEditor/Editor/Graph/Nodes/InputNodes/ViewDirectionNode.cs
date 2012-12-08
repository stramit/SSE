using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("View Direction", "Input", typeof(ViewDirectionNode),"View direction, relative to the surface. Used internally in the Fresnel node, and somewhat useful for isolating backfacing, you can manually compute reflection vectors by using this with World Normal and Reflect.")]
	public class ViewDirectionNode : Node, IStructInput{
		private const string NodeName = "ViewDirection";
		
		[DataMember] private OutputChannel _view;
		
		public ViewDirectionNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_view = _view ?? new Float4OutputChannel( 0, "View" );
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
			var ret = new List<OutputChannel> {_view};
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
			return "viewDir";
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
