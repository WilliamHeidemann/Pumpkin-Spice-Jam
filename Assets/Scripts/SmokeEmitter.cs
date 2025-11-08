using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class SmokeEmitter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject smoke;
    private float _timeSinceSpawn; 
    public Transform emissionPoint;
    public float emissionRate;

    public SmokeEmitter(Transform emissionPoint)
    {
        this.emissionPoint = emissionPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (!(_timeSinceSpawn < Time.timeSinceLevelLoad - emissionRate)) return;
        _timeSinceSpawn = Time.timeSinceLevelLoad;
        Instantiate(smoke, emissionPoint.position + Vector3.up + Vector3.forward, emissionPoint.rotation);
    }
}
