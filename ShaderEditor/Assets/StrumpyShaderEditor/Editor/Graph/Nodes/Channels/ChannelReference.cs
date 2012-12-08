using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public abstract class ChannelReference {
		[DataMember] private string nodeIdentifier;
		[DataMember] private uint channelId;
		
		public ChannelReference( string nodeIdentifier, uint channelId )
		{
			this.nodeIdentifier = nodeIdentifier;
			this.channelId = channelId;
		}
		
		public string NodeIdentifier
		{
			get{ return nodeIdentifier; }
			set{ nodeIdentifier = value; }
		}
		
		public uint ChannelId
		{
			get{ return channelId; }
		}
	}
}