using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Mask", "Function", typeof(MaskNode),"Explicitly remove components from input, this works with the compiler and will select the proper internal instructions where applicable, so don't worry about the cost of using it. You will mostly be using it to trim off the fourth component where it could cause problems, like with Normalize, or Distance")]
	public class MaskNode : Node, IResultCacheNode {
		private const string NodeName = "Mask";
		[DataMember] private EditorBool _xMask;
		[DataMember] private EditorBool _yMask;
		[DataMember] private EditorBool _zMask;
		[DataMember] private EditorBool _wMask;
		
		[DataMember] private Float4OutputChannel _result;
		[DataMember] private Float4InputChannel _vector;
		
		public MaskNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_xMask = _xMask ?? new EditorBool();
			_yMask = _yMask ?? new EditorBool();
			_zMask = _zMask ?? new EditorBool();
			_wMask = _wMask ?? new EditorBool();
		
			_result = _result ?? new Float4OutputChannel( 0, "Result" );
			_vector = _vector ?? new Float4InputChannel( 0, "Vector", Vector4.zero );
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
			string result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=";
			result += "float4(";
			result += (_xMask ? arg1.QueryResult + ".x" : "0.0") + ",";
			result += (_yMask ? arg1.QueryResult + ".y" : "0.0") + ",";
			result += (_zMask ? arg1.QueryResult + ".z" : "0.0") + ",";
			result += (_wMask ? arg1.QueryResult + ".w" : "0.0") + ");\n";
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
			_xMask.Value = EditorGUILayout.Toggle( "Mask x", _xMask.Value );
			_yMask.Value = EditorGUILayout.Toggle( "Mask y", _yMask.Value );
			_zMask.Value = EditorGUILayout.Toggle( "Mask z", _zMask.Value );
			_wMask.Value = EditorGUILayout.Toggle( "Mask w", _wMask.Value );
		}
	}
}