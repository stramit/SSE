using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

namespace StrumpyShaderEditor
{
	public class NodeEditor : EditorWindow
	{
		private Vector3 _currentMousePosition = Vector3.zero;
		
		public OutputChannelReference SelectedOutputChannel { set; private get; }
		public InputChannelReference SelectedInputChannel { set; private get; }

		private ShaderGraph _nextGraph;
		private ShaderGraph _selectedGraph;
		
		private readonly List<Type> _serializableTypes = new List<Type>();

		private readonly string _shaderEditorResourceDir;
		private readonly string _autosavePath;
		private readonly string _internalTempDir;
		private readonly string _internalTempUnityPath;
		private readonly string _tempShaderPathFull;
		private readonly string _shaderTemplatePath;
		private readonly string _graphsDir;

		private readonly PopupMenu _popupMenu;

		private const string TempShaderName = "TempShader";
		
		private Node _selectedNode;
		private Node NextSelectedNode;
		
		private GraphHistory _undoChain;
		
		private SubGraphType _currentSubGraphType = SubGraphType.Pixel;
		
		private PreviewWindowInternal _previewWindow;
		
		private bool _shouldOpenPreviewWindow = true;
		
		protected NodeEditor( )
		{
			_shaderEditorResourceDir = Application.dataPath
												+ Path.DirectorySeparatorChar
												+ "StrumpyShaderEditor"
												+ Path.DirectorySeparatorChar
												+ "Editor"
												+ Path.DirectorySeparatorChar
												+ "Resources"
												+ Path.DirectorySeparatorChar;
			_internalTempDir = "Internal"
							+ Path.DirectorySeparatorChar
							+ "Temp"
							+ Path.DirectorySeparatorChar;
			_internalTempUnityPath = "Internal/Temp/";
			_autosavePath = _shaderEditorResourceDir
							+ _internalTempDir
							+ "autosave.sgraph";
			_tempShaderPathFull = _shaderEditorResourceDir
								+ _internalTempDir
								+ TempShaderName + ".shader";
			_shaderTemplatePath = _shaderEditorResourceDir
								+ "Internal"
								+ Path.DirectorySeparatorChar
								+ "ShaderTemplate.template";
			_graphsDir = _shaderEditorResourceDir
								+ "Public"
								+ Path.DirectorySeparatorChar
								+ "Graphs";
			
			_selectedGraph = new ShaderGraph();
			
			_popupMenu = new PopupMenu( );
			
			//Find all the nodes that exist in the assembly (for submission to popup menu)
			var types = AppDomain.CurrentDomain.GetAssemblies().ToList().SelectMany(s => s.GetTypes());
			foreach (var type in types)
			{
				foreach (var attribute in Attribute.GetCustomAttributes(type).OfType<NodeMetaData>())
				{
					NodeMetaData attribute2 = attribute;
					_popupMenu.AddItem( new ExecutableMenuItem( attribute.Category, attribute.DisplayName, 
								delegate( Vector2 location ){ 
									var toAdd = Activator.CreateInstance(attribute2.NodeType) as Node;
									if (toAdd == null)
									{
										return;
									}
									_selectedGraph.CurrentSubGraph.AddNode(toAdd);
									toAdd.NodePositionInGraph = new EditorRect( location.x, location.y, 10f, 10f);
									MarkDirty();
								}, String.IsNullOrEmpty(attribute.Descriptor) ? "" : attribute.Descriptor ) );
				}
			}
			_popupMenu.SortMenu();
			
			//Find all the serializable types
			_serializableTypes.Clear();
			foreach (var type in types)
			{
				var typeAttributes = Attribute.GetCustomAttributes(type);
				foreach (var attribute in typeAttributes)
				{
					if (attribute is DataContractAttribute)
					{
						_serializableTypes.Add(type);
					}
				}
			}
			
			//Finally load the last graph
			LoadLastGraph();
			_selectedGraph.Initialize( new Rect( 0,0, Screen.width, Screen.height ), true );
			_undoChain = new GraphHistory( _serializableTypes );
			
			_shouldOpenPreviewWindow = true;
		}
		
		public void OnLostFocus()
		{
			_doingSelectBox = false;
			SelectedInputChannel = null;
			SelectedOutputChannel = null;
		}
		
		public ShaderGraph CurrentGraph
		{
			get{ return _selectedGraph; }
		}
		
		private void MarkDirty()
		{
			_selectedGraph.MarkDirty();
			
			//Add an undo layer here
			CacheLastGraph();
			_undoChain.AddUndoLevel( _selectedGraph );
		}
		
		private bool _shouldUpdateShader;
		public void UpdateShader()
		{
			_shouldUpdateShader = true;
		}
		
		public bool ShaderNeedsUpdate()
		{
			return _graphNeedsUpdate;
		}
		
		//State variables
		private bool _shouldExportShader;
		private bool _shouldSaveGraph;
		private bool _shouldLoadGraph;
		private bool _isPlayingInEditor;
		private bool _markDirtyOnLoad;
		private bool _quickSaving;
		private string _lastGraphPath;
		private bool _quickExport;
		private string _lastExportPath;
		private string _overrideLoadPath;
		
		private static bool PreviewSupported()
		{
			return SystemInfo.supportsRenderTextures && SystemInfo.SupportsRenderTextureFormat( RenderTextureFormat.ARGB32 );
		}

		private void DisablePreview()
		{
			if( _previewWindow != null )
			{
				_previewWindow.ShouldUpdate = false;
				_previewWindow.Close();
				_previewWindow = null;
			}
		}
		
		public void OnDisable()
		{
			CacheLastGraph();
			DisablePreview();
		}
		
