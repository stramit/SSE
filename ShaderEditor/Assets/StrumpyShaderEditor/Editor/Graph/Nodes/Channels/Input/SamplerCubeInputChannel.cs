using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class SamplerCubeInputChannel : InputChannel {
		public SamplerCubeInputChannel( uint id, string name ) : base( id, name )
		{}
		
		public override TypeEnum ChannelType
		{
			get{ return TypeEnum.SamplerCube; }
		}
		
		public override string GetDefaultInput( Node parent )
		{
			throw new UnityException( "Default not supported on: " + GetType() );
		}
	}
}
