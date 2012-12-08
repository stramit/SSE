using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("MxM", "Operation", typeof(MxMNode),"Multiply a matrix by another matrix. For use with vertex shaders primarily, relatively little use unless you know what your doing.")]
	public class MxMNode : Node, IResultCacheNode {
		private const string NodeName = "MxM";
		
		[DataMember] private MatrixOutputChannel _result;
		[DataMember] private MatrixInputChannel _matrix1;
		[DataMember] private MatrixInputChannel _matrix2;
		
		public MxMNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_result = _result ?? new MatrixOutputChannel( 0, "Result" );
			_matrix1 = _matrix1 ?? new MatrixInputChannel( 0, "Matrix1", Matrix4x4.identity );
			_matrix2 = _matrix2 ?? new MatrixInputChannel( 1, "Matrix2", Matrix4x4.identity );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_result};
			return ret;
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			var ret = new List<InputChannel> {_matrix1, _matrix2};
			return ret;
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public string GetAdditionalFields()
		{
			var arg1 = _matrix1.ChannelInput( this );
			var arg2 = _matrix2.ChannelInput( this );
			var ret = arg1.AdditionalFields;
			ret += arg2.AdditionalFields;
			return ret;
		}
		
		public string GetUsage()
		{
			var arg1 = _matrix1.ChannelInput( this );
			var arg2 = _matrix2.ChannelInput( this );
			var result = "float4x4 ";
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