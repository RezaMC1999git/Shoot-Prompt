using UnityEngine;

public class Movements : MonoBehaviour
{
    public CharacterController characterController;
    public float movementSpeed, runningSpeed, turnSmoothTime = 0.1f;
    public Animator animator;
    public Actions actions;
    public Transform cameraTransform;
    public InputsHandler inputsHandler;

    float horizontal, vertical, targerAngle, angle, turnSmoothVelocity;
    Vector3 directions;

    private void Start()
    {
        runningSpeed = movementSpeed * 2;
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        directions = new Vector3(horizontal, 0f, vertical).normalized;

        if (directions.magnitude >= 0.1f && !inputsHandler.isInPromptMode)
        {
            targerAngle = Mathf.Atan2(directions.x, directions.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targerAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDirections = Quaternion.Euler(0f, targerAngle, 0f) * Vector3.forward;
            if (Input.GetKey(KeyCode.LeftShift))
            {   
                characterController.Move(moveDirections.normalized * runningSpeed * Time.deltaTime);
                animator.SetBool("Run", true);
            }
            else 
            {
                characterController.Move(moveDirections.normalized * movementSpeed * Time.deltaTime);
                animator.SetBool("Run", false);
                animator.SetBool("Walk", true);
            }

            //Vector3 move = transform.right * horizontal + transform.forward * vertical;
            //characterController.Move(move * movementSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
        }
    }
}
