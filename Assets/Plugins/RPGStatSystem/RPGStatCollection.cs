using UnityEngine;
using System.Collections.Generic;

namespace RPGStatSystem {
    /// <summary>
    /// The base class used to define a collection of RPGStats.
    /// Also used to apply and remove RPGStatModifiers from individual
    /// RPGStats.
    /// </summary>
    public class RPGStatCollection : MonoBehaviour {

        [SerializeField]
        private int _statCollectionId = -1;
        public int StatCollectionId {
            get { return _statCollectionId; }
            set { _statCollectionId = value; }
        }

        private bool _isCollectionSetup = false;
        public bool IsCollectionSetup {
            get { return _isCollectionSetup; }
            set { _isCollectionSetup = value; }
        }

        private RPGStatCollectionCore _core = new RPGStatCollectionCore();
        
        /// <summary>
        /// Dictionary containing all stats held within the collection
        /// </summary>
        public Dictionary<string, RPGStat> StatDict {
            get { return _core.StatDict; }
        }

        public RPGStatCollectionCore Core {
            get { return _core; }
            set { _core = value; }
        }

        /// <summary>
        /// Initializes the RPGStats class
        /// </summary>
        private void Awake() {
//            if (IsCollectionSetup == false) {
//                SetupCollection();
//        
//                SetLevel(NormalLevel);
//                CurrentExp = GetExpForLevel(NormalLevel);
//            }
//            Debug.Log("Collection Awake!");
        }

        /// <summary>
        /// Checks if there is a RPGStat with the given type and id
        /// </summary>
        public bool ContainStat(string statTypeId) {
            return _core.ContainStat(statTypeId);
        }

        /// <summary>
        /// Gets the RPGStat with the given RPGStatTyp and ID
        /// </summary>
        public RPGStat GetStat(string statTypeId) {
            return _core.GetStat(statTypeId);
        }

        /// <summary>
        /// Trys to get the RPGStat with the given RPGStatTypeId.
        /// If stat exists, returns true.
        /// </summary>
        public bool TryGetStat(string statTypeId, out RPGStat stat) {
            return _core.TryGetStat(statTypeId, out stat);
        }

        /// <summary>
        /// Gets the RPGStat with the given int and ID as type T
        /// </summary>
        public T GetStat<T>(string statTypeId) where T : RPGStat {
            return _core.GetStat<T>(statTypeId);
        }

        /// <summary>
        /// Trys to get the RPGStat with the given RPGStatTypeId as
        /// type T. If stat exists, returns true.
        /// </summary>
        public bool TryGetStat<T>(string statTypeId, out T stat) where T : RPGStat {
            return _core.TryGetStat<T>( statTypeId, out stat );
        }

		/// <summary>
		/// Creates a new instance of the stat ands adds it to the StatDict
		/// </summary>
		public T AddStat<T> (string statTypeId, T stat, bool replace) where T : RPGStat {
            return _core.AddStat<T>(statTypeId, stat, replace);
		}
        
        /// <summary>
        /// Change the StatBaseValue by a set amount
        /// </summary>
        /// <param name="amount"></param>
        public void ModifyBaseValue ( string targetId, float amount ) {
            _core.ModifyBaseValue(targetId, amount);
        }
        
        /// <summary>
        /// Adds a Stat Modifier to the Target stat.
        /// </summary>
        public void AddStatModifier(string targetId, RPGStatModifier mod) {
            if ( mod == null ) {
                return;
            }
            _core.AddStatModifier(targetId, mod);
        }

        /// <summary>
        /// Adds a Stat Modifier to the Target stat and then updates the stat's value.
        /// </summary>
        public void AddStatModifier(string targetId, RPGStatModifier mod, bool update) {
            _core.AddStatModifier(targetId, mod, update);
        }

        /// <summary>
        /// Removes a Stat Modifier to the Target stat.
        /// </summary>
        public void RemoveStatModifier(string targetId, RPGStatModifier mod) {
            if ( mod == null ) {
                return;
            }
            _core.RemoveStatModifier(targetId, mod);
        }

        /// <summary>
        /// Removes a Stat Modifier to the Target stat and then updates the stat's value.
        /// </summary>
        public void RemoveStatModifier(string targetId, RPGStatModifier mod, bool update) {
            _core.RemoveStatModifier(targetId, mod, update);
        }

        /// <summary>
        /// Clears all stat modifiers from all stats in the collection.
        /// </summary>
        public void ClearStatModifiers() {
            _core.ClearStatModifiers(false);
        }

        /// <summary>
        /// Clears all stat modifiers from all stats in the collection then updates all the stat's values.
        /// </summary>
        /// <param name="update"></param>
        public void ClearStatModifiers(bool update) {
            _core.ClearStatModifiers (update);
        }

        /// <summary>
        /// Clears all stat modifiers from the target stat.
        /// </summary>
        public void ClearStatModifier(string targetId) {
            _core.ClearStatModifier(targetId, false);
        }

        /// <summary>
        /// Clears all stat modifiers from the target stat then updates the stat's value.
        /// </summary>
        public void ClearStatModifier(string targetId, bool update) {
            _core.ClearStatModifier (targetId, update);
        }

        /// <summary>
        /// Updates all stat modifier's values
        /// </summary>
        public void UpdateStatModifiers() {
            _core.UpdateStatModifiers ();
        }

        /// <summary>
        /// Updates the target stat's modifier value
        /// </summary>
        public void UpdateStatModifer(string targetId) {
            _core.UpdateStatModifer (targetId);
        }

        /// <summary>
        /// Scales all stats in the collection to the same target level
        /// </summary>
        public void ScaleStatCollection(int level) {
            _core.ScaleStatCollection (level);
        }

        /// <summary>
        /// Scales the target stat in the collection to the target level
        /// </summary>
        public void ScaleStat(string targetId, int level) {
            _core.ScaleStat (targetId, level);
        }

        public void LoadFromDefine ( RPGStatCollectionDefine define, bool replaceIfExist ) {
            _core.LoadFromDefine (define, replaceIfExist);
        }

        public float GetStatValue ( string statTypeId ) {
            return _core.GetStatValue (statTypeId);
        }
        
        public void SetStatValue ( string statTypeId, float value ) {
            _core.SetStatValue (statTypeId, value);
        }

        #region Save & Load
        public Dictionary< string, float > CreateSaveData () {
            Dictionary< string, float > data = new Dictionary < string , float >();

            foreach ( var kv in StatDict ) {
                data.Add ( kv.Key, kv.Value.StatBaseValue );
            }

            return data;
        }
        public void LoadSaveData ( Dictionary< string, float > data ) {
            foreach ( var kv in data ) {
                var stat = GetStat ( kv.Key );
                if ( stat != null ) {
                    stat.SetBaseValue ( kv.Value );
                }
            }
        }
        #endregion
    }
}
