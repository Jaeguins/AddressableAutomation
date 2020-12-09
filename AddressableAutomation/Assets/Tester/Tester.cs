using System.Collections;
using System.Collections.Generic;
using Assets.AddressableAutomation.Core;
using UnityEngine;
[CreateAssetMenu(fileName = "temp",menuName="tester")]
public class Tester:ScriptableObject{
    [AAField(AAProcessType.Set,"Val1")]
    public int Val1Field;


    [AAField(AAProcessType.Set,"Val2")]
    public List<string> Val2Data=new List<string>();

    
    public int[] resultValue = new int[2];


    [AAField(AAProcessType.Nested, "Nested")]
    public NestedData dat;

    [AAMethod("Method1")]
    public void InnerCall(object[] data) {
        resultValue[0] = (int) data[0];
    }
    [AAMethod("Method2")]
    public void InnerCall2(object[] data) {
        resultValue[1] = (int) data[0];
    }
}
[SerializeField]
public struct NestedData {
    
    public int a;
    
    public int aa;
}