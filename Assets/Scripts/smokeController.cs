using UnityEngine;

public class SmokeController : MonoBehaviour
{
    public float scale;
    public float speed;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       Destroy(gameObject, 4); 
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = scale*Vector3.one;
        scale += Time.deltaTime;
        transform.position += Vector3.up * (speed * Time.deltaTime);
    }
}
