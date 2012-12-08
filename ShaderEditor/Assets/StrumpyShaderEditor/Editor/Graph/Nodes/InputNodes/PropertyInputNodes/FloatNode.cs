using UnityEditor;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Float", "Properties", typeof(FloatNode),"Single unbounded float input for the user. See Range for a bounded input (with a slider), and FloatConst for a constant value.")]
	public class FloatNode : InputNode {
		private const string NodeName = "Float";
		[DataMember] private Float4OutputChannel _value; 
		
		public FloatNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			base.Initialize();
			_value = _value ??  new Float4OutputChannel( 0, "Value" );
		}
		
		//Beta3->Beta4 compatability.
		//TODO: Remove in B5
		[DataMember] private EditorFloat _floatValue;
		protected override ShaderProperty InputPropertyFromOldInput()
		{
			var property = new FloatProperty();
			property.PropertyName = _inputName;
			property.Float = _floatValue;
			return property;
		}
		
		protected override bool OldPropertyConfigured()
		{
			return _inputName != null && _floatValue != null;
		}
		
		protected override void RemoveObsoleteInputConfig()
		{
			_inputName = null;
			_floatValue = null;
		}


		protected override ShaderProperty NewPropertyInstance ()
		{
			return new FloatProperty();
		}
		
		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_value};
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
			return GetFieldName() + ".xxxx";
		}

		protected override InputType GetFieldType()
		{
			return InputType.Float;
		}
	}
}