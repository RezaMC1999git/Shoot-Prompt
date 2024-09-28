using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    public Transform hitVFX, VFXPoint;
    public Rigidbody rigidbody;
    public BoxCollider boxCollider;
    public float speed = 10f;
    public gunType gunTypeEnum;

    private void Start()
    {
        rigidbody.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(gunTypeEnum == gunType.pistol) 
        {
            if (other.GetComponent<BulletTarget>())
            {
                Instantiate(hitVFX, VFXPoint.position, Quaternion.identity);

                BulletTarget target = other.GetComponent<BulletTarget>();
                target.ShootDetected(this.gameObject, gunTypeEnum, boxCollider);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gunTypeEnum == gunType.riffle)
        {
            if (collision.collider.GetComponent<BulletTarget>())
            {
                Instantiate(hitVFX, transform.position, Quaternion.identity);

                BulletTarget target = collision.collider.GetComponent<BulletTarget>();
                target.ShootDetected(this.gameObject, gunTypeEnum, boxCollider);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    public enum gunType 
    {
        pistol,
        riffle
    }
}