		public void Update()
		{
			wantsMouseMove = true;
			
			//Need to force recreate the preview window if transitioning from
			//game to editor or back
			if( _isPlayingInEditor != EditorApplication.isPlaying )
			{
				DisablePreview();
				_shouldOpenPreviewWindow = true;
				_isPlayingInEditor = EditorApplication.isPlaying;
			}
			
			if( _shouldOpenPreviewWindow && PreviewSupported() )
			{
				//Find the preview window
				var types = AppDomain.CurrentDomain.GetAssemblies().ToList().SelectMany(s => s.GetTypes());
				var previewWindowType = types.FirstOrDefault( x => x.FullName == "StrumpyPreviewWindow" );
				if( previewWindowType != null )
				{
					_previewWindow = GetWindow( previewWindowType, true, "Preview" ) as PreviewWindowInternal;
					if (_previewWindow != null)
					{
						_previewWindow.Initialize(this);
						_previewWindow.wantsMouseMove = true;
						_previewWindow.ShouldUpdate = true;
						_shouldOpenPreviewWindow = false;
					}
				}
			}
			
			//Update preview
			if (_shouldUpdateShader)
			{
				if (_selectedGraph.IsGraphValid())
				{
					File.WriteAllText(_tempShaderPathFull, _selectedGraph.GetShader(_shaderTemplatePath, true));
					var currentShader = Resources.Load(_internalTempUnityPath + TempShaderName) as Shader;
					var path = AssetDatabase.GetAssetPath(currentShader);
					AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
					
					if( _previewWindow != null )
						_previewWindow.PreviewMaterial = new Material(currentShader);
					
					
					_graphNeedsUpdate = false;
					InstructionCounter.CountInstructions(); // Update the instruction count
					CacheCount(); // Solve the tooltip (Cached)
				}
				else
				{
					EditorUtility.DisplayDialog("Save Shader Error", "Cannot update shader, there are errors in the graph", "Ok");
				}
				_shouldUpdateShader = false;
			}
			
			//Save graph
			if (_shouldSaveGraph)
			{
				var path = _quickSaving ? _lastGraphPath : EditorUtility.SaveFilePanel("Save shader graph to file...", _graphsDir, "shader.sgraph", "sgraph");
				if (!string.IsNullOrEmpty(path))
				{
					var writer = new FileStream(path, FileMode.Create);
					var ser = new DataContractSerializer(typeof (ShaderGraph), _serializableTypes, int.MaxValue, false, false, null);
					ser.WriteObject(writer, _selectedGraph);
					writer.Close();
					_lastGraphPath = path;
				}
				_shouldSaveGraph = false;
				_quickSaving = false;
			}
			
			//Load graph
			if( _shouldLoadGraph )
			{
				var loadDir = _graphsDir;

				if(!String.IsNullOrEmpty(_lastGraphPath))
					loadDir = _lastGraphPath;

				var path = string.IsNullOrEmpty(_overrideLoadPath) ? EditorUtility.OpenFilePanel("Load shader graph...", loadDir, "sgraph") : _overrideLoadPath;
				bool didLoad = false;
						
				if (!string.IsNullOrEmpty(path))
				{
					try
					{
						using( var fs = new FileStream(path, FileMode.Open) )
						{
							using( var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()) )
							{
								var ser = new DataContractSerializer(typeof (ShaderGraph), _serializableTypes, int.MaxValue, false, false, null);
								var loaded = ser.ReadObject(reader, true) as ShaderGraph;
								if (loaded != null)
								{
									_undoChain = new GraphHistory( _serializableTypes );
									_nextGraph = loaded;
									loaded.Initialize( new Rect(0, 0, Screen.width, Screen.height), true);
									_lastGraphPath = path;
									_lastExportPath = "";
									didLoad = true;
								}
							}
						}
					}
					catch( Exception e ){
						Debug.Log( e );
					}
					//Try loading autosave graph (it's in a differnt xml format)
					try
					{
						if( !didLoad )
						{
							using( var fs = new FileStream(path, FileMode.Open) )
							{
								using( var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()) )
								{
									var ser = new DataContractSerializer(typeof(AutoSaveGraph), _serializableTypes, int.MaxValue, false, false, null);
									var loaded = ser.ReadObject(reader, true) as AutoSaveGraph;
									if (loaded != null)
									{
										_undoChain = new GraphHistory( _serializableTypes );
										_nextGraph = loaded.Graph;
										_nextGraph.Initialize( new Rect(0, 0, Screen.width, Screen.height), true);
										_lastGraphPath = "";
										_lastExportPath = "";
										didLoad = true;
									}
									
								}
							}
						}
					}
					catch( Exception e)
					{
						Debug.Log(e);
					}
					
					if( !didLoad )
					{
						EditorUtility.DisplayDialog("Load Shader Error", "Could not load shader", "Ok");
					}
				}
				_shouldLoadGraph = false;
				_shouldUpdateShader = true;
				_overrideLoadPath = "";
			}
			
