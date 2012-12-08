using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Fresnel", "Function", typeof(FresnelNode),"When provided with a view angle and optional normalmap input, this function will return zero for points of the surface facing the camera, and one for points facing perpendicuarly.")]
	public class FresnelNode : Node, IResultCacheNode {
		private const string NodeName = "Fresnel";
		
		[DataMember] private Float4OutputChannel _result;
		[DataMember] private Float4InputChannel _view;
		[DataMember] private Float4InputChannel _normal;
		
		public FresnelNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_result = _result ?? new Float4OutputChannel( 0, "Result" );
			_result.DisplayName = "Result";
			_view = _view ?? new Float4InputChannel( 0, "View", Vector4.zero );
			_view.DisplayName = "View";
			_normal = _normal ?? new Float4InputChannel( 1, "Normal", new Vector4(0.0f,0.0f,1.0f,1.0f) );
			_normal.DisplayName = "Normal";
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
			var ret = new List<InputChannel> {_view, _normal};
			return ret;
		}
		
		public string GetAdditionalFields()
		{
			var arg1 = _view.ChannelInput( this );
			var arg2 = _normal.ChannelInput( this );
			var ret = arg1.AdditionalFields;
			ret += arg2.AdditionalFields;
			return ret;
		}
		
		public string GetUsage()
		{
			var arg1 = _view.ChannelInput( this );
			var arg2 = _normal.ChannelInput( this );
			var result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=";
			//1- Dot( normalize(normal), normalise(viewdir) )
			result += "(1.0 - dot( ";
			result += "normalize( " + arg1.QueryResult + ".xyz), ";
			result += "normalize( " + arg2.QueryResult + ".xyz ) )).xxxx;\n";
			return result;
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return UniqueNodeIdentifier;
		}
	}
}