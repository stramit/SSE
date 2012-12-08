using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("SimpleSampler2D", "Properties", typeof(SimpleSampler2DNode),"Basic 2D texture input, used in conjunction with Tex2D and Tex2DNormal.")]
	public class SimpleSampler2DNode : InputNode {
		private const string NodeName = "SimpleSampler2D";
		
		[DataMember] private Sampler2DOutputChannel _sampler;
		
		public SimpleSampler2DNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			base.Initialize();
			_sampler = _sampler ?? new Sampler2DOutputChannel( 0, "Sampler2d" );
		}
		
		//Beta3->Beta4 compatability.
		//TODO: Remove in B5
		protected override ShaderProperty InputPropertyFromOldInput()
		{
			return new Texture2DProperty();
		}
		
		protected override bool OldPropertyConfigured()
		{
			return _inputName != null;
		}
		
		protected override void RemoveObsoleteInputConfig()
		{
			_inputName = null;
		}

		protected override ShaderProperty NewPropertyInstance ()
		{
			return new Texture2DProperty();
		}
		
		public override IEnumerable<string> IsValid ( SubGraphType graphType )
		{
			var errors = new List<string> ();
			errors.AddRange( base.IsValid( graphType ) );
			
			if( graphType == SubGraphType.Vertex )
			{
				errors.Add( "Node not valid in graph type: " + graphType );
			}
			return errors;
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> { _sampler };
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
			return GetFieldName();
		}

		protected override InputType GetFieldType()
		{
			return InputType.Texture2D;
		}
		
		public override void DrawProperties()
		{
			base.DrawProperties();
		}
	}
}
