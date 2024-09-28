using UnityEngine;
using static BulletProjectile;

public class BulletTarget : MonoBehaviour
{
    public targetType targetType;
    public int health;
    public Rigidbody rigidbody;
    public int woodenHealth = 5, ironHealth = 10;
    [SerializeField] AudioClip woodClip, ironClip, missedClip;
    [SerializeField] AudioSource audioSource;

    private void Start()
    {
        audioSource.clip = missedClip;
        switch (targetType)
        {
            case targetType.wood:
                audioSource.clip = woodClip;
                health = woodenHealth;
                break;
            case targetType.iron:
                audioSource.clip = ironClip;
                health = ironHealth;
                break;
            default:
                break;
        }
    }

    public void ShootDetected(GameObject bullet, gunType gunTypeEnum, BoxCollider boxCollider)
    {
        audioSource.Play();
        if (targetType == targetType.other)
        {
            Destroy(bullet);
            return;
        }
        switch (gunTypeEnum)
        {
            case gunType.pistol:
                if (targetType == targetType.wood)
                {
                    if (rigidbody != null) 
                    {
                        boxCollider.enabled = true;
                        //rigidbody.AddExplosionForce(50f, transform.position, 0.5f);
                    }       
                    health--;
                }
                else
                {
                    boxCollider.enabled = false;
                }
                break;

            case gunType.riffle:
                if (targetType == targetType.iron)
                {
                    if (rigidbody != null)
                    {
                        boxCollider.enabled = true;
                        //rigidbody.AddExplosionForce(50f, transform.position, 0.5f);
                    }   
                    health--;
                }
                else
                {
                    boxCollider.enabled = false;
                }
                break;
        }
        Destroy(bullet);
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}

public enum targetType
{
    wood,
    iron,
    other
}
