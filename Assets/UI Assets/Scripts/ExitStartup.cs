using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitStartup : MonoBehaviour
{
    public float transitionDuration = 1.0f;
    public float time = 0f;
    public RectTransform rectTransform;
    public GameObject studentPanel;

    // Update is called once per frame
    void Update()
    {
        // Highlight Over Transition
        if (time < transitionDuration)
        {
            rectTransform.anchoredPosition = new Vector2(
            Mathf.Lerp(0, 1000f, time / transitionDuration), rectTransform.anchoredPosition.y);
            time += Time.deltaTime;
        }
        else
        {
            studentPanel.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
