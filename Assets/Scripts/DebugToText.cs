
using UnityEngine;
using TMPro;
using System.Collections;

public class DebugToText : MonoBehaviour
{
    private TextMeshProUGUI mText; 
    public float fadeDuration = 0.5f;
    private Coroutine fadeRoutine;
    void Awake()
    {
        Application.logMessageReceived += LogCallback;
        mText = gameObject.GetComponent<TextMeshProUGUI>();
    }
    
    void Start()
    {
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
        fadeRoutine = StartCoroutine(IntroFade(mText));
    }
    private IEnumerator IntroFade (TextMeshProUGUI textToUse) {
        yield return StartCoroutine(FadeTextToFullAlpha(fadeDuration, mText));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(FadeTextToZeroAlpha(fadeDuration, mText));
        //End of transition, do some extra stuff!!
    }
    public IEnumerator FadeTextToFullAlpha(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }
}
