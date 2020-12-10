using System;
using System.Collections;
using System.Collections.Generic;
using Assets.AddressableAutomation.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "temp",menuName="tester")]
public class Tester:ScriptableObject{
    [AAField(AAProcessType.Set,"Val1")]
    public int Val1Field;


    [AAField(AAProcessType.Set,"Val2")]
    public string[] Val2Data;

    
    public int[] resultValue = new int[2];

    [AAField(AAProcessType.AssetReferenceLink,"Nested")]
    public AssetReference Nested;

    [AAField(AAProcessType.Nested, "NNested")]
    public NNestedData NNested=new NNestedData();
    
    [AAMethod("Method1")]
    public void InnerCall(object[] data) {
        resultValue[0] = (int) data[0];
    }
    [AAMethod("Method2")]
    public void InnerCall2(object[] data) {
        resultValue[1] = (int) data[0];
    }
}
[Serializable]
public class NNestedData {
    [AAField(AAProcessType.Set, "A")]
    public int a;
    [AAField(AAProcessType.Set, "AA")]
    public int b;
}