using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("GrabSampler", "Input", typeof(GrabSamplerNode),"Results from objects already drawn, when used with the opaque queue sorting is not guaranteed and will thus have inconsistent results. Normally used with the transparency queue. Used for refraction, whole object transparency, and complex transparency effects. Only functions with Unity Pro.")]
	public class GrabSamplerNode : Node, IFieldInput{
		private const string NodeName = "GrabSampler";
		
		[DataMember] private Sampler2DOutputChannel _sampler;
		
		public GrabSamplerNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_sampler = _sampler ?? new Sampler2DOutputChannel( 0, "GrabTexture" );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_sampler };
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
			return GetFieldName();
		}
		
		public string GetFieldDefinition()
		{
			return GetFieldType().ShaderTypeString() + " " + GetFieldName() + ";\n";
		}

		public InputType GetFieldType()
		{
			return InputType.Texture2D;
		}
		
		public string GetFieldName()
		{
			return "_GrabTexture";
		}
		
		public override void DrawProperties()
		{
			base.DrawProperties();
		}
		
		public override bool RequiresGrabPass {
			get {
				return true;
			}
		}
	}
}
