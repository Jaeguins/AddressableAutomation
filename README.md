# AddressableAutomation
Easy attribute based JSON to ScriptableObject parser
From one JSON file to multiple ScriptableObject found by path and sync values with same attribute signature or trigger method with parameter with value in same attribute signature.

Have dependancy with Addressable because of linking AssetReference.

## How to Use

### JSON data

```json
[
  {
    "Path" : "Asset/First.asset",
    "Sign1" : 1,
    "Sign2" : ["a","b","c"],
    "Sign3" : 1.2
  },
  {
    "Path" : "Asset/Second.asset",
    "Sign3" : {
      "Sign4-1" : 1,
      "Sign4-2" : 2
      },
    "Sign5" : "Asset/AssetReferenceTarget.asset!SubobjectName",
    "Sign6" : "Asset/AssetReferenceWithoutSubObj.asset"
  }
]
```
 - `Path` attribute : Target asset's path. Can be alternated in project settings.
 - Values for `AssetReference` : Due to subobject's name, you should add `!` and subobject's name. (`!` is optional.)
 - Currently, normal object link is not supported.

### ScriptableObject/Nested object

```csharp
public class FirstTestClass : UnityEngine.ScriptableObject{//somewhat scriptable object at Asset/First.asset
  [AAMethod("Sign1")]
  public int Method1(object[] param){}
  
  [AAField(AAProcessType.Set,"Sign2")]
  public string[] Value2;
  
  [AAField(AAProcessType.Set,"Sign3")]
  public float Value3;
}
public class SecondTestClass : UnityEngine.ScriptableObject{//somewhat scriptable object at Asset/First.asset
  [AAField(AAProcessType.Nested,"Sign3")]
  public NestedData nestedObject;
  
  [AAField(AAProcessType.AssetReferenceLink,"Sign5")]
  public AssetReference link1;
  [AAField(AAProcessType.AssetReferenceLink,"Sign6")]
  public AssetReference link2;
}
[System.Serializable]
public struct NestedData{//nested data structure in SecondTestClass
  [AAField(AAProcessType.Set,"Sign4-1")]
  public int nestedValue1;
  [AAField(AAProcessType.Set,"Sign4-2")]
  public int nestedValue2;
}
```

 - `AAMethod` attribute can be binded to methods, each can be invoked with JSON data as parameter.
 - `AAField` attribute can be binded to fields, each can be set by JSON data.
 - `AAProcessType` states what will be done with JSON data for field.
   - `Set` means just try setting values to fields. Casting will be occured with `Convert.ChangeType()`. If this goes not very well, it will canceled and log warning to unity console.
   - `Nested` means try putting values to fields, but field is object and its inside has attributes too. With this type, process will do the job recursively.
   - `AssetReferenceLink` means that JSON data should be string formatted with asset reference and converted to asset reference. Asset path's validation is not contained.
   - 
