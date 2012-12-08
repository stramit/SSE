using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class MatrixInputChannel : InputChannel {
		[DataMember] private EditorMatrix defaultValue;
		public MatrixInputChannel( uint id, string name, Matrix4x4 defaultValue ) : base( id, name )
		{
			this.defaultValue = defaultValue;
		}
		
		public override TypeEnum ChannelType
		{
			get{ return TypeEnum.Matrix; }
		}
		
		public override string GetDefaultInput( Node parent )
		{
			string result = "float4x4 " + GetDefaultInputName( parent ) + " = "; 
			result += "float4x4(";
			result += defaultValue.m00 + ",";
			result += defaultValue.m01 + ",";
			result += defaultValue.m02 + ",";
			result += defaultValue.m03 + ",";
			result += defaultValue.m10 + ",";
			result += defaultValue.m11 + ",";
			result += defaultValue.m12 + ",";
			result += defaultValue.m13 + ",";
			result += defaultValue.m20 + ",";
			result += defaultValue.m21 + ",";
			result += defaultValue.m22 + ",";
			result += defaultValue.m23 + ",";
			result += defaultValue.m30 + ",";
			result += defaultValue.m31 + ",";
			result += defaultValue.m32 + ",";
			result += defaultValue.m33 + ");\n";
			return result;
		}
	}
}
