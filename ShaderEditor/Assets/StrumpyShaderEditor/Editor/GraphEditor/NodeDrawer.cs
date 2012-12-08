using UnityEngine;

namespace StrumpyShaderEditor
{
	public static class NodeDrawer{
		public static Vector2 GetAbsoluteInputChannelPosition( Node node, uint channelId )
		{
			var inputChannel = node.GetInputChannel( channelId );
			var result = new Vector2( node.NodePositionInGraph.x, node.NodePositionInGraph.y);
			result += new Vector2( inputChannel.Position.x, inputChannel.Position.y );
			result += new Vector2( inputChannel.Position.width / 2, inputChannel.Position.height / 2 );
			return result;
		}
		
		public static Vector2 GetAbsoluteOutputChannelPosition( Node node, uint channelId )
		{
			var outputChannel = node.GetOutputChannel( channelId );
			var result = new Vector2( node.NodePositionInGraph.x, node.NodePositionInGraph.y);
			result += new Vector2( outputChannel.Position.x, outputChannel.Position.y );
			result += new Vector2( outputChannel.Position.width / 2, outputChannel.Position.height / 2 );
			return result;
		}
	}
}
