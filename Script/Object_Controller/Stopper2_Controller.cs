using UnityEngine;

public class Stopper2_Controller : MonoBehaviour
{
    private SystemManager systemManager;

    public bool moveforward =true;
    private float speed = 120f; // �Ǹ��� �̵� �ӵ�
    private float minY = 900f; // �Ǹ����� �̵��� �� �ִ� �ּ� X��
    private float maxY = 930f; // �Ǹ����� �̵��� �� �ִ� �ִ� X��

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
        controlstopper1(systemManager.ReadModbusCoil(1104, 1));
    }



    void RunSimulation()
    {
        if (moveforward)
        {
            controlstopper1(true);
        }
        else 
        {
            controlstopper1(false);
        }
    }

    void RunManualControl()
    {
        controlstopper1(moveforward);
        systemManager.WriteModbusCoil(204,moveforward);
    }

    private void controlstopper1(bool moveForward)
    {
        
        Vector3 position = transform.position;

        if (moveForward)
        {
            position.y -= speed * Time.deltaTime;
        }
        else
        {
            position.y += speed * Time.deltaTime;
        }

        // �̵� ���� ���� ����
        position.y = Mathf.Clamp(position.y, minY , maxY);

        // ���ο� ��ġ�� ����
        transform.position = position;
    }
}
