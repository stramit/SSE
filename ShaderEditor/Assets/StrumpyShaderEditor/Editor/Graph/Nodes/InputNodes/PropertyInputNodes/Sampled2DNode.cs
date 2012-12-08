using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Sampled2D", "Properties", typeof(Sampled2DNode),"Color from a texture sampled on it's default UV's. Purely shorthand, it is no more optimal then directly wiring a sampler to Tex2D. Cleanest way to sample a single texture once by default, and should be used for most texture inputs unless you are sampling multiple times.")]
	public class Sampled2DNode : InputNode, IStructInput, IResultCacheNode{
		private const string NodeName = "Sampled2D";
		
		[DataMember] private Float4OutputChannel _rgba;
		[DataMember] private Float4OutputChannel _a;
		
		public Sampled2DNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			base.Initialize();
			_rgba = _rgba ?? new Float4OutputChannel( 0, "RGBA" );
			_a = _a ?? new Float4OutputChannel( 1, "A" );
		}

		protected override ShaderProperty NewPropertyInstance ()
		{
			return new Texture2DProperty();
		}
		
		//Beta3->Beta4 compatability.
		//TODO: Remove in B5
		[DataMember] private EditorGroup _defaultTexture;
		protected override ShaderProperty InputPropertyFromOldInput()
		{
			var property = new Texture2DProperty {PropertyName = _inputName};
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
			var ret = new List<OutputChannel> {_rgba, _a};
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
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			
			if( channelId == 0 )
			{
				return UniqueNodeIdentifier;
			}
			return UniqueNodeIdentifier + ".aaaa";
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
			result += "tex2D(" + GetFieldName() + "," + "IN." + GetStructFieldName() + ".xy);\n";
			return result;
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
			return GetStructFieldType().ShaderTypeString() + " " + GetStructFieldName() + ";\n";
		}

		public string GetStructVertexShaderString()
		{
			return "";
		}
		
		public bool RequiresStructFieldInclusion()
		{
			return true;
		}
	}
}
