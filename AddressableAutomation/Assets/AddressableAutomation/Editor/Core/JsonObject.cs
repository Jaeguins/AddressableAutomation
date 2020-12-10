using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LitJson;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using static System.String;
using Object = UnityEngine.Object;

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
                        ret.Sort();
                        return ret;
                    case JsonToken.PropertyName:
                        lastPropertyName = reader.Value.ToString();
                        lastValue = new List<object>();
                        break;
                    case JsonToken.ObjectStart:
                        lastValue.Add(ReadObject(reader));
                        ret.Add(new SortableDataPair(lastPropertyName, lastValue?.ToArray()));
                        break;
                    case JsonToken.ArrayStart:
                        multipleValue = true;
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
                object paramVal = null;
                paramVal = Value.Length == 1 ? Value[0] : Value.ToArray();
                if (Value.Length == 1 && Value[0] is JsonObject jsonObj) {
                    jsonObj.Apply(target.GetType().GetField(fieldName).GetValue(target));
                } else if (target.GetType().GetField(fieldName).GetValue(target) is AssetReference) {
                    string[] splitted = Value[0].ToString().Split('!');
                    AssetReference reference = new AssetReference(splitted[0]);
                    if (splitted.Length > 1) {
                        reference.SubObjectName = splitted[1];
                    }
                    target.GetType().GetField(fieldName).SetValue(target, reference);
                } else {
                    
                    target.GetType().GetField(fieldName).SetValue(target, paramVal);
                }
            }
        }
    }

}