using TMPro;
using UnityEngine;

public class Dispose_Cylinder_Controller : MonoBehaviour
{
    private SystemManager systemManager;

    private float speed = 80f; // 실린더 이동 속도
    private float minX = -397.5f; // 실린더가 이동할 수 있는 최소 X값
    private float maxX = -338.3f; // 실린더가 이동할 수 있는 최대 X값
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
        // Modbus에서 값을 읽어와 실린더를 이동시킴
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

    // 실린더를 이동시키는 메서드
    private void MoveCylinder(bool moveForward)
    {
        Vector3 position = transform.position;

        // 이동 방향에 따른 X축 이동 처리
        if (moveForward)
        {
            position.x += speed * Time.deltaTime;
        }
        else
        {
            position.x -= speed * Time.deltaTime;
        }

        // X축 이동 범위 제한
        position.x = Mathf.Clamp(position.x, minX, maxX);

        // 새로운 위치 적용
        transform.position = position;
    }


   




}
