using UnityEngine;

[CreateAssetMenu(fileName = "FastenerData", menuName = "Item/Fastener")]
public class ScriptableFasteners : ScritableInteractable
{
    public FastenerType type;
    public float unfastenDuration;

}

[SerializeField]
public enum FastenerType
{
    SCREW,
    NUTS
}
