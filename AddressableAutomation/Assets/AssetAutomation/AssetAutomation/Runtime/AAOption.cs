using UnityEditor;
using UnityEngine;

namespace AssetAutomation.Runtime {

    [System.Serializable]
    public class AAOption : ScriptableObject {
        public static string PathKeyword => Instance._pathKeyword;
        public static string NoAssetFoundAlert => Instance._noAssetFoundAlert;
        public static string JumpTo => Instance._jumpButton;
        public static string NoneKeyword => Instance._none;
        public static string CreateKeyword => Instance._create;
        public static string AssetReferenceKeyword => Instance._assetReference;
        public static string NestedKeyword => Instance._nested;
        public static string MethodKeyword => Instance._method;
        public static string SetKeyword => Instance._set;
        public static string NothingInJsonAlert => Instance._nothingInJsonAlert;
        public static string NotProperTypeAlert => Instance._notProperTypeAlert;
        public static string NotConvertibelAlert => Instance._notConvertibleAlert;
        public static string MethodCallAlert => Instance._methodCallAlert;
        public static string ToPreview => Instance._previewButton;
        public static string ToClear => Instance._clearButton;
        public static string ToApply => Instance._applyButton;
        public static string ToInfo => Instance._infoButton;

        public static Color ColorCreate => Instance._createColor;
        public static Color ColorNone => Instance._noneColor;
        public static Color ColorSet => Instance._setColor;
        public static Color ColorMethod => Instance._methodColor;
        public static Color ColorNested => Instance._nestedColor;
        public static Color ColorAssetReference => Instance._assetReferenceColor;

        private static AAOption instance;
        public static AAOption GetInstance() => Instance;
        public static AAOption Instance {
            get {
                if (instance == null) {
                    instance = Resources.Load<AAOption>("AAOption");
                }
                return instance;
            }
        }

#region Option
        [Header("Data")]
        [SerializeField]
        private string _pathKeyword = "Path";

#endregion

#region AAView Localization
        [Header("Localization - Label")]
        [SerializeField]
        private string _assetReference = "AssetReference";
        [SerializeField]
        private string _create = "Create";
        [SerializeField]
        private string _method = "MethodCall";
        [SerializeField]
        private string _nested = "Nested";
        [SerializeField]
        private string _none = "None";
        [SerializeField]
        private string _set = "Set";
        [Header("Localization - Alert")]
        [SerializeField]
        private string _noAssetFoundAlert = "Asset not found.";
        [SerializeField]
        private string _nothingInJsonAlert = "Nothing in this json object.";
        [SerializeField]
        private string _notProperTypeAlert = " is not proper Type.";
        [SerializeField]
        private string _notConvertibleAlert= " is not convertible.";
        [SerializeField]
        private string _methodCallAlert = " is called.";
        [Header("Localization - Button")]
        [SerializeField]
        private string _jumpButton = "Jump";
        [SerializeField]
        private string _previewButton = "Preview";
        [SerializeField]
        private string _clearButton= "Clear";
        [SerializeField]
        private string _applyButton= "Apply";
        [SerializeField]
        private string _infoButton= "Info";

#endregion

#region AAView Customization

        [SerializeField]
        private Color _assetReferenceColor = new Color(.2f, .5f, .2f);
        [SerializeField]
        private Color _createColor = Color.red;
        [SerializeField]
        private Color _methodColor = Color.green;
        [SerializeField]
        private Color _nestedColor = Color.blue;
        [SerializeField]
        private Color _noneColor = Color.red;
        [SerializeField]
        private Color _setColor = Color.black;

#endregion
    }

    [CustomEditor(typeof(AAOption))]
    public class AAOptionEditor : Editor {
#region Overrides of Editor

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
        }

#endregion

        static AAOption GetOption() {
            return Resources.Load<AAOption>("AAOption");
        }
        [SettingsProvider]
        static SettingsProvider GetSettingProvider() {
            SettingsProvider ret = null;
            if (GetOption() != null) {
                ret = new AssetSettingsProvider("Project/AssetAutomation", GetOption);
            }
            return ret;
        }
    }

}