using System.Collections;
using System.Collections.Generic;
using Assets.AddressableAutomation.Core;
using UnityEngine;

public class Tester:MonoBehaviour{
    [AAData(ProcessType.Set,"Val1")]
    public int Val1Field;


    [AAData(ProcessType.Callback,"Val2","InsertVal2Data")]
    public List<string> Val2Data=new List<string>();
        
    public void InsertVal2Data(object[] data) {
        for (int i = 0; i < data.Length; i++) {
            Val2Data.Add((string)data[i]);
        }
    }
    public void foo() {
        Tester t=null;
        foreach (var field in t.GetType().GetFields()) {
            foreach (var attr in field.GetCustomAttributes(true)) {
                if (attr is AAData attrAA) {
                    Debug.Log(attrAA.Sign);
                }
            }
        }
    }
}