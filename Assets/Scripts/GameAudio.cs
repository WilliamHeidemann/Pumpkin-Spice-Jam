using UnityEngine;

public class GameAudio : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _building;
    
    public void PlayBuildingSound()
    {
        _audioSource.pitch = Random.Range(0.8f, 1.2f);
        _audioSource.PlayOneShot(_building);
    }
}