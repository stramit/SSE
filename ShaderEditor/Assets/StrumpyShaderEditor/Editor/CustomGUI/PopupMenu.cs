using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System;

namespace StrumpyShaderEditor
{
	//An item in the menu that can get executed
	public class ExecutableMenuItem
	{
		public delegate void MenuExecute( Vector2 position );
		
		private readonly string _name;
		private readonly string _category;
		private readonly MenuExecute _execute;
		
		public readonly string Desc;
		
		public string Name{
			get{ return _name; }
		}
		
		public string Category{
			get{ return _category; }
		}
		
		public void Execute( Vector2 position )
		{
			_execute( position );
		}
		
		public ExecutableMenuItem( string category, string name, MenuExecute execute )
		{
			_category = category;
			_name = name;
			_execute = execute;
		}
		
		// Construct with descriptor
		public ExecutableMenuItem( string category, string name, MenuExecute execute, string descriptor) { 
			_category = category;
			_name = name;
			_execute = execute;
			Desc = descriptor;
		}
	}
	
	//The menu itself... has a list of items that get called via their deligate
	public class PopupMenu {
		private List<ExecutableMenuItem> _menuItems = new List<ExecutableMenuItem>();
		public void AddItem( ExecutableMenuItem item )
		{
			_menuItems.Add( item );
		}
		
		public void SortMenu()
		{
			_menuItems.Sort( delegate(ExecutableMenuItem item1, ExecutableMenuItem item2) { return item1.Name.CompareTo( item2.Name); } );
		}
		
		Vector2 lastShowPosition = Vector2.zero;
		public void Show( Vector2 mousePos, SubGraphType graphType )
		{
			lastShowPosition = mousePos;
			GenericMenu menu = new GenericMenu();
			
			foreach( var item in _menuItems )
			{
				if( (item.Category == "Vertex" && graphType != SubGraphType.Vertex )
				   || item.Category == "Lighting" && graphType != SubGraphType.SimpleLighting )
				{
					continue;
				}
				menu.AddItem(new GUIContent( item.Category + "/" + item.Name, item.Desc), false, HandleMenuClick, item);
			}
			
			menu.ShowAsContext();
		}
		
		private void HandleMenuClick( object arg )
		{
			var item = arg as ExecutableMenuItem;
			
			if( item != null )
			{
				item.Execute( lastShowPosition );
			}
		}
		
		//Filter stores the filter to be used for searching the nodes
		private string currentFilter = "";
		private Dictionary<string, bool> categoryState = new Dictionary<string, bool>();
		public void GuiPaneDraw( Vector2 paneStartPosition )
		{
			Vector2 SpawnPos = paneStartPosition + new Vector2( -170, 20 );
			currentFilter = EditorGUILayout.TextField("Filter:", currentFilter );
			
			var items = from item in _menuItems
						where item.Name.IndexOf(currentFilter, StringComparison.InvariantCultureIgnoreCase ) >= 0
						select item;
			
			var cats = (from item in items
						select item.Category).Distinct();
			
			foreach( var cat in cats )
			{
				if( !categoryState.ContainsKey( cat ) )
				{
					categoryState.Add( cat, true );
				}
				categoryState[cat] = EditorGUILayout.Foldout( categoryState[cat], cat );
				
				if( categoryState[cat] )
				{
					var itemsToDraw = items.Where( x => x.Category == cat );
					
					foreach( var item in itemsToDraw )
					{
						if( GUILayout.Button( new GUIContent( item.Name, item.Desc ) ) )
						{
							item.Execute( SpawnPos );
						}
					}
				}
			}
		}
	}
}