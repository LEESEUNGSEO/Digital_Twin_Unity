using UnityEngine;

public class Out_Cylinder_Controller : MonoBehaviour
{
    private SystemManager systemManager;

    public bool currentValue = false; // �о�� Ư�� ��

    private float speed = 250f; // �Ǹ��� �̵� �ӵ�
    private float maxDistance = 25f; // �Ǹ����� �̵��� �� �ִ� �ִ� �Ÿ�

    private Vector3 startPosition; // ���� ��ġ

    void Start()
    {
        systemManager = FindObjectOfType<SystemManager>();

        if (systemManager == null)
        {
            Debug.LogError("SystemManager not found in the scene.");
        }

        startPosition = transform.localPosition; // ���� ��ġ ����
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
        controlstopper1(systemManager.ReadModbusCoil(1105, 1));
    }

    void RunSimulation()
    {
        controlstopper1(currentValue);
    }

    private void RunManualControl()
    {
        controlstopper1(currentValue);
        systemManager.WriteModbusCoil(205, currentValue);
    }





    private void controlstopper1(bool moveForward)
    {
        // �̵��� ���� �� �ӵ� ���
        float moveAmount = speed * Time.deltaTime;

        if (moveForward)
        {
            // ���� Z�� ���� ������ �̵�
            transform.Translate(0, 0, -moveAmount, Space.Self);
        }
        else
        {
            // ���� Z�� ���� �ڷ� �̵�
            transform.Translate(0, 0, moveAmount, Space.Self);
        }

        // �̵� ������ �Ÿ��� �����ϱ� ���� ���� ��ġ ��������
        Vector3 currentPosition = transform.localPosition;

        // ���� ��ġ�κ����� �̵� �Ÿ��� ���
        float distanceFromStart = Vector3.Distance(startPosition, currentPosition);

        // �̵� ������ �ִ� �Ÿ� ���� ����
        if (distanceFromStart > maxDistance)
        {
            // �ִ� �Ÿ��� �Ѿ�� ��� ��ġ�� ����
            Vector3 direction = (currentPosition - startPosition).normalized; // �̵� ���� ���
            transform.localPosition = startPosition + direction * maxDistance; // �ִ� �Ÿ���ŭ ��ġ �缳��
        }
    }
}
