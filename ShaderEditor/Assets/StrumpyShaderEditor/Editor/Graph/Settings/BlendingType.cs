using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public enum BlendingType {
		[EnumMember] Default, 
		[EnumMember] Custom, 
		[EnumMember] DecalAdd, 
		[EnumMember] DecalBlend
	}
}
