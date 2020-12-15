using System;
using System.Collections;
using System.Reflection;
using AssetAutomation.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AssetAutomation.Editor.Core {

    public class SortableDataPair : IComparable<SortableDataPair> {
        public string Key;
        public object[] Value;

#region Overrides of Object

        public override string ToString() {
            return $"Key : {Key} / Value : {(Value.Length > 1 ? Value.Length.ToString() : Value[0].ToString())}";
        }

#endregion

        public int CompareTo(SortableDataPair other) {
            return String.Compare(Key, other.Key, StringComparison.Ordinal);
        }
        public SortableDataPair(string key, object[] value) {
            Key = key;
            Value = value;
        }

        public void Apply(object target) {
            if (target == null) return;
            string methodName = String.Empty,
                   fieldName = String.Empty;
            bool already = false,
                 isMethod = false;

            foreach (MethodInfo method in target.GetType().GetMethods()) {
                foreach (Attribute tAttr in method.GetCustomAttributes()) {
                    if (tAttr is AAMethod attr) {
                        if (attr.Sign == Key) {
                            if (already) {
                                return;
                            }
                            methodName = method.Name;
                            isMethod = true;
                            already = true;
                        }
                    }
                }
            }
            foreach (FieldInfo field in target.GetType().GetFields()) {
                foreach (Attribute tAttr in field.GetCustomAttributes()) {
                    if (tAttr is AAField attr) {
                        if (attr.Sign == Key) {
                            if (already) {
                                return;
                            }
                            fieldName = field.Name;
                            isMethod = false;
                            already = true;
                        }
                    }
                }
            }
            if (!already) return;
            if (isMethod) {
                foreach (var t in Value) {
                    if (t is JsonObject) {
                        Debug.LogWarning($"{methodName}{AAOption.MethodCallNestedAlert}");
                        return;
                    }
                }
                target.GetType().GetMethod(methodName).Invoke(target, new object[] {Value});
                string paramView = string.Empty;
                for (int i = 0; i < Value.Length; i++) {
                    paramView += $"{Value[i].ToString()}{(i==Value.Length-1?"":", ")}";
                }
                Debug.Log($"{methodName}({paramView}){AAOption.MethodCallAlert}");
            } else {
                FieldInfo fieldInfo = target.GetType().GetField(fieldName);
                if (fieldInfo.GetValue(target) is AssetReference) {
                    string[] splitted = Value[0].ToString().Split(AAOption.AssetReferenceDiffKeyword);
                    AssetReference reference = new AssetReference(AssetDatabase.AssetPathToGUID(splitted[0]));
                    if (splitted.Length > 1) {
                        reference.SubObjectName = splitted[1];
                    }
                    target.GetType().GetField(fieldName).SetValue(target, reference);
                } else if (fieldInfo.GetValue(target) is IList list) {
                    if (fieldInfo.GetValue(target) is Array arr) {
                        fieldInfo.SetValue(target, Array.CreateInstance(arr.GetType().GetElementType(), Value.Length));
                        list = fieldInfo.GetValue(target) as Array;
                        if (list != null) {
                            Type targetType = list.GetType().GetElementType();
                            try {
                                for (int i = 0; i < Value.Length; i++) {
                                    if (targetType == typeof(AssetReference)) {
                                        string[] temp = Value[i].ToString().Split(AAOption.AssetReferenceDiffKeyword);
                                        AssetReference refe = new AssetReference(AssetDatabase.AssetPathToGUID(temp[0]));
                                        if (temp.Length > 1 && temp[1] != string.Empty) refe.SubObjectName = temp[1];
                                        list[i] = refe;
                                    } else if (Value[i] is JsonObject jsonObj) {
                                        object targetObj = Activator.CreateInstance(targetType);
                                        jsonObj.Apply(targetObj);
                                        list[i] = targetObj;
                                    } else {
                                        list[i] = Convert.ChangeType(Value[i], targetType);
                                    }
                                }
                            }
                            catch (Exception e) {
                                if (e is InvalidCastException || e is ArgumentException || e is FormatException)
                                    Debug.LogWarning(
                                        $"{fieldInfo.Name}{AAOption.NotProperTypeAlert}"
                                    );
                            }
                        }
                    } else {
                        list.Clear();
                        if (list != null) {
                            Type targetType = list.GetType().GetGenericArguments()[0];

                            try {
                                for (int i = 0; i < Value.Length; i++) {
                                    if (targetType == typeof(AssetReference)) {
                                        string[] temp = Value[i].ToString().Split(AAOption.AssetReferenceDiffKeyword);
                                        AssetReference refe = new AssetReference(AssetDatabase.AssetPathToGUID(temp[0]));
                                        if (temp.Length > 1 && temp[1] != string.Empty) refe.SubObjectName = temp[1];
                                        list.Add(refe);
                                    } else if (Value[i] is JsonObject jsonObj) {
                                        object targetObj = Activator.CreateInstance(targetType);
                                        jsonObj.Apply(targetObj);
                                        list.Add(targetObj);
                                    } else {
                                        list.Add(Convert.ChangeType(Value[i], targetType));
                                    }
                                }
                            }
                            catch (Exception e) {
                                if (e is InvalidCastException || e is ArgumentException || e is FormatException)
                                    Debug.LogWarning(
                                        $"{fieldInfo.Name}{AAOption.NotProperTypeAlert}"
                                    );
                            }
                        }
                    }
                } else if (Value[0] is JsonObject jsonObj) {
                    object targetObj = Activator.CreateInstance(fieldInfo.FieldType);
                    jsonObj.Apply(targetObj);
                    fieldInfo.SetValue(target, targetObj);
                } else {
                    try {
                        fieldInfo.SetValue(target, Value[0]);
                    }
                    catch (Exception e) {
                        if (e is InvalidCastException || e is ArgumentException || e is FormatException)
                            Debug.LogWarning(
                                $"{fieldInfo}<--->{Value[0]}{AAOption.NotConvertibelAlert}"
                            );
                    }
                }
            }
        }
    }

}