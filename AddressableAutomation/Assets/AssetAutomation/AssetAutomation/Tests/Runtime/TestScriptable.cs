using System;
using System.Collections;
using System.Collections.Generic;
using AssetAutomation.Runtime;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// Confirmed Testing Case
/// </summary>
[CreateAssetMenu(fileName = "temp", menuName = "tester")]
public class TestScriptable : ScriptableObject {
#region Correct Cases

    [AAField(AAProcessType.Set, "corV")]
    public int CorrectValue;
    [AAField(AAProcessType.Set, "corV-Array")]
    public string[] CorrectArray;
    [AAField(AAProcessType.Set, "corV-List")]
    public List<float> CorrectList;
    [AAField(AAProcessType.AssetReferenceLink, "corA")]
    public AssetReference CorrectAssetReference;
    [AAField(AAProcessType.AssetReferenceLink, "corA-List")]
    public List<AssetReference> CorrectAssetReferenceList;
    [AAField(AAProcessType.AssetReferenceLink, "corA-Array")]
    public AssetReference[] CorrectAssetReferenceArray;
    [AAField(AAProcessType.Nested, "corN")]
    public NestedData CorrectNestedData;
    [AAField(AAProcessType.Nested, "corN-Array")]
    public NestedData[] CorrectNestedArray;
    [AAField(AAProcessType.Nested, "corN-List")]
    public List<NestedData> CorrectNestedList;

    [AAMethod("corM")]
    public void CorrectMethod(object[] param) => ViewParam("Correct", param);

#endregion

#region Wrong Cases - By Assets/AssetAutomation/Resources/Tests/testData.json

    /// <summary>
    /// Wrong value type
    /// </summary>
    [AAField(AAProcessType.Set, "badV")]
    public float WrongTypeValue;
    /// <summary>
    /// Wrong value type with Array
    /// </summary>
    [AAField(AAProcessType.Set, "badV-Array")]
    public int[] WrongTypeArray;
    /// <summary>
    /// Wrong value type with List
    /// </summary>
    [AAField(AAProcessType.Set, "badV-List")]
    public List<char> WrongTypeList;
    /// <summary>
    /// Wrong value type with asset reference
    /// </summary>
    [AAField(AAProcessType.AssetReferenceLink, "badA")]
    public AssetReference WrongTypeAssetReference;
    /// <summary>
    /// Wrong value type with nested object - non AA object binding
    /// </summary>
    [AAField(AAProcessType.Nested, "badN-Not")]
    public NotNested WrongNestedData;


    /// <summary>
    /// Wrong attribute type from value to asset reference
    /// </summary>
    [AAField(AAProcessType.AssetReferenceLink, "badAttr-SA")]
    public int WrongAttrSetAsset;
    /// <summary>
    /// Wrong attribute type from value to nested object
    /// </summary>
    [AAField(AAProcessType.Nested, "badAttr-SN")]
    public int WrongAttrSetNested;
    /// <summary>
    /// Wrong attribute type from asset reference to value
    /// </summary>
    [AAField(AAProcessType.Set, "badAttr-AS")]
    public AssetReference WrongAttrAssetSet;
    /// <summary>
    /// Wrong attribute type from asset reference to nested object
    /// </summary>
    [AAField(AAProcessType.Nested, "badAttr-AN")]
    public AssetReference WrongAttrAssetNested;
    /// <summary>
    /// Wrong attribute type from nested object to value
    /// </summary>
    [AAField(AAProcessType.Set, "badAttr-NS")]
    public NestedData WrongAttrNestedSet;
    /// <summary>
    /// Wrong attribute type from nested object to asset reference
    /// </summary>
    [AAField(AAProcessType.AssetReferenceLink, "badAttr-NA")]
    public NestedData WrongAttrNestedAsset;

#endregion

    private void ViewParam(string sign, object[] param) {
        string toSay = sign;
        foreach (var t in param) toSay += $" {param}";
    }
}
/// <summary>
/// example use of nested object
/// </summary>
[Serializable]
public class NestedData {
    [AAField(AAProcessType.Set, "NestedValue1")]
    public int Nested1;
    [AAField(AAProcessType.Set, "NestedValue2")]
    public int Nested2;
}
/// <summary>
/// non nested object
/// </summary>
[Serializable]
public class NotNested {
    public int NotNested1;
    public int NotNested2;
}