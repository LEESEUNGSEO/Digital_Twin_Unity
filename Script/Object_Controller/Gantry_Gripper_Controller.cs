using UnityEngine;

public class Gantry_Gripper_Controller : MonoBehaviour
{

    private SystemManager systemManager;

    public bool Grip;
    public GameObject leftObject; // 왼쪽 오브젝트
    public GameObject rightObject; // 오른쪽 오브젝트

    private float moveSpeed = 1f; // 오브젝트가 움직이는 속도
    private float openDistance = 0.1f; // 그리퍼가 벌어질 거리

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


        // 오브젝트들의 초기 위치 저장
        initialLeftPosition = leftObject.transform.localPosition;
        initialRightPosition = rightObject.transform.localPosition;

        // 초기 목표 위치 설정 (닫힌 상태)
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
        // 그리퍼 상태에 따라 목표 위치 설정
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

        // 오브젝트들을 목표 위치로 이동
        leftObject.transform.localPosition = Vector3.MoveTowards(leftObject.transform.localPosition, targetLeftPosition, moveSpeed * Time.deltaTime);
        rightObject.transform.localPosition = Vector3.MoveTowards(rightObject.transform.localPosition, targetRightPosition, moveSpeed * Time.deltaTime);
    }
}
