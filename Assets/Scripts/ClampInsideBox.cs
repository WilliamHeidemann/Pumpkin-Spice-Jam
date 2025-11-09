using UnityEngine;
using UnityEngine.Serialization;

public class ClampInsideBox : MonoBehaviour
{
    [SerializeField] private BoxCollider _boxCollider; // Assign your trigger box here

    private void Update()
    {
        Bounds bounds = _boxCollider.bounds;
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, bounds.min.x, bounds.max.x);
        pos.y = Mathf.Clamp(pos.y, bounds.min.y, bounds.max.y);
        pos.z = Mathf.Clamp(pos.z, bounds.min.z, bounds.max.z);

        transform.position = pos;
    }
}