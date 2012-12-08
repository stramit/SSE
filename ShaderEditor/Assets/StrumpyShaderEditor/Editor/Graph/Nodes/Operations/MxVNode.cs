using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("MxV", "Operation", typeof(MxVNode),"Multiply a Matrix with a Vertex. Equivalent to translating it by that vertex, this is mostly used internally to vertex shaders, but may be of use for transfering values to the pixel graph.")]
	public class MxVNode : Node, IResultCacheNode {
		private const string NodeName = "MxV";
		
		[DataMember] private Float4OutputChannel _result;
		[DataMember] private MatrixInputChannel _matrix;
		[DataMember] private Float4InputChannel _vector;
		
		public MxVNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_result = _result ?? new Float4OutputChannel( 0, "Result" );
			_matrix = _matrix ?? new MatrixInputChannel( 0, "Matrix", Matrix4x4.identity );
			_vector = _vector ?? new Float4InputChannel( 1, "Vector", Vector4.zero );
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_result};
			return ret;
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			var ret = new List<InputChannel> { _matrix,_vector};
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
			var arg1 = _matrix.ChannelInput( this );
			var arg2 = _vector.ChannelInput( this );
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