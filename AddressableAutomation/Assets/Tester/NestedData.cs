using Assets.AddressableAutomation.Core;
using UnityEngine;
[CreateAssetMenu(fileName = "temp",menuName="tester2")]
public class NestedData : ScriptableObject{
    [AAField(AAProcessType.Set,"A")]
    public int a;
    [AAField(AAProcessType.Set,"AB")]
    public int aa;
}