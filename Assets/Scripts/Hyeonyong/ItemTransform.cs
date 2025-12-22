using UnityEngine;

public class ItemTransform
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public ItemTransform(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
    }
}
