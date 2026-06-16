using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class overMenuScript : MonoBehaviour
{
    public skillButton skill;
    public TMP_Text skillName;
    public TMP_Text skillDescription;
    public float fadeDuration = 0.3f;
    public float screenPadding = 12f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private RectTransform canvasRect;
    private RectTransform boundsRect;
    private RectTransform parentRect;
    private Canvas canvas;
    private Vector2 startPosition;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        skill = GetComponentInParent<skillButton>();
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        boundsRect = FindBoundsRect();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        rectTransform = GetComponent<RectTransform>();
        parentRect = rectTransform.parent as RectTransform;
        startPosition = rectTransform.anchoredPosition;

        if (screenPadding < 0f)
        {
            screenPadding = 12f;
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (skill == null)
        {
            skill = GetComponentInParent<skillButton>();
        }

        skillName.text = skill.skill.skillName;
        skillDescription.text = skill.skill.skillDescription;
        rectTransform.anchoredPosition = startPosition;
        KeepInsideBounds();
        canvasGroup.alpha = 0f;
        StartFade(0.8f);
    }

    public void despawn()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeOutAndDisable());
    }

    private void StartFade(float to)
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(fade(canvasGroup.alpha, to));
    }

    private RectTransform FindBoundsRect()
    {
        Transform current = transform.parent;
        while (current != null)
        {
            if (current.name == "skillTree")
            {
                return current as RectTransform;
            }

            current = current.parent;
        }

        return canvasRect;
    }

    private void KeepInsideBounds()
    {
        Canvas.ForceUpdateCanvases();

        Vector3[] menuCorners = new Vector3[4];
        Vector3[] boundsCorners = new Vector3[4];
        rectTransform.GetWorldCorners(menuCorners);
        boundsRect.GetWorldCorners(boundsCorners);

        Vector2 menuMin = WorldToScreen(menuCorners[0]);
        Vector2 menuMax = WorldToScreen(menuCorners[2]);
        Vector2 boundsMin = WorldToScreen(boundsCorners[0]);
        Vector2 boundsMax = WorldToScreen(boundsCorners[2]);
        Vector2 screenDelta = Vector2.zero;

        if (menuMin.x < boundsMin.x + screenPadding)
        {
            screenDelta.x = boundsMin.x + screenPadding - menuMin.x;
        }
        else if (menuMax.x > boundsMax.x - screenPadding)
        {
            screenDelta.x = boundsMax.x - screenPadding - menuMax.x;
        }

        if (menuMin.y < boundsMin.y + screenPadding)
        {
            screenDelta.y = boundsMin.y + screenPadding - menuMin.y;
        }
        else if (menuMax.y > boundsMax.y - screenPadding)
        {
            screenDelta.y = boundsMax.y - screenPadding - menuMax.y;
        }

        if (screenDelta != Vector2.zero)
        {
            Vector2 currentScreenPosition = WorldToScreen(rectTransform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, currentScreenPosition, canvas.worldCamera, out Vector2 currentLocalPosition);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, currentScreenPosition + screenDelta, canvas.worldCamera, out Vector2 targetLocalPosition);
            rectTransform.anchoredPosition += targetLocalPosition - currentLocalPosition;
        }

        rectTransform.SetAsLastSibling();
    }

    private Vector2 WorldToScreen(Vector3 worldPosition)
    {
        return RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, worldPosition);
    }

    private IEnumerator FadeOutAndDisable()
    {
        yield return fade(canvasGroup.alpha, 0f);
        gameObject.SetActive(false);
    }

    private IEnumerator fade(float from,float to)
    {
        float elapsed = 0f;
        canvasGroup.alpha = from;
        while (elapsed < fadeDuration) 
            {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha=Mathf.Lerp(from,to,elapsed/fadeDuration);
            yield return null;
            }
        canvasGroup.alpha = to;
    }
}
