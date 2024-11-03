using UnityEngine;

public class Gantry_Gripper_Controller : MonoBehaviour
{

    private SystemManager systemManager;

    public bool Grip;
    public GameObject leftObject; // ���� ������Ʈ
    public GameObject rightObject; // ������ ������Ʈ

    private float moveSpeed = 1f; // ������Ʈ�� �����̴� �ӵ�
    private float openDistance = 0.1f; // �׸��۰� ������ �Ÿ�

    private Vector3 initialLeftPosition;
    private Vector3 initialRightPosition;
    private Vector3 targetLeftPosition;
    private Vector3 targetRightPosition;

    void Start()
    {
        systemManager = FindObjectOfType<SystemManager>();

        if (systemManager == null)
        {
            Debug.LogError("SystemManager not found in the scene.");
        }


        // ������Ʈ���� �ʱ� ��ġ ����
        initialLeftPosition = leftObject.transform.localPosition;
        initialRightPosition = rightObject.transform.localPosition;

        // �ʱ� ��ǥ ��ġ ���� (���� ����)
        targetLeftPosition = initialLeftPosition;
        targetRightPosition = initialRightPosition;
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



    private void RunDigitalTwin()
    {
        controlgripper(systemManager.ReadModbusCoil(1109, 1));

    }

    private void RunSimulation()
    {
        controlgripper(Grip);
    }

    private void RunManualControl()
    {
        controlgripper(Grip);
        systemManager.WriteModbusCoil(209, Grip);
    }




    private void controlgripper(bool isOpen)
    {
        // �׸��� ���¿� ���� ��ǥ ��ġ ����
        if (isOpen)
        {
            targetLeftPosition = initialLeftPosition + Vector3.forward * openDistance;
            targetRightPosition = initialRightPosition + Vector3.back * openDistance;
        }
        else
        {
            targetLeftPosition = initialLeftPosition;
            targetRightPosition = initialRightPosition;
        }

        // ������Ʈ���� ��ǥ ��ġ�� �̵�
        leftObject.transform.localPosition = Vector3.MoveTowards(leftObject.transform.localPosition, targetLeftPosition, moveSpeed * Time.deltaTime);
        rightObject.transform.localPosition = Vector3.MoveTowards(rightObject.transform.localPosition, targetRightPosition, moveSpeed * Time.deltaTime);
    }
}
