#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace AssetAutomation.Editor.View {

    public class AAViewInfo : EditorWindow {
        public static void ShowWindow() {
            GetWindow<AAViewInfo>(false,"AssetAutomation Info");
        }
        public const string InfoMessage = "<color=#ffffff>help</color>";
        private static GUIStyle infoStyle;

        private Vector2 _scroll=Vector2.zero;
        void OnGUI() {
            if (infoStyle == null) {
                infoStyle = new GUIStyle(GUI.skin.label) {
                    richText = true
                };
            }
            _scroll=EditorGUILayout.BeginScrollView(_scroll);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(InfoMessage, infoStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }

}
#endif