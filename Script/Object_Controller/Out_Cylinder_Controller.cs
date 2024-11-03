using UnityEngine;

public class Out_Cylinder_Controller : MonoBehaviour
{
    private SystemManager systemManager;

    public bool currentValue = false; // 읽어올 특정 값

    private float speed = 250f; // 실린더 이동 속도
    private float maxDistance = 25f; // 실린더가 이동할 수 있는 최대 거리

    private Vector3 startPosition; // 시작 위치

    void Start()
    {
        systemManager = FindObjectOfType<SystemManager>();

        if (systemManager == null)
        {
            Debug.LogError("SystemManager not found in the scene.");
        }

        startPosition = transform.localPosition; // 시작 위치 저장
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
        // 이동할 방향 및 속도 계산
        float moveAmount = speed * Time.deltaTime;

        if (moveForward)
        {
            // 로컬 Z축 기준 앞으로 이동
            transform.Translate(0, 0, -moveAmount, Space.Self);
        }
        else
        {
            // 로컬 Z축 기준 뒤로 이동
            transform.Translate(0, 0, moveAmount, Space.Self);
        }

        // 이동 가능한 거리를 제한하기 위해 현재 위치 가져오기
        Vector3 currentPosition = transform.localPosition;

        // 시작 위치로부터의 이동 거리를 계산
        float distanceFromStart = Vector3.Distance(startPosition, currentPosition);

        // 이동 가능한 최대 거리 제한 적용
        if (distanceFromStart > maxDistance)
        {
            // 최대 거리를 넘어갔을 경우 위치를 보정
            Vector3 direction = (currentPosition - startPosition).normalized; // 이동 방향 계산
            transform.localPosition = startPosition + direction * maxDistance; // 최대 거리만큼 위치 재설정
        }
    }
}
