using UnityEngine;
using System.Collections;

public class OperatorIcons : MonoBehaviour
{
    public static OperatorIcons _instance = null;
    [SerializeField]
    DroppedItemIcon iconPrefab = null;

    void Start()
    {
        if (iconPrefab == null)
        {
            Debug.Log("OperatorIcons is missing an icon prefab!  Disabling...");
            gameObject.SetActive(false);
        }
        ObjectPool.CreatePool<DroppedItemIcon>(iconPrefab, 10);

        _instance = this;
    }

    public static void NewIcon(Vector2 pos, NetMessage.APBResponse.Hint.HintType hintType)
    {
        if (_instance)
        {
            _instance.SpawnNewIcon(pos, hintType);
        }
    }

    public void SpawnNewIcon(Vector2 pos, NetMessage.APBResponse.Hint.HintType hintType)
    {
        DroppedItemIcon icon = iconPrefab.Spawn(transform);
        if (icon != null)
        {
            if (!icon.Init(pos, hintType))
            {
                Debug.LogWarning("Unable to create icon for type " + NetMessage.APBResponse.Hint.TypeToName(hintType));
                icon.Recycle();
            }
        }
    }

    public bool doIt = false;
    public void Update()
    {
        if (doIt)
        {
            OperatorIcons.NewIcon(new Vector2(Random.Range(0f, 2400f), Random.Range(0f, 2400f))
                                            , NetMessage.APBResponse.Hint.HintType.Screwdriver);
            doIt = false;
        }
    }
}
