using System.Collections.Generic;
using AssetAutomation.Runtime;

namespace AssetAutomation.Editor.Core {

    public class JsonObjectPreviewer : List<SortableDataPairPreviewer> {
        public bool Opening = true;
        public static JsonObjectPreviewer GetPreviewOfProcedure(object target, JsonObject input) {
            JsonObjectPreviewer ret = new JsonObjectPreviewer();

            if (input == null) return null;
            foreach (SortableDataPair pair in input) {
                if (pair.Key != AAOption.PathKeyword) ret.Add(SortableDataPairPreviewer.GetPreviewOfData(target, pair, target == null));
            }

            return ret;
        }
    }

}