using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public abstract class LightingNode : Node{
		public override IEnumerable<string> IsValid ( SubGraphType graphType )
		{
			var errors = new List<string> ();
			if( graphType != SubGraphType.SimpleLighting )
			{
				errors.Add( "Node not valid in graph type: " + graphType );
			}
			return errors;
		}
	}
}
