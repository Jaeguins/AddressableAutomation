using System;
using System.Collections.Generic;
using LitJson;

namespace Assets.AddressableAutomation.Core {

    public class DataProcedure {
        public string AddressableName=string.Empty;
        public JsonObject Object;
        private const string _addressKeyword = "Address";
        public static DataProcedure ReadOneData(JsonReader reader) {
            DataProcedure ret = new DataProcedure();
            ret.Object=JsonObject.ReadObject(reader);
            foreach (var t in ret.Object) {
                if (t.Key == _addressKeyword) {
                    ret.AddressableName = t.Value[0].ToString();
                }
            }
            if(ret.AddressableName==string.Empty)throw new Exception("No named procedure");
            return ret;
        }
        public static List<DataProcedure> GenerateProcedureFromJson(string data) {
            string log = string.Empty;

            JsonReader reader = new JsonReader(data);
            List<DataProcedure> procedures = new List<DataProcedure>();
            bool recording = false;
            while (reader.Read()) {
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
            return procedures;
        }
    }

}