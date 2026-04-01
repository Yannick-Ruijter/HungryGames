using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransitionAnimation : MonoBehaviour
{
    [SerializeField] private Image panelLeft;
    [SerializeField] private Image panelRight;
    [SerializeField] private SceneTransition sceneTransition;
    [SerializeField] private bool animateReset = false;

    [SerializeField] private TMP_Text m_ProgressText;

    private float _transitionEnd = 0.0f;

    void Start()
    {
        _transitionEnd = Time.time;
    }

    public void onTransitionStart()
    {
        
    }

    public void onTransitionEnd()
    {
        _transitionEnd = Time.time;
    }

    public void TransitionTimer(float time)
    {
        float hScreenWidth = Screen.width / 2.0f;
        float interp = 1.0f - easeInOutQuint(time);
        Vector2 position = panelLeft.rectTransform.anchoredPosition;
        panelLeft.rectTransform.sizeDelta = new Vector2(hScreenWidth, panelLeft.rectTransform.sizeDelta.y);

        position.x = Mathf.Lerp(0.0f, -hScreenWidth, interp);

        panelLeft.rectTransform.anchoredPosition = position;
                
        position = panelRight.rectTransform.anchoredPosition;
                
        panelRight.rectTransform.sizeDelta = new Vector2(hScreenWidth, panelRight.rectTransform.sizeDelta.y);

        position.x = Mathf.Lerp(0.0f, hScreenWidth, interp);
                
        panelRight.rectTransform.anchoredPosition = position;
    }

    void Update()
    {
        if (!sceneTransition.IsTransitioning)
        {
            float t = Time.time - _transitionEnd;
            if (animateReset && t <= 1.0f)
            {
                float hScreenWidth = Screen.width / 2.0f;
                float interp = easeInOutQuint(t);
                Vector2 position = panelLeft.rectTransform.anchoredPosition;
                panelLeft.rectTransform.sizeDelta = new Vector2(hScreenWidth, panelLeft.rectTransform.sizeDelta.y);

                position.x = Mathf.Lerp(0.0f, -hScreenWidth, interp);

                panelLeft.rectTransform.anchoredPosition = position;
                
                position = panelRight.rectTransform.anchoredPosition;
                
                panelRight.rectTransform.sizeDelta = new Vector2(hScreenWidth, panelRight.rectTransform.sizeDelta.y);

                position.x = Mathf.Lerp(0.0f, hScreenWidth, interp);
                
                panelRight.rectTransform.anchoredPosition = position;
            }
            else
            {
                panelLeft.enabled = false;
                panelRight.enabled = false;
            }
        }
        else
        {
            panelLeft.enabled = true;
            panelRight.enabled = true;
        }
    }

    public void SetLoadingProgress(float progress)
    {
        m_ProgressText.text = Mathf.FloorToInt(progress * 100.0f) + "%";
    }
    
    float easeInOutQuint(float x) {
        return x < 0.5f ? 16f * x * x * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 5f) / 2f;
    }
}
