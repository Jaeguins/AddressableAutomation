using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Assets.AddressableAutomation.Core {

    public enum ProcessType {
        Set,
        Callback,
        Nested

    }
    [AttributeUsage(AttributeTargets.Field)]
    public class AAData :System.Attribute {
        public string Sign;
        public ProcessType Type;
        public string CallbackName;

        public AAData(ProcessType type, string signature, string callback=default) {
            Sign = signature;
            Type = type;
            CallbackName = callback;
        }

    }
    
    
  

}