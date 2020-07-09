using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Nez.AI.GOAP;

namespace Nez.AI
{
	[CustomEditor ( typeof ( AIScenario ) )]
	public class AIScenarioEditor : Editor {
		private AIScenario              _self;
		private GUIStyle                _rowStyleA;
		private GUIStyle                _rowStyleB;
		private GUIStyle                _rowStyleC;
		private GUIStyle                _fieldStyle;
		private Vector2                 _scrollPosition;
		private Type                    _currentEditType;
		private AIScenarioConditionItem _currentEditInstance;

		#region Unity Callbacks

		private void OnEnable () {
			_self = ( AIScenario ) target;

			_rowStyleA                   = new GUIStyle ();
			_rowStyleA.alignment         = TextAnchor.MiddleLeft;
			_rowStyleA.normal.background = CreateBG ( 0.225f );

			_rowStyleB                   = new GUIStyle ();
			_rowStyleB.alignment         = TextAnchor.MiddleLeft;
			_rowStyleB.normal.background = CreateBG ( 0.255f ); // 0.285f

			_rowStyleC                   = new GUIStyle ();
			_rowStyleC.alignment         = TextAnchor.MiddleLeft;
			_rowStyleC.normal.background = CreateBG ( 0.295f ); // 0.325f
		}

		private void OnDisable () {
			AIScenarioCondition conditions = (AIScenarioCondition)serializedObject.FindProperty ( "conditions" ).FindPropertyRelative ( LIST_PROPERTY_NAME ).GetValue ();
			foreach ( var item in conditions.list ) {
				item.Save ();
			}
		}

		public override void OnInspectorGUI () {
			serializedObject.Update ();

			DrawActionList ();
			DrawGoalList ();

			EditorGUILayout.Separator ();
			base.OnInspectorGUI ();
			
			GetList ( serializedObject.FindProperty ( "conditions" ) ).DoLayoutList ();

			DrawConditionInspectorGUI ();

			EditorUtility.SetDirty ( _self );
			serializedObject.ApplyModifiedProperties ();
		}

		#endregion

		#region Private Methods

