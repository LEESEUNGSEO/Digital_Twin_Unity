using UnityEngine;
using UnityEngine.UIElements;

public class FreeCameraController : MonoBehaviour
{
    private float moveSpeed = 300f; // 이동 속도
    private float lookSpeed = 1f; // 시점 회전 속도
    private float maxLookX = 80f; // 상하 시점 제한

    public Vector3 defultPosiotion ;
    public Quaternion defultRotation ;


    public Vector3 targetPosition;
    public Quaternion targetRotation;
    public Transform targetObject =null; // 카메라가 바라볼 목표 오브젝트

    private float rotX = 0f; // 상하 회전 각도
    private SystemManager systemManager;

    void Start()
    {
        defultPosiotion =this.transform.position;
        defultRotation = this.transform.rotation;

        systemManager = FindObjectOfType<SystemManager>();

        if (systemManager == null)
        {
            Debug.LogError("SystemManager not found in the scene.");
        }
    }

    void Update()
    {
        switch (systemManager.CurrentMode)
        {
            case OperationMode.DigitalTwinMode:
                RunDigitalTwin();
                break;

            case OperationMode.SimulationMode:
                RunSimulation();
                break;

            case OperationMode.ManualMode:
                RunManualControl();
                break;
        }
    }

    private float M_moveSpeed = 1f; // 카메라 이동 속도
    private float M_rotateSpeed = 1.5f; // 카메라 회전 속도

    void RunDigitalTwin()
    {
        Freecamcontrol();
    }

    void RunSimulation()
    {
        Freecamcontrol();
    }

    private void RunManualControl()
    {
        if (targetObject != null)
        {
            Debug.Log(targetObject);
            // 목표 오브젝트의 방향을 계산하여 그 방향을 바라보는 회전 설정 
            Vector3 direction = targetObject.position - transform.position;
            targetRotation = Quaternion.LookRotation(direction);

            ManualCamControl(targetPosition, targetRotation);
        }
        else
        {
            ManualCamControl();
        }
    }

    void Freecamcontrol()
    {
        // 목표 위치 계산
        Vector3 moveDirection = Vector3.zero;
        float moveForward = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float moveRight = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveUp = 0f;

        if (Input.GetKey(KeyCode.E))
            moveUp = moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q))
            moveUp = -moveSpeed * Time.deltaTime;

        moveDirection = transform.right * moveRight + transform.up * moveUp + transform.forward * moveForward;

        // 위치 이동
        transform.position += moveDirection;

        // 마우스 왼쪽 버튼이 클릭된 상태에서만 카메라 회전
        if (Input.GetMouseButton(0)) // 0은 왼쪽 마우스 버튼
        {
            rotX -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotX = Mathf.Clamp(rotX, -maxLookX, maxLookX);

            float rotY = Input.GetAxis("Mouse X") * lookSpeed;

            // 회전 적용
            transform.localRotation = Quaternion.Euler(rotX, transform.localRotation.eulerAngles.y + rotY, 0);
        }
    }

    public void ManualCamControl(Vector3? targetPosition = null, Quaternion? targetRotation = null)
    {
        // targetPosition이나 targetRotation이 null이면 기본값 사용
        Vector3 finalPosition = targetPosition ?? defultPosiotion;
        Quaternion finalRotation = targetRotation ?? defultRotation;

        // 현재 위치와 목표 위치 사이를 moveSpeed 속도로 이동
        transform.position = Vector3.Lerp(transform.position, finalPosition, M_moveSpeed * Time.deltaTime);

        // 목표 회전 방향으로 부드럽게 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, M_rotateSpeed * Time.deltaTime);
    }

}
