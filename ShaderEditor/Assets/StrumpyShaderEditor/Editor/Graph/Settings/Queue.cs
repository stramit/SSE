using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public enum Queue {
		[EnumMember] Geometry,
		[EnumMember] Background,
		[EnumMember] Transparent,
		[EnumMember] Overlay
	}
}
