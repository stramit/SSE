using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("VxM", "Operation", typeof(VxMNode),"Multiply a Vector by a Matrix. It should be noted this is not the same as MxV. This is primarily used in vertex programs, see also MxV")]
	public class VxMNode : Node, IResultCacheNode {
		private const string NodeName = "VxM";
		
		[DataMember] private Float4OutputChannel _result;
		[DataMember] private MatrixInputChannel _matrix;
		[DataMember] private Float4InputChannel _vector;
		
		public VxMNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_result = _result ?? new Float4OutputChannel( 0, "Result" );
			_vector = _vector ?? new Float4InputChannel( 0, "Vector", Vector4.zero );
			_matrix = _matrix ?? new MatrixInputChannel( 1, "Matrix", Matrix4x4.identity );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_result};
			return ret;
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			var ret = new List<InputChannel> {_vector, _matrix};
			return ret;
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public string GetAdditionalFields()
		{
			var arg1 = _vector.ChannelInput( this );
			var arg2 = _matrix.ChannelInput( this );
			var ret = arg1.AdditionalFields;
			ret += arg2.AdditionalFields;
			return ret;
		}
		
		public string GetUsage()
		{
			var arg1 = _vector.ChannelInput( this );
			var arg2 = _matrix.ChannelInput( this );
			var result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=";
			result += "mul( ";
			result += arg1.QueryResult + ", ";
			result += arg2.QueryResult + " );\n";
			return result;
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return UniqueNodeIdentifier;
		}
	}
}