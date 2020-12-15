asdf# AddressableAutomation
Easy attribute-based JSON to ScriptableObject parser
From one JSON file to multiple ScriptableObject found with path and sync values with same attribute signature or trigger method with parameters with value in same attribute signature.

Have dependency on Addressable because of linking AssetReference.

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
 - Values for `AssetReference` : Due to the subobject's name, you should add `!` and the subobject's name. (`!` is optional.)
 - Currently, a normal object link is not supported.

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

 - `AAMethod` attribute can be bound to methods, each can be invoked with JSON data as a parameter. Method invoking with asset reference and nested JSON object is not supported now.
 - `AAField` attribute can be bound to fields, each can be set by JSON data.
 - `AAProcessType` states what will be done with JSON data for fields.
   - `Set` means just try setting values to fields. Casting will be occurred with `Convert.ChangeType()`. If this goes not very well, it will be canceled, and log a warning to unity console.
   - `Nested` means try putting values to fields, but the field is object and its inside has attributes too. With this type, the process will do the job recursively.
   - `AssetReferenceLink` means that JSON data should be a string formatted with asset reference and converted to asset reference. Asset path validation is not contained.
   - `None` means nothing. Just for an exception.

### Project Settings
![Project Setting Image](https://github.com/Jaeguins/ImagePool/blob/main/Files/AA.jpg)

 - `Path Keyword` is what JSON attribute means about the target asset's path.
 - `Asset Reference Keyword` is seperator character for dividing value to asset path and subobject.

### Notice

- This package uses Json reader by [LitJson](https://litjson.net/) and integrated only reading stuff.
- This package is [Unlicense](https://unlicense.org/) license.
