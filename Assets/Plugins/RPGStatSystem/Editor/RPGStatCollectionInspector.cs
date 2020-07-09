using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace RPGStatSystem.Editor {

    [CustomEditor(typeof(RPGStatCollection))]
    public class RPGStatCollectionEditor : UnityEditor.Editor {

        private float increaseAmount = 1;
        private List<string> activeStatIds = new List<string> ();
        private bool showStatModifiers = false;

        public override void OnInspectorGUI() {

            var collection = (RPGStatCollection)target;

            GUILayout.Space(4);

            DisplayCollectionGUI(collection);
        }

        private void DisplayCollectionGUI(RPGStatCollection collection) {

            // Show controls for editing values of stats in the editor
            GUILayout.BeginHorizontal(EditorStyles.toolbarButton);
            GUILayout.Label("Stats");
            GUILayout.Label("Increment By", GUILayout.Width(90));
            // Increase the amount stat values are adjusted by multiples of 10
            if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(24)))
                increaseAmount = Mathf.Min(increaseAmount * 10, 10000);
            // Shows the current amount stat values are adjusted by in editor
            GUILayout.Label(increaseAmount.ToString("F2"), GUILayout.Width(50));
            // Decrease the amount stat values are adjusted by multiples of 10
            if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(24)))
                increaseAmount = Mathf.Max(increaseAmount / 10, 0.01f);
            GUILayout.EndHorizontal();

            GUILayout.Space(-4);

            // Show all the stats within the editor only when the
            // editor is playing, else the stat list will not be
            // initialized.
            GUILayout.BeginVertical("Box");
            if (Application.isPlaying) {

                foreach (var pair in collection.StatDict ) {

                    var currentValue = pair.Value as IStatValueCurrent;

                    bool isActive = activeStatIds.Contains(pair.Key);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(string.Format("{0, 5}", pair.Key.ToString()), EditorStyles.miniButtonLeft, GUILayout.Width(130));
                    //GUILayout.Label(RPGStatTypeDatabase.Instance.Get(pair.Key).Name, EditorStyles.miniButtonMid);

                    if (currentValue == null) {
                        GUILayout.Label(string.Format("{0, 8}", pair.Value.StatValue), EditorStyles.miniButtonMid, GUILayout.Width(110));
                    } else {
                        GUILayout.Label(string.Format("{0, 5}", currentValue.StatValueCurrent), EditorStyles.miniButtonMid, GUILayout.Width(60));
                        GUILayout.Label(string.Format("{0, 5}", pair.Value.StatValue), EditorStyles.miniButtonMid, GUILayout.Width(60));
                    }

                    var clicked = GUILayout.Toggle(isActive, isActive ? '\u25BC'.ToString() : '\u25B6'.ToString(), EditorStyles.miniButtonRight, GUILayout.Width(25));
                    if (clicked == true && isActive == false) {
                        activeStatIds.Add(pair.Key);
                    } else if (clicked == false && isActive == true) {
                        activeStatIds.Remove(pair.Key);
                    }
                    GUILayout.EndHorizontal();


                    // Show the name and id of the current stat
                    //var clicked = GUILayout.Toggle(isActive, string.Format("ID {0, 4}: {1, -20}", 
                    //    pair.Key.ToString(), statName), EditorStyles.foldout);
                    //
                    //if (clicked) {
                    //    activeStatId = pair.Key;
                    //}

                    if (activeStatIds.Contains(pair.Key) == false) continue;

                    GUILayout.Space(-3);

                    // Display the stat's value along with controls to 
                    // modifiy the value by the current increase amount.
                    GUILayout.BeginVertical("Box");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Value", GUILayout.Width(60));
                    GUILayout.Label(pair.Value.StatValue.ToString("F2"), EditorStyles.centeredGreyMiniLabel);

//                    float newBaseValue = pair.Value.StatBaseValue;
//                    if ( float.TryParse(GUILayout.TextField(pair.Value.StatValue.ToString("F2")), out newBaseValue) ) {
//                        pair.Value.StatBaseValue = newBaseValue;
//                    }

                    if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(30))) {
                        pair.Value.ModifyBaseValue(increaseAmount);
                    }

                    if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(30))) {
                        pair.Value.ModifyBaseValue(-increaseAmount);
                    }
                    GUILayout.EndHorizontal();

                    // If the stat implements IStatCurrentValue display the
                    // current value of the given stat.
                    if (currentValue != null) {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Current", GUILayout.Width(60));
                        GUILayout.Label(currentValue.StatValueCurrent.ToString("F2"), EditorStyles.centeredGreyMiniLabel);
                        
//                        float newCurrentValue = currentValue.StatValueCurrent;
//                        if ( float.TryParse(GUILayout.TextField(currentValue.StatValueCurrent.ToString("F2")), out newCurrentValue) ) {
//                            currentValue.StatValueCurrent = newCurrentValue;
//                        }
                        
                        if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(30))) {
                            currentValue.StatValueCurrent += increaseAmount;
                        }

                        if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(30))) {
                            currentValue.StatValueCurrent -= increaseAmount;
                        }
                        GUILayout.EndHorizontal();
                    }

                    var iModifiable = pair.Value as IStatModifiable;
                    if (iModifiable != null) {
                        EditorGUI.indentLevel++;
                        GUILayout.BeginHorizontal();
                        showStatModifiers = GUILayout.Toggle(showStatModifiers, $"Active Modifiers ({iModifiable.GetModifierCount()})", EditorStyles.miniButton);
                        GUILayout.EndHorizontal();

                        if (showStatModifiers) {
                            GUILayout.BeginVertical("Box");
                            for (int i = 0; i < iModifiable.GetModifierCount(); i++) {
                                var mod = iModifiable.GetModifierAt(i);

                                if (mod != null) {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label(mod.GetType().Name);
                                    GUILayout.Label(mod.Value.ToString());
                                    GUILayout.Label(mod._description);
                                    if (GUILayout.Button("-", EditorStyles.miniButton)) {
                                        iModifiable.RemoveModifier(mod);
                                    }
                                    GUILayout.EndHorizontal();
                                }
                            }

                            if (iModifiable.GetModifierCount() <= 0) {
                                GUILayout.Label("No Active Modifier", EditorStyles.centeredGreyMiniLabel);
                            }
                            GUILayout.EndVertical();
                        }
                        EditorGUI.indentLevel--;
                    }

                    GUILayout.EndVertical();
                }

                if (collection.StatCollectionId <= 0) {
//                    GUILayout.Label("No Stat Collection Selected", EditorStyles.centeredGreyMiniLabel);
                } else if (collection.StatDict.Count <= 0) {
                    GUILayout.Label("No Stats are contained within the collection", EditorStyles.centeredGreyMiniLabel);
                }
            } else {
                GUILayout.Label("Stats Appear in Playmode", EditorStyles.centeredGreyMiniLabel);
            }

            GUILayout.EndVertical();
        }
    }
}
