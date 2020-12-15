using AssetAutomation.Runtime;
using UnityEditor;
using Object = UnityEngine.Object;

namespace AssetAutomation.Editor.Core {

    public class ProcedurePreviewer {
        public string Path;
        public bool AssetFound = false;
        public JsonObjectPreviewer Objects;
        public static ProcedurePreviewer GetPreviewOfProcedure(DataProcedure input) {
            ProcedurePreviewer ret = new ProcedurePreviewer();
            ret.Path = input.Path;



            Object asset = AssetDatabase.LoadAssetAtPath<Object>(input.Path);
            if (asset != null) ret.AssetFound = true;
            foreach (SortableDataPair pair in input.Object) {
                if (pair.Key != AAOption.PathKeyword) ret.Objects = (JsonObjectPreviewer.GetPreviewOfProcedure(asset, input.Object));
            }
            return ret;
        }

        public bool Opening = true;
    }

}