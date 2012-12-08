using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Swizzle", "Function", typeof(SwizzleNode),"Swizzle components of an input. This lets you rearrange and repeat components, common patterns are xyxy, zwzw, and xyzx. If your copying one value across the whole register, such as .xxxx, Splat is a node dedicated to that.")]
	public class SwizzleNode : Node, IResultCacheNode {
		private const string NodeName = "Swizzle";
		[DataMember] private EditorGroup _xChannel;
		[DataMember] private EditorGroup _yChannel;
		[DataMember] private EditorGroup _zChannel;
		[DataMember] private EditorGroup _wChannel;
		[DataMember] private Float4OutputChannel _result;
		[DataMember] private Float4InputChannel _vector;
		
		public SwizzleNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_xChannel = _xChannel ?? new EditorGroup( 0, new[] { "x", "y", "z", "w" }, 4 );
			_yChannel = _yChannel ?? new EditorGroup( 1, new[] { "x", "y", "z", "w" }, 4 );
			_zChannel = _zChannel ?? new EditorGroup( 2, new[] { "x", "y", "z", "w" }, 4 );
			_wChannel = _wChannel ?? new EditorGroup( 3, new[] { "x", "y", "z", "w" }, 4 );
			_result = _result ?? new Float4OutputChannel( 0, "Result" );
			_vector = _vector ?? new Float4InputChannel( 1, "Vector", Vector4.zero );
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string DisplayName {
			get { return NodeName + "(" + (_xChannel.Selected + _yChannel.Selected + _zChannel.Selected + _wChannel.Selected).ToUpper() + ")"; }
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_result};
			return ret;
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			var ret = new List<InputChannel> {_vector};
			return ret;
		}
		
		public string GetAdditionalFields()
		{
			var arg1 = _vector.ChannelInput( this );
			return arg1.AdditionalFields;
		}
		
		public string GetUsage()
		{
			var arg1 = _vector.ChannelInput( this );
			var result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=";
			var channelName = arg1.QueryResult;
			result += "float4(" + channelName + "." + _xChannel.Selected + ", " +
								channelName + "." +_yChannel.Selected + ", " +
								channelName + "." + _zChannel.Selected + ", " +
								channelName + "." + _wChannel.Selected + ");\n";
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
		
			GUILayout.Label( "Swizzle 1" );
			_xChannel.Value = GUILayout.SelectionGrid( _xChannel.Value, _xChannel.GridValues.ToArray(), _xChannel.GuiRowElementsNum );
			GUILayout.Label( "Swizzle 2" );
			_yChannel.Value = GUILayout.SelectionGrid( _yChannel.Value, _yChannel.GridValues.ToArray(), _yChannel.GuiRowElementsNum );
			GUILayout.Label( "Swizzle 3" );
			_zChannel.Value = GUILayout.SelectionGrid( _zChannel.Value, _zChannel.GridValues.ToArray(), _zChannel.GuiRowElementsNum );
			GUILayout.Label( "Swizzle 4" );
			_wChannel.Value = GUILayout.SelectionGrid( _wChannel.Value, _wChannel.GridValues.ToArray(), _wChannel.GuiRowElementsNum );
		}
	}
}