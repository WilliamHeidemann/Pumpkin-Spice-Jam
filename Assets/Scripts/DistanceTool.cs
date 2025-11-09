using UnityEngine;
// using UtilityToolkit.Editor;

public class DistanceTool : MonoBehaviour
{
    [SerializeField] private Transform _object1;
    [SerializeField] private Transform _object2;

    // [Button]
    public void MeasureDistance()
    {
        if (_object1 == null || _object2 == null)
        {
            Debug.LogWarning("Please assign both Object 1 and Object 2.");
            return;
        }

        float distance = Vector3.Distance(_object1.position, _object2.position);
        Debug.Log($"Distance between {_object1.name} and {_object2.name}: {distance} units.");
    }
}