using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace RPGStatSystem.Editor {

	[CustomEditor ( typeof ( RPGStatCollectionDefine ) )]
	public class RPGStatCollectionDefineInspector : UnityEditor.Editor {

		ReorderableList reorderableList;
		RPGStatCollectionDefine define;
		Texture2D elmBackgroundFocusedTex;

		void OnEnable () {

			define = (RPGStatCollectionDefine)target;

			elmBackgroundFocusedTex = new Texture2D ( 1, 1 );
			elmBackgroundFocusedTex.SetPixel ( 0, 0, new Color ( 1f, 0.5f, 0.5f, 0.5f ) );
			elmBackgroundFocusedTex.Apply ();

			var prop = serializedObject.FindProperty ( "datas" );

			reorderableList = new ReorderableList ( serializedObject, prop );
			reorderableList.elementHeight = 35; // 90

			reorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField ( rect, $"{define.name} 属性定义" );

			reorderableList.drawElementCallback += (rect, index, isActive, isFocused) => {
				var element = prop.GetArrayElementAtIndex ( index );
				rect.height -= 4;
				rect.y += 2;
				EditorGUI.PropertyField ( rect, element );
			};

			//reorderableList.onSelectCallback = (list) =>
			//{
			//    var element = prop.GetArrayElementAtIndex(list.index);
			//    selectedClip = element.FindPropertyRelative("clip").objectReferenceValue as AudioClip;
			//};

			var defaultColor = GUI.backgroundColor;
			reorderableList.drawElementBackgroundCallback += (rect, index, isActive, isFocused) => {
				GUI.backgroundColor = Color.yellow;
				if ( isFocused ) {
					GUI.DrawTexture ( rect, elmBackgroundFocusedTex );
				}
			};
			
//			reorderableList.onAddDropdownCallback += ( Rect buttonRect, ReorderableList list ) => {
//				
//				//追加するメニュー作成
//				GenericMenu menu = new GenericMenu ();
//				
//				//チェックマークが付いたボタン追加(funcで押した時の処理を設定)
//				menu.AddItem ( new GUIContent ( "RPGStat" ),  on : true,
//					func : () => { _stringList.Add ( _stringList.Count.ToString () ); } );
//
//				//チェックマークが付いてないボタン追加(funcで押した時の処理を設定)
//				menu.AddItem ( new GUIContent ( "RPGStatModifiable" ), on : false,
//					func : () => { _stringList.Add ( _stringList.Count.ToString () ); } );
//
//				//区切り線追加
//				menu.AddSeparator ( "" );
//
//				//押せないボタン追加
//				menu.AddDisabledItem ( new GUIContent ( "Button3" ) );
//
//				//メニュー表示
//				menu.DropDown ( buttonRect );
//			};
		}

		public override void OnInspectorGUI () {

			//var originColor = GUI.backgroundColor;

			//EditorGUILayout.LabelField("音效组命名");
			//GUI.backgroundColor = Color.red;
			//soundList.GroupName = EditorGUILayout.TextField(soundList.GroupName);
			//GUI.backgroundColor = originColor;

			EditorGUILayout.Space ();

			serializedObject.Update ();
			reorderableList.DoLayoutList ();
			
			EditorUtility.SetDirty ( define );
			serializedObject.ApplyModifiedProperties ();

			//EditorGUILayout.Space ();

			GUI.backgroundColor = Color.gray;

			EditorGUILayout.HelpBox ( "RPGStat: 基本属性", MessageType.Info );
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ( "RPGStatModifiable: 可以被多个Modifier修饰", MessageType.Info );
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ( "RPGAttribute: 属性之间可以相互影响", MessageType.Info );
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ( "RPGVital: 带有初始值和实时变动的数值属性", MessageType.Info );
		}

		/// <summary>
		/// 验证当前预设索引名
		/// </summary>
		/// <returns></returns>
		private bool hasSameIndexName () {

			var list = new System.Collections.Generic.HashSet<string> ();

			foreach ( var c in define.getDefinedCollection () )
				if ( list.Contains ( c.key ) )
					return true;
				else list.Add ( c.key );
			return false;
		}
	}
}
