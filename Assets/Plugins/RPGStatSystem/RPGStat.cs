using System;

namespace RPGStatSystem {
    /// <summary>
    /// The base class for all other Stats.
    /// </summary>
    public class RPGStat : IStatValue, IStatScalable {
        /// <summary>
        /// Used by the StatBase Value Property
        /// </summary>
        private float _statBaseValue;

        /// <summary>
        /// Used by the StatScaleValue Property
        /// </summary>
        private float _statScaleValue;

        /// <summary>
        /// Event that triggers when the stat value changes
        /// </summary>
        private StatValueChangeEvent _onValueChange;

        /// <summary>
        /// Instance of stat scale handler used by this stat;
        /// </summary>
        private RPGStatScaler _statScaler;

        /// <summary>
        /// The Total Value of the stat
        /// </summary>
        public virtual float StatValue {
            get { return StatBaseValue + StatScaleValue; }
        }

        /// <summary>
        /// The amount the stat is increased by it's
        /// currenty level scale.
        /// </summary>
        public float StatScaleValue {
            get { return _statScaleValue; }
        }

        /// <summary>
        /// The Base Value of the stat
        /// </summary>
        public virtual float StatBaseValue {
            get { return _statBaseValue; }
            set {
                if (_statBaseValue != value) {
                    _statBaseValue = value;
                    TriggerValueChange();
                }
            }
        }

        /// <summary>
        /// Constructor that takes a stat asset
        /// </summary>
        public RPGStat() {
            this.StatBaseValue = 0;
            //this.AssignedStatId = asset.AssignedStatId;
            //this.StatCategoryId = asset.StatCategoryId;

            //if (asset.StatScaler != null) {
            //    this._statScaler = asset.StatScaler.CreateInstance();
            //} else {
            //    this._statScaler = null;
            //}
        }

        /// <summary>
        /// Triggers the OnValueChange Event
        /// </summary>
        protected void TriggerValueChange() {
#if DEBUG_STAT_INFO
            Debug.LogFormat("[Stat Category {0}]: Trigger stat value change", StatCategoryId);
#endif
            if (_onValueChange != null) {
                _onValueChange(this);
            }
        }

        /// <summary>
        /// Change the StatBaseValue by a set amount
        /// </summary>
        public void ModifyBaseValue(float amount) {
#if DEBUG_STAT_INFO
            UnityEngine.Debug.LogFormat("[Stat Category {0}]: Modify Base Value by {1}", StatCategoryId, amount);
#endif
			if ( amount != 0) {
                _statBaseValue += amount;
                TriggerValueChange();
            }
        }
        
        /// <summary>
        /// Change the StatBaseValue by a value
        /// </summary>
        public void SetBaseValue(float val) {
#if DEBUG_STAT_INFO
            UnityEngine.Debug.LogFormat("[Stat Category {0}]: Modify Base Value by {1}", StatCategoryId, amount);
#endif
            _statBaseValue = val;
            TriggerValueChange();
        }

        /// <summary>
        /// Adds a function of type RPGStatModifierEvent to the OnValueChange delagate.
        /// </summary>
        public void AddValueListener(StatValueChangeEvent func) {
#if DEBUG_STAT_INFO
            UnityEngine.Debug.LogFormat("[Stat Category {0}]: Add Value Listener", StatCategoryId);
#endif
			_onValueChange += func;
        }

        /// <summary>
        /// Removes a function of type RPGStatModifierEvent to the OnValueChange delagate.
        /// </summary>
        public void RemoveValueListener(StatValueChangeEvent func) {
#if DEBUG_STAT_INFO
            UnityEngine.Debug.LogFormat("[Stat Category {0}]: Remove Value Listener", StatCategoryId);
#endif
			_onValueChange -= func;
        }

        /// <summary>
        /// Scales the stat to the given level. Uses the instance 
        /// of the stat scale handler assigned to the stat. If handler
        /// is null, the StatScaleValue remains at zero.
        /// </summary>
        /// <param name="level"></param>
        public void ScaleStatToLevel(int level) {
            if (_statScaler != null) {
                _statScaleValue = _statScaler.GetValue(level);
            } else {
                _statScaleValue = 0;
            }
			UnityEngine.Debug.Log ( "Scaling stat to " + level + " new scaled value " + _statScaleValue );
		}
        
        public static implicit operator float ( RPGStat stat ) {
            return stat.StatValue;
        }
        
        public static implicit operator int ( RPGStat stat ) {
            return (int)stat.StatValue;
        }
    }
}
