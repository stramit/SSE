using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Assemble", "Function", typeof(MergeNode),"Assemble a new vector our of components from other vectors. You may select which components to use. Consumes one instruction per input. Empty inputs default to zero.")]
	public class MergeNode : Node, IResultCacheNode {
		private const string NodeName = "Assemble";
		[DataMember] private EditorGroup _xChannel;
		[DataMember] private EditorGroup _yChannel;
		[DataMember] private EditorGroup _zChannel;
		[DataMember] private EditorGroup _wChannel;
		[DataMember] private Float4OutputChannel _result;
		[DataMember] private Float4InputChannel _v1;
		[DataMember] private Float4InputChannel _v2;
		[DataMember] private Float4InputChannel _v3;
		[DataMember] private Float4InputChannel _v4;
		
		public MergeNode() {
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_xChannel = _xChannel ?? new EditorGroup( 0, new[] { "x", "y", "z", "w" }, 4 );
			_yChannel = _yChannel ?? new EditorGroup( 1, new[] { "x", "y", "z", "w" }, 4 );
			_zChannel = _zChannel ?? new EditorGroup( 2, new[] { "x", "y", "z", "w" }, 4 );
			_wChannel = _wChannel ?? new EditorGroup( 3, new[] { "x", "y", "z", "w" }, 4 );
			_result = _result ?? new Float4OutputChannel( 0, "Result" );
			_v1 = _v1 ?? new Float4InputChannel( 0, "X", Vector4.zero );
			_v2 = _v2 ?? new Float4InputChannel( 1, "Y", Vector4.zero );
			_v3 = _v3 ?? new Float4InputChannel( 2, "Z", Vector4.zero );
			_v4 = _v4 ?? new Float4InputChannel( 3, "W", Vector4.zero );
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_result};
		    return ret;
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			var ret = new List<InputChannel> {_v1,_v2,_v3,_v4};
		    return ret;
		}
		
		public string GetAdditionalFields()
		{
			var arg1 = _v1.ChannelInput( this );
			var arg2 = _v2.ChannelInput( this );
			var arg3 = _v3.ChannelInput( this );
			var arg4 = _v4.ChannelInput( this );
			
			var ret = arg1.AdditionalFields;
			ret += arg2.AdditionalFields;
			ret += arg3.AdditionalFields;
			ret += arg4.AdditionalFields;
			
			return ret;
		}
		
		public string GetUsage()
		{
			var arg1 = _v1.ChannelInput( this );
			var arg2 = _v2.ChannelInput( this );
			var arg3 = _v3.ChannelInput( this );
			var arg4 = _v4.ChannelInput( this );
			
			var result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=";
			result +=  "float4(";
			
			/*switch (_xChannel.Selected) {
				case "v1" : {
					result += arg1.QueryResult;
					break; }
				case "v2" : {
					result += arg2.QueryResult;
					break; }
				case "v3" : {
					result += arg3.QueryResult;
					break; }
				case "v4" : {
					result += arg4.QueryResult;
					break; }
				default : { 
					result += "(0.0)"; break;}
			}
			result += ".x, ";
			
			switch (_yChannel.Selected) {
				case "v1" : {
					result += arg1.QueryResult;
					break; }
				case "v2" : {
					result += arg2.QueryResult;
					break; }
				case "v3" : {
					result += arg3.QueryResult;
					break; }
				case "v4" : {
					result += arg4.QueryResult;
					break; }
				default : { 
					result += "(0.0)"; break;}
			}
			result += ".y, ";
			
			switch (_zChannel.Selected) {
				case "v1" : {
					result += arg1.QueryResult;
					break; }
				case "v2" : {
					result += arg2.QueryResult;
					break; }
				case "v3" : {
					result += arg3.QueryResult;
					break; }
				case "v4" : {
					result += arg4.QueryResult;
					break; }
				default : { 
					result += "(0.0)"; break;}
			}
			result += ".z, ";
			
			switch (_wChannel.Selected) {
				case "v1" : {
					result += arg1.QueryResult;
					break; }
				case "v2" : {
					result += arg2.QueryResult;
					break; }
				case "v3" : {
					result += arg3.QueryResult;
					break; }
				case "v4" : {
					result += arg4.QueryResult;
					break; }
				default : { 
					result += "(0.0)"; break;}
			}
			result += ".w);\n";*/
			
			result += arg1.QueryResult + "." + _xChannel.Selected + ", ";
			result += arg2.QueryResult + "." + _yChannel.Selected + ", ";
			result += arg3.QueryResult + "." + _zChannel.Selected + ", ";
			result += arg4.QueryResult + "." + _wChannel.Selected + ");\n";
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
		
			GUILayout.Label( "X Component:" );
			_xChannel.Value = GUILayout.SelectionGrid( _xChannel.Value, _xChannel.GridValues.ToArray(), _xChannel.GuiRowElementsNum );
			GUILayout.Label( "Y Component:" );
			_yChannel.Value = GUILayout.SelectionGrid( _yChannel.Value, _yChannel.GridValues.ToArray(), _yChannel.GuiRowElementsNum );
			GUILayout.Label( "Z Component:" );
			_zChannel.Value = GUILayout.SelectionGrid( _zChannel.Value, _zChannel.GridValues.ToArray(), _zChannel.GuiRowElementsNum );
			GUILayout.Label( "W Component:" );
			_wChannel.Value = GUILayout.SelectionGrid( _wChannel.Value, _wChannel.GridValues.ToArray(), _wChannel.GuiRowElementsNum );
		}
	}
}