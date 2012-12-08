using System;
using UnityEditor;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Float4", "Properties", typeof(Float4Node),"Four component input for the user. See the Float4Const for a constant value.")]
	public class Float4Node : InputNode {
		private const string NodeName = "Float4";
		[DataMember] private Float4OutputChannel _value;
		
		public Float4Node( )
		{
			Initialize();
		}

		public override sealed void Initialize ()
		{
			base.Initialize();
			_value = _value ?? new Float4OutputChannel( 0, "Value" );
		}
		
		//Beta3->Beta4 compatability.
		//TODO: Remove in B5
		[DataMember] private EditorFloat4 _float4Value;
		protected override ShaderProperty InputPropertyFromOldInput()
		{
			var property = new Float4Property();
			property.PropertyName = _inputName;
			property.Float4 = _float4Value;
			return property;
		}
		
		protected override bool OldPropertyConfigured()
		{
			return _inputName != null && _float4Value != null;
		}
		
		protected override void RemoveObsoleteInputConfig()
		{
			_inputName = null;
			_float4Value = null;
		}

		protected override ShaderProperty NewPropertyInstance ()
		{
			return new Float4Property();
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

		protected override InputType GetFieldType()
		{
			return InputType.Vector;
		}
	}
}