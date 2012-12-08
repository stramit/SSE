using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public struct ChannelQueryResult {
		public string AdditionalFields;
		public string QueryResult;
	}
}
