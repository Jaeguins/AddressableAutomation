#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Assets.AddressableAutomation.EditorView {

    public class AAViewInfo : EditorWindow {
        public static void ShowWindow() {
            GetWindow(typeof(AAViewInfo));
        }
        public const string InfoMessage = "<color=#ffffff>help</color>";
        private static GUIStyle infoStyle;

        void OnGUI() {
            if (infoStyle == null) {
                infoStyle = new GUIStyle(GUI.skin.label) {
                    richText = true
                };
            }
            EditorGUILayout.LabelField(InfoMessage, infoStyle);
            EditorGUILayout.LabelField("Json Path keyword");
            AAOption.PathKeyword = EditorGUILayout.TextField(AAOption.PathKeyword);
            EditorGUILayout.LabelField("No Asset Found Alert");
            AAOption.NoAssetFoundAlert = EditorGUILayout.TextField(AAOption.NoAssetFoundAlert);
        }
    }

    public static class AAOption {
        public static string PathKeyword="Path";
        public static string NoAssetFoundAlert="No Asset Found";
        public static string JumpTo="Jump";
    }
}
#endif