using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Luminance", "Constant", typeof(LumNode),"Luminance constant. (0.2125,.7154,0.072,0.0), This represents how much each color channel contributes to the luminance of an image. To find luminosity, use Dot with this and the desired color. Used for several other color effects, such as special blends, aswell.")]
	public class LumNode : Node{
		private const string NodeName = "LuminanceConst";
		[DataMember] private Float4OutputChannel _constValue;
		
		public LumNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_constValue = _constValue ?? new Float4OutputChannel( 0, "LumConst" );
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
			return "float4(0.2125,.7154,0.072,0.0)";
		}
	}
}