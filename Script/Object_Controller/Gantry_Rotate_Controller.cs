using UnityEngine;
using System.Collections;

public class ConditionalRotateObject : MonoBehaviour
{
    private SystemManager systemManager;
    private Quaternion targetRotation;
    private float rotationSpeed = 150.0f; // 초당 회전 속도 (도 단위)

    public bool currentValue = true; // 기본값을 false로 설정
    private bool previousValue = true; // 이전 값도 false로 설정
    private bool isRotating = false; // 현재 회전 중인지 확인하기 위한 변수

    void Start()
    {
        // 초기 목표 회전 설정
        targetRotation = transform.rotation;

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
        currentValue = !systemManager.ReadModbusCoil(1108, 1);

        if (currentValue != previousValue && !isRotating)
        {
            // 값이 변경되었을 때 회전 시작
            if (currentValue)
            {
                StartCoroutine(RotateCounterClockwise()); // false -> true 변경 시 반시계방향 회전
            }
            else
            {
                StartCoroutine(RotateClockwise()); // true -> false 변경 시 시계방향 회전
            }

            previousValue = currentValue; // 이전 값 업데이트
        }
    }

    void RunSimulation()
    {
        if (currentValue != previousValue && !isRotating)
        {
            // 값이 변경되었을 때 회전 시작
            if (currentValue)
            {
                StartCoroutine(RotateCounterClockwise()); // false -> true 변경 시 반시계방향 회전
            }
            else
            {
                StartCoroutine(RotateClockwise()); // true -> false 변경 시 시계방향 회전
            }

            previousValue = currentValue; // 이전 값 업데이트
        }
    }

    private void RunManualControl()
    {
        if (currentValue != previousValue && !isRotating)
        {
            // 값이 변경되었을 때 회전 시작
            if (currentValue)
            {
                StartCoroutine(RotateCounterClockwise()); // false -> true 변경 시 반시계방향 회전
            }
            else
            {
                StartCoroutine(RotateClockwise()); // true -> false 변경 시 시계방향 회전
            }

            previousValue = currentValue; // 이전 값 업데이트
        }
        systemManager.WriteModbusCoil(208, currentValue);
    }

    private IEnumerator RotateClockwise()
    {
        isRotating = true;
        float angle = 0;
        while (angle < 180)
        {
            float step = rotationSpeed * Time.deltaTime;

            // 남은 각도가 step보다 작을 경우 정확히 180도에 맞춰서 회전
            if (angle + step > 180)
            {
                step = 180 - angle;
            }

            transform.Rotate(0, 0, -step); // 시계방향 회전
            angle += step;
            yield return null;
        }
        isRotating = false;
    }

    private IEnumerator RotateCounterClockwise()
    {
        isRotating = true;
        float angle = 0;
        while (angle < 180)
        {
            float step = rotationSpeed * Time.deltaTime;

            // 남은 각도가 step보다 작을 경우 정확히 180도에 맞춰서 회전
            if (angle + step > 180)
            {
                step = 180 - angle;
            }

            transform.Rotate(0, 0, step); // 반시계방향 회전
            angle += step;
            yield return null;
        }
        isRotating = false;
    }
}
