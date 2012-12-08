using System;
using System.Collections.Generic;

namespace StrumpyShaderEditor
{
	public class CircularReferenceException : Exception {
		public string CausalNodeId;
		public Stack<string> CircularTrace;
	}
}
