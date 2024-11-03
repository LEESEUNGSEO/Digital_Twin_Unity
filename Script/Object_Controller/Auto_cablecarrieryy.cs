using UnityEngine;

public class ParentChildOscillateY : MonoBehaviour
{

    private SystemManager systemManager;

    private int intinputvalue;

    public Transform childObject; // �ڽ� ��ü

    // 0���� 100 ������ �Է� ��
    public float inputValue = 0.0f;

    // �θ� ��ü�� �̵��� �� �ִ� Y ��ǥ�� �ּҰ��� �ִ밪
    public float minYPosition = 900.0f;
    public float maxYPosition = 1125.0f;

    // �̵� �ӵ� ���� (�⺻�� 10)
    private float moveSpeed = 1f;

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
        readPos();
    }

    void RunSimulation()
    {
        writeMyPos();
    }

    private void RunManualControl() 
    {
        writeMyPos();
    }
    private void readPos()
    {
        int registerValue = systemManager.ReadModbusRegister(1103, 1);

        switch (registerValue)
        {
            case 0:
                // null ó��
                break;
            case 1:
                // Zero Position
                inputValue = 0;
                break;
            case 2:
                // Clamp Position
                inputValue = 4350;
                break;
            case 3:
                // Clamp up Position
                inputValue = 20000;
                break;
            case int n when (n >= 20 && n <= 28):
                // Storage input position���� 20~28 ó��
                inputValue = Basket_Controller.coordination[n % 10, 1];
                break;
            case int n when (n >= 30 && n <= 38):
                // Storage Unclamp position���� 30~38 ó��
                inputValue = Basket_Controller.coordination[n % 10, 1]-1000;
                break;
            default:
                // ���� ����� ��ġ���� �ʴ� ���
                break;
        }
    



        // inputValue�� 0�� 100 ���̷� ����
        inputValue = Mathf.Clamp(inputValue, 0.0f, 20000f);

        // inputValue�� 951�� 1149 ������ Y ��ġ�� ����
        float newYPosition = Mathf.Lerp(minYPosition, maxYPosition, inputValue / 20000f);

        // �θ� ��ü�� Y ��ġ ����
        Vector3 parentPosition = transform.position;
        parentPosition.y = newYPosition;
        transform.position = Vector3.Lerp(transform.position, parentPosition, moveSpeed * Time.deltaTime);




        // �ڽ� ��ü �̵� (�θ��� �̵��� ���ݸ�ŭ Z ������ �̵�)
        if (childObject != null)
        {
            // �ڽ� ��ü�� Z ��ġ ���
            float childZPosition = Mathf.Lerp(minYPosition, maxYPosition, inputValue / 100.0f) * 0.5f;

            // �ڽ� ��ü�� Z ��ġ ���� (X�� Y�� ����)
            Vector3 childPosition = childObject.position;
            childPosition.z = childZPosition;
            childObject.position = childPosition;
        }
    }


    private void writeMyPos() 
    {
        intinputvalue = (int)inputValue;
        systemManager.WriteModbusRegisters(202, intinputvalue);

        // inputValue�� 0�� 100 ���̷� ����
        inputValue = Mathf.Clamp(inputValue, 0.0f, 20000f);

        // inputValue�� 951�� 1149 ������ Y ��ġ�� ����
        float newYPosition = Mathf.Lerp(minYPosition, maxYPosition, inputValue / 20000f);

        // �θ� ��ü�� Y ��ġ ����
        Vector3 parentPosition = transform.position;
        parentPosition.y = newYPosition;
        transform.position = Vector3.Lerp(transform.position, parentPosition, moveSpeed * Time.deltaTime);




        // �ڽ� ��ü �̵� (�θ��� �̵��� ���ݸ�ŭ Z ������ �̵�)
        if (childObject != null)
        {
            // �ڽ� ��ü�� Z ��ġ ���
            float childZPosition = Mathf.Lerp(minYPosition, maxYPosition, inputValue / 100.0f) * 0.5f;

            // �ڽ� ��ü�� Z ��ġ ���� (X�� Y�� ����)
            Vector3 childPosition = childObject.position;
            childPosition.z = childZPosition;
            childObject.position = childPosition;
        }
    }
}
