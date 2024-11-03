using UnityEngine;

public class Stopper2_Controller : MonoBehaviour
{
    private SystemManager systemManager;

    public bool moveforward =true;
    private float speed = 120f; // 실린더 이동 속도
    private float minY = 900f; // 실린더가 이동할 수 있는 최소 X값
    private float maxY = 930f; // 실린더가 이동할 수 있는 최대 X값

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

        // 이동 가능 범위 제한
        position.y = Mathf.Clamp(position.y, minY , maxY);

        // 새로운 위치를 적용
        transform.position = position;
    }
}
