using UnityEngine;

public class Stopper1_Controller : MonoBehaviour
{
    private SystemManager systemManager;

    public bool currentValue = true; // �о�� Ư�� ��

    private float speed = 120f; // �Ǹ��� �̵� �ӵ�
    private float minZ = 527f; // �Ǹ����� �̵��� �� �ִ� �ּ� X��
    private float maxZ = 552f; // �Ǹ����� �̵��� �� �ִ� �ִ� X��



    public Camera secondCamera;



    void Start()
    {
        secondCamera = GameObject.FindGameObjectWithTag("SecondCamera").GetComponent<Camera>();

        systemManager = FindObjectOfType<SystemManager>();

        if (systemManager == null)
        {
            Debug.LogError("SystemManager not found in the scene.");
        }

        if (secondCamera != null)
        {
            secondCamera.enabled = false;
        }
        else
        {
            Debug.LogError("Second Camera�� ã�� �� �����ϴ�. �±׸� Ȯ���ϼ���.");
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


    void RunDigitalTwin()
    {
        controlstopper1(systemManager.ReadModbusCoil(1103, 1));
    }



    void RunSimulation()
    {
        if (currentValue)
        {
            controlstopper1(true);
            secondCamera.enabled = true;
        }
        else {
            controlstopper1(false);
            secondCamera.enabled = false;
        }
    }



    private void RunManualControl()
    {
        controlstopper1(currentValue);
        systemManager.WriteModbusCoil(203, currentValue);
    }

    private void controlstopper1(bool moveForward)
    {
        
        Vector3 position = transform.position;

        // ����/���� ���ο� ���� X�� �������� �̵�
        if (moveForward)
        {
            position.z += speed * Time.deltaTime;
        }
        else
        {
            position.z -= speed * Time.deltaTime;
        }

        // �̵� ���� ���� ����
        position.z = Mathf.Clamp(position.z, minZ, maxZ);

        // ���ο� ��ġ�� ����
        transform.position = position;
    }
}
