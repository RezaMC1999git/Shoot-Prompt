using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    public Animator aimCameraShakeAnimator;

    public void DisableCameraAnimatorEvent()
    {
        aimCameraShakeAnimator.enabled = false;
    }
}
