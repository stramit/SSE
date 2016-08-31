using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public abstract class Node
	{
		[DataMember]
		public int NodeID { get; private set; }
		[DataMember]
		public EditorRect Position { get; private set; }
		[DataMember]
		private EditorString _comment;
		
		public SubGraph Owner{ get; set; }

		private EditorString Comment {
			get { return _comment ?? (_comment = ""); }
		}
		
		public Rect NodePositionInGraph
		{
			get{
				var position = Position.Value;
				position.x += Owner.DrawOffset.X;
				position.y += Owner.DrawOffset.Y;
				return position;
			}
			set{
				var position = value;
				position.x -= Owner.DrawOffset.X;
				position.y -= Owner.DrawOffset.Y;
				Position = position;
			}
		}

		//Not serialized as we generate this each draw tick
		public NodeState CurrentState;// { private get; set; } // Texel - Uniform access to tint node button
		private List<String> _errorMessages;
		
		public List<String> ErrorMessages // Texel Similarly with this, I wanted to display the error message elsewhere aswell
		{
			get { return _errorMessages ?? (_errorMessages = new List<string>()); }
		}

		protected Node ()
		{
			Position = new EditorRect (10, 40, 30, 30);
		}

		protected abstract IEnumerable<OutputChannel> GetOutputChannels ();
		public abstract IEnumerable<InputChannel> GetInputChannels ();

		public abstract void Initialize ();

		public override int GetHashCode ()
		{
			int nameHash = NodeTypeName == null ? 0 : NodeTypeName.GetHashCode ();
			return NodeID ^ nameHash;
		}
		
		public void AddErrors( IEnumerable<string> errors )
		{
			ErrorMessages.AddRange( errors );
		}
		
		public void ClearErrors()
		{
			ErrorMessages.Clear();
		}
		
		public virtual bool NeedsTime()
		{
			return false;
		}
		
		public virtual bool NeedsCosTime()
		{
			return false;
		}
		
		public virtual bool NeedsSinTime()
		{
			return false;
		}
		
		public InputChannel GetInputChannel (uint channelId)
		{
			return GetInputChannels ().First (x => x.ChannelId == channelId);
		}

		public OutputChannel GetOutputChannel (uint channelId)
		{
			return GetOutputChannels ().First (x => x.ChannelId == channelId);
		}

		protected bool IsInputChannelConnected (uint channelId)
		{
			return GetInputChannel (channelId).IncomingConnection != null;
		}

		public void AssertInputChannelExists (uint channelId)
		{
			if (GetInputChannel (channelId) == null) {
				Debug.LogError ("Channel: " + channelId + " does not exist on node " + UniqueNodeIdentifier);
			}
		}

		public bool IsOutputChannelConnected (uint channelId)
		{
			foreach (var node in Owner.Nodes) {
				foreach (var inChannel in node.GetInputChannels ()) {
					if (inChannel.IncomingConnection != null && inChannel.IncomingConnection.NodeIdentifier == UniqueNodeIdentifier && inChannel.IncomingConnection.ChannelId == channelId) {
						return true;
					}
				}
			}
			return false;
		}

		public void AssertOutputChannelExists (uint channelId)
		{
			if (GetOutputChannel (channelId) == null) {
				Debug.LogError ("Channel: " + channelId + " does not exist on node " + UniqueNodeIdentifier);
			}
		}

		public virtual IEnumerable<string> IsValid ( SubGraphType graphType )
		{
			return new List<string> ();
		}
		
		public void AddedToGraph (int newNodeID)
		{
			NodeID = newNodeID;
		}

		public abstract string NodeTypeName { get; }

		public virtual string DisplayName {
			get { return NodeTypeName; }
		}

		public string UniqueNodeIdentifier {
			get { return NodeTypeName.RemoveWhiteSpace() + NodeID; }
		}

		public void DrawCommentField ()
		{
			GUILayout.Label ("Comment");
			// Instead of NormalGUILayout for copy/paste etc
			_comment = GUILayout.TextArea (Comment); // Texel Modified to text field, so it expands as needed.
		}

		public virtual void DrawProperties ()
		{
			//GUILayout.Label (UniqueNodeIdentifier);
		}

		public abstract string GetExpression (uint channelId);

		public virtual bool RequiresInternalData {
			get { return false; }
		}

		public virtual bool RequiresGrabPass {
			get { return false; }
		}

		public virtual bool RequiresSceneDepth {
			get { return false; }
		}
		
		public void DeltaMove (Vector2 delta)
		{
			Position.X += delta.x;
			Position.Y += delta.y;
		}
		
		public bool ButtonAt( Vector2 location )
		{
			foreach( var channel in GetInputChannels() )
			{
				Rect channelRect = NodePositionInGraph;
				channelRect.x += channel.Position.x;
				channelRect.y += channel.Position.y;
				channelRect.width = channel.Position.width;
				channelRect.height = channel.Position.height;
				
				if( channelRect.Contains( location ) )
				{
					return true;
				}
			}
			
			foreach( var channel in GetOutputChannels() )
			{
				Rect channelRect = NodePositionInGraph;
				channelRect.x += channel.Position.x;
				channelRect.y += channel.Position.y;
				channelRect.width = channel.Position.width;
				channelRect.height = channel.Position.height;
				
				if( channelRect.Contains( location ) )
				{
					return true;
				}
			}
			return false;
		}
		
		// Texel : Shorthand for the editor window style
		private static GUIStyle WindowStyle {
			get { return EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).window; }
		}

		public void Draw ( NodeEditor editor, bool showComments, bool selected, Vector2 drawOffset )
		{
			GUI.skin.box.alignment = TextAnchor.UpperCenter;
			
			const float nodeDrawWidth = 100;
			const float headerHeight = 18;
			const float nodeDrawHeightPerChannel = 21;
			
			//Figure out node width:
			Rect drawPos = Position;
			
			int inChannelNum = GetInputChannels ().Count ();
			int outChannelNum = GetOutputChannels ().Count ();
			if (inChannelNum > 0 && outChannelNum > 0) {
				drawPos.width = nodeDrawWidth * 1.36f;
			} else {
				drawPos.width = nodeDrawWidth;
			}
			
			int maxChannels = inChannelNum > outChannelNum ? inChannelNum : outChannelNum;
			drawPos.height = headerHeight + (maxChannels * nodeDrawHeightPerChannel);
			
			var windowName = new GUIContent (DisplayName);
			float minWidth;
			float maxWidth;
			//GUI.skin.window.CalcMinMaxWidth (windowName, out minWidth, out maxWidth);
			WindowStyle.CalcMinMaxWidth(windowName, out minWidth, out maxWidth); // Texel - Window style changes
			windowName.text = windowName.text.Trim("~"[0]); // Texel - Trim padding character
			drawPos.width = maxWidth > drawPos.width ? maxWidth : drawPos.width;
			Position = drawPos;
			
			var drawPosOffset = drawPos;
			drawPosOffset.x += drawOffset.x;
			drawPosOffset.y += drawOffset.y;
			
			
			float boxOffset = 0f;
			// For calculating the box vertical offset
			// This area is potentially shared by the comments, so comments would be drawn below errors.
			// Also made excessively large, to fail more elegantly.
			switch (CurrentState) {
			case (NodeState.Valid):
				GUI.color = Color.white;
				break;
			case (NodeState.NotConnected):
				GUI.color = new Color (0.8f, 0.8f, 1f);
				break;
			case (NodeState.CircularReferenceInGraph):
				GUI.color = new Color (0.8f, 0.8f, 0f);
				break;
			case (NodeState.Error):
				{
					GUI.color = Color.red;
					
					//GUILayout.BeginArea(new Rect(drawPos.x, drawPos.yMax, 300, 200));
					foreach (var error in ErrorMessages) {
						var content = new GUIContent (error);
						float wMin, wMax;
						GUI.skin.box.CalcMinMaxWidth (content, out wMin, out wMax);
						var height = GUI.skin.box.CalcHeight (content, wMax);
						GUI.Box (new Rect (drawPosOffset.x, drawPosOffset.yMax + boxOffset, wMax, height), content);
						boxOffset += height;
					}
					break;
				}
			}
				GUI.backgroundColor = Color.gray; // Texel - Blend, rather then hard set

            if ( selected )
			{
				GUI.backgroundColor = Color.Lerp(GUI.backgroundColor,Color.green,0.5f); // Texel - Blend, rather then hard set
			}
			
			// Texel - Special case, for nodes which don't have any inputs
			if (maxChannels > 0)
				GUI.Box (drawPosOffset, windowName, WindowStyle);
			else {
				if (selected)
					GUI.Box(drawPosOffset,GUIContent.none,WindowStyle);
				
				GUIStyle empty = GUIStyle.none;
				empty.clipping = TextClipping.Overflow;
				GUI.Box (drawPosOffset, windowName, empty);
			}
			
			//Do custom layout to stop unity throwing errors for some strange reason
			// Texel - Seperated draw size from actual size, using blank style for real buttons
			var nodeIoSize = new Vector2( 15f, nodeDrawHeightPerChannel * (2f / 3f) );
			var nodeIoDrawSize = new Vector2( 7f, 7f );
			
			var currentDrawPosition = new Vector2( drawPosOffset.x, drawPosOffset.y );
			currentDrawPosition.y += headerHeight;
			
			TextAnchor oldTextAnchor = GUI.skin.label.alignment;
			TextClipping oldTextClipping = GUI.skin.label.clipping;
			bool oldWordWrap = GUI.skin.label.wordWrap;
			
			GUI.skin.label.alignment = TextAnchor.UpperLeft;
			GUI.skin.label.clipping = TextClipping.Overflow;
			GUI.skin.label.wordWrap = false;
			foreach (var channel in GetOutputChannels ())
			{
				var absoluteIOVisualPos = 
					new Rect( 
						currentDrawPosition.x- ( nodeIoDrawSize.x * 0.5f), 
						currentDrawPosition.y + nodeIoDrawSize.y/2 + 2, 
						nodeIoDrawSize.x, 
						nodeIoDrawSize.y );
				
				var absoluteIODrawPos =
					new Rect( 
						currentDrawPosition.x- ( nodeIoSize.x * 0.5f), 
						currentDrawPosition.y + 2, 
						nodeIoSize.x, 
						nodeIoSize.y );
				
				GUI.Box( absoluteIOVisualPos, "", GUI.skin.box);
				if ( GUI.Button( absoluteIODrawPos, "", GUIStyle.none) ) 
				{
					editor.SelectedOutputChannel = new OutputChannelReference( UniqueNodeIdentifier, channel.ChannelId);
				}
				var relativeIODrawPos = absoluteIODrawPos;
				relativeIODrawPos.x -= drawPosOffset.x;
				relativeIODrawPos.y -= drawPosOffset.y;
				channel.Position = relativeIODrawPos;
				
				var labelDrawPos = new Rect( currentDrawPosition.x + ( nodeIoSize.x * 0.5f), currentDrawPosition.y, drawPosOffset.width / 2f, nodeDrawHeightPerChannel );
				GUI.Label(labelDrawPos, channel.DisplayName);
				currentDrawPosition.y += nodeDrawHeightPerChannel;
			}
			
			//Do the other side now...
			currentDrawPosition = new Vector2( drawPosOffset.x + (drawPosOffset.width / 2f), drawPosOffset.y );
			currentDrawPosition.y += headerHeight;
			
			GUI.skin.label.alignment = TextAnchor.UpperRight;
			GUI.skin.label.clipping = TextClipping.Overflow;
			GUI.skin.label.wordWrap = false;
			foreach (var channel in GetInputChannels ())
			{
				var absoluteIOVisualPos = 
					new Rect( 
						drawPosOffset.xMax- ( nodeIoSize.x * 0.5f) + 3, 
						currentDrawPosition.y + nodeIoDrawSize.y/2 + 2, 
						nodeIoDrawSize.x, 
						nodeIoDrawSize.y );
				
				var absoluteIODrawPos =
					new Rect( 
						drawPosOffset.xMax- ( nodeIoSize.x * 0.5f), 
						currentDrawPosition.y + 2, 
						nodeIoSize.x, 
						nodeIoSize.y );
				
				GUI.Box( absoluteIOVisualPos, "", GUI.skin.box);
				if ( GUI.Button( absoluteIODrawPos, "" , GUIStyle.none) ) 
				{
					editor.SelectedInputChannel = new InputChannelReference( UniqueNodeIdentifier, channel.ChannelId );
				}
				var relativeIODrawPos = absoluteIODrawPos;
				relativeIODrawPos.x -= drawPosOffset.x;
				relativeIODrawPos.y -= drawPosOffset.y;
				channel.Position = relativeIODrawPos;
				
				var labelDrawPos = new Rect( currentDrawPosition.x, currentDrawPosition.y, (drawPosOffset.width / 2f) - ( nodeIoSize.x * 0.5f), nodeDrawHeightPerChannel );
				GUI.Label(labelDrawPos, channel.DisplayName);
				currentDrawPosition.y += nodeDrawHeightPerChannel;
			}
			
			GUI.color = Color.white;
			GUI.skin.label.alignment = oldTextAnchor;
			GUI.skin.label.clipping = oldTextClipping;
			GUI.skin.label.wordWrap = oldWordWrap;
			
			if ( showComments && Comment != "") {
				// Texel - Comment Field
				var oldColor = GUI.color;
				GUI.color = Vector4.Scale (GUI.color, new Vector4 (0.95f, 0.95f, 0.95f, 0.7f));
				
				var oldAnchor = GUI.skin.box.alignment;
				GUI.skin.box.alignment = TextAnchor.UpperLeft;
				var oldState = GUI.skin.box.normal;
				
				// Draw the text opaque
				var textColor = GUI.skin.box.normal.textColor;
				textColor.a = 1f;
				GUI.skin.box.normal.textColor = textColor;
				
				//var oldWrap = GUI.skin.box.wordWrap;
				GUI.skin.box.wordWrap = true;
				
				var content = new GUIContent ( Comment);
				float wMin, wMax;
				
				GUI.skin.box.CalcMinMaxWidth (content, out wMin, out wMax);
				
				var aWidth = Mathf.Min (drawPosOffset.width * 1.5f, wMax);
				
				var height = GUI.skin.box.CalcHeight (content, aWidth);
				
				GUI.Box (new Rect (drawPosOffset.x, drawPosOffset.yMax + boxOffset, aWidth, height), content);
				
				GUI.skin.box.alignment = oldAnchor;
				GUI.skin.box.normal = oldState;
				GUI.skin.box.normal.textColor = textColor;
				GUI.color = oldColor;
			}
			
			GUI.backgroundColor = Color.white;
		}
	}
}
