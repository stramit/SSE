using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class MatrixProperty : ShaderProperty
	{
		public MatrixProperty()
		{
		}
		
		public override void Draw()
		{
		}

		public override InputType GetPropertyType() 
		{
			return InputType.Matrix;
		}
		
		public override string GetPropertyDefinition()
		{
			return "";
		}
	}
}
