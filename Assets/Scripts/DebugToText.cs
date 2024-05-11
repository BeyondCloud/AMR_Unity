
using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.UIElements;


public class DebugToText : MonoBehaviour
{
    private TextMeshProUGUI mText; 
    public float fadeDuration = 0.5f;
    private float lifetime = 2f;
    private Coroutine fadeRoutine;
    void Awake()
    {
        Application.logMessageReceived += LogCallback;
        mText = gameObject.GetComponent<TextMeshProUGUI>();
    }
    void OnDestroy()
    {
        Application.logMessageReceived -= LogCallback;
    }

    private void LogCallback(string message, string stackTrace, LogType type)
    {
        mText.text = message;
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        fadeRoutine =  StartCoroutine(FadeInAndOut(fadeDuration, mText));
    }
    public IEnumerator FadeInAndOut(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
        yield return new WaitForSeconds(lifetime);
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

}
