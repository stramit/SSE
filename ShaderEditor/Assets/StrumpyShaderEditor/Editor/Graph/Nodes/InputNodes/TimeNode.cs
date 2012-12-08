using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Time", "Input", typeof(TimeNode),"Time in various components, (t/20, t, t*2, t*3) Used to animate things inside of shaders. Like the other time inputs, this is depdent on the Time scripting class timescale, so it will by in sync the scene timescales. Be aware each component is seperate, so they will need to be splatted for a result consistent across all channels. For periodic time, consider using SinTime and CosTime first.")]
	public class TimeNode : Node, IProxyOnSave {
		private const string NodeName = "Time";
		public bool Exporting{ get; set; }
		
		[DataMember] private Float4OutputChannel _value;
		
		public TimeNode( )
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
			return Exporting ? "_Time" : "_EditorTime";
		}
		
		public override bool NeedsTime ()
		{
			return true;
		}
	}
}