			//Export the shader to a .shader file
			if (_shouldExportShader)
			{
				if (_selectedGraph.IsGraphValid())
				{
					var path = _quickExport ? _lastExportPath : EditorUtility.SaveFilePanel("Export shader to file...", Application.dataPath, "shader.shader", "shader");
					_lastExportPath = path; // For quick exporting - Texel
					if (!string.IsNullOrEmpty(path))
					{
						File.WriteAllText(path, _selectedGraph.GetShader(_shaderTemplatePath, false));
						AssetDatabase.Refresh(); // Investigate if this is optimal
					}
					
					InstructionCounter.CountInstructions(); // Update the instruction count
					CacheCount(); // Solve the tooltip (Cached)
				}
				else
				{
					EditorUtility.DisplayDialog("Export Shader Error", "Cannot export shader, there are errors in the graph", "Ok");
				}
				_shouldExportShader = false;
				_quickExport = false;
			}
			Repaint();
		}
		
		/* Autosave structure */
		[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
		private class AutoSaveGraph
		{
			[DataMember]
			public ShaderGraph Graph;
			[DataMember]
			public string ExportPath;
			[DataMember]
			public string SavePath;
		}
		
		//Create an 'autosave' of the last graph
		public void CacheLastGraph()
		{
			if (!Directory.Exists(Application.dataPath))
			{
				Directory.CreateDirectory(Application.dataPath);
			}
			
			var toSave = new AutoSaveGraph
				{
				Graph = _selectedGraph,
				ExportPath = _lastExportPath,
				SavePath = _lastGraphPath
			};
			
			var writer = new FileStream(_autosavePath, FileMode.Create);
			var ser = new DataContractSerializer(typeof(AutoSaveGraph), _serializableTypes, int.MaxValue, false, false, null);
			ser.WriteObject(writer, toSave);
			writer.Close();
		}

		//Load the last 'autosave' graph
		private void LoadLastGraph()
		{
			var fi = new FileInfo(_autosavePath);
			if (fi.Exists)
			{
				try{
					var fs = fi.OpenRead();
					var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
					var ser = new DataContractSerializer(typeof(AutoSaveGraph), _serializableTypes, int.MaxValue, false, false, null);
					var loaded = ser.ReadObject(reader, true) as AutoSaveGraph;
					if (loaded != null)
					{
						_nextGraph = loaded.Graph;
						if( !string.IsNullOrEmpty( loaded.ExportPath ) && File.Exists( loaded.ExportPath ) )
							_lastExportPath = loaded.ExportPath;
						if( !string.IsNullOrEmpty( loaded.SavePath ) && File.Exists( loaded.SavePath ) )
							_lastGraphPath = loaded.SavePath;
						_nextGraph.Initialize( new Rect( 0, 0, Screen.width, Screen.height ), true);
						_markDirtyOnLoad = true;
					}
					reader.Close();
					fs.Close();
				}
				catch( Exception e )
				{
					Debug.Log( e );
					EditorUtility.DisplayDialog("Load Shader Error", "Could not load shader", "Ok");
				}
			}
		}
		
		//Build a list of how many instructions the last compiled shader uses.
		private bool _isInstructionCountCached = false;
		private string _instructionCountDetails;
		private string _instructionCountTooltip;
		private void CacheCount() {
			var count = "Instruction Count: \n";
			count += "ALU: ";
			count += InstructionCounter.lastCount.GL.ALU < 0 ? "N/A" : InstructionCounter.lastCount.GL.ALU.ToString();
			count += "\n";
			count += "TEX: ";
			count += InstructionCounter.lastCount.GL.TEX < 0 ? "N/A" : InstructionCounter.lastCount.GL.TEX.ToString();
			_instructionCountDetails = count;
			
			var verbose = "GL: ";
			verbose += "\nALU: ";
			verbose += InstructionCounter.lastCount.GL.ALU < 0 ? "N/A" : InstructionCounter.lastCount.GL.ALU.ToString();
			verbose += " V_ALU: ";
			verbose += InstructionCounter.lastCount.GL.ALU < 0 ? "N/A" : InstructionCounter.lastCount.GL.V_ALU.ToString();
			verbose += " TEX: ";
			verbose += InstructionCounter.lastCount.GL.TEX < 0 ? "N/A" : InstructionCounter.lastCount.GL.TEX.ToString();
			
			verbose += "\nD3D: ";
			verbose += "\nALU: ";
			verbose += InstructionCounter.lastCount.D3D.ALU < 0 ? "N/A" : InstructionCounter.lastCount.D3D.ALU.ToString();
			verbose += " V_ALU: ";
			verbose += InstructionCounter.lastCount.D3D.ALU < 0 ? "N/A" : InstructionCounter.lastCount.D3D.V_ALU.ToString();
			verbose += " TEX: ";
			verbose += InstructionCounter.lastCount.D3D.TEX < 0 ? "N/A" : InstructionCounter.lastCount.D3D.TEX.ToString();
			_instructionCountTooltip = verbose;
		}
		
		//Drawing related states / areas
		private bool _graphNeedsUpdate;
		private bool _showComments = true;
		private Vector2 _editorFieldOffset = Vector2.zero;
		private Rect _drawArea;
		private Rect _detailsBox;
		private Rect _optionsBox;
		private bool _focusChanged;
		private bool _focusChangedUpdate;
		public void OnGUI()
		{
			if (!_isInstructionCountCached) 
				CacheCount();
			GUI.Label(new Rect(5,0,95,45),new GUIContent(_instructionCountDetails,_instructionCountTooltip));
			
			_reservedArea.Clear();
			_drawArea = new Rect( 0, 0, Screen.width-300, Screen.height-23 );
			_detailsBox = new Rect(Screen.width - 300,0, 300, Screen.height - 40);
			_optionsBox = new Rect(Screen.width - 300,Screen.height - 40, 300 , 25);
			
			//Mouse clicks in the reserved area do not attemp to select on graph
			_reservedArea.Add( _detailsBox );
			_reservedArea.Add( _optionsBox );
			
			_currentMousePosition.x = Event.current.mousePosition.x;
			_currentMousePosition.y = Event.current.mousePosition.y;
			
			// Handle Minimap! (Texel)
			if (_middleMouseDrag) {
				var oldColor = GUI.color;
				var trans = GUI.color * new Vector4(1,1,1,0.3f); // Fairly transparent
				// First, draw the bounds of the graph. This requires min/maxing the graph
				var drawCenter = new Vector2((_drawArea.x + (_drawArea.width * 0.5f)),_drawArea.y + (_drawArea.height * 0.5f));
				drawCenter.x -= _drawArea.width * 0.1f;
				drawCenter.y -= _drawArea.height * 0.1f;
				
				var redSize = new Vector2(_drawArea.width,_drawArea.height) * 0.2f;
				
				GUI.color = trans;
				GUI.Box(new Rect(drawCenter.x,drawCenter.y,redSize.x,redSize.y),"");
				GUI.color = oldColor;
				var oldBkg = GUI.backgroundColor;
				
				// Now we will draw the graph centered around the offset (Scaled down ten times)
				//Rect[] rects = _selectedGraph.CurrentSubGraph.nodePositions;
				foreach(Node node in _selectedGraph.CurrentSubGraph.Nodes) {
					var rect = node.NodePositionInGraph;
					
					var delta = new Vector2(rect.x,rect.y);// - offset;
					var size = new Vector2(rect.width,rect.height);
					delta *= 0.2f; size *= 0.2f;
					delta += drawCenter;
					
					switch (node.CurrentState) {
						case (NodeState.Valid):
							GUI.color = Color.white;
							break;
						case (NodeState.NotConnected):
							GUI.color = new Color (0.8f, 0.8f, 1f);
							break;
						case (NodeState.CircularReferenceInGraph):
							GUI.color = new Color (0.8f, 0.8f, 0f);
							break;
						case (NodeState.Error):
							GUI.color = Color.red;
							break;
					}
					
					if( node == _selectedNode )
						GUI.backgroundColor = Color.Lerp(GUI.backgroundColor,Color.green,0.5f);
					
					GUI.Box(new Rect(delta.x,delta.y,size.x,size.y),"");
					GUI.color = oldColor;
					GUI.backgroundColor = oldBkg;
				}
			}
			
			HandleEvents();
			
			//GUI fixup for changed focus
			GUI.SetNextControlName("FocusFixup");
			EditorGUI.Toggle(new Rect(-100, -100, 1, 1), false);

			if (_focusChangedUpdate)
			{
				GUI.FocusControl("FocusFixup");
				_focusChangedUpdate = false;
			}
			
			//Update each node and draw them
			_selectedGraph.UpdateErrorState();
			GUILayout.BeginArea( _drawArea );
			_selectedGraph.Draw( this, _showComments, _drawArea );
			UpdateIOChannels();
			DrawIOLines(_drawArea);
			GUILayout.EndArea( );
			
			if( _doingSelectBox )
			{
				var oldColor = GUI.color;
				var newColor = GUI.color;
				newColor.a = 0.4f;
				GUI.color = newColor;
				GUI.Box( GetSelectionArea(), "" );
				GUI.color = oldColor;
			}
			
			DrawSettings();
			
			//Handle node delete / undo and redo
			if( !GUI.changed && Event.current.type == EventType.KeyDown ) 
			{
				if (Event.current.keyCode == KeyCode.Z 
					&& ( Event.current.modifiers == EventModifiers.Alt ) )
				{
					var graph = _undoChain.Undo();
					if( graph != null )
					{
						_nextGraph = graph;
						_nextGraph.Initialize( new Rect( 0, 0, Screen.width, Screen.height ), false);
						_graphNeedsUpdate = true;
						NextSelectedNode = _nextGraph.FirstSelected;
						_updateSelection = true;
						Event.current.Use();
					}
					return;
				}
				if (Event.current.keyCode == KeyCode.Z
					&& ( Event.current.modifiers == (EventModifiers.Alt | EventModifiers.Shift ) ) )
				{
					var graph = _undoChain.Redo();
					if( graph != null )
					{
						_nextGraph = graph;
						_nextGraph.Initialize( new Rect( 0, 0, Screen.width, Screen.height ), false);
						_graphNeedsUpdate = true;
						NextSelectedNode = _nextGraph.FirstSelected;
						_updateSelection = true;
						Event.current.Use();
					}
					return;
				}
					
				if (Event.current.keyCode == KeyCode.Delete || Event.current.keyCode == KeyCode.Backspace)
				{
					_selectedGraph.CurrentSubGraph.DeleteSelected();
					NextSelectedNode = null;
					_graphNeedsUpdate = true;
					_updateSelection = true;

					SelectedInputChannel = null;
					SelectedOutputChannel = null;
					MarkDirty();
					Event.current.Use();
				}
			}
			
			//Draw the current subgraph type
			var oldFontSize = GUI.skin.label.fontSize;
			var oldFontStyle = GUI.skin.label.fontStyle;
			GUI.skin.label.fontSize = 30;
			GUI.skin.label.fontStyle = FontStyle.BoldAndItalic;
			GUILayout.BeginArea( _drawArea );
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label( _currentSubGraphType.DisplayName(), GUI.skin.label );
			GUILayout.Space( 10 );
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			GUI.skin.label.fontSize = oldFontSize;
			GUI.skin.label.fontStyle = oldFontStyle;
			
			DrawOptions();
		}
		
		private enum FileMenuOptions
		{
			NewGraph,
			LoadGraph,
			SaveGraph,
			SaveAsGraph,
			ExportGraph,
			ExportAsGraph
		}

		private void FileMenuSelect( object objProperty )
		{
			if( objProperty.GetType() != typeof( FileMenuOptions ) )
			{
				return;
			}
			var option = (FileMenuOptions)objProperty;
			
			switch( option )
			{
			case FileMenuOptions.NewGraph:
			{
				if (EditorUtility.DisplayDialog("New Graph","Old graph will be lost, are you sure?","Confirm","Cancel")) {
					_nextGraph = new ShaderGraph();
					_nextGraph.Initialize( new Rect( 0, 0, Screen.width, Screen.height ), true);
					_graphNeedsUpdate = true;
					_lastGraphPath = "";
					_lastExportPath = "";
					_undoChain = new GraphHistory( _serializableTypes );
					_markDirtyOnLoad = true;
				}
				break;
			}
			case FileMenuOptions.ExportGraph:
			case FileMenuOptions.ExportAsGraph:
			{
				_shouldExportShader = true;
				if (!String.IsNullOrEmpty(_lastExportPath) && 
					option == FileMenuOptions.ExportGraph)
				{
					_quickExport = true;
				}

				if (!String.IsNullOrEmpty(_lastGraphPath))
				{
					_shouldSaveGraph = true;
					_quickSaving = true;
				}
				break;
			}
			case FileMenuOptions.SaveGraph:
			case FileMenuOptions.SaveAsGraph:
			{
				_shouldSaveGraph = true;
				if (!String.IsNullOrEmpty(_lastGraphPath) &&
					option == FileMenuOptions.SaveGraph)
				{
					_quickSaving = true;
				}
				break;
			}
			case FileMenuOptions.LoadGraph:
			{
				_shouldLoadGraph = true;
				break;
			}
			}
		}
		
		private void SubGraphSelect( object objProperty )
		{
			if( objProperty.GetType() != typeof( SubGraphType ) )
			{
				return;
			}
			_currentSubGraphType = (SubGraphType) objProperty;
			SelectedInputChannel = null;
			SelectedOutputChannel = null;
		}
		
		private enum OptionsSelection
		{
			None=-1,
			File,
			Comments,
			Graphs,
			Preview
		}
		
		private void DrawOptions()
		{
			GUILayout.BeginArea( _optionsBox );
			GUILayout.BeginHorizontal();
			
			var file =    GUI.skin.buttonLeft();//GUI.skin.FindStyle("ButtonLeft");
			var comments= GUI.skin.buttonMid();//new GUIStyle(GUI.skin.FindStyle("ButtonMid"));
			var graphs  = PreviewSupported() ? 
			GUI.skin.buttonMid() : //GUI.skin.FindStyle("ButtonMid") :
			GUI.skin.buttonRight();//GUI.skin.FindStyle("ButtonRight");
			GUI.skin.buttonRight();
			
			var selectedOption = OptionsSelection.None;
			
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(new GUIContent("File","Save, Load, And Export features."),file))
				selectedOption = OptionsSelection.File;
			
			if (_showComments) {
				comments = GUI.skin.buttonMidOn();
			}
			
			if (GUILayout.Button(new GUIContent("Comments","Enable ot disable the display of comments"),comments))
				selectedOption = OptionsSelection.Comments;
			
			if (GUILayout.Button(new GUIContent("Graphs","Change the active subgraph"),graphs))
				selectedOption = OptionsSelection.Graphs;
			
			if (PreviewSupported()) {				
				if (GUILayout.Button(new GUIContent("Preview","Call forth the preview window"),graphs))
					selectedOption = OptionsSelection.Preview;
			}
			
			GUILayout.EndHorizontal();
			
			/*if( PreviewSupported() )
			{
				selectedOption = (OptionsSelection)GUILayout.Toolbar( (int)selectedOption, 
						new [] { OptionsSelection.File.ToString(), OptionsSelection.Comments.ToString(), OptionsSelection.Graphs.ToString() ,OptionsSelection.Preview.ToString() } );
			}
			else
			{
				selectedOption = (OptionsSelection)GUILayout.Toolbar( (int)selectedOption, 
						new [] { OptionsSelection.File.ToString(), OptionsSelection.Comments.ToString() } );
			}*/
			
			switch( selectedOption )
			{
			case OptionsSelection.File:
			{
				var menu = new GenericMenu();
				menu.AddItem( new GUIContent( "New Graph..." ), false, FileMenuSelect, FileMenuOptions.NewGraph );
				menu.AddItem( new GUIContent( "Load..." ), false, FileMenuSelect,FileMenuOptions.LoadGraph );
				if( String.IsNullOrEmpty(_lastGraphPath) )
				{
					menu.AddDisabledItem( new GUIContent( "Save" ) );
				}
				else
				{
					menu.AddItem( new GUIContent( "Save" ), false, FileMenuSelect, FileMenuOptions.SaveGraph );
				}
				menu.AddItem( new GUIContent( "Save As..." ), false, FileMenuSelect, FileMenuOptions.SaveAsGraph );
				if( String.IsNullOrEmpty(_lastExportPath) )
				{
					menu.AddDisabledItem( new GUIContent( "Export" ) );
				}
				else
				{
					menu.AddItem( new GUIContent( "Export" ), false, FileMenuSelect, FileMenuOptions.ExportGraph );
				}
				menu.AddItem( new GUIContent( "Export As..." ), false, FileMenuSelect, FileMenuOptions.ExportAsGraph );
				menu.ShowAsContext();
				break;
			}
			case OptionsSelection.Graphs:
			{
				var menu = new GenericMenu();
				menu.AddItem( new GUIContent( "Pixel" ), false, SubGraphSelect, SubGraphType.Pixel );
				menu.AddItem( new GUIContent( "Vertex" ), false, SubGraphSelect, SubGraphType.Vertex );
				menu.AddItem( new GUIContent( "Lighting" ), false, SubGraphSelect, SubGraphType.SimpleLighting );
				menu.ShowAsContext();
				break;
			}
			case OptionsSelection.Comments:
			{
				_showComments = !_showComments;
				break;
			}
			case OptionsSelection.Preview:
			{
				_shouldOpenPreviewWindow = true;
				break;
			}
			}
			
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
		
		private Rect GetSelectionArea()
		{
			float left = _selectBoxStart.x < _currentMousePosition.x ? _selectBoxStart.x : _currentMousePosition.x;
			float right = _selectBoxStart.x > _currentMousePosition.x ? _selectBoxStart.x : _currentMousePosition.x;
			float top = _selectBoxStart.y < _currentMousePosition.y ? _selectBoxStart.y : _currentMousePosition.y;
			float bottom = _selectBoxStart.y > _currentMousePosition.y ? _selectBoxStart.y : _currentMousePosition.y;
			return new Rect( left, top, right - left, bottom - top );
		}
		
		private bool _updateSelection;
		private bool _middleMouseDrag;
		private void HandleEvents() // Texel added parent input for drag and drop
		{
			switch( Event.current.type )
			{
			case EventType.DragUpdated:
			case EventType.DragPerform :
			{
				if (position.Contains(_currentMousePosition)) {
					var path = DragAndDrop.paths[0];
					if (path.EndsWith("sgraph"))
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
						if (Event.current.type == EventType.dragPerform)
						{
							_shouldLoadGraph = true;
							_overrideLoadPath = path;
							_lastGraphPath = path;
							_markDirtyOnLoad = true;
						}
						Event.current.Use();
					}
				}
				break;
			}

			case EventType.MouseDown:
			{
				if (Event.current.button == 0 )
				{
					if( LeftMouseDown() )
					{
						Event.current.Use();
						return;
					}
				}
				break;
			}
				
			case EventType.ContextClick:
			{
				if( RightMouseDown() )
				{
					Event.current.Use();
					return;
				}
				break;
			}
			//Handle drag
			case EventType.MouseDrag:
			{
				_currentMousePosition.x = Event.current.mousePosition.x;
				_currentMousePosition.y = Event.current.mousePosition.y;
				
				//If Left click drag...
				if (Event.current.button == 0 )
				{
					if( LeftMouseDragged() )
					{
						Event.current.Use();
					}
				}
				
				if( Event.current.button == 2 )
				{
					_middleMouseDrag = true;
					_selectedGraph.CurrentSubGraph.DrawOffset += Event.current.delta;
					
					Event.current.Use();
				}
				break;
			}
			case EventType.MouseUp:
			{
				//Mouse button released, unmark hot
				_selectedGraph.UnmarkSelectedHot();

				if( _focusChanged )
				{
					_focusChangedUpdate = true;
					_focusChanged = false;
				}
				
				//Selection box
				if( _doingSelectBox )
				{
					_doingSelectBox = false;
					
					if( _selectedGraph.Select( GetSelectionArea(), Event.current.modifiers == EventModifiers.Shift ) )
					{
						NextSelectedNode = _selectedGraph.FirstSelected;
						_updateSelection = true;
						MarkDirty();
					}
					Event.current.Use();
				}
				
				if( _movingNodes )
				{
					_movingNodes = false;
					MarkDirty();
					Event.current.Use();
				}
				
				if( _middleMouseDrag )
				{
					_middleMouseDrag = false;
					MarkDirty();
					Event.current.Use();
				}
				break;
			}
			case EventType.Layout:
			{
				if (_nextGraph != null && _nextGraph != _selectedGraph)
				{
					_selectedGraph = _nextGraph;
					if( _markDirtyOnLoad )
					{
						MarkDirty();
						_markDirtyOnLoad = false;
					}
					_selectedNode = null;
				}
				if( _selectedGraph.CurrentSubGraphType != _currentSubGraphType )
				{
					_selectedGraph.CurrentSubGraphType = _currentSubGraphType;
					_selectedGraph.Deselect();
					MarkDirty();
					_selectedNode = null;
				}
				if( _updateSelection )
				{
					_focusChanged = true;
					_selectedNode = NextSelectedNode;
					_updateSelection = false;
				}
				break;
			}
			case EventType.ValidateCommand:
			{
				if( Event.current.commandName == "Copy"
					|| Event.current.commandName == "Paste"
					|| Event.current.commandName == "Duplicate"
					|| Event.current.commandName == "SelectAll")
				{
					Event.current.Use();
				}
				break;
			}
			case EventType.ExecuteCommand:
			{
				if( Event.current.commandName == "Copy" )
				{
					CopySelectedNodes();
					Event.current.Use();
				}

				if( Event.current.commandName == "Paste" )
				{
					PasteNodes();
					Event.current.Use();
				}
				
				if( Event.current.commandName == "Duplicate" )
				{
					CopySelectedNodes();
					PasteNodes();
					MarkDirty();
					Event.current.Use();
				}
				
				if( Event.current.commandName == "SelectAll" )
				{
					_selectedGraph.SelectAll();
					NextSelectedNode = _selectedGraph.FirstSelected;
					_updateSelection = true;
					MarkDirty();
					Event.current.Use();
				}
				break;
			}
			}
			return;
		}
		
		private void UpdateIOChannels()
		{
			//Update channel selection state
			if (SelectedInputChannel != null && SelectedOutputChannel != null)
			{
				var inputChannel = _selectedGraph.CurrentSubGraph.GetInputChannel(SelectedInputChannel);
				var outputChannel = _selectedGraph.CurrentSubGraph.GetOutputChannel(SelectedOutputChannel);

				if (SelectedInputChannel.NodeIdentifier != SelectedOutputChannel.NodeIdentifier
					&& inputChannel.ChannelType == outputChannel.ChannelType)
				{
					inputChannel.IncomingConnection = SelectedOutputChannel;
				}
				SelectedInputChannel = null;
				SelectedOutputChannel = null;
				_graphNeedsUpdate = true;
				MarkDirty();
			}
		}
		
		private enum SelectedSettings
		{
			Node,
			Inputs,
			Settings,
			Nodes
		}
		
		private SelectedSettings _currentSettings = SelectedSettings.Node;
		private Vector2 _optionsScrollPosition = Vector2.zero;
		
		private void DrawSettings()
		{
			GUI.Box( _detailsBox, "");
			GUILayout.BeginArea( new Rect( _detailsBox.x + 5, _detailsBox.y + 5, _detailsBox.width - 10, 25) );
			
			var setContent = new[]
				{ new GUIContent(SelectedSettings.Node.ToString(),"Settings for the active selected node"),
				  new GUIContent(SelectedSettings.Inputs.ToString(),"Input manager to add/remove/rename inputs for the graph"),
				  new GUIContent(SelectedSettings.Settings.ToString(),"Settings for the shader itself"),
				  new GUIContent(SelectedSettings.Nodes.ToString(),"Searchable, tooltipped list of nodes.") };
			
			// Texel fun time
			/*if (_selectedGraph.IsGraphValid()) {
			
			currentSettings = (SelectedSettings)GUILayout.Toolbar( (int)currentSettings, 
			                                                    new [] { 	SelectedSettings.Node.ToString(),
																			SelectedSettings.Inputs.ToString(), 
																			SelectedSettings.Settings.ToString(),
																			SelectedSettings.Nodes.ToString() } );
			} else */ { // Settings are not valid, time to draw a custom view
				// Custom style time!
				/* // Too Small
				var node = new GUIStyle(GUI.skin.FindStyle("minibuttonleft"));
				var inputs  = new GUIStyle(GUI.skin.FindStyle("minibuttonmid"));
				var settings  = new GUIStyle(GUI.skin.FindStyle("minibuttonmid"));
				var nodes =  new GUIStyle(GUI.skin.FindStyle("minibuttonright")); */
				
				/* // Too Big
				var node = GUI.skin.FindStyle("LargeButtonLeft");
				var inputs  = GUI.skin.FindStyle("LargeButtonMid");
				var settings  = GUI.skin.FindStyle("LargeButtonMid");
				var nodes= GUI.skin.FindStyle("LargeButtonRight");
				*/
				var node = GUI.skin.buttonLeft();//new GUIStyle(GUI.skin.FindStyle("ButtonLeft"));
				var inputs  = GUI.skin.buttonMid();//new GUIStyle(GUI.skin.FindStyle("ButtonMid"));
				var settings  = GUI.skin.buttonMid();//new GUIStyle(GUI.skin.FindStyle("ButtonMid"));
				var nodes =  GUI.skin.buttonMid();//new GUIStyle(GUI.skin.FindStyle("ButtonRight"));
				
				
				// Reflect active button settings (since we will emulate using Button)
				/*switch (currentSettings) {
					case SelectedSettings.Node:
						node.normal = node.onNormal;
						node.hover = node.onHover;
						node.active = node.onActive;
						break;
					case SelectedSettings.Inputs:
						inputs.normal = inputs.onNormal;
						inputs.hover = inputs.onHover;
						inputs.active = inputs.onActive;
						break;
					case SelectedSettings.Settings:
						settings.normal = settings.onNormal;
						settings.hover = settings.onHover;
						settings.active = settings.onActive;
						break;
					case SelectedSettings.Nodes:
						nodes.normal = nodes.onNormal;
						nodes.hover = nodes.onHover;
						nodes.active = nodes.onActive;
						break;
				}*/
				
				switch (_currentSettings) {
					case SelectedSettings.Node:
						node = GUI.skin.buttonLeftOn();
						break;
					case SelectedSettings.Inputs:
						inputs = GUI.skin.buttonMidOn();
						break;
					case SelectedSettings.Settings:
						settings = GUI.skin.buttonMidOn();
						break;
					case SelectedSettings.Nodes:
						nodes = GUI.skin.buttonRightOn();
						break;
				}
				
				var oldColor = GUI.color;
				
				GUILayout.BeginHorizontal(); // Begin our virtual toolbar
				
				if (_selectedNode != null)
					if(_selectedNode.CurrentState == NodeState.Error)
						GUI.color = Color.red;
				
				if (GUILayout.Button(setContent[0],node))
					_currentSettings = SelectedSettings.Node;
				
				GUI.color = oldColor;
				
				if (!_selectedGraph.IsInputsValid())
					GUI.color = Color.red;
				
				if (GUILayout.Button(setContent[1],inputs))
					_currentSettings = SelectedSettings.Inputs;
				
				GUI.color = oldColor;
				
				if (!_selectedGraph.IsSettingsValid())
					GUI.color = Color.red;
				
				if (GUILayout.Button(setContent[2],settings))
					_currentSettings = SelectedSettings.Settings;
				
				GUI.color = oldColor;
				
				if (GUILayout.Button(setContent[3],nodes))
					_currentSettings = SelectedSettings.Nodes;
					
				GUILayout.EndHorizontal();
				
			}
			
			GUILayout.EndArea();
			
			GUILayout.BeginArea( new Rect( _detailsBox.x, _detailsBox.y + 25, _detailsBox.width, _detailsBox.height - 30 ) );
			_optionsScrollPosition = GUILayout.BeginScrollView(_optionsScrollPosition, GUILayout.Width(_detailsBox.width), GUILayout.Height(_detailsBox.height - 30));
			
			switch( _currentSettings )
			{
			case SelectedSettings.Node:
				if ( _selectedNode != null)
				{
					GUI.changed = false;
					_selectedNode.DrawProperties();
					_selectedNode.DrawCommentField();
					// Texel - Good place to remind users of errors aswell, now that we have more space.
					var errors = _selectedNode.ErrorMessages.Aggregate("", (current, error) => current + (error + "\n"));

					if (errors != "")
						errors = "ERROR: " + errors;
					
					var oldColor = GUI.color;
					GUI.color = Color.red;
					GUILayout.Label(new GUIContent(errors,"Better fix this."));
					GUI.color = oldColor;
					
					//Potential change to node name ect... need to force update of error states
					if( GUI.changed )
					{
						//Do not do full mark dirty on simple GUI change
						_selectedGraph.MarkDirty();
					}
					GUILayout.FlexibleSpace();
				}
				else
				{
					GUILayout.Label( "Select a node to edit" );
				}
				break;
			case SelectedSettings.Inputs:
				_selectedGraph.DrawInput();
				break;
			case SelectedSettings.Settings:
				_selectedGraph.DrawSettings();
				break;
			case SelectedSettings.Nodes:
				_popupMenu.GuiPaneDraw( new Vector2( _detailsBox.x, _detailsBox.y ) );
				break;
			}
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}
		
		private readonly List<Rect> _reservedArea = new List<Rect>();
		private bool InsideReservedArea( Vector2 thePosition )
		{
			if( _reservedArea.Any( x => x.Contains( thePosition ) ) )
			{
				return true;
			}
			return false;
		}
		
		private Vector2 _selectBoxStart = Vector2.zero;
		private bool _doingSelectBox;
		private bool LeftMouseDown()
		{
			if( InsideReservedArea( _currentMousePosition ) )
			{
				return false;
			}
			if( Event.current.modifiers == EventModifiers.Alt ||  Event.current.modifiers == EventModifiers.Command )
			{
				return true;
			}
				
			//We have clicked on an already selected node...
			var clickedNode = _selectedGraph.NodeAt( _currentMousePosition );
			if( clickedNode != null && _selectedGraph.IsSelected( clickedNode ) )
			{
				//If shift is held down deselect that node
				if( Event.current.modifiers == EventModifiers.Shift )
				{
					_selectedGraph.Deselect( clickedNode );
					MarkDirty();
					return true;
				}
				//just mark hot
				_selectedGraph.MarkSelectedHot();
				return true;
			}
				
			if( _selectedGraph.Select( _currentMousePosition, Event.current.modifiers == EventModifiers.Shift ) )
			{
				_selectedGraph.MarkSelectedHot();
				
				NextSelectedNode = _selectedGraph.FirstSelected;
				_updateSelection = true;
				
				MarkDirty();
				return true;
			}
			
			if( _selectedGraph.ButtonAt( _currentMousePosition ) )
			{
				return false;
			}
			
			if( Event.current.modifiers != EventModifiers.Shift )
			{
				_selectedGraph.Deselect();
				NextSelectedNode = null;
				MarkDirty();
				_updateSelection = true;
			}
			
			//Nothing is selected... start a drag
			_doingSelectBox = true;
			_selectBoxStart = Event.current.mousePosition;
			
			return false;
		}
		
		private bool _movingNodes;
		private bool LeftMouseDragged()
		{
			if( Event.current.modifiers == EventModifiers.Alt )
			{
				_middleMouseDrag = true;
				_selectedGraph.CurrentSubGraph.DrawOffset += Event.current.delta;
				return true;
			}
			if( Event.current.modifiers == EventModifiers.Command )
			{
				_selectedGraph.CurrentSubGraph.DrawScale += Event.current.delta.y;
				return true;
			}
			_movingNodes = _selectedGraph.DragSelectedNodes( Event.current.delta );
			return _movingNodes;
		}
		
		private bool RightMouseDown()
		{
			if( InsideReservedArea( _currentMousePosition ) )
			{
				return false;
			}
			
			//First right click clears any lines that are currenly being drawn
			if (SelectedInputChannel != null || SelectedOutputChannel != null)
			{
				SelectedInputChannel = null;
				SelectedOutputChannel = null;
				return false;
			}
			
			var didDelete = false; // Were we over another node on RMB? - Texel
			//If we clicked on a bezier curve... delete the link!
			foreach (var node in _selectedGraph.CurrentSubGraph.Nodes)
			{
				foreach (var inputChannel in node.GetInputChannels())
				{
					if (inputChannel.IncomingConnection != null)
					{
						var startPos = NodeDrawer.GetAbsoluteInputChannelPosition(node, inputChannel.ChannelId);
						var endNode = _selectedGraph.CurrentSubGraph.GetNode(inputChannel.IncomingConnection.NodeIdentifier);
						var endPos = NodeDrawer.GetAbsoluteOutputChannelPosition(endNode, inputChannel.IncomingConnection.ChannelId);

						var distanceBetweenNodes = Mathf.Abs(startPos.x - endPos.x);

						var distance = HandleUtility.DistancePointBezier(_currentMousePosition,
															new Vector3(startPos.x, startPos.y, 0.0f),
															new Vector3(endPos.x, endPos.y, 0.0f),
															new Vector3(startPos.x + distanceBetweenNodes / 3.0f, startPos.y, 0.0f),
															new Vector3(endPos.x - distanceBetweenNodes / 3.0f, endPos.y, 0.0f));

						if (distance < 5.0f)
						{
							inputChannel.IncomingConnection = null;
							_graphNeedsUpdate = true;
							MarkDirty();
							didDelete = true;
						}
					}
				}
			}
			
			if( !didDelete )
			{
				_popupMenu.Show( _currentMousePosition, _currentSubGraphType );
			}

			return true;
		}

		private Texture2D _bezierTexture;
		private void DrawIOLines( Rect viewArea )
		{
			Handles.BeginGUI( );
			Handles.color = Color.black;

			_bezierTexture =_bezierTexture ?? Resources.Load("Internal/1x2AA", typeof(Texture2D)) as Texture2D;
			
			foreach (var node in _selectedGraph.CurrentSubGraph.Nodes)
			{
				foreach (var inputChannel in node.GetInputChannels())
				{
					if (inputChannel.IncomingConnection != null)
					{
						var startPos = NodeDrawer.GetAbsoluteInputChannelPosition(node, inputChannel.ChannelId);
						var endNode = _selectedGraph.CurrentSubGraph.GetNode(inputChannel.IncomingConnection.NodeIdentifier);
						var endPos = NodeDrawer.GetAbsoluteOutputChannelPosition(endNode, inputChannel.IncomingConnection.ChannelId);
						
						//Make a compound box
						float left = startPos.x < endPos.x ? startPos.x : endPos.x;
						float right = startPos.x > endPos.x ? startPos.x : endPos.x;
						float top = startPos.y < endPos.y ? startPos.y : endPos.y;
						float bottom = startPos.y > endPos.y ? startPos.y : endPos.y;
						var channelBounds =  new Rect( left, top, right - left, bottom - top );
						
						if( channelBounds.xMin > viewArea.xMax 
							|| channelBounds.xMax < viewArea.xMin 
							|| channelBounds.yMin > viewArea.yMax 
							|| channelBounds.yMax < viewArea.yMin )
						{
							continue;
						}
						
						var distanceBetweenNodes = Mathf.Abs(startPos.x - endPos.x);

						Handles.DrawBezier(new Vector3(startPos.x, startPos.y),
											new Vector3(endPos.x, endPos.y),
											new Vector3(startPos.x + distanceBetweenNodes / 3.0f, startPos.y),
											new Vector3(endPos.x - distanceBetweenNodes / 3.0f, endPos.y),
											Color.black,
											_bezierTexture,
											1.25f);
					}
				}
			}
			
			//Draw IO for selected
			if (SelectedInputChannel != null || SelectedOutputChannel != null)
			{
				Vector3 start;
				if (SelectedInputChannel != null)
				{
					var channelPosition = NodeDrawer.GetAbsoluteInputChannelPosition(_selectedGraph.CurrentSubGraph.GetNode(SelectedInputChannel.NodeIdentifier), SelectedInputChannel.ChannelId);
					start = new Vector3(channelPosition.x, channelPosition.y, 0f);
				}
				else
				{
					var channelPosition = NodeDrawer.GetAbsoluteOutputChannelPosition(_selectedGraph.CurrentSubGraph.GetNode(SelectedOutputChannel.NodeIdentifier), SelectedOutputChannel.ChannelId);
					start = new Vector3(channelPosition.x, channelPosition.y, 0f);
				}
				var mouseDrawPos = _currentMousePosition + new Vector3(_editorFieldOffset.x, _editorFieldOffset.y, 0f);
				
				var startPos = start;
				var endPos = mouseDrawPos;
				
				if (startPos.x > endPos.x) {
					startPos = mouseDrawPos;
					endPos = start; 
				}
				
				var distanceBetweenNodes = Mathf.Abs(startPos.x - endPos.x);

				Handles.DrawBezier( 
					new Vector3(startPos.x, startPos.y),
					new Vector3(endPos.x, endPos.y),
					new Vector3(startPos.x + distanceBetweenNodes / 3.0f, startPos.y),
					new Vector3(endPos.x - distanceBetweenNodes / 3.0f, endPos.y),
					Color.black,
					_bezierTexture,
					1.25f);
			}
			Handles.EndGUI();
		}
		
		/**
		 * Copy / paste for nodes
		 */
		private class SerializedNodeList
		{
			private readonly List<Node> _nodes = new List<Node> ();
			private readonly List<Type> _serializableTypes = new List<Type>();
			
			public SerializedNodeList( IEnumerable<Node> nodes, List<Type> serializableTypes )
			{
				_serializableTypes = serializableTypes;
				foreach (var node in nodes.Where(node => !(node is RootNode)))
				{
					_nodes.Add( node );
				}
			}

			public IEnumerable<Node> CopiedNodes
			{
				get
				{
					//Save the nodes into a memory stream
					var ser = new DataContractSerializer(typeof(List<Node>), _serializableTypes, int.MaxValue, false, false, null);
					var stream = new MemoryStream();
					ser.WriteObject(stream, _nodes);
					stream.Flush();
					stream.Position = 0;
					
					//Reload them back out...
					var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());
					var loaded = ser.ReadObject(reader, true) as List<Node>;
					return loaded;
				}
			}
		}
		
		private SerializedNodeList _copiedNodes;
		private void CopySelectedNodes()
		{
			_copiedNodes = new SerializedNodeList( _selectedGraph.CurrentSubGraph.SelectedNodes, _serializableTypes);
		}
		
		private void PasteNodes()
		{
			if( _copiedNodes == null )
			{
				return;
			}
			
			var copied = _copiedNodes.CopiedNodes;
			if( copied == null )
			{
				return;
			}
			
			//Add the nodes into the new graph
			var nameMap = new Dictionary<string, string>();
			foreach( var node in copied )
			{
				var oldName = node.UniqueNodeIdentifier;
				_selectedGraph.CurrentSubGraph.AddNode(node);
				node.Initialize();
				var newName = node.UniqueNodeIdentifier;
				
				nameMap.Add( oldName, newName );
			}
			
			//Patch up the channel references...
			foreach( var node in copied )
			{
				foreach( var channel in node.GetInputChannels() )
				{
					if( channel.IncomingConnection != null && nameMap.ContainsKey( channel.IncomingConnection.NodeIdentifier ) )
					{
						channel.IncomingConnection.NodeIdentifier = nameMap[channel.IncomingConnection.NodeIdentifier];
					}
					else
					{
						channel.IncomingConnection = null;
					}
					
				}
			}
			
			//Mark these nodes as selected
			_selectedGraph.Deselect();
			foreach( var node in copied )
			{
				_selectedGraph.Select( node, true );
			}
			NextSelectedNode = _selectedGraph.FirstSelected;
			_updateSelection = true;
			
			MarkDirty();
		}
	
		private class GraphHistory
		{
			private readonly Stack<MemoryStream> _undoStack;
			private readonly Stack<MemoryStream> _redoStack;
			private MemoryStream _currentState;
			private readonly DataContractSerializer _serializer;
			
			public GraphHistory( IEnumerable<Type> serializableTypes )
			{
				_undoStack = new Stack<MemoryStream>();
				_redoStack = new Stack<MemoryStream>();
				_serializer = new DataContractSerializer(typeof(ShaderGraph), serializableTypes, int.MaxValue, false, false, null);
			}
			
			public void AddUndoLevel( ShaderGraph graph )
			{
				var stream = new MemoryStream();
				_serializer.WriteObject(stream, graph);
				stream.Flush();
				
				if( _currentState != null )
				{
					_undoStack.Push( _currentState );
				}
				_currentState = stream;
				_redoStack.Clear();
			}
			
			private ShaderGraph Deserialize( Stream graphStream )
			{
				graphStream.Position = 0;
				var reader = XmlDictionaryReader.CreateTextReader(graphStream, new XmlDictionaryReaderQuotas());
				return _serializer.ReadObject(reader, true) as ShaderGraph;
			}
			
			public ShaderGraph Undo()
			{
				if( _undoStack.Count == 0 )
				{
					return null;
				}
				_redoStack.Push( _currentState );
				_currentState = _undoStack.Pop();
				return Deserialize( _currentState );
			}
			
			public ShaderGraph Redo()
			{
				if( _redoStack.Count == 0 )
				{
					return null;
				}
				_undoStack.Push( _currentState );
				_currentState = _redoStack.Pop();
				return Deserialize( _currentState );
			}
		}
	}
}
