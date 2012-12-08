using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public enum FogMode {
		[EnumMember] Global,
		[EnumMember] Off,
		[EnumMember] Linear,
		[EnumMember] Exp,
		[EnumMember] Exp2
	}
}
