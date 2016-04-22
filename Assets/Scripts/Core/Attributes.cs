using UnityEngine;

public class ShowOnly : PropertyAttribute
{
}

public class MinMaxSlider : PropertyAttribute
{
    public readonly float min;
    public readonly float max;

    public MinMaxSlider(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}
