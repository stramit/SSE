using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public enum ExcludePath {
		[EnumMember] None,
		[EnumMember] Forward,
		[EnumMember] Prepass
	}
}
