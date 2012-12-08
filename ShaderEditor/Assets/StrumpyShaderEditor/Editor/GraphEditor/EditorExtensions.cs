using UnityEngine;using UnityEditor;using System.Reflection;using System.Collections.Generic;using System.Linq;namespace StrumpyShaderEditor{public static class EditorExtensions{		public static void DrawMeshOnly(this Camera cam)		{			typeof(Handles).InvokeMember( "SetCameraOnlyDrawMesh",											BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic,											null,											null,											new object[]{cam});		}				public static string RemoveWhiteSpace( this string s)		{			var result = s.Replace( " ", "" );			result = result.Replace( "\r", "" );			result = result.Replace( "\n", "" );			result = result.Replace( "\t", "" );			return result;		}
		
		// Texel - Extending GUISkin to refer to some of our common styles so we don't have to keep regenerating them
		private static GUIStyle _buttonLeft = null;
		public static GUIStyle buttonLeft (this GUISkin g){
			if (_buttonLeft == null)
				_buttonLeft = GUI.skin.FindStyle("ButtonLeft");
			return _buttonLeft;
		}
		
		private static GUIStyle _buttonMid = null;
		public static GUIStyle buttonMid (this GUISkin g){
			if (_buttonMid == null)
				_buttonMid = GUI.skin.FindStyle("ButtonMid");
			return _buttonMid;
		}
		
		private static GUIStyle _buttonRight = null;
		public static GUIStyle buttonRight (this GUISkin g){
			if (_buttonRight == null)
				_buttonRight = GUI.skin.FindStyle("ButtonRight");
			return _buttonRight;
		}
		
		// And the "On" styles
		
		private static GUIStyle _buttonLeftOn = null;
		public static GUIStyle buttonLeftOn (this GUISkin g){
			if (_buttonLeftOn == null) {
				_buttonLeftOn = new GUIStyle(GUI.skin.buttonLeft());
				_buttonLeftOn.active = _buttonLeftOn.onActive;
				_buttonLeftOn.normal = _buttonLeftOn.onNormal;
				_buttonLeftOn.hover  = _buttonLeftOn.onHover;
			}
			return _buttonLeftOn;
		}
		
		private static GUIStyle _buttonMidOn = null;
		public static GUIStyle buttonMidOn (this GUISkin g){
			if (_buttonMidOn == null) {
				_buttonMidOn = new GUIStyle(GUI.skin.buttonMid());
				_buttonMidOn.active = _buttonMidOn.onActive;
				_buttonMidOn.normal = _buttonMidOn.onNormal;
				_buttonMidOn.hover  = _buttonMidOn.onHover;
			}
			return _buttonMidOn;
		}
		
		private static GUIStyle _buttonRightOn = null;
		public static GUIStyle buttonRightOn (this GUISkin g){
			if (_buttonRightOn == null) {
				_buttonRightOn = new GUIStyle(GUI.skin.buttonRight());
				_buttonRightOn.active = _buttonRightOn.onActive;
				_buttonRightOn.normal = _buttonRightOn.onNormal;
				_buttonRightOn.hover  = _buttonRightOn.onHover;
			}
			return _buttonRightOn;
		}
		
		// Texel - Extended togglebutton for tooltips, overload for backwards compatibility
		public static bool ToggleButton(bool value, string label) {
			return ToggleButton(value,label,"");
		}
		
		public static bool ToggleButton(bool value, string label, string tooltip) {
			GUIStyle state = new GUIStyle(GUI.skin.button);
			
			if (value)
				state.normal = state.onNormal;
			
			GUIContent content = new GUIContent(label,tooltip);
			
			bool pressed = GUILayout.Button(content,state);
			
			if (pressed)
				return !value;
			return value;
		}				public static bool Contains( this Rect rect, Rect other )		{			var points = new List<Vector2> {				new Vector2( other.xMin, other.yMin ),				new Vector2( other.xMax, other.yMin ),				new Vector2( other.xMin, other.yMax ),				new Vector2( other.xMax, other.yMax ) };						if( points.All( x => rect.Contains( x ) ) )			{				return true;			}			return false;		}				public static bool OverLaps( this Rect rect, Rect other )		{			var points = new List<Vector2> {				new Vector2( other.xMin, other.yMin ),				new Vector2( other.xMax, other.yMin ),				new Vector2( other.xMin, other.yMax ),				new Vector2( other.xMax, other.yMax ) };						if( points.Any( x => rect.Contains( x ) ) )			{				return true;			}			return false;		}	}}