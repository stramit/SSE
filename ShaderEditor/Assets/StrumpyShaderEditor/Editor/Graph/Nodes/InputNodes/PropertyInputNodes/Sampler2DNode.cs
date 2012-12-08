using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Sampler2D", "Properties", typeof(Sampler2DNode),"Basic 2D texture input, used in conjunction with Tex2D and Tex2DNormal. The UV supplied are those set in the inspector when the user defines the texture, and may be offset, modified, or simply ignored.")]
	public class Sampler2DNode : InputNode, IStructInput{
		private const string NodeName = "Sampler2D";
		
		[DataMember] private Sampler2DOutputChannel _sampler;
		[DataMember] private Float4OutputChannel _uv;
		
		public Sampler2DNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			base.Initialize();
			_sampler = _sampler ?? new Sampler2DOutputChannel( 0, "Sampler2d" );
			_uv = _uv ?? new Float4OutputChannel( 1, "UV" );
		}
		
		//Beta3->Beta4 compatability.
		//TODO: Remove in B5
		[DataMember] private EditorGroup _defaultTexture;
		protected override ShaderProperty InputPropertyFromOldInput()
		{
			var property = new Texture2DProperty();
			property.PropertyName = _inputName;
			if( _defaultTexture.Selected == "white" )
			{
				property._defaultTexture = DefaultTextureType.White;
			}
			if( _defaultTexture.Selected == "normal" )
			{
				property._defaultTexture = DefaultTextureType.Bump;
			}
			return property;
		}
		
		protected override bool OldPropertyConfigured()
		{
			return _inputName != null && _defaultTexture != null;
		}
		
		protected override void RemoveObsoleteInputConfig()
		{
			_inputName = null;
			_defaultTexture = null;
		}

		protected override ShaderProperty NewPropertyInstance ()
		{
			return new Texture2DProperty();
		}
		
		public override IEnumerable<string> IsValid ( SubGraphType graphType )
		{
			var errors = new List<string> ();
			errors.AddRange( base.IsValid( graphType ) );
			
			if( graphType != SubGraphType.Pixel )
			{
				errors.Add( "Node not valid in graph type: " + graphType );
			}
			return errors;
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> {_sampler, _uv };
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
			
			if( channelId == 1 )
			{
				return "(IN." + GetStructFieldName() + ".xyxy)";
			}
			return GetFieldName();
		}

		protected override InputType GetFieldType()
		{
			return InputType.Texture2D;
		}
		
		public StructTypes GetStructFieldType()
		{
			return StructTypes.Float2;
		}
		
		public string GetStructFieldName()
		{
			return "uv" + GetFieldName();
		}
		
		public string GetStructFieldDefinition()
		{
			return IsOutputChannelConnected(1)
			       	? GetStructFieldType().ShaderTypeString() + " " + GetStructFieldName() + ";\n"
			       	: "";
		}

		public string GetStructVertexShaderString()
		{
			return "";
		}
		
		public override void DrawProperties()
		{
			base.DrawProperties();
		}
		
		public bool RequiresStructFieldInclusion()
		{
			return IsOutputChannelConnected( 1 );
		}
	}
}
