using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class MatrixOutputChannel : OutputChannel {
		public MatrixOutputChannel( uint id, string name ) : base( id, name )
		{}
		
		public override TypeEnum ChannelType
		{
			get{ return TypeEnum.Matrix; }
		}
		
	}
}