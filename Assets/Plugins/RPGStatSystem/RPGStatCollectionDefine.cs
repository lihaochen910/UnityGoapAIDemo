using System;
using UnityEngine;

namespace RPGStatSystem {

	[CreateAssetMenu ( menuName = "PluggableItem/RPGStatCollectionDefine" )]
	public class RPGStatCollectionDefine : ScriptableObject {

		[SerializeField]
		private RPGStatDefineData[] datas;

		public RPGStatDefineData[] getDefinedCollection () => datas;

		public enum RPGStatType : byte {
			RPGStat,
			RPGStatModifiable,
			RPGAttribute,
			RPGVital,
		}

		[Serializable]
		public class RPGStatDefineData {

			[SerializeField]
			public string key;

			[SerializeField]
			public RPGStatType type;

			[SerializeField]
			public float defaultValue;
		}
	}
}
