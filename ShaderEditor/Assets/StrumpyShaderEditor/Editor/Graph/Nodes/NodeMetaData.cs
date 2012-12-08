using System;
using System.Reflection;

namespace StrumpyShaderEditor
{
	// Texel : Modified for Descriptor addition
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class NodeMetaData : Attribute{
		public string DisplayName;
		public string Category;
		
		public string Descriptor = "";
		
		public Type NodeType;
		
		public bool isComplex = false;
		
		public NodeMetaData(string displayName, string category, Type nodeType )
		{
			DisplayName = displayName;
			NodeType = nodeType;
			Category = category;
		}
		
		// Texel: Overloads
		
		public NodeMetaData(string displayName, string category, Type nodeType, string descriptor )
		{
			DisplayName = displayName;
			NodeType = nodeType;
			Category = category;
			Descriptor = descriptor;
		}
		
		public NodeMetaData(string displayName, string category, Type nodeType, string descriptor, bool complex )
		{
			DisplayName = displayName;
			NodeType = nodeType;
			Category = category;
			Descriptor = descriptor;
			isComplex = complex;
		}
		
		public NodeMetaData(string displayName, Type nodeType) {
			DisplayName = displayName;
			NodeType = nodeType;
			Category = "Unsorted";
		}
		
		public NodeMetaData(string displayName, Type nodeType, bool complex) {
			DisplayName = displayName;
			NodeType = nodeType;
			Category = "Unsorted";
			isComplex = complex;
		}

		
		public NodeMetaData(Type nodeType) {
			DisplayName = nodeType.Name;
			Category = "Unsorted";
			NodeType = nodeType;
		}
		
		public NodeMetaData(Type nodeType, bool complex) {
			DisplayName = nodeType.Name;
			Category = "Unsorted";
			NodeType = nodeType;
			isComplex = complex;
		}
	}
}
