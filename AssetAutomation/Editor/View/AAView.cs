#if UNITY_EDITOR
using System.Collections.Generic;
using AssetAutomation.Editor.Core;
using AssetAutomation.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace AssetAutomation.Editor.View {

    public class AAView : EditorWindow {
        [MenuItem("Window/AssetAutomation")]
        public static void ShowWindow() {
            GetWindow<AAView>(false,"AssetAutomation");
        }
        List<DataProcedure> _proced = new List<DataProcedure>();

        List<ProcedurePreviewer> _results = new List<ProcedurePreviewer>();

        private static GUIStyle coloredLabelStyle,coloredFoldoutStyle;
        private TextAsset _jsonAsset;
        void OnGUI() {
            if (coloredLabelStyle == null) {
                coloredLabelStyle = new GUIStyle(GUI.skin.label) {
                    richText = true
                };
            }
            if (coloredFoldoutStyle== null) {
                coloredFoldoutStyle = new GUIStyle(EditorStyles.foldout) {
                    richText = true
                };
            }
            
            _jsonAsset = (TextAsset) EditorGUILayout.ObjectField(_jsonAsset, typeof(TextAsset), false);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(AAOption.ToPreview)) {
                _proced.Clear();
                _results.Clear();
                if (_jsonAsset != null) {
                    _proced = DataProcedure.GenerateProcedureFromJson(_jsonAsset.text);

                    foreach (var t in _proced) {
                        _results.Add(ProcedurePreviewer.GetPreviewOfProcedure(t));
                    }
                }
            }
            if (GUILayout.Button(AAOption.ToClear)) {
                _proced.Clear();
                _results.Clear();
            }
            GUI.enabled = _proced.Count>0;
            if (GUILayout.Button(AAOption.ToApply)) {
                Apply();
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            if (_results.Count > 0) {
                DrawData(_results);
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
            string foldoutText = result.Path;
            if (!result.AssetFound) foldoutText += CoverWithColor($" - {AAOption.NoAssetFoundAlert}", Color.red);
            result.Opening = EditorGUILayout.Foldout(result.Opening, foldoutText,true,coloredLabelStyle);
            if (result.Opening) {
                if (result.Objects != null) {
                    DrawJsonObject(result.Objects);
                }else EditorGUILayout.LabelField(AAOption.NothingInJsonAlert);
                
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
            
            
            string signatureText = pair.Signature;
            switch (pair.Type) {
                case AAInternal.AssetReferenceLink:
                    signatureText += CoverWithColor($" - {AAOption.AssetReferenceKeyword}",AAOption.ColorAssetReference);
                    break;
                case AAInternal.Set:
                    signatureText += CoverWithColor($" - {AAOption.SetKeyword}",AAOption.ColorSet);
                    break;
                case AAInternal.MethodCall:
                    signatureText += CoverWithColor($" - {AAOption.MethodKeyword}",AAOption.ColorMethod);
                    break;
                case AAInternal.Nested:
                    signatureText += CoverWithColor($" - {AAOption.NestedKeyword}",AAOption.ColorNested);
                    break;
                case AAInternal.None:
                    signatureText += CoverWithColor($" - {AAOption.NoneKeyword}",AAOption.ColorNone);
                    break;
                case AAInternal.Create:
                    signatureText += CoverWithColor($" - {AAOption.CreateKeyword}",Color.black);
                    signatureColor = Color.green;
                    break;
            }
            if (pair.Duplicated) signatureColor = Color.blue;
            
            if (pair.NotFound) signatureColor = Color.red;

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
                            jsonPair.Opening = EditorGUILayout.Foldout(jsonPair.Opening, "Object");
                            if (jsonPair.Opening) {
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
                    EditorGUI.indentLevel += 1;
                    string toSay = string.Empty;
                    for (int i = 0; i < pair.NewValues.Count; i++) {
                        toSay += pair.NewValues[i].ToString();
                        if (i < pair.NewValues.Count-1) {
                            toSay += ", ";
                        }
                    }
                    EditorGUILayout.LabelField(toSay);
                    EditorGUI.indentLevel -= 1;
                        
                    
                }
                    
                    break;
            }
            EditorGUI.indentLevel -= 1;
        }
        private string CoverWithColor(string value, Color color) {
            return $"<color=#{(int) (color.r * 255):X2}{(int)( color.g * 255):X2}{(int) (color.b * 255):X2}{(int) (color.a * 255):X2}>{value}</color>";
        }
        private void LabelWithColor(string value, Color color) {
            EditorGUILayout.LabelField(CoverWithColor(value, color), coloredLabelStyle);
        }

        public void Apply() {
            HashSet<string> needToReserialize = new HashSet<string>();
            for (int i = 0; i < _proced.Count; i++) {
                string temp = _proced[i].Apply();
                if(temp!=null)
                    needToReserialize.Add(temp);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.ForceReserializeAssets(needToReserialize);
        }
    }

}
#endif