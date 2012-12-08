using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class VertexOutputMasterNode : RootNode {
		private const string NodeName = "VertexOutputMaster";
		
		[DataMember] private Float4InputChannel _position;
		[DataMember] private Float4InputChannel _color;
		[DataMember] private Float4InputChannel _normal;
		[DataMember] private Float4InputChannel _tangent;
		
		public VertexOutputMasterNode( )
		{
			Initialize(); 
		}
		
		public override sealed void Initialize ()
		{
			_position = _position ?? new Float4InputChannel( 0, "Position", Vector4.zero );
			_color = _color ?? new Float4InputChannel( 1, "Color", Vector4.zero );
			_normal = _normal ?? new Float4InputChannel( 2, "Normal", Vector4.zero );
			_tangent = _tangent ?? new Float4InputChannel( 3, "Tangent", Vector4.zero );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel>();
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			return new List<InputChannel> { _position, _color, _normal, _tangent };
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string GetExpression( uint channelId )
		{
			Debug.LogError( "Can not get node based expression from a master node" );
			return "";
		}
		
		public string GetAdditionalFields()
		{
			var ret = _position.ChannelInput( this ).AdditionalFields;
			ret +=  _color.ChannelInput( this ).AdditionalFields;
			ret +=  _normal.ChannelInput( this ).AdditionalFields;
			ret +=  _tangent.ChannelInput( this ).AdditionalFields;
			return ret;
		}
		
		public bool PositionConnected()
		{
			return _position.IncomingConnection != null;
		}
		
		public string GetPositionExpression()
		{
			return _position.ChannelInput( this ).QueryResult;
		}
		
		public bool ColorConnected()
		{
			return _color.IncomingConnection != null;
		}
		
		public string GetColorExpression()
		{
			return _color.ChannelInput( this ).QueryResult;
		}
		
		public bool NormalConnected()
		{
			return _normal.IncomingConnection != null;
		}
		
		public string GetNormalExpression()
		{
			return _normal.ChannelInput( this ).QueryResult;
		}
		
		public bool TangentConnected()
		{
			return _tangent.IncomingConnection != null;
		}
		
		public string GetTangentExpression()
		{
			return _tangent.ChannelInput( this ).QueryResult;
		}
		
		public override void DrawProperties()
		{
			base.DrawProperties();
		}
	}
}
