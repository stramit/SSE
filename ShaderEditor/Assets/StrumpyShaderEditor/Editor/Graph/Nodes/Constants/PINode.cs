using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("PI", "Constant", typeof(PINode),"3.1415926535 - Used extensively with trig functions, see also 1/PI. Because we cannot perfectly approximate PI, rounding errors will become noticable with large numbers.")]
	public class PINode : Node{
		private const string NodeName = "PI";
		[DataMember] private Float4OutputChannel _constValue;
		
		public PINode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_constValue = _constValue ?? new Float4OutputChannel( 0, "PI" );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_constValue};
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
			return "float4( 3.1415926535, 3.1415926535, 3.1415926535, 3.1415926535 )";
		}
	}
}