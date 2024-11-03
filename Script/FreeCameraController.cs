using UnityEngine;
using UnityEngine.UIElements;

public class FreeCameraController : MonoBehaviour
{
    private float moveSpeed = 300f; // �̵� �ӵ�
    private float lookSpeed = 1f; // ���� ȸ�� �ӵ�
    private float maxLookX = 80f; // ���� ���� ����

    public Vector3 defultPosiotion ;
    public Quaternion defultRotation ;


    public Vector3 targetPosition;
    public Quaternion targetRotation;
    public Transform targetObject =null; // ī�޶� �ٶ� ��ǥ ������Ʈ

    private float rotX = 0f; // ���� ȸ�� ����
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

    private float M_moveSpeed = 1f; // ī�޶� �̵� �ӵ�
    private float M_rotateSpeed = 1.5f; // ī�޶� ȸ�� �ӵ�

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
            // ��ǥ ������Ʈ�� ������ ����Ͽ� �� ������ �ٶ󺸴� ȸ�� ���� 
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
        // ��ǥ ��ġ ���
        Vector3 moveDirection = Vector3.zero;
        float moveForward = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float moveRight = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveUp = 0f;

        if (Input.GetKey(KeyCode.E))
            moveUp = moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q))
            moveUp = -moveSpeed * Time.deltaTime;

        moveDirection = transform.right * moveRight + transform.up * moveUp + transform.forward * moveForward;

        // ��ġ �̵�
        transform.position += moveDirection;

        // ���콺 ���� ��ư�� Ŭ���� ���¿����� ī�޶� ȸ��
        if (Input.GetMouseButton(0)) // 0�� ���� ���콺 ��ư
        {
            rotX -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotX = Mathf.Clamp(rotX, -maxLookX, maxLookX);

            float rotY = Input.GetAxis("Mouse X") * lookSpeed;

            // ȸ�� ����
            transform.localRotation = Quaternion.Euler(rotX, transform.localRotation.eulerAngles.y + rotY, 0);
        }
    }

    public void ManualCamControl(Vector3? targetPosition = null, Quaternion? targetRotation = null)
    {
        // targetPosition�̳� targetRotation�� null�̸� �⺻�� ���
        Vector3 finalPosition = targetPosition ?? defultPosiotion;
        Quaternion finalRotation = targetRotation ?? defultRotation;

        // ���� ��ġ�� ��ǥ ��ġ ���̸� moveSpeed �ӵ��� �̵�
        transform.position = Vector3.Lerp(transform.position, finalPosition, M_moveSpeed * Time.deltaTime);

        // ��ǥ ȸ�� �������� �ε巴�� ȸ��
        transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, M_rotateSpeed * Time.deltaTime);
    }

}
