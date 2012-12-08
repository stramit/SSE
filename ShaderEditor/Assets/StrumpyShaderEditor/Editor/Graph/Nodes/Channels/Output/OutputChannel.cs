using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public abstract class OutputChannel : Channel {
		public OutputChannel( uint id, string name ) : base( id, name )
		{}
	}
}