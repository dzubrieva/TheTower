using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderController : MonoBehaviour
{
    public float defaultYScale = 0.25f;

    private bool isScaleIncreased = false;
    private bool isPerfect = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnDisable()
    {
        transform.localScale = new Vector3(0, defaultYScale, 0);
        GetComponent<Renderer>().material.color = Color.white;
    }

    public void PaintToRed()
    {
        GetComponent<Renderer>().material.color = Settings.s.failCylinderColor;
    }

    public void DisableAfterTime(float time)
    {
        StartCoroutine(DisableMe(time));
    }

    public void SetPerfect(bool value)
    {
        isPerfect = value;
    }


    /* Проверяем, если данный цилиндр был на данный момент тем цилиндром, из-за которого началась волна, если нет, то проверяем является ли этот
     * цилиндр таковым, который в своё время так же начал волну, если да, то не меняем его конечный скейл
     */

    public void PerfectMove(bool wasItPerfectCylinder)
    {
        if (wasItPerfectCylinder)
        {
            StartCoroutine(ScaleOutIn(0.4f, 0.2f));
            isPerfect = true;
        }
        else
        {
            if (isPerfect)
            {
                StartCoroutine(ScaleOutIn(0.3f, 0.3f));
            }
            else
            {
                StartCoroutine(ScaleOutIn(0.3f, (transform.localScale.x + 0.3f) * 0.8f));
            }
        }
    }

    
    IEnumerator ScaleOutIn(float outValue, float inValue)
    {
        float handler = outValue;
        Vector3 scaleBeforeScaleing = transform.localScale;
        Vector3 desiredScale = new Vector3(
            transform.localScale.x + outValue, 
            transform.localScale.y,
            transform.localScale.z + outValue);

        while (handler > 0)
        {
            transform.localScale = new Vector3(
                Mathf.Clamp(transform.localScale.x + Settings.s.increaseDecreaseStep, scaleBeforeScaleing.x, desiredScale.x),
                transform.localScale.y,
                Mathf.Clamp(transform.localScale.z + Settings.s.increaseDecreaseStep, scaleBeforeScaleing.z, desiredScale.z)
                );
            handler -= Settings.s.increaseDecreaseStep;
            yield return new WaitForFixedUpdate();
        }
        handler = inValue;
        desiredScale = new Vector3(transform.localScale.x - inValue, transform.localScale.y, transform.localScale.z - inValue);
        while (handler > 0)
        {
            transform.localScale = new Vector3(
                Mathf.Clamp(transform.localScale.x - Settings.s.increaseDecreaseStep, scaleBeforeScaleing.x, desiredScale.x),
                transform.localScale.y,
                Mathf.Clamp(transform.localScale.z - Settings.s.increaseDecreaseStep, scaleBeforeScaleing.z, desiredScale.z)
                );
            handler -= Settings.s.increaseDecreaseStep;
            yield return new WaitForFixedUpdate();
        }
        transform.localScale = new Vector3(
                Mathf.Clamp(transform.localScale.x, 0, 1f),
                transform.localScale.y,
                Mathf.Clamp(transform.localScale.z, 0, 1f)
                );
    }

    IEnumerator DisableMe(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
