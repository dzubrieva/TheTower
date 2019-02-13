using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject сylinder;
    public CameraController cameraController;
    public Transform ground;


    public int poolSize = 100;
    public int poolIncrementStep = 10;
    public float yAxisStep = 0.25f;
    public float ScaleStep = 0.04f;

    private GameObject lastCylinderHandler;
    private List<GameObject> Cylinders;
    private bool isCapableToIncreaseSize;
    private bool isGameEnded;
    private bool isCapableToCreateNewBlock = true;
    private bool isTouching = false;
    private readonly WaitForSeconds timeStep = new WaitForSeconds(0.04f);

    private void Awake()
    {
        Cylinders = new List<GameObject>();
        IncreasePool(poolSize);
    }



    void Start()
    {

    }

    /*Это не полноценная реализация пула, так как по бизнес правилам можно определить, что работа с данными в нашем пуле будет происходить как с данными в стеке
     * Последним включили - первым выключили
     */
    private void IncreasePool(int valueToIncrease)
    {
        GameObject newCylinderHandler;
        for (int i = Cylinders.Count; i < valueToIncrease; i++)
        {
            newCylinderHandler = Instantiate(сylinder, new Vector3(), Quaternion.identity, ground);
            newCylinderHandler.SetActive(false);
            Cylinders.Add(newCylinderHandler);
        }
    }

    private GameObject GetNewCylinder()
    {
        for (int i = 0; i < Cylinders.Count; i++)
        {
            if (!Cylinders[i].activeSelf)
                return Cylinders[i];
        }
        IncreasePool(poolIncrementStep);
        return Cylinders[Cylinders.Count - poolIncrementStep];
    }


    Vector3 GetPreviousCylinderScale()
    {
        if (lastCylinderHandler != null && lastCylinderHandler.activeSelf)
        {
            return lastCylinderHandler.transform.localScale;
        }
        return new Vector3(1, 0.25f, 1);
    }


    /*
     * В корутине CreateCylinder мы получаем значения скейла последнего цилиндра. Далее получаем следующий неактивный цилиндр из пула, 
     * после присваиваем ему координаты, исходя из координат последнего цилиндра, устанавливаем текущий цилиндр в качестве последнего цилиндра и обновляем цель камеры
     * Далее в цикле скейлим его один раз в каждый заданный промежуток времени и проверяем не перешел ли он заданные лимиты относительно прошлого цилиндра.
     * Цикл прекращает выполнение в 2 случаях. Либо игрок отжал палец, либо при проверке в цикле оказалось, что скейл вышел за границу.
     * После цикла делаем проверку на то, больше наш цилиндр прошлого или нет и проверку на PerfectMove
    */

    IEnumerator CreateCylinder()
    {
        Vector3 onStartedValueKeeper = GetPreviousCylinderScale();
        GameObject currentCylinder = GetNewCylinder();
        currentCylinder.transform.position = GetPositionForNewCylinder();
        currentCylinder.SetActive(true);
        lastCylinderHandler = currentCylinder;
        cameraController.SetNewTargetPosition(currentCylinder.transform.position);

        while (isCapableToIncreaseSize)
        {
            currentCylinder.transform.localScale = new Vector3(
                currentCylinder.transform.localScale.x + ScaleStep,
                currentCylinder.transform.localScale.y,
                currentCylinder.transform.localScale.z + ScaleStep
                );
            CheckIfGameOver(onStartedValueKeeper, currentCylinder.transform.localScale);
            yield return timeStep;
        }
        CheckIfGameOver(onStartedValueKeeper, currentCylinder.transform.localScale);
        CheckIfPerfectMove(onStartedValueKeeper, currentCylinder.transform.localScale);
    }

    void CheckIfPerfectMove(Vector3 previousCylinderScale, Vector3 currentScale)
    {
        if (isGameEnded)
            return;
        if (previousCylinderScale.x - 0.05 < currentScale.x)
        {
            lastCylinderHandler.GetComponent<CylinderController>().SetPerfect(true);
            StartCoroutine(PerfectVave());
            StartCoroutine(DisableCylinderCreation(1f));
        }
    }


    //Блокировка создания нового цилиндра для избежания возможных проблем с анимациями perfect move и улучшения геймплея в целом
    IEnumerator DisableCylinderCreation(float time)
    {
        isCapableToCreateNewBlock = false;
        yield return new WaitForSeconds(Settings.s.waitWhenPerfectWave);
        isCapableToCreateNewBlock = true;
    }



    IEnumerator PerfectVave()
    {
        for (int i = Cylinders.Count - 1; i >= 0; i--)
        {
            if (Cylinders[i].activeSelf)
            {
                bool value = lastCylinderHandler.Equals(Cylinders[i]) ? true : false;
                Cylinders[i].GetComponent<CylinderController>().PerfectMove(value);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    void CheckIfGameOver(Vector3 previousCylinderScale, Vector3 currentScale)
    {
        if (isCapableToIncreaseSize)
        {

            if (currentScale.x / previousCylinderScale.x >= 1.1f)
            {
                GameOver();
            }
        }
        else
        {
            if (currentScale.x > previousCylinderScale.x)
                GameOver();
        }
    }

    void GameOver()
    {
        if (isGameEnded)
            return;
        lastCylinderHandler.GetComponent<CylinderController>().PaintToRed();
        lastCylinderHandler.GetComponent<CylinderController>().DisableAfterTime(1f);
        cameraController.ShowAllTower();
        isGameEnded = true;
        isCapableToIncreaseSize = false;
    }

    private Vector3 GetPositionForNewCylinder()
    {
        if (lastCylinderHandler != null && lastCylinderHandler.activeSelf)
            return new Vector3(
                lastCylinderHandler.transform.position.x,
                lastCylinderHandler.transform.position.y + yAxisStep,
                lastCylinderHandler.transform.position.z
                );
        return new Vector3(0, 0.5f, 0);
    }

    void NewGame()
    {
        lastCylinderHandler.SetActive(false);
        isGameEnded = false;
        isCapableToCreateNewBlock = true;
        cameraController.SetDefaultValues();
        cameraController.SetNewTargetPosition(new Vector3(0, 0.25f, 0));
        for (int i = 0; i < Cylinders.Count; i++)
        {
            if (Cylinders[i].activeSelf)
            {
                Cylinders[i].SetActive(false);
            }
            else
            {
                return;
            }
        }
    }

    void AndroidInput()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId == 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    if (isGameEnded)
                    {
                        NewGame();
                    }
                    else
                    {
                        if (!isCapableToCreateNewBlock)
                            return;
                        isCapableToIncreaseSize = true;
                        StartCoroutine(CreateCylinder());
                    }
                }
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    isCapableToIncreaseSize = false;
                }
            }
        }
    }

    void WindowsInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isGameEnded)
            {
                NewGame();
            }
            else
            {
                if (!isCapableToCreateNewBlock)
                    return;
                isCapableToIncreaseSize = true;
                StartCoroutine(CreateCylinder());
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isCapableToIncreaseSize = false;
        }
    }

    void Update()
    {
#if UNITY_ANDROID
        AndroidInput();
#else
        WindowsInput();
#endif
    }
}
