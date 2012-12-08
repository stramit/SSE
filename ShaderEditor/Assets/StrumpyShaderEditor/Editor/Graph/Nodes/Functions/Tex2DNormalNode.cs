using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Tex2DNormal", "Function", typeof(Tex2DNormalNode),"Shorthand for Tex2D -> UnpackNormal, used to keep the graph slim when using both. No performance difference.")]
	public class Tex2DNormalNode : Node, IResultCacheNode{
		private const string NodeName = "Tex2DNormal";
		
		[DataMember] private Float4OutputChannel _normal;
		
		[DataMember] private Sampler2DInputChannel _sampler;
		[DataMember] private Float4InputChannel _uv;
		
		public Tex2DNormalNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_normal = _normal ?? new Float4OutputChannel( 0, "Normal" );
		
			_sampler = _sampler ?? new Sampler2DInputChannel( 0, "Sampler" );
			_uv = _uv ?? new Float4InputChannel( 1, "UV", Vector4.zero );
		}
		
		public override IEnumerable<string> IsValid ( SubGraphType graphType )
		{
			var errors = new List<string> ();
			if( graphType != SubGraphType.Pixel )
			{
				errors.Add( "Node not valid in graph type: " + graphType );
			}
			return errors;
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			var ret = new List<OutputChannel> {_normal};
			return ret;
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			var ret = new List<InputChannel> {_sampler, _uv};
			return ret;
		}
		
		public string GetAdditionalFields()
		{
			string result = "";
			if( IsInputChannelConnected( 0 ) )
			{
				var samplerInput = _sampler.ChannelInput( this );
				var uvInput = _uv.ChannelInput( this );
				
				result += samplerInput.AdditionalFields;
				result += uvInput.AdditionalFields;
			}
			return result;
		}
		
		public string GetUsage()
		{
			if( IsInputChannelConnected( 0 ) )
			{
				var samplerInput = _sampler.ChannelInput( this );
				var uvInput = _uv.ChannelInput( this );
				
				var result = "float4 ";
				result += UniqueNodeIdentifier;
				result += "=";
				result += "float4(UnpackNormal( tex2D(" + samplerInput.QueryResult + "," + uvInput.QueryResult +".xy)).xyz, 1.0 );\n";
				return result;
			}
			else
			{
				var result = "";
				result += "float4 ";
				result += UniqueNodeIdentifier;
				result += "=float4(0.0,0.0,0.0,0.0);\n";
				return result;
			}
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return UniqueNodeIdentifier;
		}
	}
}