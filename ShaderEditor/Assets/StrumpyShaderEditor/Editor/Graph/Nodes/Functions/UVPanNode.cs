using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("UV Pan", "Function", typeof(UVPanNode),"Shorthand node for adjusting vector over time, generally used with UV coordinates")]
	public class UVPanNode : Node, IResultCacheNode {
		private const string NodeName = "UV_Pan";
		[DataMember] private EditorGroup _inputAddChannel;
		
		[DataMember] private EditorBool _xPan;
		[DataMember] private EditorBool _yPan;
		[DataMember] private EditorBool _zPan;
		[DataMember] private EditorBool _wPan;
		
		[DataMember] private Float4OutputChannel _result;
		[DataMember] private Float4InputChannel _uv;
		[DataMember] private Float4InputChannel _time;
		
		public UVPanNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_inputAddChannel = _inputAddChannel ?? new EditorGroup( 0, new[] { "x", "y", "z", "w" }, 4 );
		
			_xPan = _xPan ?? new EditorBool();
			_yPan = _yPan ?? new EditorBool();
			_zPan = _zPan ?? new EditorBool();
			_wPan = _wPan ?? new EditorBool();
			
			_result = _result ?? new Float4OutputChannel( 0, "Result" );
			_uv = _uv ?? new Float4InputChannel( 0, "UV", Vector4.zero );
			_time = _time ?? new Float4InputChannel( 1, "Time", Vector4.zero );
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public string GetAdditionalFields()
		{
			var uvInput = _uv.ChannelInput( this );
			var timeInput = _time.ChannelInput( this );
			
			string result = uvInput.AdditionalFields;
			result += timeInput.AdditionalFields;
			return result;
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_result};
		    return ret;
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			var ret = new List<InputChannel> {_uv, _time};
		    return ret;
		}
		
		public string GetUsage()
		{
			var uvInput = _uv.ChannelInput( this );
			var timeInput = _time.ChannelInput( this );
			
			string result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=";
			result += "float4(";
			result += (_xPan ? (uvInput.QueryResult + ".x" + " + " + timeInput.QueryResult + "." + _inputAddChannel.Selected) : uvInput.QueryResult + ".x") + ",";
			result += (_yPan ? (uvInput.QueryResult + ".y" + " + " + timeInput.QueryResult + "." + _inputAddChannel.Selected) : uvInput.QueryResult + ".y") + ",";
			result += (_zPan ? (uvInput.QueryResult + ".z" + " + " + timeInput.QueryResult + "." + _inputAddChannel.Selected) : uvInput.QueryResult + ".z") + ",";
			result += (_wPan ? (uvInput.QueryResult + ".w" + " + " + timeInput.QueryResult + "." + _inputAddChannel.Selected) : uvInput.QueryResult + ".w") + ");\n";
			return result;
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return UniqueNodeIdentifier;
		}
		
		public override void DrawProperties()
		{
			base.DrawProperties();
			GUILayout.Label( "Input Channel:" );
			_inputAddChannel.Value = GUILayout.SelectionGrid( _inputAddChannel.Value, _inputAddChannel.GridValues.ToArray(), _inputAddChannel.GuiRowElementsNum );
			
			_xPan.Value = EditorGUILayout.Toggle( "Pan x", _xPan.Value );
			_yPan.Value = EditorGUILayout.Toggle( "Pan y", _yPan.Value );
			_zPan.Value = EditorGUILayout.Toggle( "Pan z", _zPan.Value );
			_wPan.Value = EditorGUILayout.Toggle( "Pan w", _wPan.Value );
		}
	}
}