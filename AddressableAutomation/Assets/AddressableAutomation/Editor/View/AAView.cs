#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.AddressableAutomation.Core;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.AddressableAutomation.EditorView {

    public class AAView : EditorWindow {
        [MenuItem("Window/AddressableAutomation/AAView")]
        public static void ShowWindow() {
            GetWindow(typeof(AAView));
        }
        List<DataProcedure> proced = new List<DataProcedure>();

        List<ProcedurePreviewer> results = new List<ProcedurePreviewer>();

        private static GUIStyle ColoredStyle;
        private TextAsset jsonAsset;
        void OnGUI() {
            if (ColoredStyle == null) {
                ColoredStyle = new GUIStyle(GUI.skin.label) {
                    richText = true
                };
            }
            jsonAsset = (TextAsset) EditorGUILayout.ObjectField(jsonAsset, typeof(TextAsset), false);
            if (GUILayout.Button("Test")) {
                proced.Clear();
                results.Clear();
                if (jsonAsset != null) {
                    proced = DataProcedure.GenerateProcedureFromJson(jsonAsset.text);

                    foreach (var t in proced) {
                        results.Add(ProcedurePreviewer.GetPreviewOfProcedure(t));
                    }
                }
            }
            if (GUILayout.Button("Help")) {
                AAViewInfo.ShowWindow();
            }
            if (results.Count > 0) {
                DrawData(results);
            }
        }
        public void DrawData(List<ProcedurePreviewer> results) {
            foreach (var t in results) DrawInternal(t);
        }
        private void DrawInternal(ProcedurePreviewer result) {
            result.opening = EditorGUILayout.Foldout(result.opening, result.Path);
            if (result.opening) {
                if (GUILayout.Button(AAOption.JumpTo)) {
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(result.Path));
                }
                DrawJsonObject(result.Objects);
                
            }
        }
        private void DrawJsonObject(JsonObjectPreviewer target) {
            EditorGUI.indentLevel += 1;
            foreach (var t in target) {
                DrawPair(t);
            }
            EditorGUI.indentLevel -= 1;
        }
        private void DrawPair(SortableDataPairPreviewer pair) {
            EditorGUI.indentLevel += 1;
            Color signatureColor = Color.black;
            if (pair.Duplicated) signatureColor = Color.blue;
            if (pair.MethodCalling) signatureColor = Color.green;
            if (pair.NotFound) signatureColor = Color.red;
            LabelWithColor(pair.Signature,signatureColor);
            switch (pair.Type) {
                case AAInternal.AssetReferenceLink:
                case AAInternal.Set:
                {
                    int totalCount = pair.OldValues.Count;
                    if (totalCount < pair.NewValues.Count) totalCount = pair.NewValues.Count;
                    for (int i = 0; i < totalCount; i++) {
                        string toSay = string.Empty;
                        toSay += pair.OldValues.Count > i ? $"{pair.OldValues[i]}":"null";
                        toSay += "->";
                        if (pair.NewValues.Count > i) {
                            toSay += $"{pair.NewValues[i]}";
                        } else {
                            toSay += "null";
                        }
                        EditorGUILayout.LabelField(toSay);
                    }
                }
                    break;
                case AAInternal.MethodCall:
                {
                    string toSay = $"{pair.OldValues[0]}(";
                    for(int i=0;i<pair.NewValues.Count;i++) {
                        toSay += $"{pair.NewValues[i]}";
                        toSay += (i == pair.NewValues.Count - 1 ? "" : ", ");
                    }
                    toSay += ")";
                    EditorGUILayout.LabelField(toSay);
                }
                    break;
                case AAInternal.Nested:
                    DrawJsonObject(pair.NewValues[0] as JsonObjectPreviewer);
                    break;
                case AAInternal.None:
                    LabelWithColor("None type is Not Allowed",Color.red);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            EditorGUI.indentLevel -= 1;
        }
        private string CoverWithColor(string value, Color color) {
            return $"<color=#{(int) color.r * 255:X2}{(int) color.g * 255:X2}{(int) color.b * 255:X2}{(int) color.a * 255:X2}>{value}</color>";
        }
        private void LabelWithColor(string value, Color color) {
            EditorGUILayout.LabelField(CoverWithColor(value,color),ColoredStyle);
        }
    }

}
#endif