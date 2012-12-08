using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("SamplerCube", "Properties", typeof(SamplerCubeNode),"Cubemap sampler, provides the input to go with TexCube. Used for reflections.")]
	public class SamplerCubeNode : InputNode {
		private const string NodeName = "SamplerCube";
		[DataMember] private SamplerCubeOutputChannel _sampler;

		public SamplerCubeNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			base.Initialize();
			_sampler = _sampler ?? new SamplerCubeOutputChannel( 0, "SamplerCube" );
		}
		
		//Beta3->Beta4 compatability.
		//TODO: Remove in B5
		protected override ShaderProperty InputPropertyFromOldInput()
		{
			var property = new TextureCubeProperty();
			property.PropertyName = _inputName;
			return property;
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
			return new TextureCubeProperty();
		}
		
		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> {_sampler};
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			return new List<InputChannel>();
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}

		protected override InputType GetFieldType()
		{
			return InputType.TextureCube;
		}
	}
}
