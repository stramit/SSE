using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public abstract class InputChannel : Channel {
		[DataMember] public OutputChannelReference IncomingConnection{ get; set;}
		
		public InputChannel( uint id, string name ) : base( id, name )
		{
			IncomingConnection = null;
		}
		
		protected string GetDefaultInputName( Node parent )
		{
			return parent.UniqueNodeIdentifier +  "_" + channelId + "_NoInput";
		}
		
		public abstract string GetDefaultInput( Node parent );
		
		public ChannelQueryResult ChannelInput(Node parent)
		{
			ChannelQueryResult result;
			if( IncomingConnection == null )
			{
				result.AdditionalFields = GetDefaultInput( parent );
				result.QueryResult = GetDefaultInputName( parent );
			}
			else
			{
				result.AdditionalFields = "";  
				var outputNode = parent.Owner.GetNode( IncomingConnection.NodeIdentifier );
				result.QueryResult = outputNode.GetExpression( IncomingConnection.ChannelId );
			}
			return result;
		}
	}
}
