using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Nez.AI.GOAP {
	public class GoapAIScenarioWindow : EditorWindow {
		
		//the root graph that was first opened in the editor
		[System.NonSerialized]
//		private static AIScenario aiScenario;
		private int _rootGraphID;

		//the GrapOwner if any, that was used to open the editor and from which to read the rootGraph
		[System.NonSerialized]
		private static AIScenarioAgentComponent aiOwner;
		private int _targetOwnerID;
		private static bool isEditorLocked;

		///----------------------------------------------------------------------------------------------

		//the current instance of the opened editor
		public static GoapAIScenarioWindow current;
		//the current graph loaded for editing. Can be a nested graph of the root graph
		public static AIScenario currentScenario;
		
		///----------------------------------------------------------------------------------------------
		const float TOOLBAR_HEIGHT = 22;
		const float TOP_MARGIN    = 22;
		const float BOTTOM_MARGIN = 5;
		const int   GRID_SIZE     = 15;
		private static Rect  canvasRect;  //rect within which the graph is drawn (the window)
		private static Rect  viewRect;    //the panning rect that is drawn within canvasRect
		private static float _zoomFactor;
		
		///----------------------------------------------------------------------------------------------

		private static Event e;
		private static bool          isMultiSelecting;
		private static Vector2       selectionStartPos;
		private static System.Action OnDoPopup;
		private static bool          isResizingMinimap;
		private static bool          isDraggingMinimap;
		private static bool          willRepaint  = true;
		private static bool          fullDrawPass = true;
		
		///----------------------------------------------------------------------------------------------

		private static float lastUpdateTime = -1;
		private static Vector2? smoothPan        = null;
		private static float?   smoothZoomFactor = null;
		private static Vector2  _panVelocity     = Vector2.one;
		private static float    _zoomVelocity    = 1;
		private static float    pingValue;
		private static Rect     pingRect;
		
		//The translation of the graph
		private static Vector2 pan {
			get { return viewCanvasCenter; }
			set
			{
				if ( currentScenario != null ) {
					var t = value;
					if ( smoothPan == null ) {
						t.x = Mathf.Round(t.x); //pixel perfect correction
						t.y = Mathf.Round(t.y); //pixel perfect correction
					}
				}
			}
		}

		//The zoom factor of the graph
		private static float zoomFactor {
			get { return currentScenario != null ? Mathf.Clamp(_zoomFactor, 0.25f, 1f) : 1f; }
			set { if ( currentScenario != null ) _zoomFactor = Mathf.Clamp(value, 0.25f, 1f); }
		}

		//The center of the canvas
		private static Vector2 viewCanvasCenter {
			get { return viewRect.size / 2; }
		}

		//The mouse position in the canvas
		private static Vector2 mousePosInCanvas {
			get { return ViewToCanvas ( Event.current.mousePosition ); }
		}

		//window width. Handles retina
		private static float screenWidth {
			get { return Screen.width / EditorGUIUtility.pixelsPerPoint; }
		}

		//window height. Handles retina
		private static float screenHeight {
			get { return Screen.height / EditorGUIUtility.pixelsPerPoint; }
		}
		
		#region Initialize Window

		[MenuItem("Window/Goap AI编辑器")]
		private static void ShowWindow() {
			GoapAIScenarioWindow window =
				( GoapAIScenarioWindow ) EditorWindow.GetWindow ( typeof ( GoapAIScenarioWindow ), false, "Goap AI编辑器" );
			window.autoRepaintOnSceneChange = true;
		}

		#endregion

		#region Callback

		void OnEnable() {
            current = this;
            titleContent = new GUIContent ( "Goap", EditorGUIUtility.FindTexture ( "tree_icon_leaf" ) );

            willRepaint = true;
            fullDrawPass = true;
            wantsMouseMove = true;
            minSize = new Vector2 ( 700, 300 );

            isEditorLocked = EditorPrefs.GetBool ( "GoapAIScenarioWindow.isEditorLocked", isEditorLocked );
            zoomFactor = EditorPrefs.GetFloat ( "GoapAIScenarioWindow.zoomFactor", zoomFactor );
            
            EditorApplication.playModeStateChanged -= PlayModeChange;
            EditorApplication.playModeStateChanged += PlayModeChange;

            Selection.selectionChanged -= OnSelectionChange;
            Selection.selectionChanged += OnSelectionChange;
		}

		void OnDisable () {
			current                                =  null;
			
			EditorPrefs.SetBool ( "GoapAIScenarioWindow.isEditorLocked", isEditorLocked );
			EditorPrefs.SetFloat ( "GoapAIScenarioWindow.zoomFactor", zoomFactor );
			
			EditorApplication.playModeStateChanged -= PlayModeChange;
			Selection.selectionChanged             -= OnSelectionChange;
		}

		///Editor update
		void Update () {
			var currentTime = Time.realtimeSinceStartup;
			var deltaTime   = currentTime - lastUpdateTime;
			lastUpdateTime = currentTime;

			var needsRepaint = false;
			needsRepaint |= UpdateSmoothPan ( deltaTime );
			needsRepaint |= UpdateSmoothZoom ( deltaTime );
			needsRepaint |= UpdatePing ( deltaTime );
			if ( needsRepaint ) {
				Repaint ();
			}
		}

		void OnGUI () {
			
			//Init
			GUI.color               = Color.white;
			GUI.backgroundColor     = Color.white;
			GUI.skin.label.richText = true;
			e                       = Event.current;

			if ( currentScenario == null ) {
				current.ShowNotification ( new GUIContent ( "Please select a AI Component GameObject or a AI Asset." ) );
				return;
			}
			
			bool willDirty = false;
			if (
				( e.rawType == EventType.MouseUp && e.button != 2 ) ||
				( e.type == EventType.DragPerform ) ||
				( e.type == EventType.KeyUp && ( e.keyCode == KeyCode.Return || GUIUtility.keyboardControl == 0 ) )
			) {
				willDirty = true;
			}
			
			//initialize rects
			canvasRect = Rect.MinMaxRect ( 5, TOP_MARGIN, position.width - 5, position.height - BOTTOM_MARGIN );
			var aspect = canvasRect.width / canvasRect.height;
			var originalCanvasRect = canvasRect;

			//canvas background
			GUI.Box ( canvasRect, string.Empty, GUI.skin.box );
			
			//background grid
			DrawGrid ( canvasRect, pan, zoomFactor );
				
			HandlePreNodesGraphEvents ( mousePosInCanvas );
			
			//begin zoom
			var oldMatrix = default ( Matrix4x4 );
			if ( zoomFactor != 1f ) {
				canvasRect = StartZoomArea ( canvasRect, zoomFactor, out oldMatrix );
			}

			// calc viewRect
			{
				viewRect          =  canvasRect;
				viewRect.x        =  0;
				viewRect.y        =  0;
				viewRect.position -= pan / zoomFactor;
			}

			//main group
			GUI.BeginClip ( canvasRect, pan / zoomFactor, default ( Vector2 ), false );
			{
				BeginWindows ();

				EndWindows ();
			}
			GUI.EndClip ();

			//end zoom
			if ( zoomFactor != 1f && oldMatrix != default ( Matrix4x4 ) ) {
				EndZoomArea ( oldMatrix );
			}






			//dirty?
			if ( willDirty ) {
				willDirty   = false;
				willRepaint = true;
				
				// TODO: save asset
				
				EditorUtility.SetDirty ( currentScenario );
			}

			//repaint?
			if ( willRepaint || e.type == EventType.MouseMove ) {
				Repaint ();
			}

			if ( e.type == EventType.Repaint ) {
				fullDrawPass = false;
				willRepaint  = false;
			}
		}

		void PlayModeChange
		(
#if UNITY_2017_2_OR_NEWER
			PlayModeStateChange state
#endif
		) {
			willRepaint                      = true;
			fullDrawPass                     = true;
		}
		
		//Change viewing graph based on Graph or GraphOwner
		void OnSelectionChange() {
			
			if ( isEditorLocked ) {
				return;
			}

			if ( Selection.activeObject is AIScenario ) {
				SetReferences ( ( AIScenario )Selection.activeObject, null );
				return;
			}

			if ( Selection.activeObject is AIScenarioAgentComponent ) {
				SetReferences ( (( AIScenarioAgentComponent )Selection.activeObject).scenario, ( AIScenarioAgentComponent )Selection.activeObject );
				return;
			}

			if ( Selection.activeGameObject != null ) {
				var foundOwner = Selection.activeGameObject.GetComponent< AIScenarioAgentComponent > ();
				if ( foundOwner != null ) {
					SetReferences ( foundOwner.scenario, foundOwner );
				}
			}
		}
		
		///Update smooth pan
		bool UpdateSmoothPan(float deltaTime) {

			if ( smoothPan == null ) {
				return false;
			}

			var targetPan = (Vector2)smoothPan;
			if ( ( targetPan - pan ).magnitude < 0.1f ) {
				smoothPan = null;
				return false;
			}

			targetPan = new Vector2(Mathf.FloorToInt(targetPan.x), Mathf.FloorToInt(targetPan.y));
			pan       = Vector2.SmoothDamp(pan, targetPan, ref _panVelocity, 0.08f, Mathf.Infinity, deltaTime);
			return true;
		}

		///Update smooth pan
		bool UpdateSmoothZoom(float deltaTime) {

			if ( smoothZoomFactor == null ) {
				return false;
			}

			var targetZoom = (float)smoothZoomFactor;
			if ( Mathf.Abs(targetZoom - zoomFactor) < 0.00001f ) {
				smoothZoomFactor = null;
				return false;
			}

			zoomFactor = Mathf.SmoothDamp(zoomFactor, targetZoom, ref _zoomVelocity, 0.08f, Mathf.Infinity, deltaTime);
			if ( zoomFactor > 0.99999f ) { zoomFactor = 1; }
			return true;
		}

		///Update ping value
		bool UpdatePing(float deltaTime) {
			if ( pingValue > 0 ) {
				pingValue -= deltaTime;
				return true;
			}
			return false;
		}
		
		///----------------------------------------------------------------------------------------------

		//GUI space to canvas space
		static Vector2 ViewToCanvas(Vector2 viewPos) {
			return ( viewPos - pan ) / zoomFactor;
		}

		//Canvas space to GUI space
		static Vector2 CanvasToView ( Vector2 canvasPos ) {
			return canvasPos * zoomFactor + pan;
		}

		//Show modal quick popup
		static void DoPopup ( System.Action Call ) {
			OnDoPopup = Call;
		}

		//Just so that there is some repainting going on
		void OnInspectorUpdate () {
			if ( !willRepaint ) {
				Repaint ();
			}
		}

		///Set GraphEditor inspected references
		public static void SetReferences ( AIScenario scenario, AIScenarioAgentComponent comp ) {
			
			if ( current == null ) {
				Debug.Log ( "AIEditor is not open." );
				return;
			}

			willRepaint  = true;
			fullDrawPass = true;
			currentScenario = scenario;
			aiOwner = comp;

			current.Repaint ();
		}
		
		//Starts a zoom area, returns the scaled container rect
		static Rect StartZoomArea ( Rect container, float zoomFactor, out Matrix4x4 oldMatrix ) {
			GUI.EndGroup ();
			container.y      += TOOLBAR_HEIGHT;
			container.width  *= 1 / zoomFactor;
			container.height *= 1 / zoomFactor;
			oldMatrix        =  GUI.matrix;
			var matrix1 = Matrix4x4.TRS ( new Vector2 ( container.x, container.y ), Quaternion.identity, Vector3.one );
			var matrix2 = Matrix4x4.Scale ( new Vector3 ( zoomFactor, zoomFactor, 1f ) );
			GUI.matrix = matrix1 * matrix2 * matrix1.inverse * GUI.matrix;
			return container;
		}

		//Ends the zoom area
		static void EndZoomArea ( Matrix4x4 oldMatrix ) {
			GUI.matrix = oldMatrix;
			GUI.BeginGroup ( new Rect ( 0, TOOLBAR_HEIGHT, screenWidth, screenHeight ) );
		}
		
		private static bool mouse2Down;

        ///Graph events BEFORE nodes
        static void HandlePreNodesGraphEvents ( Vector2 canvasMousePos ) {

            if ( e.button == 2 && ( e.type == EventType.MouseDown || e.type == EventType.MouseUp ) ) {
	            Undo.RecordObject ( currentScenario, "Graph Pan" );
            }

            if ( e.type == EventType.MouseUp || e.type == EventType.KeyUp ) {
	            
            }

            if ( e.type == EventType.KeyDown && e.keyCode == KeyCode.F && GUIUtility.keyboardControl == 0 ) {
	            
            }

            if ( e.type == EventType.MouseDown && e.button == 2 && e.clickCount == 2 ) {
	            FocusPosition ( ViewToCanvas ( e.mousePosition ) );
            }

            if ( e.type == EventType.ScrollWheel ) {
	            if ( canvasRect.Contains ( e.mousePosition ) ) {
		            var zoomDelta = e.shift ? 0.1f : 0.25f;
//		            ZoomAt ( e.mousePosition, -e.delta.y > 0 ? zoomDelta : -zoomDelta );
		            ZoomAt ( mousePosInCanvas, -e.delta.y > 0 ? zoomDelta : -zoomDelta );
	            }
            }

            if ( e.type == EventType.MouseDrag && e.alt && e.button == 1 ) {
	            ZoomAt ( new Vector2 ( screenWidth / 2, screenHeight / 2 ), e.delta.x / 100 );
	            e.Use ();
            }

            if ( ( e.button == 2 && e.type == EventType.MouseDrag && canvasRect.Contains(e.mousePosition) ) ||
                ( ( e.type == EventType.MouseDown || e.type == EventType.MouseDrag ) && e.alt && e.isMouse ) ) {
                pan += e.delta;
                smoothPan = null;
                smoothZoomFactor = null;
                e.Use();
            }

            if ( e.type == EventType.MouseDown && e.button == 2 ) { mouse2Down = true; }
            if ( e.type == EventType.MouseUp && e.button == 2 ) { mouse2Down = false; }
            if ( e.alt || mouse2Down ) {
                EditorGUIUtility.AddCursorRect(new Rect(0, 0, screenWidth, screenHeight), MouseCursor.Pan);
            }
        }
        
		#endregion
		
		///Translate the graph to to center of the target pos
		public static void FocusPosition(Vector2 targetPos, bool smooth = true) {
			if ( smooth ) {
				smoothPan =  -targetPos;
				smoothPan += new Vector2(viewRect.width / 2, viewRect.height / 2);
				smoothPan *= zoomFactor;
			} else {
				pan              =  -targetPos;
				pan              += new Vector2(viewRect.width / 2, viewRect.height / 2);
				pan              *= zoomFactor;
				smoothPan        =  null;
				smoothZoomFactor =  null;
			}
		}
		
		///Zoom with center position
		static void ZoomAt(Vector2 center, float delta) {
			if ( zoomFactor == 1 && delta > 0 ) return;
			var pinPoint = ( center - pan ) / zoomFactor;
			var newZ     = zoomFactor;
			newZ             += delta;
			newZ             =  Mathf.Clamp(newZ, 0.25f, 1f);
			smoothZoomFactor =  newZ;

			var a    = ( pinPoint * newZ ) + pan;
			var b    = center;
			var diff = b - a;
			smoothPan = pan + diff;
		}
		
		//Draw a simple grid
		static void DrawGrid ( Rect container, Vector2 offset, float zoomFactor ) {

			if ( Event.current.type != EventType.Repaint ) {
				return;
			}

			Handles.color = new Color ( 0, 0, 0, 0.15f );

			var drawGridSize = zoomFactor > 0.5f ? GRID_SIZE : GRID_SIZE * 5;
			var step         = drawGridSize * zoomFactor;

			var xDiff  = offset.x % step;
			var xStart = container.xMin + xDiff;
			var xEnd   = container.xMax;
			for ( var i = xStart; i < xEnd; i += step ) {
				Handles.DrawLine ( new Vector3 ( i, container.yMin, 0 ), new Vector3 ( i, container.yMax, 0 ) );
			}

			var yDiff  = offset.y % step;
			var yStart = container.yMin + yDiff;
			var yEnd   = container.yMax;
			for ( var i = yStart; i < yEnd; i += step ) {
				Handles.DrawLine ( new Vector3 ( 0, i, 0 ), new Vector3 ( container.xMax, i, 0 ) );
			}

			Handles.color = Color.white;
		}
	}
}