		private void DrawActionList () {
			Color c        = GUI.color;
			int   delIndex = -1;

			GUILayout.BeginVertical ();
			EditorGUILayout.LabelField ( "行动列表", EditorStyles.boldLabel );

			AIScenarioAction action;
			if ( _self.actions.Length == 0 ) {
				GUILayout.BeginVertical ( "box" );
				EditorGUILayout.LabelField ( "The Action List is Empty." );
				GUILayout.EndVertical ();
			}

			string actionName = "";
			for ( int i = 0, n = _self.actions.Length; i < n; i++ ) {
				GUILayout.BeginVertical ( "box" );

				action = _self.actions[ i ];
				GUILayout.BeginVertical ( _rowStyleA );
				GUILayout.BeginHorizontal ();
				GUI.color = c * new Color ( 1.0f, 1.0f, 0.5f );
				if ( action.cost > 0 ) {
					EditorGUILayout.LabelField ( string.Format ( "{0} [{1}]", action.name, action.cost ),
						EditorStyles.boldLabel );
				}
				else {
					EditorGUILayout.LabelField ( action.name, EditorStyles.boldLabel );
				}

				GUI.color = c;

				action.isActived = GUILayout.Toggle ( action.isActived, string.Empty, GUILayout.MaxWidth ( 18.0f ),
					GUILayout.MaxHeight ( 16.0f ) );
				
				if ( GUILayout.Button ( ( action.isOpened ) ? "-" : "+", GUILayout.MaxWidth ( 18.0f ),
					GUILayout.MaxHeight ( 16.0f ) ) ) {
					action.isOpened = !action.isOpened;
				}
				
				if ( GUILayout.Button ( "↑", GUILayout.MaxWidth ( 18.0f ), GUILayout.MaxHeight ( 16.0f ) ) ) {
					if ( i - 1 >= 0 ) {
						var temp = _self.actions[ i - 1 ];
						_self.actions[ i - 1 ] = action;
						_self.actions[ i ]     = temp;
					}
				}
				
				if ( GUILayout.Button ( "↓", GUILayout.MaxWidth ( 18.0f ), GUILayout.MaxHeight ( 16.0f ) ) ) {
					if ( i + 1 < _self.actions.Length ) {
						var temp = _self.actions[ i + 1 ];
						_self.actions[ i + 1 ] = action;
						_self.actions[ i ]     = temp;
					}
				}

				GUI.color = c * new Color ( 1.0f, 1.0f, 0.5f );
				if ( GUILayout.Button ( "x", GUILayout.MaxWidth ( 18.0f ), GUILayout.MaxHeight ( 16.0f ) ) ) {
					delIndex = i;
				}

				GUI.color = c;
				GUILayout.EndHorizontal ();
				GUILayout.Space ( 2.0f );
				GUILayout.EndVertical ();

				if ( action.isOpened ) {
					GUILayout.Space ( 2.0f );

					actionName = EditorGUILayout.TextField ( "Action", action.name );
					if ( !string.Equals ( actionName, action.name ) ) {
//						if ( string.Equals ( action.name, action.state ) ) {
//							action.state = actionName;
//						}

						action.name = actionName;
					}

//					action.state = EditorGUILayout.TextField ( "State", action.state );
					action.cost  = EditorGUILayout.IntField ( "Cost", action.cost );

					GUILayout.Space ( 10.0f );
					DrawConditionsList ( "前提条件", ref action.pre );
					GUILayout.Space ( 10.0f );
					DrawConditionsList ( "执行结果", ref action.post );

					GUILayout.Space ( 2.0f );
				}

				GUILayout.EndVertical ();
			}

			if ( GUILayout.Button ( "Add Action" ) ) {
				AddToArray< AIScenarioAction > ( ref _self.actions, new AIScenarioAction () );
			}

			if ( delIndex > -1 ) {
				RemoveFromArrayAt< AIScenarioAction > ( ref _self.actions, delIndex );
			}

			GUILayout.EndVertical ();
		}

		private void DrawGoalList () {
			Color c        = GUI.color;
			int   delIndex = -1;

			EditorGUILayout.Separator ();
			EditorGUILayout.BeginVertical ();
			EditorGUILayout.LabelField ( "目标列表", EditorStyles.boldLabel );

			AIScenarioGoal goal;
			if ( _self.goals.Length == 0 ) {
				GUILayout.BeginVertical ( "box" );
				EditorGUILayout.LabelField ( "The Goal List is Empty." );
				GUILayout.EndVertical ();
			}

			for ( int i = 0, n = _self.goals.Length; i < n; i++ ) {
				GUILayout.BeginVertical ( "box" );

				goal = _self.goals[ i ];
				GUILayout.BeginVertical ( _rowStyleA );
				GUILayout.BeginHorizontal ();
				GUI.color = c * new Color ( 1.0f, 1.0f, 0.5f );
				EditorGUILayout.LabelField ( goal.name, EditorStyles.boldLabel );
				GUI.color = c;

				if ( GUILayout.Button ( ( goal.isOpened ) ? "-" : "+", GUILayout.MaxWidth ( 18.0f ),
					GUILayout.MaxHeight ( 16.0f ) ) ) {
					goal.isOpened = !goal.isOpened;
				}
				
				if ( GUILayout.Button ( "↑", GUILayout.MaxWidth ( 18.0f ), GUILayout.MaxHeight ( 16.0f ) ) ) {
					if ( i - 1 >= 0 ) {
						var temp = _self.goals[ i - 1 ];
						_self.goals[ i - 1 ] = goal;
						_self.goals[ i ] = temp;
					}
				}
				
				if ( GUILayout.Button ( "↓", GUILayout.MaxWidth ( 18.0f ), GUILayout.MaxHeight ( 16.0f ) ) ) {
					if ( i + 1 < _self.goals.Length ) {
						var temp = _self.goals[ i + 1 ];
						_self.goals[ i + 1 ] = goal;
						_self.goals[ i ]     = temp;
					}
				}

				GUI.color = c * new Color ( 1.0f, 1.0f, 0.5f );
				if ( GUILayout.Button ( "x", GUILayout.MaxWidth ( 18.0f ), GUILayout.MaxHeight ( 16.0f ) ) ) {
					delIndex = i;
				}

				GUI.color = c;
				GUILayout.EndHorizontal ();
				GUILayout.Space ( 2.0f );
				GUILayout.EndVertical ();

				if ( goal.isOpened ) {
					GUILayout.Space ( 2.0f );
					goal.name = EditorGUILayout.TextField ( "Goal", goal.name );
					GUILayout.Space ( 10.0f );
					DrawConditionsList ( "达成目标所需条件", ref goal.conditions );
					GUILayout.Space ( 2.0f );
				}

				GUILayout.EndVertical ();
			}

			if ( GUILayout.Button ( "Add Goal" ) ) {
				AddToArray< AIScenarioGoal > ( ref _self.goals, new AIScenarioGoal () );
			}

			if ( delIndex > -1 ) {
				RemoveFromArrayAt< AIScenarioGoal > ( ref _self.goals, delIndex );
			}

			EditorGUILayout.EndVertical ();
		}

