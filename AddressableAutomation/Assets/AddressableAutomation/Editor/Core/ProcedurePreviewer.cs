using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Assets.AddressableAutomation.EditorView;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Assets.AddressableAutomation.Core {

    internal class AAInternal : AAProcessType {
        public const int MethodCall = 4;
        public const int Create = 5;
    }

    public class ProcedurePreviewer {
        public string Path;
        public JsonObjectPreviewer Objects;
        public static ProcedurePreviewer GetPreviewOfProcedure(DataProcedure input) {
            ProcedurePreviewer ret = new ProcedurePreviewer();
            ret.Path = input.Path;



            Object asset = AssetDatabase.LoadAssetAtPath<Object>(input.Path);
            foreach (SortableDataPair pair in input.Object) {
                if (pair.Key != AAOption.PathKeyword) ret.Objects = (JsonObjectPreviewer.GetPreviewOfProcedure(asset, input.Object));
            }
            return ret;
        }

        public bool opening = true;
    }

    public class JsonObjectPreviewer : List<SortableDataPairPreviewer> {
        public bool opening = true;
        public static JsonObjectPreviewer GetPreviewOfProcedure(object target, JsonObject input) {
            JsonObjectPreviewer ret = new JsonObjectPreviewer();

            if (input == null) return null;
            foreach (SortableDataPair pair in input) {
                if (pair.Key != AAOption.PathKeyword) ret.Add(SortableDataPairPreviewer.GetPreviewOfData(target, pair, target == null));
            }

            return ret;
        }
    }

    public class SortableDataPairPreviewer : IComparable<SortableDataPairPreviewer> {
        public string Signature;
        public int Type = AAInternal.None;
        public List<object> OldValues = new List<object>(),
                            NewValues = new List<object>();
        public bool NotFound = true,
                    Duplicated = false,
                    MethodCalling = false;

#region Overrides of Object

        public override string ToString() {
            return $"{Signature} - {OldValues.Count}/{NewValues.Count}";
        }

#endregion

        public int CompareTo(SortableDataPairPreviewer other) => string.Compare(Signature, other.Signature, StringComparison.Ordinal);

        public static SortableDataPairPreviewer GetPreviewOfData(object target, SortableDataPair input, bool isNull) {
            SortableDataPairPreviewer ret = new SortableDataPairPreviewer();
            ret.Signature = input.Key;
            if (isNull) {
                ret.NotFound = true;
                ret.MethodCalling = false;
                ret.Type = AAInternal.Create;

                foreach (var t in input.Value) {
                    ret.NewValues.Add(t);
                }
                return ret;
            }


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
                            foreach (object indivVal in input.Value) ret.NewValues.Add(indivVal);
                            ret.MethodCalling = true;
                            ret.Type = AAInternal.MethodCall;
                        }
                    }
                }
                if (breakFlag) break;
            }
            if (breakFlag || pastFound) return ret;
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
                            ret.MethodCalling = false;
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
                                    for (int i = 0; i < input.Value.Length; i++) {
                                        int tC=0;
                                        if (value is IEnumerable enumerableValue) {
                                            foreach (var t in enumerableValue) {
                                                if(tC<input.Value.Length)
                                                    ret.NewValues.Add(JsonObjectPreviewer.GetPreviewOfProcedure(t, input.Value[tC] as JsonObject));
                                                tC += 1;
                                            }
                                                
                                        }
                                        
                                    }
                                    
                                    break;
                                case AAInternal.AssetReferenceLink:
                                {
                                    if (value is IEnumerable enumerableValue) {
                                        foreach (var t in enumerableValue) {
                                            ret.OldValues.Add(t);
                                        }
                                        foreach (var val in input.Value) {
                                            string[] splitted = val.ToString().Split('!');
                                            var newVal = new AssetReference(AssetDatabase.AssetPathToGUID(splitted[0]));
                                            if (splitted.Length > 1) {
                                                newVal.SubObjectName = splitted[1];
                                            }
                                            ret.NewValues.Add(newVal);
                                        }
                                    } else if (value is AssetReference oldRef) {
                                        ret.OldValues.Add(value);
                                        string[] splitted = input.Value[0].ToString().Split('!');
                                        var newVal = new AssetReference(AssetDatabase.AssetPathToGUID(splitted[0]));
                                        if (splitted.Length > 1) {
                                            newVal.SubObjectName = splitted[1];
                                        }
                                        ret.NewValues.Add(newVal);
                                    }
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