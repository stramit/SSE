using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Float4 Const", "Constant", typeof(Float4ConstNode),"Constant vector set by user (x,y,z,w)")]
	public class Float4ConstNode : Node {
		private const string NodeName = "Float4Const";
		[DataMember] private EditorFloat4 _float4Value;
		[DataMember] private Float4OutputChannel _constValue;
		
		public Float4ConstNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_float4Value = _float4Value ?? new EditorFloat4();
			_constValue = _constValue ?? new Float4OutputChannel( 0, "Value" );
			_constValue.DisplayName = "Value";
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		private string channelDisplayName { 
			get { 
				return "(" + _float4Value.X.ToString("G4") + 
					  "," + _float4Value.Y.ToString("G4") + 
					  "," + _float4Value.Z.ToString("G4") + 
					  "," + _float4Value.W.ToString("G4") + ")"; }
		}
		
		public override string DisplayName {
			get {
				return NodeName.PadRight(channelDisplayName.Length,"~"[0]); // Make it about the same length using padding character trimmed after width calculation
			}
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			_constValue.DisplayName = channelDisplayName;
			
			var ret = new List<OutputChannel> {_constValue};
			return ret;
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			return new List<InputChannel>();
		}
		
		public override string GetExpression( uint channelId )
		{
			return "float4( " 
					+ _float4Value.X + ","
					+ _float4Value.Y + ","
					+ _float4Value.Z + ","
					+ _float4Value.W + ")";
		}
		
		public override void DrawProperties()
		{
			base.DrawProperties();
			_float4Value.Value = EditorGUILayout.Vector4Field( "Value:", _float4Value.Value );
		}
	}
}