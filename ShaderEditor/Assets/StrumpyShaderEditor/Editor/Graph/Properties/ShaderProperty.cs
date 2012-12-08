using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public abstract class ShaderProperty {
		[DataMember]
		protected string _propertyName = "";
		
		[DataMember]
		protected string _propertyDescription = "";
		
		[DataMember]
		protected int _propertyId;
		
		[DataMember]
		public bool Expanded = false;
		
		public ShaderProperty( )
		{
			Initialize();
		}
		
		public virtual void Initialize()
		{
			_propertyName = _propertyName ?? "";
			_propertyDescription = _propertyDescription ?? "";
			
			if( _propertyDescription.Length > 30 )
			{
				_propertyDescription = _propertyDescription.Substring(0, 30);
			}
		}
		
		public string PropertyName
		{
			get{ 
				if( _propertyName.StartsWith( "_" ) )
				{
					return _propertyName.RemoveWhiteSpace();
				}
				return "_" + _propertyName.RemoveWhiteSpace();
			}
			set{ _propertyName = value; }
		}
		
		public string PropertyDescription
		{
			get{ return string.IsNullOrEmpty(_propertyDescription) ? _propertyName : _propertyDescription; }
			set{ _propertyDescription = value; }
		}
		
		public string PropertyDescriptionDisplay
		{
			get{ return _propertyDescription; }
			set{ _propertyDescription = value.Length > 30 ? value.Substring(0, 30) : value; }
		}
		
		public int PropertyId
		{
			get{ 
				return _propertyId;
			}
			set{ _propertyId = value; }
		}
		
		public virtual void Draw()
		{
			GUILayout.Label( GetType().ToString() );
		}
		
		public virtual bool IsValid()
		{
			if( PropertyName == "_" )
				return false;
			
			return !string.IsNullOrEmpty(_propertyName);
		}
	
		public  string GetVariableDefinition()
		{
			var result = "";
			result += GetPropertyType().ShaderTypeString() + " ";
			result += PropertyName;
			result += ";\n";
			return result;
		}
		
		public abstract InputType GetPropertyType();
		public abstract string GetPropertyDefinition();
	}
}
