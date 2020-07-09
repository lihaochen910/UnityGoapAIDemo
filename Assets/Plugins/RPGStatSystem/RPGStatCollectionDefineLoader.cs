using UnityEngine;

namespace RPGStatSystem {
	
	[RequireComponent(typeof(RPGStatCollection))]
	public class RPGStatCollectionDefineLoader : MonoBehaviour {
		
		[Header("初始化属性定义")]
		public RPGStatCollectionDefine[] defines;

		private void Awake ( ) {
			var collection = GetComponent < RPGStatCollection > ();
			foreach ( var define in defines ) {
				collection.LoadFromDefine ( define, true );
			}
			
			Object.Destroy(this);
			
//			Debug.Log($"Import {gameObject.name} RPGStatCollectionDefine Ok!");
		}

		public bool HasStat ( string key ) {
			foreach ( var define in defines ) {
				foreach ( var data in define.getDefinedCollection () ) {
					if ( data.key.Equals ( key ) ) {
						return true;
					}
				}
			}

			return false;
		}

		public float GetStatValue ( string key ) {
			foreach ( var define in defines ) {
				foreach ( var data in define.getDefinedCollection () ) {
					if ( data.key.Equals ( key ) ) {
						return data.defaultValue;
					}
				}
			}

			return default ( float );
		}
	}
}
