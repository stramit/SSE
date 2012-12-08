using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class LightingOutputMasterNode : RootNode {
		private const string NodeName = "LightingOutputMaster";
		
		[DataMember] private Float4InputChannel _lighting;
		
		public LightingOutputMasterNode( )
		{
			Initialize(); 
		}
		
		public override sealed void Initialize ()
		{
			_lighting = _lighting ?? new Float4InputChannel( 0, "Lighting", Vector4.zero );
		}
		
		public bool LightingFunctionPresent
		{
			get{
				return IsInputChannelConnected( _lighting.ChannelId );
			}
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel>();
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			var ret = new List<InputChannel> { _lighting };
			return ret;
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
			return _lighting.ChannelInput( this ).AdditionalFields;
		}
		
		public bool PositionConnected()
		{
			return _lighting.IncomingConnection != null;
		}
		
		public string GetLightingExpression()
		{
			return _lighting.ChannelInput( this ).QueryResult;
		}
		
		public override void DrawProperties()
		{
			base.DrawProperties();
		}
	}
}
