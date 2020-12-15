using System.Collections.Generic;
using AssetAutomation.Runtime;
using TrimmedLitJson;
using UnityEditor;
using UnityEngine;

namespace AssetAutomation.Editor.Core {

    public class DataProcedure {
        public string Path=string.Empty;
        public JsonObject Object;
        
        public static DataProcedure ReadOneData(JsonReader reader) {
            DataProcedure ret = new DataProcedure {
                Object = JsonObject.ReadObject(reader)
            }; 
            foreach (var t in ret.Object) {
                if (t.Key == AAOption.PathKeyword) {
                    ret.Path = t.Value[0].ToString();
                }
            }
            if (ret.Path == string.Empty) {
                Debug.LogWarning(AAOption.NoAssetFoundAlert);
            };
            return ret;
        }
        public static List<DataProcedure> GenerateProcedureFromJson(string data) {
            JsonReader reader = new JsonReader(data);
            List<DataProcedure> procedures = new List<DataProcedure>();
            string log = string.Empty;
            bool recording = false;
            while (reader.Read()) {
                log += $"{reader.Token}-{reader.Value}\n";
                switch (reader.Token) {
                    case JsonToken.ArrayStart:
                        recording = true;
                        break;
                    case JsonToken.ArrayEnd:
                        recording = false;
                        break;
                    case JsonToken.ObjectStart:
                        if(recording)procedures.Add(ReadOneData(reader));
                        break;
                    
                }
            }
            reader.Close();
            return procedures;
        }
        public string Apply() {
            UnityEngine.Object obj= AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(Path);
            if (obj != null) {
                Object.Apply(obj);
                EditorUtility.SetDirty(obj);
                return Path;
            }
            return null;
        }
    }

}