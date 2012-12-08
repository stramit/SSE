using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("SinTime", "Input", typeof(SinTimeNode),"Sine of time: (t/8, t/4, t/2, t), see also Cos Time.")]
	public class SinTimeNode : Node, IProxyOnSave {
		private const string NodeName = "SinTime";
		public bool Exporting{ get; set; }
		
		[DataMember] private Float4OutputChannel _value;
		
		public SinTimeNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_value = _value ?? new Float4OutputChannel( 0, "Value" );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> {_value};
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
			return Exporting ? "_SinTime" : "_EditorSinTime";
		}
		
		public override bool NeedsSinTime ()
		{
			return true;
		}
	}
}