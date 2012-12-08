using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Range", "Properties", typeof(RangeNode),"Float bound within the specified range exposed to the user. This is ideal for making the shader as easy to use as possible, but isn't always applicable. See also Float, and FloatConst")]
	public class RangeNode : InputNode {
		private const string NodeName = "Range";
		[DataMember] private Float4OutputChannel _value;
		
		public RangeNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			base.Initialize();
			_value = _value ?? new Float4OutputChannel( 0, "Value" );
		}

		protected override ShaderProperty NewPropertyInstance ()
		{
			return new RangeProperty();
		}
		
		//Beta3->Beta4 compatability.
		//TODO: Remove in B5
		[DataMember] private EditorRange _range;
		protected override ShaderProperty InputPropertyFromOldInput()
		{
			var property = new RangeProperty();
			property.PropertyName = _inputName;
			property.Range = _range;
			return property;
		}
		
		protected override bool OldPropertyConfigured()
		{
			return _inputName != null && _range != null;
		}
		
		protected override void RemoveObsoleteInputConfig()
		{
			_inputName = null;
			_range = null;
		}
		
		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_value};
			var property = ReferencedProperty();

			if( property != null && property is RangeProperty )
			{
				var rangeProperty = (RangeProperty)property;
				_value.DisplayName = "[" + rangeProperty.Range.Min.ToString("G2") + "," + rangeProperty.Range.Max.ToString("G2") + "]";
			}
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
			return InputType.Range;
		}
	}
}