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
 - values for `AssetReference` : Due to subobject's name, you should add `!` and subobject's name. (`!` is optional.)

### ScriptableObject/Nested object
