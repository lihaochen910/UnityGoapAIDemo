﻿using System;

namespace RPGStatSystem {
    /// <summary>
    /// The base class used by all stat linkers
    /// </summary>
    public abstract class RPGStatLinker {
        /// <summary>
        /// The RPGStat linked to by the stat linker
        /// </summary>
        public RPGStat LinkedStat { get; private set; }

        /// <summary>
        /// Gets the value of the stat linker
        /// </summary>
        public abstract float GetValue();


        /// <summary>
        /// Basic constructor that only takes a stat linker asset
        /// </summary>
        /// <param name="asset"></param>
        public RPGStatLinker() {
            SetLinkedStat(null);
        }

        /// <summary>
        /// Basic constructor that only takes a stat linker asset and 
        /// a linked stat. Listens to the Stat's OnValueChange
        /// event if the stat implements IStatValueChange.
        /// </summary>
        public RPGStatLinker(RPGStat linkedStat) {
            SetLinkedStat(linkedStat);
        }

        /// <summary>
        /// Sets the Linker to listen to the passed stat's value change
        /// event and set's the value of LinkedStat.
        /// </summary>
        private void SetLinkedStat(RPGStat stat) {
            if (stat == null) {
                UnityEngine.Debug.LogWarning("Stat Linker was created with null reference to a stat.");
            }
            LinkedStat = stat;
        }
    }
}