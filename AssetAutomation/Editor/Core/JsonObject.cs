using System;
using System.Collections.Generic;
using TrimmedLitJson;
using UnityEngine;
using static System.String;

namespace AssetAutomation.Editor.Core {

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

}