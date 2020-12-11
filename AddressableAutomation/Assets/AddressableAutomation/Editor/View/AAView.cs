#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.AddressableAutomation.Core;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Preview")) {
                proced.Clear();
                results.Clear();
                if (jsonAsset != null) {
                    proced = DataProcedure.GenerateProcedureFromJson(jsonAsset.text);

                    foreach (var t in proced) {
                        results.Add(ProcedurePreviewer.GetPreviewOfProcedure(t));
                    }
                }
            }
            if (GUILayout.Button("Clear")) {
                proced.Clear();
                results.Clear();
            }
            if (GUILayout.Button("Apply")) {
                Apply();
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Help")) {
                AAViewInfo.ShowWindow();
            }
            if (results.Count > 0) {
                DrawData(results);
            }
        }
        private Vector2 _scrollPos;
        public void DrawData(List<ProcedurePreviewer> results) {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            foreach (var t in results) DrawInternal(t);
            EditorGUILayout.EndScrollView();
        }
        private void DrawInternal(ProcedurePreviewer result) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            result.opening = EditorGUILayout.Foldout(result.opening, result.Path);
            if (result.opening) {
                if (result.Objects != null) {
                    DrawJsonObject(result.Objects);
                }else EditorGUILayout.LabelField("Nothing in this json object");
                
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button(AAOption.JumpTo)) {
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(result.Path));
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
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
            // if (pair.MethodCalling) signatureColor = Color.green;
            if (pair.NotFound) signatureColor = Color.red;
            
            string signatureText = pair.Signature;
            switch (pair.Type) {
                case AAInternal.AssetReferenceLink:
                    signatureText += " - Asset";
                    break;
                case AAInternal.Set:
                    signatureText += " - Set";
                    break;
                case AAInternal.MethodCall:
                    signatureText += " - MethodCall";
                    break;
                case AAInternal.Nested:
                    signatureText += " - Nested";
                    break;
                case AAInternal.None:
                    signatureText += " - None";
                    break;
                case AAInternal.Create:
                    signatureText += " - Create";
                    signatureColor = Color.green;
                    break;
            }
            LabelWithColor(signatureText, signatureColor);
            switch (pair.Type) {
                case AAInternal.AssetReferenceLink:
                {
                    int totalCount = pair.OldValues.Count;
                    if (totalCount < pair.NewValues.Count) totalCount = pair.NewValues.Count;
                    EditorGUI.indentLevel += 1;
                    for (int i = 0; i < totalCount; i++) {
                        string toSay = string.Empty;
                        AssetReference oldRef=pair.OldValues.Count > i ? (AssetReference)(pair.OldValues[i]):default,
                                       newRef=pair.NewValues.Count > i ? (AssetReference)(pair.NewValues[i]):default;
                        toSay += (oldRef!=null&&oldRef.editorAsset!=null)?oldRef.editorAsset.name:"null";
                        if (newRef != null) {
                            toSay += $"->{((newRef!=null&&newRef.editorAsset!=null)?newRef.editorAsset.name:"null")}{(newRef.SubObjectName!=null?"/"+newRef.SubObjectName:"")}";
                        }

                        EditorGUILayout.LabelField(toSay);
                        
                    }
                    EditorGUI.indentLevel -= 1;
                }
                    break;
                case AAInternal.Set:
                {
                    int totalCount = pair.OldValues.Count;
                    if (totalCount < pair.NewValues.Count) totalCount = pair.NewValues.Count;
                    EditorGUI.indentLevel += 1;
                    for (int i = 0; i < totalCount; i++) {
                        string toSay = string.Empty;
                        toSay += pair.OldValues.Count > i ? $"{pair.OldValues[i]}" : "null";
                        toSay += "->";
                        if (pair.NewValues.Count > i) {
                            toSay += $"{pair.NewValues[i]}";
                        } else {
                            toSay += "null";
                        }
                        EditorGUILayout.LabelField(toSay);
                    }
                    EditorGUI.indentLevel -= 1;
                }
                    break;
                case AAInternal.MethodCall:
                {
                    EditorGUI.indentLevel += 1;
                    string toSay = $"{pair.OldValues[0]}(";
                    for (int i = 0; i < pair.NewValues.Count; i++) {
                        toSay += $"{pair.NewValues[i]}";
                        toSay += (i == pair.NewValues.Count - 1 ? "" : ", ");
                    }
                    toSay += ")";
                    EditorGUILayout.LabelField(toSay);
                    EditorGUI.indentLevel -= 1;
                }
                    break;
                case AAInternal.Nested:
                {
                    EditorGUI.indentLevel += 1;
                    if(pair.NewValues.Count>0)
                        if (pair.NewValues[0] is JsonObjectPreviewer jsonPair) {
                            jsonPair.opening = EditorGUILayout.Foldout(jsonPair.opening, "Object");
                            if (jsonPair.opening) {
                                DrawJsonObject(jsonPair);
                            }
                        }
                    EditorGUI.indentLevel -= 1;
                }
                    break;
                case AAInternal.None:
                    break;
                case AAInternal.Create:
                {
                    string toSay = string.Empty;
                    for (int i = 0; i < pair.NewValues.Count; i++) {
                        toSay += pair.NewValues[i].ToString();
                        if (i < pair.NewValues.Count-1) {
                            toSay += ", ";
                        }
                    }
                    EditorGUILayout.LabelField(toSay);
                    
                        
                    
                }
                    
                    break;
            }
            EditorGUI.indentLevel -= 1;
        }
        private string CoverWithColor(string value, Color color) {
            return $"<color=#{(int) color.r * 255:X2}{(int) color.g * 255:X2}{(int) color.b * 255:X2}{(int) color.a * 255:X2}>{value}</color>";
        }
        private void LabelWithColor(string value, Color color) {
            EditorGUILayout.LabelField(CoverWithColor(value, color), ColoredStyle);
        }

        public void Apply() {
            HashSet<string> needToReserialize = new HashSet<string>();
            for (int i = 0; i < proced.Count; i++) {
                string temp = proced[i].Apply();
                if(temp!=null)
                    needToReserialize.Add(temp);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.ForceReserializeAssets(needToReserialize);
        }
    }

}
#endif