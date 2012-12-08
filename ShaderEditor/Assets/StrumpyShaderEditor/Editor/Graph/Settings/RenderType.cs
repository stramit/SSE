using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public enum RenderType {
		[EnumMember] Opaque,
		[EnumMember] Transparent,
		[EnumMember] TransparentCutout,
		[EnumMember] Background,
		[EnumMember] Overlay,
		[EnumMember] TreeOpaque,
		[EnumMember] TreeTransparentCutout,
		[EnumMember] TreeBillboard,
		[EnumMember] Grass,
		[EnumMember] GrassBillboard,
		[EnumMember] Custom
	}
}
