using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("CosTime", "Input", typeof(CosTimeNode),"Cosine of time: (t/8, t/4, t/2, t)- There is no cost to using these values, as they are computed by CPU for all objects in scene. Used with splat to seperate an individual frequency.")]
	public class CosTimeNode : Node, IProxyOnSave {
		private const string NodeName = "CosTime";
		public bool Exporting{ get; set; }
		
		[DataMember] private Float4OutputChannel _value;
		
		public CosTimeNode( )
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
			return Exporting ? "_CosTime" : "_EditorCosTime";
		}
		
		public override bool NeedsCosTime ()
		{
			return true;
		}
	}
}