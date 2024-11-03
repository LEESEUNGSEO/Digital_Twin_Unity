using UnityEngine;
using System.Collections;

public class Gantry_ForwardBackward_Controller : MonoBehaviour
{
    private SystemManager systemManager;

    private float moveSpeed = 120f; // �Ǹ��� �̵� �ӵ�
    private float minX = 0f; // �Ǹ����� �̵��� �� �ִ� �ּ� X��
    private float maxX = 0.85f; // �Ǹ����� �̵��� �� �ִ� �ִ� X��
    public bool moveForward = false; // ���� ���θ� �����ϴ� ����

    private float initialX; // �ʱ� X ��ġ

    void Start()
    {
        systemManager = FindObjectOfType<SystemManager>();

        if (systemManager == null)
        {
            Debug.LogError("SystemManager not found in the scene.");
        }

        initialX = transform.localPosition.x; // �ʱ� ��ġ ����

        
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
        moveForward = systemManager.ReadModbusCoil(1107, 1);
        controlgantryforward(moveForward);
       
    }

    void RunSimulation()
    {
        if(moveForward){
            controlgantryforward(moveForward);
        }
        else
        {
            controlgantryforward(moveForward);
        }
    }

    private void RunManualControl()
    {
        controlgantryforward(moveForward);
        systemManager.WriteModbusCoil(207, moveForward);
    }

    private void controlgantryforward(bool moveForward)
    {
        // ���� ��ǥ�迡�� �̵� ó��
        float direction = moveForward ? 1 : -1;
        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);

        // ���� ��ǥ�踦 �������� ��ġ ����
        Vector3 localPosition = transform.localPosition;
        localPosition.x = Mathf.Clamp(localPosition.x, initialX + minX, initialX + maxX);

        // ���ѵ� ��ġ�� �ٽ� ����
        transform.localPosition = localPosition;
    }
   

}
