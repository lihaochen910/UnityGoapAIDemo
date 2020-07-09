using UnityEngine;
using System.Collections.Generic;

namespace RPGStatSystem {
    /// <summary>
    /// The base class used to define a collection of RPGStats.
    /// Also used to apply and remove RPGStatModifiers from individual
    /// RPGStats.
    /// </summary>
    public class RPGStatCollectionCore {
        
        private Dictionary<string, RPGStat> _statDict = new Dictionary<string, RPGStat>();
        
        /// <summary>
        /// Dictionary containing all stats held within the collection
        /// </summary>
        public Dictionary<string, RPGStat> StatDict {
            get { return _statDict; }
            set { _statDict = value; }
        }

        /// <summary>
        /// Initializes the RPGStats class
        /// </summary>
        public RPGStatCollectionCore() {}

        /// <summary>
        /// Checks if there is a RPGStat with the given type and id
        /// </summary>
        public bool ContainStat(string statTypeId) {
            return StatDict.ContainsKey(statTypeId);
        }

        /// <summary>
        /// Gets the RPGStat with the given RPGStatTyp and ID
        /// </summary>
        public RPGStat GetStat(string statTypeId) {
            if (ContainStat(statTypeId)) {
                return StatDict[statTypeId];
            }
            return null;
        }

        /// <summary>
        /// Trys to get the RPGStat with the given RPGStatTypeId.
        /// If stat exists, returns true.
        /// </summary>
        public bool TryGetStat(string statTypeId, out RPGStat stat) {
            stat = GetStat(statTypeId);
            return stat != null;
        }

        /// <summary>
        /// Gets the RPGStat with the given int and ID as type T
        /// </summary>
        public T GetStat<T>(string statTypeId) where T : RPGStat {
            return GetStat(statTypeId) as T;
        }

        /// <summary>
        /// Trys to get the RPGStat with the given RPGStatTypeId as
        /// type T. If stat exists, returns true.
        /// </summary>
        public bool TryGetStat<T>(string statTypeId, out T stat) where T : RPGStat {
            stat = GetStat<T>(statTypeId);
            return stat != null;
        }

		/// <summary>
		/// Creates a new instance of the stat ands adds it to the StatDict
		/// </summary>
		public T AddStat<T> (string statTypeId, T stat, bool replace) where T : RPGStat {
			if ( stat == null ) {
				return null;
			}

			if ( !StatDict.ContainsKey ( statTypeId ) ) {
				StatDict.Add ( statTypeId, stat );
			}
			else {
				if ( replace ) {
//                    Debug.Log ( $"RPGStatCollectionCore::AddStat() override {statTypeId} #{StatDict[statTypeId].GetHashCode()} => #{stat.GetHashCode()}" );
					StatDict[statTypeId] = stat;
				}
			}

			return stat;
		}

		/// <summary>
		/// Creates a new instance of the stat ands adds it to the StatDict
		/// </summary>
		protected T CreateStat<T>(string statTypeId) where T : RPGStat, new() {
            T stat = new T();
            StatDict.Add(statTypeId, stat);
            return stat;
        }

        /// <summary>
        /// Creates or Gets a RPGStat of type T. Used within the setup method during initialization.
        /// </summary>
        protected T GetOrCreateStat<T>(string statTypeId) where T : RPGStat, new() {
            T stat = GetStat<T>(statTypeId);
            if (stat == null) {
                stat = CreateStat<T>(statTypeId);
            }
            return stat;
        }

        /// <summary>
        /// Change the StatBaseValue by a set amount
        /// </summary>
        /// <param name="amount"></param>
        public void ModifyBaseValue ( string targetId, float amount ) {
            var stat = GetStat (targetId);
            if ( stat != null ) {
                stat.ModifyBaseValue(amount);
            }
        }
        
        /// <summary>
        /// Adds a Stat Modifier to the Target stat.
        /// </summary>
        public void AddStatModifier(string targetId, RPGStatModifier mod) {
            AddStatModifier(targetId, mod, true);
        }

        /// <summary>
        /// Adds a Stat Modifier to the Target stat and then updates the stat's value.
        /// </summary>
        public void AddStatModifier(string targetId, RPGStatModifier mod, bool update) {
            if (ContainStat(targetId)) {
                var modStat = GetStat(targetId) as IStatModifiable;
                if (modStat != null) {
                    modStat.AddModifier(mod);
                    mod.OnModifierApply(this, targetId);
                    if (update == true) {
                        modStat.UpdateModifiers();
                    }
                } else {
                    Debug.Log("[RPGStats] Trying to add Stat Modifier to non modifiable stat \"" + targetId.ToString() + "\"");
                }
            } else {
                Debug.Log("[RPGStats] Trying to add Stat Modifier to \"" + targetId.ToString() + "\", but RPGStats does not contain that stat");
            }
        }

        /// <summary>
        /// Removes a Stat Modifier to the Target stat.
        /// </summary>
        public void RemoveStatModifier(string targetId, RPGStatModifier mod) {
            RemoveStatModifier(targetId, mod, true);
        }