		private void DrawConditionsList ( string aLabel, ref AIScenarioItem[] aConditions ) {
			Color c = GUI.color;
			GUILayout.BeginVertical ( _rowStyleA );

			var list = new string[_self.conditions.list.Length + 1];
			list[ 0 ] = "- Select to Add -";
			for ( int i = 1, n = list.Length; i < n; i++ ) {
				list[ i ] = _self.conditions.list[ i - 1 ].name;
			}

			int addIndex = EditorGUILayout.Popup ( aLabel, 0, list );
			if ( addIndex > 0 ) {
				var item = new AIScenarioItem () {
					id    = _self.conditions.GetID ( list[ addIndex ] ),
					value = true
				};
				AddToArray< AIScenarioItem > ( ref aConditions, item );
			}

			GUILayout.EndVertical ();

			int delIndex = -1;
			for ( int i = 0, n = aConditions.Length; i < n; i++ ) {
				GUILayout.BeginVertical ( ( i % 2 == 0 ) ? _rowStyleB : _rowStyleC );
				GUILayout.BeginHorizontal ();
				GUI.color = c * ( ( aConditions[ i ].value )
					            ? new Color ( 0.5f, 1.0f, 0.5f )
					            : new Color ( 1.0f, 0.5f, 0.5f ) ); // green : red
				if ( GUILayout.Button ( ( aConditions[ i ].value ) ? "I" : "O", GUILayout.MaxWidth ( 20.0f ),
					GUILayout.MaxHeight ( 16.0f ) ) ) {
					aConditions[ i ].value = !aConditions[ i ].value;
				}

				GUILayout.Label ( _self.conditions.GetName ( aConditions[ i ].id ) );
				GUI.color = c;
				if ( GUILayout.Button ( "x", GUILayout.MaxWidth ( 20.0f ), GUILayout.MaxHeight ( 16.0f ) ) ) {
					delIndex = i;
				}

				GUILayout.EndHorizontal ();
				GUILayout.Space ( 2.0f );
				GUILayout.EndVertical ();
			}

			if ( delIndex > -1 ) {
				RemoveFromArrayAt< AIScenarioItem > ( ref aConditions, delIndex );
			}
		}

		private void DrawConditionInspectorGUI () {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			_scrollPosition = EditorGUILayout.BeginScrollView ( _scrollPosition, GUILayout.Height ( 300 ) );
//			EditorGUI.BeginDisabledGroup ( true );

			if ( _currentEditType != null && _currentEditInstance != null && _currentEditInstance.typeInstance != null ) {
				DrawClassPublicFields ( _currentEditType, _currentEditInstance.typeInstance );
			}

//			EditorGUI.EndDisabledGroup ();
			EditorGUILayout.EndScrollView ();
			
			if ( _currentEditType != null && _currentEditInstance != null && _currentEditInstance.typeInstance != null ) {
				EditorGUI.BeginDisabledGroup ( true );
				EditorGUILayout.TextArea ( _currentEditInstance.typeData );
				EditorGUI.EndDisabledGroup ();
			}
		}

