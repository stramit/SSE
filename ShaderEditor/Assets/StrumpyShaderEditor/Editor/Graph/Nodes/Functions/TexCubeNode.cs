using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("TexCube", "Function", typeof(TexCubeNode),"Cubic texture read. Supplied with a direction (See World Normal, World Reflection) it will sample read from the cubemap in that direction. Used for reflections and fake lighting.")]
	public class TexCubeNode : Node, IResultCacheNode{
		private const string NodeName = "TexCUBE";
		
		[DataMember] private Float4OutputChannel _rgba;
		[DataMember] private Float4OutputChannel _a;
		
		[DataMember] private SamplerCubeInputChannel _sampler;
		[DataMember] private Float4InputChannel _normal;
		
		public TexCubeNode()
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_rgba = _rgba ?? new Float4OutputChannel( 0, "RGBA" );
			_a = _a ?? new Float4OutputChannel( 1, "A" );
		
			_sampler = _sampler ?? new SamplerCubeInputChannel( 0, "Sampler" );
			_normal = _normal ?? new Float4InputChannel( 1, "Normal", new Vector4(0f,0f,1f,1f) );
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
			var ret = new List<OutputChannel> {_rgba, _a};
			return ret;
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			var ret = new List<InputChannel> {_sampler, _normal};
			return ret;
		}
		
		public string GetAdditionalFields()
		{
			var result = "";
			if( IsInputChannelConnected( 0 ) )
			{
				var samplerInput = _sampler.ChannelInput( this );
				var normalInput = _normal.ChannelInput( this );
				
				result += samplerInput.AdditionalFields;
				result += normalInput.AdditionalFields;
			}
			return result;
		}
		
		public string GetUsage()
		{
			if( IsInputChannelConnected( 0 ) )
			{
				var samplerInput = _sampler.ChannelInput( this );
				var normalInput = _normal.ChannelInput( this );
				
				var result = "float4 ";
				result += UniqueNodeIdentifier;
				result += "=";
				result += "texCUBE(" + samplerInput.QueryResult + "," + normalInput.QueryResult +");\n";
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
			
			if( channelId == 0 )
			{
				return UniqueNodeIdentifier;
			}
			return UniqueNodeIdentifier + ".aaaa";
		}
	}
}