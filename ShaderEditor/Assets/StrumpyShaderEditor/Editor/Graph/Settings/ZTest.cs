using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public enum ZTest {
		[EnumMember] LEqual,
		[EnumMember] Less,
		[EnumMember] Greater,
		[EnumMember] GEqual,
		[EnumMember] Equal,
		[EnumMember] NotEqual,
		[EnumMember] Always
	}
}
