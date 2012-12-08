using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class Float4InputChannel : InputChannel {
		[DataMember] private EditorFloat4 defaultValue;
		public Float4InputChannel( uint id, string name, Vector4 defaultValue ) : base( id, name )
		{
			this.defaultValue = defaultValue;
		}
		
		public override TypeEnum ChannelType
		{
			get{ return TypeEnum.Float4; }
		}
		
		public override string GetDefaultInput( Node parent )
		{
			string result = "float4 " + GetDefaultInputName( parent ) + " = "; 
			result += "float4(";
			result += defaultValue.X + ",";
			result += defaultValue.Y + ",";
			result += defaultValue.Z + ",";
			result += defaultValue.W + ");\n";
			return result;
		}
	}
}
