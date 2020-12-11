using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LitJson;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static System.String;

namespace Assets.AddressableAutomation.Core {

    public class JsonObject : List<SortableDataPair> {
        public static JsonObject ReadObject(JsonReader reader) {
            JsonObject ret = new JsonObject();
            if (reader.Token != JsonToken.ObjectStart) {
                Debug.LogWarning("Not in object");
                return ret;
            }
            string lastPropertyName = Empty;
            bool multipleValue = false;
            List<object> lastValue = null;
            while (reader.Read()) {
                switch (reader.Token) {
                    case JsonToken.ObjectEnd:
                        // ret.Sort();
                        return ret;
                    case JsonToken.PropertyName:
                        lastPropertyName = reader.Value.ToString();
                        lastValue = new List<object>();
                        break;
                    case JsonToken.ObjectStart:
                        lastValue.Add(ReadObject(reader));
                        if(!multipleValue)
                            ret.Add(new SortableDataPair(lastPropertyName, lastValue?.ToArray()));
                        break;
                    case JsonToken.ArrayStart:
                        multipleValue = true;
                        lastValue.Clear();
                        break;
                    case JsonToken.ArrayEnd:
                        multipleValue = false;
                        ret.Add(new SortableDataPair(lastPropertyName, lastValue?.ToArray()));
                        break;
                    case JsonToken.None:
                        break;
                    default:
                        lastValue.Add(reader.Value);
                        if (!multipleValue) {
                            ret.Add(new SortableDataPair(lastPropertyName, lastValue?.ToArray()));
                        }
                        break;
                }
            }
            throw new Exception("Unreachable section");
        }
        public void Apply(object target) {
            foreach (var t in this) {
                t.Apply(target);
            }
        }
    }

    public class SortableDataPair : IComparable<SortableDataPair> {
        public string Key;
        public object[] Value;

#region Overrides of Object

        public override string ToString() {
            return $"Key : {Key} / Value : {(Value.Length > 1 ? Value.Length.ToString() : Value[0].ToString())}";
        }

#endregion

        public int CompareTo(SortableDataPair other) {
            return Compare(Key, other.Key, StringComparison.Ordinal);
        }
        public SortableDataPair(string key, object[] value) {
            Key = key;
            Value = value;
        }

        public void Apply(object target) {
            if (target == null) return;
            string methodName = Empty,
                   fieldName = Empty;
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
                target.GetType().GetMethod(methodName).Invoke(target, new object[] {Value});
            } else {
                FieldInfo fieldInfo = target.GetType().GetField(fieldName);
                if (fieldInfo.GetValue(target) is AssetReference) {
                    string[] splitted = Value[0].ToString().Split('!');
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
                                        string[] temp = Value[i].ToString().Split('!');
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
                                        $"{fieldInfo.Name} is not proper Type"
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
                                        string[] temp = Value[i].ToString().Split('!');
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
                                        $"{fieldInfo.Name} is not proper Type"
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
                                $"{fieldInfo} is not convertible with {Value[0]}"
                            );
                    }
                }
            }
        }
    }

}