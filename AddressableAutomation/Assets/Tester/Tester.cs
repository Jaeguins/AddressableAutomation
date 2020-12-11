using System;
using System.Collections;
using System.Collections.Generic;
using Assets.AddressableAutomation.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "temp",menuName="tester")]
public class Tester:ScriptableObject {
    [AAField(AAProcessType.Set,"corV")]
    public int CorrectValue;
    [AAField(AAProcessType.Set,"corV-Array")]
    public string[] CorrectArray;
    [AAField(AAProcessType.Set,"corV-List")]
    public List<float> CorrectList;
    [AAField(AAProcessType.AssetReferenceLink,"corA")]
    public AssetReference CorrectAssetReference;
    [AAField(AAProcessType.AssetReferenceLink,"corA-List")]
    public List<AssetReference> CorrectAssetReferenceList;
    [AAField(AAProcessType.AssetReferenceLink,"corA-Array")]
    public AssetReference[] CorrectAssetReferenceArray;
    [AAField(AAProcessType.Nested,"corN")]
    public NestedData CorrectNestedData;
    [AAField(AAProcessType.Nested,"corN-Array")]
    public NestedData[] CorrectNestedArray;
    [AAField(AAProcessType.Nested,"corN-List")]
    public List<NestedData> CorrectNestedList;

    [AAMethod("corM")]
    public void CorrectMethod(object[] param) => ViewParam("Correct", param);


    [AAField(AAProcessType.Set,"badV")]
    public float WrongTypeValue;
    [AAField(AAProcessType.Set,"badV-Array")]
    public int[] WrongTypeArray;
    [AAField(AAProcessType.Set,"badV-List")]
    public List<char> WrongTypeList;
    [AAField(AAProcessType.AssetReferenceLink,"badA")]
    public AssetReference WrongTypeAssetReference;
    [AAField(AAProcessType.Nested,"badN-Not")]
    public NotNested WrongNestedData;

    [AAField(AAProcessType.AssetReferenceLink,"badAttr-SA")]
    public int WrongAttrSetAsset;
    [AAField(AAProcessType.Nested,"badAttr-SN")]
    public int WrongAttrSetNested;
    [AAField(AAProcessType.Set,"badAttr-AS")]
    public AssetReference WrongAttrAssetSet;
    [AAField(AAProcessType.Nested,"badAttr-AN")]
    public AssetReference WrongAttrAssetNested;
    [AAField(AAProcessType.Set,"badAttr-NS")]
    public NestedData WrongAttrNestedSet;
    [AAField(AAProcessType.AssetReferenceLink,"badAttr-NA")]
    public NestedData WrongAttrNestedAsset;


    private void ViewParam(string sign, object[] param) {
        string toSay = sign;
        foreach(var t in param)toSay+=$" {param}";
    }
    
}
[Serializable]
public class NestedData {
    [AAField(AAProcessType.Set,"NestedValue1")]
    public int Nested1;
    [AAField(AAProcessType.Set,"NestedValue2")]
    public int Nested2;
}

[Serializable]
public class NotNested {
    public int NotNested1;
    public int NotNested2;
}