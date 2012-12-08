using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Float Const", "Constant", typeof(FloatConstNode),"A single constant swizzled to four values (x,x,x,x) - Equivalent to CG (value).xxxx")]
	public class FloatConstNode : Node {
		private const string NodeName = "FloatConst";
		[DataMember] private EditorFloat _floatValue;
		[DataMember] private Float4OutputChannel _constValue;
		
		public FloatConstNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_floatValue = _floatValue ?? new EditorFloat();
			_constValue = _constValue ?? new Float4OutputChannel( 0, "Value" );
			_constValue.DisplayName = "Value";
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			_constValue.DisplayName = _floatValue.Value.ToString("G6");
			var ret = new List<OutputChannel> {_constValue};
			return ret;
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			return new List<InputChannel>();
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return "float4( " + _floatValue.Value + "," + _floatValue.Value + "," + _floatValue.Value + "," + _floatValue.Value + " )";
		}
		
		public override string DisplayName { get { return _floatValue.Value.ToString("G6"); } } // Texel 
		
		public override void DrawProperties()
		{
			base.DrawProperties();
			_floatValue.Value = EditorGUILayout.FloatField( "Value:", _floatValue.Value );
		}
	}
}