		private void DrawClassPublicFields ( System.Type actionType, object actionInstance ) {
			
			if ( _fieldStyle == null ) {
				_fieldStyle                  = new GUIStyle ( EditorStyles.label );
				_fieldStyle.normal.textColor = Color.white;
				_fieldStyle.fontSize         = 10;
				_fieldStyle.fontStyle        = FontStyle.Normal;
				_fieldStyle.alignment        = TextAnchor.MiddleLeft;
			}

			if ( actionInstance != null ) {
				EditorGUI.BeginDisabledGroup ( true );
				EditorGUILayout.LabelField ( $"{_currentEditInstance.name}   #{actionInstance.GetHashCode ()}" );
				EditorGUI.EndDisabledGroup ();
			}

			if ( _currentEditInstance.publicFields == null ) {
				return;
			}
			
			FieldInfo[] publicFields = _currentEditInstance.publicFields;

			foreach ( FieldInfo field in publicFields ) {
				Type type = field.FieldType;
//				string title = GetAttribute<TitleAttribute>(field)?.Text;
//
//				if (title == null)
//					title = field.Name;

				HideInInspector hideAttr = GetAttribute< HideInInspector > ( field );
//
				if ( hideAttr != null ) {
					continue;
				}

				object fieldValue = GUIForFieldTypes ( field.Name, field, actionInstance );

				// 提交字段值改动
				if ( fieldValue != field.GetValue ( actionInstance ) ) {
					field.SetValue ( actionInstance, fieldValue );
				}
			}
		}

		private void AddToArray< T > ( ref T[] aSource, T aItem ) {
			var newArray = new T[aSource.Length + 1];
			for ( int i = 0, n = aSource.Length; i < n; i++ ) {
				newArray[ i ] = aSource[ i ];
			}

			newArray[ newArray.Length - 1 ] = aItem;
			aSource                         = newArray;
		}

		private void RemoveFromArrayAt< T > ( ref T[] aSource, int aRemoveIndex ) {
			int index    = 0;
			var newArray = new T[aSource.Length - 1];
			for ( int i = 0, n = aSource.Length; i < n; i++ ) {
				if ( i != aRemoveIndex ) {
					newArray[ index ] = aSource[ i ];
					index++;
				}
			}

			aSource = newArray;
		}

		private Texture2D CreateBG ( float aColorValue ) {
			Texture2D bg = new Texture2D ( 1, 1 );
			bg.SetPixel ( 0, 0, GUI.color * new Color ( aColorValue, aColorValue, aColorValue ) );
			bg.Apply ();
			return bg;
		}

		#endregion

		#region AIScenarioCondition
		private const float  FIELD_PADDING      = 2.0f;
		private const string LIST_PROPERTY_NAME = "list";
		private const string NAME_PROPERTY      = "name";
		private const string TYPE_PROPERTY      = "type";
		private const string PRE_PROPERTY       = "pre";
		private const string POST_PROPERTY      = "post";
		private const string CONDS_PROPERTY     = "conditions";
		private const string ID_PROPERTY        = "id";
		private const string SERIAL_ID_PROPERTY = "serialId";
		
		private ReorderableList _list;
		
