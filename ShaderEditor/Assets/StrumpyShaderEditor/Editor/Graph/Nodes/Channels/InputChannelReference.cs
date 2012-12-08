using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class InputChannelReference : ChannelReference {
		public InputChannelReference( string nodeIdentifier, uint channelId ) : base( nodeIdentifier, channelId )
		{}
	}
}
