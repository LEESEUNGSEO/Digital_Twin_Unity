using UnityEngine;
using System.Collections;

public class Gantry_ForwardBackward_Controller : MonoBehaviour
{
    private SystemManager systemManager;

    private float moveSpeed = 120f; // 실린더 이동 속도
    private float minX = 0f; // 실린더가 이동할 수 있는 최소 X값
    private float maxX = 0.85f; // 실린더가 이동할 수 있는 최대 X값
    public bool moveForward = false; // 전진 여부를 결정하는 변수

    private float initialX; // 초기 X 위치

    void Start()
    {
        systemManager = FindObjectOfType<SystemManager>();

        if (systemManager == null)
        {
            Debug.LogError("SystemManager not found in the scene.");
        }

        initialX = transform.localPosition.x; // 초기 위치 저장

        
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
        // 로컬 좌표계에서 이동 처리
        float direction = moveForward ? 1 : -1;
        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);

        // 로컬 좌표계를 기준으로 위치 제한
        Vector3 localPosition = transform.localPosition;
        localPosition.x = Mathf.Clamp(localPosition.x, initialX + minX, initialX + maxX);

        // 제한된 위치를 다시 적용
        transform.localPosition = localPosition;
    }
   

}
