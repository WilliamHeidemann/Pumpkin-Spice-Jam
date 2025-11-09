using UnityEngine;

public class FadeObject : MonoBehaviour
{
    public float fadeDuration = 5f;
    private Material _material;
    private float _fadeTimer;
    
    void Start()
    {
        // Get the material (works for MeshRenderer, SpriteRenderer has different approach)
        _material = GetComponent<Renderer>().material;
    }
    
    void Update()
    {
        _fadeTimer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, _fadeTimer / fadeDuration);
        
        Color color = _material.color;
        color.a = alpha;
        _material.color = color;
        
        // if (_fadeTimer >= fadeDuration)
        // {
        //     gameObject.SetActive(false); // Or Destroy(gameObject);
        // }
    }
}
