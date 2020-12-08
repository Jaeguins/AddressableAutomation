#if UNITY_EDITOR
using Assets.AddressableAutomation.Core;
using UnityEditor;
using UnityEngine;

namespace Assets.AddressableAutomation.EditorView {

    public class AAView : EditorWindow {
        [MenuItem("Window/AddressableAutomation/AAView")]
        public static void ShowWindow() {
            GetWindow(typeof(AAView));
        }
        private static GUIStyle labelStyle;
        private TextAsset jsonAsset;
        void OnGUI() {
            if (labelStyle == null) {
                labelStyle = new GUIStyle(GUI.skin.label) {
                    richText = true
                };
            }
            jsonAsset = (TextAsset) EditorGUILayout.ObjectField(jsonAsset,typeof(TextAsset),false);
            if (GUILayout.Button("Test")) {
                if(jsonAsset!=null)
                    DataProcedure.GenerateProcedureFromJson(jsonAsset.text);
            }
            if (GUILayout.Button("Help")) {
                AAViewInfo.ShowWindow();
            }
        }
    }

}
#endif