        /// <summary>
        /// Removes a Stat Modifier to the Target stat and then updates the stat's value.
        /// </summary>
        public void RemoveStatModifier(string targetId, RPGStatModifier mod, bool update) {
            if (ContainStat(targetId)) {
                var modStat = GetStat(targetId) as IStatModifiable;
                if (modStat != null) {
                    modStat.RemoveModifier(mod);
                    mod.OnModifierRemove();
                    if (update == true) {
                        modStat.UpdateModifiers();
                    }
                } else {
                    Debug.Log("[RPGStats] Trying to remove Stat Modifier from non modifiable stat \"" + targetId.ToString() + "\"");
                }
            } else {
                Debug.Log("[RPGStats] Trying to remove Stat Modifier from \"" + targetId.ToString() + "\", but RPGStatCollection does not contain that stat");
            }
        }

        /// <summary>
        /// Clears all stat modifiers from all stats in the collection.
        /// </summary>
        public void ClearStatModifiers() {
            ClearStatModifiers(false);
        }

        /// <summary>
        /// Clears all stat modifiers from all stats in the collection then updates all the stat's values.
        /// </summary>
        /// <param name="update"></param>
        public void ClearStatModifiers(bool update) {
            foreach (var key in StatDict.Keys) {
                ClearStatModifier(key, update);
            }
        }

        /// <summary>
        /// Clears all stat modifiers from the target stat.
        /// </summary>
        public void ClearStatModifier(string targetId) {
            ClearStatModifier(targetId, false);
        }

        /// <summary>
        /// Clears all stat modifiers from the target stat then updates the stat's value.
        /// </summary>
        public void ClearStatModifier(string targetId, bool update) {
            if (ContainStat(targetId)) {
                var modStat = GetStat(targetId) as IStatModifiable;
                if (modStat != null) {
                    modStat.ClearModifiers();
                    if (update == true) {
                        modStat.UpdateModifiers();
                    }
                } else {
                    Debug.Log("[RPGStats] Trying to clear Stat Modifiers from non modifiable stat \"" + targetId.ToString() + "\"");
                }
            } else {
                Debug.Log("[RPGStats] Trying to clear Stat Modifiers from \"" + targetId.ToString() + "\", but RPGStatCollection does not contain that stat");
            }
        }

        /// <summary>
        /// Updates all stat modifier's values
        /// </summary>
        public void UpdateStatModifiers() {
            foreach (var key in StatDict.Keys) {
                UpdateStatModifer(key);
            }
        }

        /// <summary>
        /// Updates the target stat's modifier value
        /// </summary>
        public void UpdateStatModifer(string targetId) {
            if (ContainStat(targetId)) {
                var modStat = GetStat(targetId) as IStatModifiable;
                if (modStat != null) {
                    modStat.UpdateModifiers();
                } else {
                    Debug.Log("[RPGStats] Trying to Update Stat Modifiers for a non modifiable stat \"" + targetId.ToString() + "\"");
                }
            } else {
                Debug.Log("[RPGStats] Trying to Update Stat Modifiers for \"" + targetId.ToString() + "\", but RPGStatCollection does not contain that stat");
            }
        }

        /// <summary>
        /// Scales all stats in the collection to the same target level
        /// </summary>
        public void ScaleStatCollection(int level) {
            foreach (var key in StatDict.Keys) {
                ScaleStat(key, level);
            }
        }

        /// <summary>
        /// Scales the target stat in the collection to the target level
        /// </summary>
        public void ScaleStat(string targetId, int level) {
            if (ContainStat(targetId)) {
                var stat = GetStat(targetId) as IStatScalable;
                if (stat != null) {
                    stat.ScaleStatToLevel(level);
                } else {
                    Debug.Log("[RPGStats] Trying to Scale Stat with a non scalable stat \"" + targetId.ToString() + "\"");
                }
            } else {
                Debug.Log("[RPGStats] Trying to Scale Stat for \"" + targetId.ToString() + "\", but RPGStatCollection does not contain that stat");
            }
        }

        public void LoadFromDefine ( RPGStatCollectionDefine define, bool replaceIfExist ) {

            if ( define == null ) {
                Debug.LogError ( $"LoadFromDefine: null" );
                return;
            }
            
            foreach ( var data in define.getDefinedCollection () ) {

                RPGStat stat = null;
                
                switch ( data.type ) {
                    case RPGStatCollectionDefine.RPGStatType.RPGStat:
                        stat = new RPGStat () { StatBaseValue = data.defaultValue }; break;
                    case RPGStatCollectionDefine.RPGStatType.RPGStatModifiable:
                        stat = new RPGStatModifiable () { StatBaseValue = data.defaultValue }; break;
                    case RPGStatCollectionDefine.RPGStatType.RPGAttribute:
                        stat = new RPGAttribute () { StatBaseValue = data.defaultValue }; break;
                    case RPGStatCollectionDefine.RPGStatType.RPGVital:
                        stat = new RPGVital () { StatBaseValue = data.defaultValue }; break;
                }
                
                AddStat ( data.key, stat, replaceIfExist );
                
//                Debug.Log ( $"RPGStatCollection::LoadFromDefine() AddStat => {data.key.Colored(Colors.yellow)} {define}" );
            }
        }

        public float GetStatValue ( string statTypeId ) {
            if ( ContainStat ( statTypeId ) ) {
                return GetStat ( statTypeId );
            }
            Debug.LogError($"RPGStatCollection::GetStat({statTypeId}) not found!");
            return default (float);
        }
        
        public void SetStatValue ( string statTypeId, float value ) {
            if ( !ContainStat ( statTypeId ) ) {
                Debug.LogError($"RPGStatCollection::SetStatValue({statTypeId}) not found!");
                return;
            }
            GetStat ( statTypeId ).StatBaseValue = value;
        }
    }
}