		private ReorderableList GetList ( SerializedProperty aProperty ) {
			
			if ( _list == null ) {
				var nullGUIContent = new GUIContent ();
				var listProperty   = aProperty.FindPropertyRelative ( LIST_PROPERTY_NAME );
				_list = new ReorderableList ( aProperty.serializedObject, listProperty );

				_list.drawHeaderCallback = ( Rect aRect ) => { EditorGUI.LabelField ( aRect, "条件定义" ); };

				_list.onSelectCallback = ( ReorderableList list ) => {
					
					AIScenarioConditionItem item = (AIScenarioConditionItem)listProperty.GetArrayElementAtIndex ( list.index ).FindPropertyRelative ( TYPE_PROPERTY ).GetValue ();
					ClassTypeReference type = item.type;

					if ( type == null || type.Type == null ) {
						_currentEditType     = null;
						_currentEditInstance = null;
						return;
					}
					
					if ( true ) {

						bool needCreateInstance = false;
						object instance = item.typeInstance;

						if ( instance == null ) {
							needCreateInstance = true;
						}
						else {
							if ( instance.GetType () != type.Type ) {
								needCreateInstance = true;
							}
						}
						
						if ( needCreateInstance ) {
//							instance = type.Type.Assembly.CreateInstance ( type.Type.ToString () );
							item.typeInstance = item.Load ();
							item.publicFields = type.Type.GetFields ( BindingFlags.Instance | BindingFlags.Public );
						}

						_currentEditType = type.Type;
						_currentEditInstance = item;
					}
				};

				_list.onReorderCallbackWithDetails = ( ReorderableList list, int oldIndex, int newIndex ) => {
					
					// bug ref issus
					object temp = _self.conditions.list[ oldIndex ].typeInstance;
					var temp_2 = _self.conditions.list[ oldIndex ].publicFields;
					_self.conditions.list[ oldIndex ].typeInstance = _self.conditions.list[ newIndex ].typeInstance;
					_self.conditions.list[ oldIndex ].publicFields = _self.conditions.list[ newIndex ].publicFields;
					_self.conditions.list[ newIndex ].typeInstance = temp;
					_self.conditions.list[ newIndex ].publicFields = temp_2;
//					Debug.Log ( $"onReorderCallbackWithDetails old[{oldIndex}]:{_self.conditions.list[ oldIndex ].typeInstance.GetHashCode()} new[{newIndex}]:{_self.conditions.list[ newIndex ].typeInstance.GetHashCode()}" );
				};

				_list.onRemoveCallback = ( ReorderableList list ) => {
//					int id = 0;
//					AIScenarioCondition cond = (AIScenarioCondition)listProperty.GetValue ();
//					foreach ( var item in cond.list ) {
//						item.id = id++;
//					}
//					
//					cond.serialId = id;

//					List<int> indexToRemove = new List< int >();
//					List<AIScenarioItem> items = new List< AIScenarioItem >();
//
//					for ( var i = 0; i < _self.actions.Length; ++i ) {
//
//						var action = _self.actions[ i ];
//						var pre = serializedObject.FindProperty ( "actions" ).GetArrayElementAtIndex ( i )
//						                          .FindPropertyRelative ( PRE_PROPERTY );
//						var post = serializedObject.FindProperty ( "actions" ).GetArrayElementAtIndex ( i )
//						                          .FindPropertyRelative ( POST_PROPERTY );
//						
//						foreach ( var condition in action.pre ) {
//							if ( !_self.conditions.Has ( condition.id ) ) {
//								indexToRemove.Add ( Array.IndexOf ( action.pre, condition ) );
//							}
//						}
//						foreach ( var index in indexToRemove ) {
//							pre.DeleteArrayElementAtIndex ( index );
//						}
//						indexToRemove.Clear ();
//						
//						foreach ( var condition in action.post ) {
//							if ( !_self.conditions.Has ( condition.id ) ) {
//								indexToRemove.Add ( Array.IndexOf ( action.post, condition ) );
//							}
//						}
//						foreach ( var index in indexToRemove ) {
//							post.DeleteArrayElementAtIndex ( index );
//						}
//						indexToRemove.Clear ();
//					}
//					
//					for ( var i = 0; i < _self.goals.Length; ++i ) {
//
//						var conds = serializedObject.FindProperty ( "goals" ).GetArrayElementAtIndex ( i )
//						                          .FindPropertyRelative ( CONDS_PROPERTY );
//
//						foreach ( var condition in _self.goals[ i ].conditions ) {
//							if ( !_self.conditions.Has ( condition.id ) ) {
//								indexToRemove.Add ( Array.IndexOf ( _self.goals[ i ].conditions, condition ) );
//							}
//						}
//						foreach ( var index in indexToRemove ) {
//							conds.DeleteArrayElementAtIndex ( index );
//						}
//						indexToRemove.Clear ();
//					}
//					
//					indexToRemove.Clear ();
//					items.Clear ();
					
					var conditions = aProperty.FindPropertyRelative ( LIST_PROPERTY_NAME );
					conditions.DeleteArrayElementAtIndex ( list.index );
				};
				
				_list.drawElementCallback = ( Rect aRect, int aIndex, bool aIsActive, bool aIsFocused ) => {
					aRect.y += FIELD_PADDING;
					string name = listProperty.GetArrayElementAtIndex ( aIndex ).FindPropertyRelative ( NAME_PROPERTY ).stringValue;
					Rect   r    = new Rect ( aRect.x, aRect.y, aRect.width * 0.35f, EditorGUIUtility.singleLineHeight );
					listProperty.GetArrayElementAtIndex ( aIndex ).FindPropertyRelative ( NAME_PROPERTY ).stringValue = EditorGUI.TextField ( r, name );
					
					r.x     += aRect.width * 0.35f;
					r.width =  aRect.width * 0.65f;
					EditorGUI.PropertyField ( r, listProperty.GetArrayElementAtIndex ( aIndex ).FindPropertyRelative ( TYPE_PROPERTY ), nullGUIContent );
				};

				_list.onAddCallback = ( ReorderableList aList ) => {
					var conditions = aProperty.FindPropertyRelative ( LIST_PROPERTY_NAME );
//					int id         = aProperty.FindPropertyRelative ( SERIAL_ID_PROPERTY ).intValue;
//					aProperty.FindPropertyRelative ( SERIAL_ID_PROPERTY ).intValue = ++id;

					int length = conditions.arraySize;
					conditions.InsertArrayElementAtIndex ( length );
					conditions.GetArrayElementAtIndex ( length ).FindPropertyRelative ( ID_PROPERTY ).intValue = conditions.GetArrayElementAtIndex ( length ).GetHashCode ();
					conditions.GetArrayElementAtIndex ( length ).FindPropertyRelative ( NAME_PROPERTY ).stringValue = "<Unnamed>";
					
//					Array.Resize ( ref _self.conditions.list, _self.conditions.list.Length + 1 );
//					
//					_self.conditions.list[ _self.conditions.list.Length - 1 ] = new AIScenarioConditionItem ();
//
//					if ( _self.conditions.list.Length > 1 ) {
//						var up = _self.conditions.list[ _self.conditions.list.Length - 2 ];
//						_self.conditions.list[ _self.conditions.list.Length - 1 ] = new AIScenarioConditionItem ();
//						_self.conditions.list[ _self.conditions.list.Length - 1 ].id = _self.conditions.list[ _self.conditions.list.Length - 1 ].GetHashCode ();
//						_self.conditions.list[ _self.conditions.list.Length - 1 ].name = "<Unnamed>";
//						_self.conditions.list[ _self.conditions.list.Length - 1 ].type = up.type != null ? new ClassTypeReference ( up.type.Type ) : null;
//					}
				};
			}

			return _list;
		}
		#endregion

