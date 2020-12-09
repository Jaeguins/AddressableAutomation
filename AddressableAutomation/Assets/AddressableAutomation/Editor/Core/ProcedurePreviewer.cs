using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Assets.AddressableAutomation.EditorView;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Assets.AddressableAutomation.Core {
    internal class AAInternal:AAProcessType {
        public const int MethodCall = 4;
    }
    public class ProcedurePreviewer{
        public string Path;
        public JsonObjectPreviewer Objects;
        public static ProcedurePreviewer GetPreviewOfProcedure(DataProcedure input) {
            ProcedurePreviewer ret=new ProcedurePreviewer();
            ret.Path = input.Path;
            


            Object asset = AssetDatabase.LoadAssetAtPath<Object>(input.Path);
            foreach (SortableDataPair pair in input.Object) {
                if(pair.Key!=AAOption.PathKeyword)
                    ret.Objects=(JsonObjectPreviewer.GetPreviewOfProcedure(asset,input.Object));
            }
            return ret;
        }

        public bool opening = false;
    }

    public class JsonObjectPreviewer : List<SortableDataPairPreviewer> {
        public static JsonObjectPreviewer GetPreviewOfProcedure(object target,JsonObject input) {
            JsonObjectPreviewer ret=new JsonObjectPreviewer();
            
            
            foreach (SortableDataPair pair in input) {
                if(pair.Key!=AAOption.PathKeyword)
                    ret.Add(SortableDataPairPreviewer.GetPreviewOfData(target,pair));
            }
            
            return ret;
        }
    }

    public class SortableDataPairPreviewer : IComparable<SortableDataPairPreviewer> {
        public string Signature;
        public int Type=AAInternal.None;
        public List<object> OldValues = new List<object>(),
                            NewValues = new List<object>();
        public bool NotFound = true,
                    Duplicated = false,
                MethodCalling=false;
        public int CompareTo(SortableDataPairPreviewer other) => string.Compare(Signature, other.Signature, StringComparison.Ordinal);

        public static SortableDataPairPreviewer GetPreviewOfData(object target, SortableDataPair input) {
            SortableDataPairPreviewer ret = new SortableDataPairPreviewer();
            ret.Signature = input.Key;



            bool pastFound = false,
                 breakFlag = false;
            foreach (MethodInfo method in target.GetType().GetMethods()) {
                foreach (Attribute tAttr in method.GetCustomAttributes()) {
                    if (tAttr is AAMethod attr) {
                        if (attr.Sign == ret.Signature) {
                            if (pastFound) {
                                breakFlag = true;
                                ret.Duplicated = true;
                                break;
                            }
                            pastFound = true;
                            ret.NotFound = false;

                            ret.OldValues.Add(method.Name);
                            foreach(object indivVal in input.Value)
                                ret.NewValues.Add(indivVal);
                            ret.MethodCalling = true;
                            ret.Type = AAInternal.MethodCall;
                        }
                    }
                }
                if (breakFlag) break;
            }
            if (breakFlag||pastFound) return ret;
            foreach (FieldInfo field in target.GetType().GetFields()) {
                foreach (Attribute tAttr in field.GetCustomAttributes()) {
                    if (tAttr is AAField attr) {
                        if (attr.Sign == ret.Signature) {
                            if (pastFound) {
                                breakFlag = true;
                                ret.Duplicated = true;
                                break;
                            }
                            pastFound = true;
                            ret.NotFound = false;
                            object value = field.GetValue(target);
                            ret.Type = attr.Type;
                            ret.MethodCalling=false;
                            switch (attr.Type) {
                                case AAInternal.Set:
                                {
                                    
                                    if (value is IEnumerable enumerableValue) {
                                        foreach (var t in enumerableValue) {
                                            ret.OldValues.Add(t);
                                        }
                                        foreach (var val in input.Value) {
                                            ret.NewValues.Add(val);
                                        }
                                    } else {
                                        ret.OldValues.Add(value);
                                        ret.NewValues.Add(input.Value[0]);
                                    }
                                }
                                    break;
                                case AAInternal.Nested:
                                    ret.NewValues.Add(JsonObjectPreviewer.GetPreviewOfProcedure(value, input.Value[0] as JsonObject));
                                    break;
                                case AAInternal.AssetReferenceLink:
                                    if (value is AssetReference oldRef) {
                                        string[] splitted = input.Value[0].ToString().Split('!');
                                        ret.OldValues.Add(value);
                                        var newVal = new AssetReference(AssetDatabase.AssetPathToGUID(input.Value[0].ToString())) {
                                            SubObjectName = splitted[1]
                                        };
                                        ret.NewValues.Add(newVal);
                                    }
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                }
                if (breakFlag) break;
            }
            return ret;
        }
        
    }

}