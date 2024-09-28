using UnityEngine;

public class SFXmanager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip pistolShoot;
    public AudioClip rifleShoot;

    public void PlayPistolShot()
    {
        audioSource.clip = pistolShoot;
        audioSource.Play();
    }

    public void PlayRifleShot()
    {
        audioSource.clip = rifleShoot;
        audioSource.Play();
    }
}
