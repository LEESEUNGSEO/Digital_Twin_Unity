using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CablecarrierXY_UI_controller : MonoBehaviour
{
    private SystemManager systemManager;

    public GameObject obj_GantryX;
    public GameObject obj_GantryY;
    public Text xValueText; // X�� �� ǥ�� �ؽ�Ʈ
    public Text yValueText; // Y�� �� ǥ�� �ؽ�Ʈ

    // +, - ��ư ���� �߰�
    public Button xIncreaseButton;
    public Button xDecreaseButton;
    public Button yIncreaseButton;
    public Button yDecreaseButton;

    private float xstep = 10000f; // ��ư�� ������ �� ����� ��
    private float step = 1000f; // ��ư�� ������ �� ����� ��
    private bool isXIncreasing = false;
    private bool isXDecreasing = false;
    private bool isYIncreasing = false;
    private bool isYDecreasing = false;

    void Start()
    {
        systemManager = FindObjectOfType<SystemManager>();

        if (systemManager == null)
        {
            Debug.LogError("SystemManager not found in the scene.");
        }

        obj_GantryX = GameObject.Find("Xaxis_moving_object");
        obj_GantryY = GameObject.Find("Yaxis_moving_object");

        xValueText.text = systemManager.ReadModbusRegister(1101,1).ToString();
        yValueText.text = systemManager.ReadModbusRegister(1102, 1).ToString();

        // EventTrigger ����
        AddEventTrigger(xIncreaseButton.gameObject, EventTriggerType.PointerDown, (e) => { isXIncreasing = true; systemManager.WriteModbusCoil(210, true); });
        AddEventTrigger(xIncreaseButton.gameObject, EventTriggerType.PointerUp, (e) => { isXIncreasing = false; systemManager.WriteModbusCoil(210, false); });

        AddEventTrigger(xDecreaseButton.gameObject, EventTriggerType.PointerDown, (e) => { isXDecreasing = true; systemManager.WriteModbusCoil(211, true); });
        AddEventTrigger(xDecreaseButton.gameObject, EventTriggerType.PointerUp, (e) => { isXDecreasing = false; systemManager.WriteModbusCoil(211, false); });

        AddEventTrigger(yIncreaseButton.gameObject, EventTriggerType.PointerDown, (e) => { isYIncreasing = true; systemManager.WriteModbusCoil(212, true); });
        AddEventTrigger(yIncreaseButton.gameObject, EventTriggerType.PointerUp, (e) => { isYIncreasing = false; systemManager.WriteModbusCoil(212, false); });

        AddEventTrigger(yDecreaseButton.gameObject, EventTriggerType.PointerDown, (e) => { isYDecreasing = true; systemManager.WriteModbusCoil(213, true); });
        AddEventTrigger(yDecreaseButton.gameObject, EventTriggerType.PointerUp, (e) => { isYDecreasing = false; systemManager.WriteModbusCoil(213 , false); });
    }

    void Update()
    {

        //��ư�� ������ ���¿��� X, Y�� ����
        if (isXIncreasing)
        {
            UpdatePositionX(obj_GantryX.GetComponent<ParentChildOscillateZ>().inputValue - xstep * Time.deltaTime);
        }
        if (isXDecreasing)
        {
            UpdatePositionX(obj_GantryX.GetComponent<ParentChildOscillateZ>().inputValue + xstep * Time.deltaTime);
        }
        if (isYIncreasing)
        {
            UpdatePositionY(obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue - step * Time.deltaTime *1);
        }
        if (isYDecreasing)
        {
            UpdatePositionY(obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue + step * Time.deltaTime);
        }
    }

    void UpdatePositionX(float value)
    {
        obj_GantryX.GetComponent<ParentChildOscillateZ>().inputValue = value;
        xValueText.text = value.ToString("F1"); // �Ҽ��� �� �ڸ����� ǥ��
    }

    void UpdatePositionY(float value)
    {
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = value;
        yValueText.text = value.ToString("F1"); // �Ҽ��� �� �ڸ����� ǥ��
    }



    private void AddEventTrigger(GameObject target, EventTriggerType triggerType, System.Action<BaseEventData> action)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null) trigger = target.AddComponent<EventTrigger>();

        var eventTriggerEntry = new EventTrigger.Entry { eventID = triggerType };
        eventTriggerEntry.callback.AddListener((data) => { action(data); });
        trigger.triggers.Add(eventTriggerEntry);
    }
}
