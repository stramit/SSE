using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Vertex Reflection", "Input", typeof(SimpleWorldReflectionNode),"Reflection vector, used together with TexCube for reflections. This does not take the normal map into account, and is accordingly not for detailed reflections, however, unlike the complex alternative, this does not require Shader Model 3.")]
	public class SimpleWorldReflectionNode : Node, IStructInput{
		private const string NodeName = "SimpleWorldReflection";
		
		[DataMember] private Float4OutputChannel _reflection;
		
		public SimpleWorldReflectionNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			_reflection = _reflection ?? new Float4OutputChannel( 0, "Reflection" );
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
			return new List<InputChannel> ();
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
			
		public StructTypes GetStructFieldType()
		{
			return StructTypes.Float3;
		}
		
		public string GetStructFieldName()
		{
			return "simpleWorldRefl";
		}
		
		public string GetStructFieldDefinition()
		{
			return GetStructFieldType().ShaderTypeString() + " " + GetStructFieldName() + ";\n";
		}
		
		public string GetStructVertexShaderString()
		{
			return "o." + GetStructFieldName() + " = -reflect( normalize(WorldSpaceViewDir(v.vertex)), normalize(mul((float3x3)_Object2World, SCALED_NORMAL)));\n";
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return this.GenerateInputUsageString();
		}
		
		public bool RequiresStructFieldInclusion()
		{
			return IsOutputChannelConnected( 0 );
		}
	}
}
