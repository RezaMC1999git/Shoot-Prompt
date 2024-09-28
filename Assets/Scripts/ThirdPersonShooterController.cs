using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonShooterController : MonoBehaviour
{
    public Camera Camera;
    [SerializeField] private CinemachineVirtualCamera ainCamera;
    [SerializeField] private InputsHandler inputsHandler;
    [SerializeField] private float normalSensitivity, aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform pistolSpawnBulletPosition, rifleSpawnBulletPosition;
    [SerializeField] private BulletProjectile pistolBulletProjectilePrefab, rifleBulletProjectilePrefab;
    [SerializeField] private Movements movements;
    [SerializeField] private Actions actions;
    [SerializeField] private Image crosshairImage;
    [SerializeField] private GameObject pistolGunGO, rifleGunGO;
    [SerializeField] private SFXmanager sfxManager;
    [SerializeField] private TextMeshProUGUI gunTypeText;
    [SerializeField] private ShakeCamera shakeCamera;
    [SerializeField] InputsHandler InputsHandler;

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector3 mouseWorldPosition = Vector3.zero;
        Ray ray = Camera.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            mouseWorldPosition = raycastHit.point;
        }

        if (inputsHandler.aim && !InputsHandler.isInPromptMode)
        {
            if (!actions.changedGunToRifle)
            {
                movements.animator.SetLayerWeight(1, 1f);
                movements.animator.SetLayerWeight(2, 0f);
                pistolGunGO.SetActive(true);
                rifleGunGO.SetActive(false);
            }
            else
            {
                movements.animator.SetLayerWeight(2, 1f);
                movements.animator.SetLayerWeight(1, 0f);
                pistolGunGO.SetActive(false);
                rifleGunGO.SetActive(true);
            }

            crosshairImage.gameObject.SetActive(true);
            movements.animator.ResetTrigger("Unarmed");
            movements.animator.SetTrigger("Armed");

            if (inputsHandler.shoot)
            {
                shakeCamera.aimCameraShakeAnimator.enabled = true;
                if (!actions.changedGunToRifle)
                {
                    sfxManager.PlayPistolShot();
                    Vector3 aimDir = (mouseWorldPosition - pistolSpawnBulletPosition.position).normalized;
                    Instantiate(pistolBulletProjectilePrefab, pistolSpawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
                    inputsHandler.shoot = false;
                }
                else
                {
                    sfxManager.PlayRifleShot();
                    Vector3 aimDir = (mouseWorldPosition - rifleSpawnBulletPosition.position).normalized;
                    Instantiate(rifleBulletProjectilePrefab, rifleSpawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
                    inputsHandler.shoot = false;
                }
            }
        }
        else
        {
            movements.animator.SetLayerWeight(1, 0f);
            movements.animator.SetLayerWeight(2, 0f);
            pistolGunGO.SetActive(false);
            rifleGunGO.SetActive(false);
            crosshairImage.gameObject.SetActive(false);
            inputsHandler.SetSensitivity(aimSensitivity);
            movements.animator.ResetTrigger("Armed");
            movements.animator.SetTrigger("Unarmed");
        }
        inputsHandler.SetSensitivity(normalSensitivity);
        ainCamera.gameObject.SetActive(inputsHandler.aim);
    }

    private IEnumerator test()
    {
        yield return new WaitForSeconds(0.5f);
        inputsHandler.shoot = false;
    }

    public void ChangedGun()
    {
        actions.changedGunToRifle = !actions.changedGunToRifle;
        if (actions.changedGunToRifle)
        {
            gunTypeText.text = "Rifle Aquired";
        }
        else
        {
            gunTypeText.text = "Pistol Aquired";
        }
    }
}
