using UnityEditor;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Color", "Properties", typeof(ColorNode),"Color input, allows the user to use the color picker. Ensured to be within the [0,1] range unless modified via scripting.")]
	public class ColorNode : InputNode {
		private const string NodeName = "Color";
		[DataMember] private Float4OutputChannel _rgba;
		
		public ColorNode( )
		{
			Initialize();
		}

		protected override ShaderProperty NewPropertyInstance ()
		{
			return new ColorProperty();
		}
		
		public override sealed void Initialize ()
		{
			base.Initialize();
			_rgba = _rgba ?? new Float4OutputChannel( 0, "RGBA" );
		}
		
		
		//Beta3->Beta4 compatability.
		//TODO: Remove in B5
		[DataMember] private EditorColor _colorValue;
		protected override ShaderProperty InputPropertyFromOldInput()
		{
			var property = new ColorProperty();
			property.PropertyName = _inputName;
			property.Color = _colorValue;
			return property;
		}
		
		protected override bool OldPropertyConfigured()
		{
			return _inputName != null && _colorValue != null;
		}
		
		protected override void RemoveObsoleteInputConfig()
		{
			_inputName = null;
			_colorValue = null;
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_rgba};
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

		protected override InputType GetFieldType()
		{
			return InputType.Color;
		}
	}
}