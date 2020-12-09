using System;

namespace Assets.AddressableAutomation.Core {
    public class AAProcessType {
        public const int None = 0,
                         Set = 1,
                         Nested = 2,
                         AssetReferenceLink = 3;

    }
    
    
    [AttributeUsage(AttributeTargets.Field)]
    public class AAField :Attribute {
        public string Sign;
        public int Type;

        public AAField(int type, string signature) {
            Sign = signature;
            Type = type;
        }
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class AAMethod : Attribute {
        public string Sign;
        public AAMethod(string signature) {
            Sign = signature;
        }
    }
    
    
  

}