		#region GUIDrawField

		private object GUIForFieldTypes ( string title, FieldInfo field, object instance ) {
			Type   type       = field.FieldType;
			object fieldValue = field.GetValue ( instance );

			return GUIForFieldTypes ( title, type, fieldValue, field );
		}

		private object GUIForFieldTypes(string title, Type type, object fieldValue, FieldInfo field = null)
        {
	        if (type == typeof(int))
                fieldValue = EditorGUILayout.IntField(title, (int)fieldValue);
            else if (type == typeof(float))
            {
//                    FloatSliderAttribute attribute = GetAttribute<FloatSliderAttribute>(field);
//                    if (attribute == null)
                    fieldValue = EditorGUILayout.FloatField(title, Convert.ToSingle(fieldValue));
//                    else
//                    {
//                        EditorGUILayout.BeginHorizontal();
//                        EditorGUILayout.PrefixLabel(title);
//                        fieldValue = EditorGUILayout.FloatField(Convert.ToSingle(fieldValue), GUILayout.MaxWidth(75f));
//                        float num1 = Convert.ToSingle(fieldValue);
//                        EditorGUI.BeginChangeCheck();
//                        float num2 = GUILayout.HorizontalSlider(num1, attribute.MinValue, attribute.MaxValue);
//                        if (EditorGUI.EndChangeCheck())
//                            fieldValue = num2;
//                        EditorGUILayout.EndHorizontal();
//                    }
            }
            else if (type == typeof(bool))
                fieldValue = EditorGUILayout.Toggle(title, (bool)fieldValue);
            else if (type == typeof(string))
                fieldValue = EditorGUILayout.TextField(title, (string)fieldValue);
            else if (type == typeof(Vector2))
                fieldValue = EditorGUILayout.Vector2Field(title, (Vector2)fieldValue);
            else if (type == typeof(Vector3))
                fieldValue = EditorGUILayout.Vector3Field(title, (Vector3)fieldValue);
            else if (type == typeof(Vector4))
                fieldValue = EditorGUILayout.Vector4Field(title, (Vector4)fieldValue);
            else if (type == typeof(Color))
                fieldValue = EditorGUILayout.ColorField(title, (Color)fieldValue);
            else if (type == typeof(GameObject))
                fieldValue = EditorGUILayout.ObjectField(title, (UnityEngine.Object)fieldValue, typeof(GameObject), false);
            else if (type == typeof(RuntimeAnimatorController))
                fieldValue = EditorGUILayout.ObjectField(title, (UnityEngine.Object)fieldValue, typeof(RuntimeAnimatorController), false);
            else if (type.IsSubclassOf(typeof(UnityEngine.Object)) || type == typeof(UnityEngine.Object))
                fieldValue = EditorGUILayout.ObjectField(title, (UnityEngine.Object)fieldValue, typeof(UnityEngine.Object), false);
            else if (type.IsSubclassOf(typeof(System.Enum)))
                fieldValue = EditorGUILayout.EnumPopup(title, (System.Enum)fieldValue);
            else if (IsSupportedArray(type))
            {
                Type elementType = type.GetElementType();
                Array array = fieldValue == null ? Array.CreateInstance(elementType, 0) : (Array)fieldValue;
    
                int newSize = array.Length;
                newSize = EditorGUILayout.IntField(title, newSize);
                newSize = Mathf.Clamp(newSize, 0, 0xFFFF);
    
                //if (Event.current.keyCode == KeyCode.Return) {
                if (newSize != array.Length)
                {
                    if (array != null && newSize != array.Length)
                    {
                        Array ins = Array.CreateInstance(elementType, newSize);
                        Array.Copy(array, ins, Math.Min(array.Length, newSize));
                        array = ins;
                        //Debug.Log("Resize Array");
                    }
                }
    
                for (int index = 0; index < array.Length; ++index) {
                    array.SetValue(GUIForFieldTypes($"      Element {index}", elementType, array.GetValue(index)), index);
                }
    
                fieldValue = array;
            }
    
            return fieldValue;
        }

		private static T GetAttribute< T > ( FieldInfo t ) where T : Attribute {
			var attributes = t.GetCustomAttributes ( typeof ( T ), false );

			if ( attributes == null )
				return default ( T );
			foreach ( Attribute attribute in attributes ) {
				T obj = attribute as T;
				if ( obj != null )
					return obj;
			}

			return default ( T );
		}

		private bool IsSupportedArray ( Type type ) {
			
			if ( !type.IsArray )
				return false;
			else {
				return type.GetElementType () == typeof ( int ) ||
				       type.GetElementType () == typeof ( float ) ||
				       type.GetElementType () == typeof ( bool ) ||
				       type.GetElementType () == typeof ( string ) ||
				       type.GetElementType () == typeof ( Vector2 ) ||
				       type.GetElementType () == typeof ( Vector3 ) ||
				       type.GetElementType () == typeof ( Vector4 ) ||
				       type.GetElementType () == typeof ( Quaternion )
					/*type.GetElementType().IsSubclassOf(typeof(UnityEngine.Object))*/;
			}
		}

		#endregion
		
	}
}
