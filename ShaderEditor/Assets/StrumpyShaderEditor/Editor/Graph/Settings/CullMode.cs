using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public enum CullMode {
		[EnumMember] Back,
		[EnumMember] Front,
		[EnumMember] Off
	}
}
