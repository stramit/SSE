using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public abstract class InputNode : Node{
		[DataMember] private EditorInt _inputId;
		
		//Beta 3->4 compatability
		//TODO: Remove in B5
		[DataMember] protected EditorString _inputName;
		protected abstract ShaderProperty InputPropertyFromOldInput();
		protected abstract bool OldPropertyConfigured();
		protected abstract void RemoveObsoleteInputConfig();
		
		public override void Initialize()
		{
			_inputId = _inputId ?? -1;
			
			if( OldPropertyConfigured() )
			{
				var validProps = Owner.FindValidProperties( GetFieldType() );
				var found = false;
				foreach( var property in validProps )
				{
					if( property.PropertyName == _inputName.Value 
					   || property.PropertyName == ("_" + _inputName.Value) )
					{
						_inputId = property.PropertyId;
						found = true;
						break;
					}
				}
				
				if( !found )
				{
					//Create new property...
					_inputId = Owner.AddProperty( InputPropertyFromOldInput() );
				}
				RemoveObsoleteInputConfig();
			}
		}

		protected abstract ShaderProperty NewPropertyInstance();

		protected ShaderProperty ReferencedProperty()
		{
			var validIDs = Owner.FindValidProperties( GetFieldType() );
			validIDs = from id in validIDs
				where id.PropertyId == _inputId
					select id;
				
			if( validIDs.Count( x => x.PropertyId == _inputId.Value ) <= 0 )
			{
				return null;
			}
			return validIDs.FirstOrDefault();
		}

		protected string GetFieldName()
		{
			var property = ReferencedProperty();
			if( property == null )
			{
				return "Not Configured";
			}
			return property.PropertyName;
		}
		
		public override IEnumerable<string> IsValid( SubGraphType graphType )
		{
			var errors = new List<string> ();
			
			var validIDs = Owner.FindValidProperties( GetFieldType() );
			if( validIDs.Count( x => x.PropertyId == _inputId.Value ) <= 0 )
			{
				errors.Add( "Not Connected to valid Property" );
			}
			
			return errors;
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return GetFieldName();
		}

		protected abstract InputType GetFieldType();
		
		private string _newInputName = "";
		public override void DrawProperties()
		{
			_newInputName = _newInputName ?? "";
			base.DrawProperties();
			
			var validIDs = Owner.FindValidProperties( GetFieldType() ).ToList();
			
			var propertyNames = (from property in validIDs select property.PropertyName).ToList();
			var propertyValues = (from property in validIDs select property.PropertyId).ToList();
			
			propertyNames.Insert( 0, "Unconfigured" );
			propertyValues.Insert( 0, -1 );
			_inputId.Value = EditorGUILayout.IntPopup( "Input Property", _inputId.Value, propertyNames.ToArray(), propertyValues.ToArray() );
			
			if( ReferencedProperty() == null )
			{
				GUILayout.Label( "Add new input?" );
				GUILayout.BeginHorizontal();
				_newInputName = GUILayout.TextField( _newInputName, GUILayout.Width( 200 ) );
				GUILayout.FlexibleSpace();
				if( GUILayout.Button( "Add", GUILayout.Width( 75 ) ) )
				{
					if( !string.IsNullOrEmpty( _newInputName ) )
					{
						var prop = NewPropertyInstance();
						prop.PropertyName = _newInputName;
						_inputId = Owner.AddProperty( prop );
					}
					else
					{
						EditorUtility.DisplayDialog("No Name specified", "You must specify an input name.", "Ok");
					}
				}
				GUILayout.EndHorizontal();
			}
		}
		
		public override string DisplayName {
			get { return NodeTypeName + ": " + GetFieldName(); }
		}
	}
}
