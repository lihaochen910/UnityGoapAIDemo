using UnityEngine;
using UnityEditor;

namespace RPGStatSystem.Editor {

	[CustomPropertyDrawer ( typeof ( RPGStatCollectionDefine.RPGStatDefineData ) )]
	public class RPGStatDefineDataDrawer : PropertyDrawer {

		private RPGStatCollectionDefine.RPGStatDefineData data;

		public override void OnGUI (Rect position,
			SerializedProperty property, GUIContent label) {

			using ( new EditorGUI.PropertyScope ( position, label, property ) ) {

				//设置属性名宽度 Name HP
				EditorGUIUtility.labelWidth = 50;
				EditorGUIUtility.fieldWidth = 50;

				//ico 位置矩形
				var typeRect = new Rect ( position ) {
					width = 100,
					height = EditorGUIUtility.singleLineHeight
				};

				var keyRect = new Rect ( typeRect ) {
					width = 140,
					x = typeRect.x + typeRect.width + 15
				};

				var defaultValueRect = new Rect ( keyRect ) {
					width = 110,
					x = keyRect.x + keyRect.width
				};

				// 找到每个属性的序列化值
				var keyProperty = property.FindPropertyRelative ( "key" );
				var typeProperty = property.FindPropertyRelative ( "type" );
				var defaultValueProperty = property.FindPropertyRelative ( "defaultValue" );

				// 绘制GUI
				GUI.backgroundColor = Color.gray;

				keyProperty.stringValue =
					EditorGUI.TextField ( keyRect, keyProperty.stringValue );

				GUI.backgroundColor = Color.yellow;

				typeProperty.enumValueIndex =
					(int)(RPGStatCollectionDefine.RPGStatType)EditorGUI.EnumPopup ( typeRect, (RPGStatCollectionDefine.RPGStatType)typeProperty.enumValueIndex );

				GUI.backgroundColor = Color.green;

				defaultValueProperty.floatValue =
					EditorGUI.FloatField ( defaultValueRect, "    =", defaultValueProperty.floatValue );

				//EditorGUIUtility.labelWidth = 70;

				EditorGUILayout.Space ();
			}
		}
	}
}
