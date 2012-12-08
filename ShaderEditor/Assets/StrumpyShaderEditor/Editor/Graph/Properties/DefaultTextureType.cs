using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public enum DefaultTextureType
	{
		[EnumMember] Black,
		[EnumMember] White,
		[EnumMember] Gray,
		[EnumMember] Bump
	}
}
