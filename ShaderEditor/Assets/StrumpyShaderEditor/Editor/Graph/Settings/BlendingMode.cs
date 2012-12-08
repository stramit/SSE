using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public enum BlendingMode {
		[EnumMember] Zero,
		[EnumMember] One,
		[EnumMember] SrcColor,
		[EnumMember] SrcAlpha,
		[EnumMember] DstColor,
		[EnumMember] DstAlpha,
		[EnumMember] OneMinusSrcColor,
		[EnumMember] OneMinusSrcAlpha,
		[EnumMember] OneMinusDstColor,
		[EnumMember] OneMinusDstAlpha
	}
}
