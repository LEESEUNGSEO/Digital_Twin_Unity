using UnityEngine;

public class ParentChildOscillateZ : MonoBehaviour
{
    private SystemManager systemManager;

    private int intinputvalue;

    public Transform childObject; // �ڽ� ��ü�� �̵���ų �ڽ� ��ü

    // 0���� 100 ������ �Է� ��
    public float inputValue = 0.0f;

    // �θ� ��ü�� �̵��� �� �ִ� Z ��ǥ�� �ּҰ��� �ִ밪
    public float minZPosition = 1185.0f;
    public float maxZPosition = 970.0f;    

    // �̵� �ӵ�
    private float moveSpeed = 90.0f;

    private float targetZPosition;
    private Vector3 targetChildPosition;

    void Start()
    {

        systemManager = FindObjectOfType<SystemManager>();

        if (systemManager == null)
        {
            Debug.LogError("SystemManager not found in the scene.");
        }
        
        // �ʱ� ��ǥ ��ġ ����
        targetZPosition = transform.position.z;
        if (childObject != null)
        {
            targetChildPosition = childObject.position;
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
                inputValue = 95167;
                break;
            case 3:
                // Clamp up Position
                inputValue = 95167;
                break;
            case int n when (n >= 20 && n <= 38):
                inputValue = Basket_Controller.coordination[n % 10, 0];
                break;
            default:
                // ���� ����� ��ġ���� �ʴ� ���
                break;
        }
        inputValue = Mathf.Clamp(inputValue, 0.0f, 247280.0f);

        // �Է� ���� 0~100 �������� minZPosition~maxZPosition ������ ����
        float newZPosition = Mathf.Lerp(minZPosition, maxZPosition, inputValue / 247280.0f);

        // �θ� ��ü�� Z ��ġ�� �̵� �ӵ��� ���� �����ϰ� �̵�
        Vector3 currentPosition = transform.position;
        transform.position = Vector3.MoveTowards(currentPosition, new Vector3(currentPosition.x, currentPosition.y, newZPosition), moveSpeed * Time.deltaTime);

        // �ڽ� ��ü �̵� (�θ� �̵��� ���ݸ�ŭ Z������ �̵�)
        if (childObject != null)
        {
            // �θ��� Z �̵��� ���� �ڽ��� Z ��ġ ���
            float childZPosition = Mathf.Lerp(minZPosition, maxZPosition, inputValue / 100.0f) * 0.5f;
            Vector3 currentChildPosition = childObject.position;
            childObject.position = Vector3.MoveTowards(currentChildPosition, new Vector3(currentChildPosition.x, currentChildPosition.y, childZPosition), moveSpeed * Time.deltaTime * 0.5f);
        }
    }


        void writeMyPos()
    {
        intinputvalue = (int)inputValue;
        systemManager.WriteModbusRegisters(201, intinputvalue);

        // �Է� ���� 0�� 100 ���̷� ����
        inputValue = Mathf.Clamp(inputValue, 0.0f, 247280.0f);

        // �Է� ���� 0~100 �������� minZPosition~maxZPosition ������ ����
        float newZPosition = Mathf.Lerp(minZPosition, maxZPosition, inputValue / 247280.0f);

        // �θ� ��ü�� Z ��ġ�� �̵� �ӵ��� ���� �����ϰ� �̵�
        Vector3 currentPosition = transform.position;
        transform.position = Vector3.MoveTowards(currentPosition, new Vector3(currentPosition.x, currentPosition.y, newZPosition), moveSpeed * Time.deltaTime);

        // �ڽ� ��ü �̵� (�θ� �̵��� ���ݸ�ŭ Z������ �̵�)
        if (childObject != null)
        {
            // �θ��� Z �̵��� ���� �ڽ��� Z ��ġ ���
            float childZPosition = Mathf.Lerp(minZPosition, maxZPosition, inputValue / 100.0f) * 0.5f;
            Vector3 currentChildPosition = childObject.position;
            childObject.position = Vector3.MoveTowards(currentChildPosition, new Vector3(currentChildPosition.x, currentChildPosition.y, childZPosition), moveSpeed * Time.deltaTime * 0.5f);
        }
    }
}
