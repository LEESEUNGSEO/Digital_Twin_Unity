using UnityEngine;
using System.Collections;

public class ConditionalRotateObject : MonoBehaviour
{
    private SystemManager systemManager;
    private Quaternion targetRotation;
    private float rotationSpeed = 150.0f; // �ʴ� ȸ�� �ӵ� (�� ����)

    public bool currentValue = true; // �⺻���� false�� ����
    private bool previousValue = true; // ���� ���� false�� ����
    private bool isRotating = false; // ���� ȸ�� ������ Ȯ���ϱ� ���� ����

    void Start()
    {
        // �ʱ� ��ǥ ȸ�� ����
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
            // ���� ����Ǿ��� �� ȸ�� ����
            if (currentValue)
            {
                StartCoroutine(RotateCounterClockwise()); // false -> true ���� �� �ݽð���� ȸ��
            }
            else
            {
                StartCoroutine(RotateClockwise()); // true -> false ���� �� �ð���� ȸ��
            }

            previousValue = currentValue; // ���� �� ������Ʈ
        }
    }

    void RunSimulation()
    {
        if (currentValue != previousValue && !isRotating)
        {
            // ���� ����Ǿ��� �� ȸ�� ����
            if (currentValue)
            {
                StartCoroutine(RotateCounterClockwise()); // false -> true ���� �� �ݽð���� ȸ��
            }
            else
            {
                StartCoroutine(RotateClockwise()); // true -> false ���� �� �ð���� ȸ��
            }

            previousValue = currentValue; // ���� �� ������Ʈ
        }
    }

    private void RunManualControl()
    {
        if (currentValue != previousValue && !isRotating)
        {
            // ���� ����Ǿ��� �� ȸ�� ����
            if (currentValue)
            {
                StartCoroutine(RotateCounterClockwise()); // false -> true ���� �� �ݽð���� ȸ��
            }
            else
            {
                StartCoroutine(RotateClockwise()); // true -> false ���� �� �ð���� ȸ��
            }

            previousValue = currentValue; // ���� �� ������Ʈ
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

            // ���� ������ step���� ���� ��� ��Ȯ�� 180���� ���缭 ȸ��
            if (angle + step > 180)
            {
                step = 180 - angle;
            }

            transform.Rotate(0, 0, -step); // �ð���� ȸ��
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

            // ���� ������ step���� ���� ��� ��Ȯ�� 180���� ���缭 ȸ��
            if (angle + step > 180)
            {
                step = 180 - angle;
            }

            transform.Rotate(0, 0, step); // �ݽð���� ȸ��
            angle += step;
            yield return null;
        }
        isRotating = false;
    }
}
