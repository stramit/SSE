using UnityEngine;
using System;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public enum ShaderTarget {
		[EnumMember] Two,
		[EnumMember] Three
	}
	
	public static class ShaderTargetExtensions
	{
		public static string TargetString(this ShaderTarget targetEnum)
		{
			switch (targetEnum)
			{
				case ShaderTarget.Two:
					return "2.0";
				case ShaderTarget.Three:
					return "3.0";
				default:
					throw new Exception("Unsupported Type");
			}
		}
	}
 
}
