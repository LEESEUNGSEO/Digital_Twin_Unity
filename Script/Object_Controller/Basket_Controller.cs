using System.Collections;
using System.Runtime.Serialization;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Basket_Controller : MonoBehaviour
{
    public GameObject obj_stopper0;
    public GameObject obj_stopper1;
    public GameObject obj_stopper2;
    public GameObject obj_GantryX;
    public GameObject obj_GantryY;
    public GameObject obj_Gantry_ROT;
    public GameObject obj_Gantry_FB;
    public GameObject obj_Gantry_CLP;
    public GameObject obj_Gantry_OUT;

    private SystemManager systemManager;

    private bool isGrabbed = false;
    private bool hasGrabbed= false;

    public Vector3[] points; // �̵��� ��ǥ��
    private float speed = 150f; // �̵� �ӵ�

    private int currentPointIndex = 0; // ���� ��ǥ ����Ʈ �ε���
    private bool hasReachedEnd = false; // ��� ����Ʈ�� �����ߴ��� ����
    private BoxCollider boxCollider; // Ȱ��ȭ�� BoxCollider

    public GameObject prefabToSpawn; // ������ �������� ���� ����
    private Rigidbody Rigid;

    public float pushForce = 5.0f; // �浹 �� ��ü�� ���� ��

    private bool isPaused = false; // �Ͻ� ���� ���� Ȯ�� ����


    private int containlocate = 0;

    Vector3 targetDisposeXPosition1 = new Vector3(0, 0, 340);
    Vector3 targetDisposeYPosition2 = new Vector3(120, -40, 341);



    private bool[] simulation_contains = new bool[9];
    public static int[,] coordination = new int[,]
    {
        { 240302,19526 },  // ù ��° ��
        { 127946, 19526 },   // �� ��° ��
        { 330, 19526 },
        { 240302, 10851 },
        { 127946, 10851 },
        { 330, 10851 },
        { 240302, 1860 },
        { 127946, 1860 },
        { 330, 1860 },

    };


    

    void Start()
    {
        Debug.Log("�ȳ�翡��");
        obj_stopper0 = GameObject.Find("Stopper_0-1");
        obj_stopper1 = GameObject.Find("Stopper_1-1");
        obj_stopper2 = GameObject.Find("Stopper_2-1");
        obj_GantryX = GameObject.Find("Xaxis_moving_object");
        obj_GantryY = GameObject.Find("Yaxis_moving_object");
        obj_Gantry_ROT = GameObject.Find("Gantry_rotator");
        obj_Gantry_FB = GameObject.Find("Gantry_Forward/Backward");
        obj_Gantry_CLP = GameObject.Find("Gantry_gripper");
        obj_Gantry_OUT = GameObject.Find("Out_Cylinder");


        systemManager = FindObjectOfType<SystemManager>();
        // �ڱ� �ڽ��� BoxCollider�� ã��
        boxCollider = GetComponent<BoxCollider>();
        Rigid = GetComponent<Rigidbody>();


        for (int i = 0; i < simulation_contains.Length; i++)
        {
            simulation_contains[i] = false;
        }

        systemManager.WriteModbusCoil(6960, true);

    }

    void Update()
    {
        if (isPaused) return; // �Ͻ� ���� ���� ���� Update ���� ����

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
       
        Debug.Log("�׸��۱׸��۱׸��� : " + systemManager.ReadModbusCoil(1109, 1));
        if (systemManager.ReadModbusCoil(1109, 1) && !hasGrabbed)
        {
            this.transform.SetParent(obj_Gantry_CLP.transform);// ���� ��ü�� �׸��ۿ� ����
            isGrabbed = true; // ���� ���·� ����
        }
        else 
        {
            Vector3 originalPosition = this.transform.position;
            Quaternion originalRotation = this.transform.rotation;

            // �θ� ���� ����
            this.transform.SetParent(null);

            // �θ� ���� ���� �� ���� ��ġ�� ȸ�� �� ����
            this.transform.position = originalPosition;
            this.transform.rotation = originalRotation;
            if(isGrabbed) hasGrabbed = true;
            
        }
            



        if (hasReachedEnd || points.Length == 0)
            return;

        // ���� ��ǥ ��ǥ�� �̵�
        Vector3 targetPoint = points[currentPointIndex];
        Vector3 direction = targetPoint - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        // ��ü�� ��ǥ ��ǥ �������� �̵�
        if (direction.magnitude <= distanceThisFrame)
        {
            transform.position = targetPoint; // ��ǥ ��ǥ�� ����

            if (currentPointIndex == 0)
            {
                if (!isPaused)
                {
                    StartCoroutine(PauseAndWaitForModbusCoil()); // �ڷ�ƾ ����
                }
            }
            else
            {
                currentPointIndex++;
            }

            // ��� ����Ʈ�� ��ȸ�ߴ��� Ȯ��
            if (currentPointIndex >= points.Length)
            {
                obj_Gantry_OUT.GetComponent<Out_Cylinder_Controller>().currentValue = false;
                hasReachedEnd = true;
            }
        }
        else
        {
            transform.position += direction.normalized * distanceThisFrame; // �̵�
        }

    }

    void RunSimulation()
    {
        if (hasReachedEnd || points.Length == 0)
            return;
            
        // ���� ��ǥ ��ǥ�� �̵�
        Vector3 targetPoint = points[currentPointIndex];
        Vector3 direction = targetPoint - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        // ��ü�� ��ǥ ��ǥ �������� �̵�
        if (direction.magnitude <= distanceThisFrame)
        {
            transform.position = targetPoint; // ��ǥ ��ǥ�� ����


            if (currentPointIndex == 0)
            {
                StartCoroutine(Pause1sec());

            }
            else if(currentPointIndex == 1)
            {
                obj_Gantry_OUT.GetComponent<Out_Cylinder_Controller>().currentValue = true;
                currentPointIndex++;
            }
            else
            {
                currentPointIndex++;
            }

            // ��� ����Ʈ�� ��ȸ�ߴ��� Ȯ��
            if (currentPointIndex >= points.Length)
            {
                obj_Gantry_OUT.GetComponent<Out_Cylinder_Controller>().currentValue = false;
                hasReachedEnd = true;

                StartCoroutine(IncomingSequence(systemManager.incomingitem));
                systemManager.incomingitem = 0;
            }
        }
        else
        {
            transform.position += direction.normalized * distanceThisFrame; // �̵�
        }
    }

    void RunManualControl()
    {
        //�޴����忡���� X
    }



    IEnumerator PauseAndWaitForModbusCoil()
    {
        isPaused = true;

        // 6960�� coil�� true���� ��� Ȯ��
        while (systemManager.ReadModbusCoil(1103, 1))
        {
            yield return null; // �� �����Ӹ��� Ȯ��
        }

        // 6960�� coil�� false�� �Ǹ� �̵� �簳
        isPaused = false;
        currentPointIndex++;
    }





    IEnumerator IncomingSequence(int item)
    {
        systemManager.ismoving = true;


        // Ŭ���� ������������ �̵�
        obj_GantryX.GetComponent<ParentChildOscillateZ>().inputValue = 95167;
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = 20000;

        // ȸ��
        obj_Gantry_ROT.GetComponent<ConditionalRotateObject>().currentValue = true;

        
        yield return new WaitForSeconds(3f);// 3�� ���

        //Ŭ���� ���������� �̵�
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = 4350;

        yield return new WaitForSeconds(6f); // 5�ʴ��

        // ���� & �׸�
        obj_Gantry_FB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward = true;
        yield return new WaitForSeconds(2f);// 3�� ���
        obj_Gantry_CLP.GetComponent<Gantry_Gripper_Controller>().Grip = true;
        
        this.transform.SetParent(obj_Gantry_CLP.transform);// ���� ��ü�� �׸��ۿ� ����


        yield return new WaitForSeconds(1f); //1�ʴ�� 

        //// Ŭ���� ������������ �̵�
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = 20000;

        // ����
        obj_Gantry_FB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward = false;

        // 0���� ȸ��
        obj_Gantry_ROT.GetComponent<ConditionalRotateObject>().currentValue = false;

        yield return new WaitForSeconds(2f);


        // �� ���丮���� �̵�
        for (containlocate = 0; containlocate < systemManager.simulation_contains.Length; containlocate++)
        {

            if(systemManager.simulation_contains[containlocate] == 0)
            {
                obj_GantryX.GetComponent<ParentChildOscillateZ>().inputValue = coordination[containlocate, 0];
                obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = coordination[containlocate, 1];
                Debug.Log($"coordination[containlocate, 0]: {coordination[containlocate, 0]}");
                Debug.Log($"coordination[containlocate, 1]: {coordination[containlocate, 1]}");

                systemManager.simulation_contains[containlocate] = item;
                this.gameObject.name = "box_" + containlocate;
                break;
            }
            if (containlocate == systemManager.simulation_contains.Length)
                Debug.LogError("�����̳� ����");
        }

        yield return new WaitForSeconds(3f);

        //����
        obj_Gantry_FB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward = true;


        yield return new WaitForSeconds(1f);// 1�� ���

        //����
        obj_Gantry_CLP.GetComponent<Gantry_Gripper_Controller>().Grip = true;

        Vector3 originalPosition = this.transform.position;
        Quaternion originalRotation = this.transform.rotation;

        // �θ� ���� ����
        this.transform.SetParent(null);

        // �θ� ���� ���� �� ���� ��ġ�� ȸ�� �� ����
        this.transform.position = originalPosition;
        this.transform.rotation = originalRotation;

        yield return new WaitForSeconds(1f);

        //����
        obj_Gantry_FB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward = false;

        yield return new WaitForSeconds(1f);


        obj_GantryX.GetComponent<ParentChildOscillateZ>().inputValue = 0;
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = 0;
        yield return new WaitForSeconds(1f);

        systemManager.ismoving= false;

    }

    public IEnumerator OutgoingSequence()
    {
        systemManager.ismoving = true;
        // ��Ʈ�� ȸ��
        obj_Gantry_ROT.GetComponent<ConditionalRotateObject>().currentValue = false;

        obj_GantryX.GetComponent<ParentChildOscillateZ>().inputValue = coordination[containlocate, 0];
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = coordination[containlocate, 1] - 20;

        
        systemManager.simulation_contains[containlocate] = 0;

        yield return new WaitForSeconds(6f);
        


        //����
        obj_Gantry_FB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward = true;

        yield return new WaitForSeconds(2f);

        // ���� ���� ����
        obj_Gantry_CLP.GetComponent<Gantry_Gripper_Controller>().Grip = true;

        yield return new WaitForSeconds(1f);

        // ���� ��ü�� �׸��ۿ� ����
        this.transform.SetParent(obj_Gantry_CLP.transform);


        // ����
        obj_Gantry_FB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward = false;

        yield return new WaitForSeconds(1f);



        // Ŭ���� ������������ �̵�
        obj_GantryX.GetComponent<ParentChildOscillateZ>().inputValue = 95167;
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = 20000;

        yield return new WaitForSeconds(1f);

        // ȸ��
        obj_Gantry_ROT.GetComponent<ConditionalRotateObject>().currentValue = true;

        

        //����
        obj_Gantry_FB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward = true;

        // Ŭ���� ���������� �̵�
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = 4350;
        yield return new WaitForSeconds(6f);


        //����
        obj_Gantry_CLP.GetComponent<Gantry_Gripper_Controller>().Grip = false;


        Vector3 originalPosition = this.transform.position;
        Quaternion originalRotation = this.transform.rotation;

        // �θ� ���� ����
        this.transform.SetParent(null);

        // �θ� ���� ���� �� ���� ��ġ�� ȸ�� �� ����
        this.transform.position = originalPosition;
        this.transform.rotation = originalRotation;

        yield return new WaitForSeconds(1f);

        //����
        obj_Gantry_FB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward = false;

        yield return new WaitForSeconds(1f);


        // Ŭ���� ������������ �̵�
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = 20000;

        yield return new WaitForSeconds(3f);
        //���͸� 0���� �̵�
        obj_Gantry_ROT.GetComponent<ConditionalRotateObject>().currentValue = false;

        //���� �̵�
        obj_GantryX.GetComponent<ParentChildOscillateZ>().inputValue = 0;
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = 0;


        obj_stopper2.GetComponent<Stopper2_Controller>().moveforward = false;



        StartCoroutine(MoveToPositionsSequentially());

        yield return new WaitForSeconds(3f);
        obj_stopper2.GetComponent<Stopper2_Controller>().moveforward = true;
        systemManager.ismoving = false;

        

    }

    // ù ��° ����Ʈ���� 1�� ���� �Ͻ� �����ϴ� �ڷ�ƾ
    IEnumerator Pause1sec()
    {
        obj_stopper0.GetComponent<Stopper0_Controller>().currentValue = true;
        isPaused = true; // �̵� ����
        yield return new WaitForSeconds(2f); // 2�� ���
        obj_stopper1.GetComponent<Stopper1_Controller>().currentValue = false;
        yield return new WaitForSeconds(1f); // 1�� ���
        obj_stopper0.GetComponent<Stopper0_Controller>().currentValue = false;
        currentPointIndex++; // ���� ����Ʈ�� �̵�
        isPaused = false; // �̵� �簳
        yield return new WaitForSeconds(2f); // 3�� ���
        obj_stopper1.GetComponent<Stopper1_Controller>().currentValue = true;

    }

    IEnumerator MoveToPositionsSequentially()
    {
        // ù ��° ��ǥ�� �̵�
        yield return StartCoroutine(MoveToPosition(targetDisposeXPosition1));

        // �� ��° ��ǥ�� �̵�
        yield return StartCoroutine(MoveToPosition(targetDisposeYPosition2));


        yield return new WaitForSeconds(2f);
        

        Destroy(this.gameObject);


    }

    IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f) // ��ǥ ��ġ�� ������� ������ �ݺ�
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null; // �� ������ ���
        }
        transform.position = targetPosition; // ���������� ��Ȯ�� ��ǥ ��ġ�� ����
    }

}
