using TMPro;
using UnityEngine;

public class Dispose_Cylinder_Controller : MonoBehaviour
{
    private SystemManager systemManager;

    private float speed = 80f; // �Ǹ��� �̵� �ӵ�
    private float minX = -397.5f; // �Ǹ����� �̵��� �� �ִ� �ּ� X��
    private float maxX = -338.3f; // �Ǹ����� �̵��� �� �ִ� �ִ� X��
    public bool moveForward;

    public Vector3 targetPosition = new Vector3(0, 5, 0);

    void Start()
    {

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

    void RunDigitalTwin()
    {
        // Modbus���� ���� �о�� �Ǹ����� �̵���Ŵ
        moveForward = systemManager.ReadModbusCoil(1106, 1);
        MoveCylinder(moveForward);
    }

    void RunSimulation()
    {
        MoveCylinder(moveForward);
    }

    private void RunManualControl()
    {
        MoveCylinder(moveForward);
        systemManager.WriteModbusCoil(206, moveForward);
    }

    // �Ǹ����� �̵���Ű�� �޼���
    private void MoveCylinder(bool moveForward)
    {
        Vector3 position = transform.position;

        // �̵� ���⿡ ���� X�� �̵� ó��
        if (moveForward)
        {
            position.x += speed * Time.deltaTime;
        }
        else
        {
            position.x -= speed * Time.deltaTime;
        }

        // X�� �̵� ���� ����
        position.x = Mathf.Clamp(position.x, minX, maxX);

        // ���ο� ��ġ ����
        transform.position = position;
    }


   




}
