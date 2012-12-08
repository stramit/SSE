using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("World Reflection", "Input", typeof(WorldReflectionNode),"Generate a reflection value using a normal map. If you are not using a normal map, for instance with smooth reflections, use Simple Reflection instead. This function generally requires Shader Model 3 due to additional information needed in it's computation.")]
	public class WorldReflectionNode : Node, IStructInput, IResultCacheNode{
		private const string NodeName = "WorldReflection";
		
		[DataMember] private Float4OutputChannel _reflection;
		[DataMember] private Float4InputChannel _normal;
		
		public WorldReflectionNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_reflection = _reflection ?? new Float4OutputChannel( 0, "Reflection" );
			_normal = _normal ?? new Float4InputChannel( 0, "Normal", new Vector4( 0f, 0f, 1f, 1f ) );
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

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> {_reflection};
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			return new List<InputChannel> { _normal };
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override bool RequiresInternalData
		{
			get{ return true; }
		}
			
		public StructTypes GetStructFieldType()
		{
			return StructTypes.Float3;
		}
		
		public string GetStructFieldName()
		{
			return "worldRefl";
		}
		
		public string GetStructFieldDefinition()
		{
			return GetStructFieldType().ShaderTypeString() + " " + GetStructFieldName() + ";\n";
		}
		
		public string GetStructVertexShaderString()
		{
			return "";
		}

		public string GetAdditionalFields()
		{
			var arg1 = _normal.ChannelInput( this );
			return arg1.AdditionalFields;
		}
		
		public string GetUsage()
		{
			var arg1 = _normal.ChannelInput( this );
			
			var result = "float4 ";
			result += UniqueNodeIdentifier;
			result += "=";
			result += "float4( WorldReflectionVector (IN, " + arg1.QueryResult + "), 1.0);\n";
			return result;
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return UniqueNodeIdentifier;
		}
		
		
		public bool RequiresStructFieldInclusion()
		{
			return IsOutputChannelConnected( 0 );
		}
	}
}
