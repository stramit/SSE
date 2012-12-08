using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	/*Subgraph parent
	 * Stores all the nodes and the offsets ect */
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public abstract class SubGraph
	{
		private Dictionary<string, Node> _nodes = new Dictionary<string, Node>();
		
		//Backing store, will be put into the dictionary on initialize for fast searching!
		[DataMember]
		private List<Node> nodes
		{
			get{
				return _nodes.Values.ToList();
			}
			set{
				_nodes = _nodes ?? new Dictionary<string, Node>();
				foreach( var node in value )
				{
					_nodes.Add( node.UniqueNodeIdentifier, node );
				}
			}
		}
		
		// Texel - Expose mechanism for getting node positions
		public Rect[] nodePositions {
			get {
				var rects = new List<Rect>();
				foreach (Node node in nodes)
					rects.Add(node.NodePositionInGraph);
				
				return rects.ToArray();
			} 
		}
		
		[DataMember]
		public EditorFloat2 DrawOffset{ get; set;}
		
		private float _drawScale;
		public float DrawScale{
			get{
				return _drawScale;
			}
			set{
				_drawScale = value;
				if( _drawScale > 2f )
				{
					_drawScale = 2f;
				}
				else if( _drawScale < 0.0001 )
				{
					_drawScale = 0.0001f;
				}
			}
		}
		
		private List<Node> _selected = new List<Node>();
		
		[DataMember]
		private List<string> _reloadSelectedList = new List<string>();

		//Cache these for performance
		private IEnumerable<Node> _depthSortedNodes = new List<Node> ();
		public bool Dirty { get; private set; }

		private CircularReferenceException _lastException;
		
		private ShaderGraph _owner;

		public IEnumerable<Node> Nodes {
			get { return _nodes.Values; }
		}

		public void Initialize ( ShaderGraph owner, Rect screenDimensions, bool updateDrawPos )
		{
			_owner = owner;
			
			//Update the offset of all the nodes for drawing / panning
			var masterPos = new Vector2(RootNode.Position.X, RootNode.Position.Y); 
			if( updateDrawPos )
			{
				var desiredPosition = Vector2.zero;
				desiredPosition.x = 10;
				desiredPosition.y =  screenDimensions.height / 4;
				DrawOffset = desiredPosition - masterPos;
			}
			DrawScale = 1f;
			
			Dirty = true;
			foreach (var node in Nodes) {
				node.Owner = this;
				node.Initialize (); 
			}
			_selected = new List<Node>();
			
			if( _reloadSelectedList == null )
			{
				_reloadSelectedList = new List<string>();
			}
			
			foreach( var node in Nodes )
			{
				if( _reloadSelectedList.Contains( node.UniqueNodeIdentifier ) )
				{
					_selected.Add( node );
				}
			}
		}
		
		public abstract string GraphTypeName { get; }
		public abstract SubGraphType GraphType { get; }
		
		public IEnumerable<ShaderProperty> FindValidProperties( InputType type )
		{
			return _owner.FindValidProperties( type );
		}
		
		public int AddProperty( ShaderProperty p )
		{
			return _owner.AddProperty( p );
		}
		
		/*Search for all nodes that export to the struct output of the vertex shader hlss */
		protected Dictionary<string, List<IStructInput>> GetValidStructNodes ()
		{
			var allStructs = GetValidNodes<IStructInput> ();
			
			//Build up a dictionary of struct inputname => node
			var structMap = new Dictionary<string, List<IStructInput> > ();
			foreach (var input in allStructs) {
				var fieldName = input.GetStructFieldName ();
				
				if (structMap.ContainsKey (fieldName)) {
					//This will only happen if by bad programming / node creation
					if (structMap[fieldName].Any( x => x.GetStructFieldType() != input.GetStructFieldType () ) ) {
						throw new Exception ("Multiple struct nodes exist with the same name, but different types");
					}
					structMap[fieldName].Add( input );
				}
				else {
					structMap.Add (fieldName, new List<IStructInput>{ input } );
				}
			}
			return structMap;
		}

		public IEnumerable<Node> SelectedNodes
		{
			get { return _selected; }
		}
		
		private void updateSerializedSelectedNodes()
		{
			_reloadSelectedList.Clear();
			foreach( var node in SelectedNodes )
			{
				_reloadSelectedList.Add( node.UniqueNodeIdentifier );
			}
		}
		
		public bool NeedsTimeNode
		{
			get
			{
				return GetConnectedNodesDepthFirst().Any( x => x.NeedsTime() == true );
			}
		}
		
		public bool NeedsSinTimeNode
		{
			get
			{
				return GetConnectedNodesDepthFirst().Any( x => x.NeedsSinTime() == true );
			}
		}
		
		public bool NeedsCosTimeNode
		{
			get
			{
				return GetConnectedNodesDepthFirst().Any( x => x.NeedsCosTime() == true );
			}
		}
		
		//Draw this graph with the offset given
		public void Draw ( NodeEditor editor, bool showComments, Rect drawWindow )
		{
			foreach (var node in Nodes) 
			{
				var topLeft = new Vector2( node.NodePositionInGraph.xMin, node.NodePositionInGraph.yMin );
				var topRight = new Vector2( node.NodePositionInGraph.xMax, node.NodePositionInGraph.yMin );
				var bottomLeft = new Vector2( node.NodePositionInGraph.xMin, node.NodePositionInGraph.yMax );
				var bottomRight = new Vector2( node.NodePositionInGraph.xMax, node.NodePositionInGraph.yMax );
				if( drawWindow.Contains( topLeft ) || drawWindow.Contains( topRight ) ||
					drawWindow.Contains( bottomLeft ) || drawWindow.Contains( bottomRight ) )
				{
					node.Draw ( editor, showComments, (_selected.Contains( node ) ), DrawOffset );
				}
			}
		}
		
		public void SetPreview( bool isPreviewShader )
		{
			//Set up proxy input shaders for export or preview
			var proxyInputs = GetValidNodes<IProxyOnSave> ();
			foreach (var proxyNode in proxyInputs) {
				proxyNode.Exporting = !isPreviewShader;
			}
		}
		
		public Node FirstSelected
		{
			get{ return _selected.LastOrDefault(); }
		}
		
		public void Deselect()
		{
			_selected.Clear();
			updateSerializedSelectedNodes();
		}
		
		public void Deselect( Node n )
		{
			_selected.Remove( n );
			updateSerializedSelectedNodes();
		}
		
		public Node NodeAt (Vector2 location )
		{
			var vec3Location = new Vector3 (location.x, location.y);
			
			//Find the last in the queue that is valid
			var clicked = (from node in Nodes
				where node.NodePositionInGraph.Contains (vec3Location)
				select node).LastOrDefault ();
			
			if( clicked != null )
			{
				if( !clicked.ButtonAt( location ) )
				{
					return clicked;
				}
			}
			return null;
		}
		
		public bool ButtonAt (Vector2 location )
		{
			return Nodes.Any( x => x.ButtonAt( location ) );
		}
		
		public bool IsSelected( Node n )
		{
			return _selected.Contains( n );
		}
		
		public bool Select (Vector2 location, bool addSelect )
		{
			var vec3Location = new Vector3 (location.x, location.y);
			
			var selectionChanged = false;
			
			//Find the last in the queue that is valid
			var clicked = (from node in Nodes
				where node.NodePositionInGraph.Contains (vec3Location)
				select node).LastOrDefault ();
			
			if( clicked != null )
			{
				if( !clicked.ButtonAt( location ) )
				{
					if( addSelect )
					{
						//Force the selected to be the last in the list
						if( _selected.Contains( clicked ) )
						{
							_selected.Remove( clicked );
						}
						_selected.Add( clicked );
					}
					else
					{
						_selected.Clear();
						_selected.Add( clicked );
					}
					selectionChanged = true;
				}
			}
			else if( !addSelect )
			{
				_selected.Clear();
			} 
			
			//Move it to the very end as it was just selected
			//if (_selected.LastOrDefault() != null ) {
			//	nodes.Remove (_selected.LastOrDefault());
			//	nodes.Add (_selected.LastOrDefault());
			//}
			
			updateSerializedSelectedNodes();
			return selectionChanged;
		}
		
		public void Select ( Node n, bool addSelect )
		{
			if( addSelect )
			{
				//Force the selected to be the last in the list
				if( _selected.Contains( n ) )
				{
					_selected.Remove( n );
				}
				_selected.Add( n );
			}
			else
			{
				_selected.Clear();
				_selected.Add( n );
			}
			
			updateSerializedSelectedNodes();
		}
		
		public bool Select ( Rect area, bool addSelect )
		{
			//Find the nodes to select
			//Find the last in the queue that is valid
			var toSelect = (from node in Nodes
				where area.OverLaps( node.NodePositionInGraph )
				select node);
			
			if( !addSelect )
			{
				_selected.Clear();
			}
			
			foreach( var n in toSelect )
			{
				//Force the selected to be the last in the list
				if( _selected.Contains( n ) )
				{
					_selected.Remove( n );
				}
				_selected.Add( n );
			}
			
			updateSerializedSelectedNodes();
			return toSelect.Count() > 0;
		}
		
		public void SelectAll( )
		{
			_selected.Clear();
			foreach( var node in Nodes )
			{
				_selected.Add( node );
			}
			
			updateSerializedSelectedNodes();
		}

		public void MarkSelectedHot ()
		{
			if (_selected == null || _selected.Count == 0 )
				return;
			
			if (GUIUtility.hotControl == 0) {
				GUIUtility.hotControl = _selected.GetHashCode ();
			}
		}

		public void UnmarkSelectedHot ()
		{
			if (_selected == null || _selected.Count == 0 )
				return;
			
			if (GUIUtility.hotControl == _selected.GetHashCode ()) {
				GUIUtility.hotControl = 0;
			}
		}

		public bool DragSelectedNodes (Vector2 delta)
		{
			if (_selected == null)
				return false;
			
			if (GUIUtility.hotControl == _selected.GetHashCode ()) {
				_selected.ForEach( x => x.DeltaMove (delta) );
				return true;
			}
			return false;
		}

		public Rect DrawSize {
			get {
				var dimension = new Rect (0, 0, 0, 0);
				
				foreach (var node in Nodes) {
					if (node.NodePositionInGraph.x < dimension.x) {
						dimension.x = node.NodePositionInGraph.x;
					}
					if (node.NodePositionInGraph.y < dimension.y) {
						dimension.y = node.NodePositionInGraph.y;
					}
					if (node.NodePositionInGraph.x + node.NodePositionInGraph.width > dimension.xMax) {
						dimension.xMax = node.NodePositionInGraph.x + node.NodePositionInGraph.width;
					}
					if (node.NodePositionInGraph.y + node.NodePositionInGraph.height > dimension.yMax) {
						dimension.yMax = node.NodePositionInGraph.y + node.NodePositionInGraph.height; 
					}
				}
				return dimension;
			}
		}

		public abstract RootNode RootNode {
			get;
		}

		public void MarkDirty()
		{
			Dirty = true;
			cachedConnectedNodes = null;
		}

		private void MarkClean()
		{
			Dirty = false;
		}

		public IEnumerable<Node> GetConnectedNodesDepthFirst (Node parent, Stack<string> visitedNodes)
		{
			if (Dirty || _depthSortedNodes == null ) {
				try {
					_depthSortedNodes = GetConnectedNodesDepthFirstInner (parent, visitedNodes);
				} 
				catch (CircularReferenceException)
				{
					throw;
				}
			}
			return _depthSortedNodes;
		}

		private IEnumerable<Node> GetConnectedNodesDepthFirstInner (Node parent, Stack<string> visitedNodes)
		{
			if (visitedNodes.Contains (parent.UniqueNodeIdentifier)) {
				visitedNodes.Push (parent.UniqueNodeIdentifier);
				
				_lastException = new CircularReferenceException { CircularTrace = visitedNodes, CausalNodeId = parent.UniqueNodeIdentifier };
				_depthSortedNodes = null;
				throw _lastException;
			}
			
			visitedNodes.Push (parent.UniqueNodeIdentifier);
			
			var toReturn = new List<Node> ();
			
			foreach (var input in parent.GetInputChannels ()) {
				if (input.IncomingConnection != null) {
					toReturn.AddRange (GetConnectedNodesDepthFirstInner (GetNode (input.IncomingConnection.NodeIdentifier), visitedNodes));
				}
			}
			
			visitedNodes.Pop ();
			
			toReturn.Add (parent);
			return toReturn;
		}

		public void UpdateErrorState ()
		{
			var graphContainsCircularReferences = ContainsCircularReferences ();
			
			foreach (var node in Nodes) {
				node.ClearErrors();
				var errors = IsNodeValid (node);
				if (errors.Count () > 0) {
					node.CurrentState = NodeState.Error;
					node.AddErrors( errors );
				} else if (graphContainsCircularReferences) {
					node.CurrentState = NodeState.CircularReferenceInGraph;
				} else if (!IsNodeConnected (node.UniqueNodeIdentifier)) {
					node.CurrentState = NodeState.NotConnected;
				} else {
					node.CurrentState = NodeState.Valid;
				}
			}
			MarkClean();
			
		}

		private static void BreakInputChannels (Node node)
		{
			foreach (var channel in node.GetInputChannels ()) {
				channel.IncomingConnection = null;
			}
		}

		private void BreakOutputChannels (Node node)
		{
			foreach (var localNode in Nodes) {
				foreach (var channel in localNode.GetInputChannels ()) {
					if (channel.IncomingConnection != null && channel.IncomingConnection.NodeIdentifier == node.UniqueNodeIdentifier) {
						channel.IncomingConnection = null;
					}
				}
			}
		}

		public void DeleteSelected()
		{
			foreach( var node in _selected )
			{
				if (node == RootNode) {
					continue;
				}
			
				BreakInputChannels (node);
				BreakOutputChannels (node);
			
				_nodes.Remove(node.UniqueNodeIdentifier);
			}
			_selected.Clear();
		}

		public IEnumerable<T> GetValidNodes<T> () where T : class
		{
			try {
				var candidateNodes = GetConnectedNodesDepthFirst ();
				var foundNodes = new List<T> ();
				
				foreach (var node in candidateNodes.Distinct ()) {
					if (node is T) {
						foundNodes.Add (node as T);
					}
				}
				return foundNodes;
			} catch (CircularReferenceException) {
				return new List<T> ();
			}
		}

		public bool ContainsCircularReferences ()
		{
			try {
				GetConnectedNodesDepthFirst ();
				return false;
			} catch (CircularReferenceException) {
				return true;
			}
		}

		public bool IsSubGraphValid ()
		{
			try {
				return GetConnectedNodesDepthFirst ().All (node => IsNodeValid (node).Count () <= 0);
			} catch (CircularReferenceException) {
				return false;
			}
		}

		public bool IsNodeConnected (string uniqueNodeId)
		{
			try {
				return (from node in GetConnectedNodesDepthFirst ()
					where node.UniqueNodeIdentifier == uniqueNodeId
					select node).Count () > 0;
			} catch (CircularReferenceException) {
				return false;
			}
		}
		
		IEnumerable<Node> cachedConnectedNodes = null;
		public IEnumerable<Node> GetConnectedNodesDepthFirst ()
		{
			if( cachedConnectedNodes == null )
			{
				var parseStack = new Stack<string> ();
				cachedConnectedNodes = GetConnectedNodesDepthFirst (RootNode, parseStack);
			}
			return cachedConnectedNodes;
		}

		public Node GetNode (string uniqueNodeId)
		{
			//return nodes.FirstOrDefault( x => x.UniqueNodeIdentifier == uniqueNodeId );
			return _nodes[uniqueNodeId];
		}

		private IEnumerable<string> IsNodeValid (Node validatingNode)
		{
			var errors = new List<string> ();
			
			//Check for infinite loops in graph
			if (_depthSortedNodes == null) {
				if (_lastException.CausalNodeId == validatingNode.UniqueNodeIdentifier) {
					var reason = "Circular Reference Detected: ";
					
					foreach (var node in _lastException.CircularTrace) {
						reason += node;
						reason += " -> ";
					}
					reason += "Error";
					errors.Add (reason);
				}
				return errors;
			}
			
			//If this is an input node
			if (validatingNode is IFieldInput) {
				var validatingInputNode = (IFieldInput)validatingNode;
				var allInputs = from node in Nodes
					where node is IFieldInput && (node.UniqueNodeIdentifier != validatingNode.UniqueNodeIdentifier)
					select node;
				errors.AddRange(from input in allInputs
				                where input != validatingInputNode
				                let fieldInputNode = (IFieldInput) input
				                where fieldInputNode.GetFieldName() == validatingInputNode.GetFieldName() && fieldInputNode.GetFieldType() != validatingInputNode.GetFieldType()
				                select "Shares input name with " + input.UniqueNodeIdentifier + " but not input type");
			}
			errors.AddRange (validatingNode.IsValid ( GraphType ));
			
			return errors;
		}

		public InputChannel GetInputChannel (InputChannelReference chanRef)
		{
			var node = GetNode (chanRef.NodeIdentifier);
			return node.GetInputChannel (chanRef.ChannelId);
		}

		public OutputChannel GetOutputChannel (OutputChannelReference chanRef)
		{
			var node = GetNode (chanRef.NodeIdentifier);
			return node.GetOutputChannel (chanRef.ChannelId);
		}

		public void AddNode (Node n)
		{
			var sameTypeNodes = from node in Nodes
				where node.GetType () == n.GetType ()
				orderby node.NodeID
				select node.NodeID;
			
			int nextId = 0;
			while (sameTypeNodes.Contains (nextId)) {
				++nextId;
			}
			
			n.Owner = this;
			n.AddedToGraph(nextId);
			_nodes.Add( n.UniqueNodeIdentifier, n );
		}
		
		public bool RequiresGrabPass
		{
			get{
				return GetConnectedNodesDepthFirst ().Any (x => x.RequiresGrabPass);
			}
		}
		
		public bool RequiresSceneDepth
		{
			get{
				return GetConnectedNodesDepthFirst ().Any (x => x.RequiresSceneDepth);
			}
		}
	}
}
