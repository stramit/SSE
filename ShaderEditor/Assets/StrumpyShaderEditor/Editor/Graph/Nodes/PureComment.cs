using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Comment", "Constant", typeof(CommentNode),"Purely visual comment node")]
	public class CommentNode : Node {
		private const string NodeName = "Comment";
		[DataMember] private EditorString _title;
		
		public CommentNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_title = _title ?? "Comment";
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string DisplayName {
			get { return _title; }
		}
		
		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {};
			return ret;
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			var ret = new List<InputChannel> {};
			return ret;
		}
		
		public string GetAdditionalFields()
		{
			return "";
		}
		
		public override void DrawProperties()
		{
			base.DrawProperties();
			GUILayout.Label( "Content" );
			_title = GUILayout.TextArea( _title );
		}
		
		public string GetUsage()
		{
			return "";
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return UniqueNodeIdentifier;
		}
	}
}