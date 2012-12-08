using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Parallax", "Function", typeof(ParallaxNode),"Compute the Parallax distortion for the fragment given a view direction and surface height. Needs to be combined with the UV for the visual effect, so ideally the displaced textures do not contain the height, since that would have to be resampled. Bias defaults to zero, and neither have to be positive. If you clamp the result, you will prevent many discrepencies at the cost of removing the parallax for small detail. Not very expensive to compute, combines well with Fresnel.")]
	public class ParallaxNode : Node, IResultCacheNode {
		private const string NodeName = "ParallaxOffset";
		
		[DataMember] private Float4OutputChannel _result;
		[DataMember] private Float4InputChannel _view;
		[DataMember] private Float4InputChannel _scale;
		[DataMember] private Float4InputChannel _bias;
		
		public ParallaxNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_result = _result ?? new Float4OutputChannel( 0, "Result" );
			_result.DisplayName = "Result";
			_view = _view ?? new Float4InputChannel( 0, "View", Vector4.zero );
			_view.DisplayName = "View";
			_scale = _scale ?? new Float4InputChannel( 1, "Height", new Vector4(0.0f,0.0f,0.0f,0.0f) );
			_scale.DisplayName = "Height";
			_bias = _bias ?? new Float4InputChannel( 2, "Bias", new Vector4(0.0f,0.0f,0.0f,0.0f) );
			_bias.DisplayName = "Bias";
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> {_result};
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			return new List<InputChannel> {_view, _scale, _bias};
		}
		
		public string GetAdditionalFields()
		{
			var arg1 = _view.ChannelInput( this );
			var arg2 = _scale.ChannelInput( this );
			var arg3 = _bias.ChannelInput( this );
			var ret = arg1.AdditionalFields;
			ret += arg2.AdditionalFields;
			ret += arg3.AdditionalFields;
			return ret;
		}
		
		public string GetUsage()
		{
			var arg1 = _view.ChannelInput( this );
			var arg2 = _scale.ChannelInput( this );
			var arg3 = _bias.ChannelInput( this );
			
			var result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=";
			result += " ParallaxOffset( " + arg2.QueryResult + ".x, " + arg3.QueryResult + ".x, " + arg1.QueryResult + ".xyz).xyxy";
			result += ";\n";
			return result;
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return UniqueNodeIdentifier;
		}
	}
}
