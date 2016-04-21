using UnityEngine;

[CreateAssetMenu(fileName = "FastenerData", menuName = "Item/Fastener")]
public class ScriptableFasteners : ScriptableInteractable
{
    public FastenerType type;
    public float unfastenDuration;
    public float fastenerLength;
    public Vector3 startPosition; 

}

[SerializeField]
public enum FastenerType
{
    SCREW,
    NUTS
}
