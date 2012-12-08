using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public abstract class Channel {
		[DataMember] protected readonly uint channelId;
		[DataMember] protected string displayName;
		
		public abstract TypeEnum ChannelType
		{
			get;
		}
		
		public Rect Position{ get; set; }
		
		public uint ChannelId{
			get{ return channelId; }
		}
		
		public string DisplayName{
			get{ return string.IsNullOrEmpty(displayName) ? "" : displayName; }
			set{ displayName = value; }
		}
		
		public Channel( uint channelId, string displayName )
		{
			Position = new Rect();
			this.channelId = channelId;
			this.displayName = displayName;
		}
	}
}
