using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KurumiVisual : MonoBehaviour
{
    [Header("Спрайты Куруми")]
    public Sprite[] eyeSprites;
    public Sprite[] mouthSprites;
    public Sprite[] bodySprites;

    [Header("UI Компоненты")]
    public Image eyesImage;
    public Image mouthImage;
    public Image bodyImage;
    public GameObject dialogBubble;
    public Text dialogText;

    [Header("Настройки")]
    public float blinkInterval = 3f;
    public float talkSpeed = 0.08f;

    private Vector3 dragOffset;
    private bool isDragging = false;
    private bool isTalking = false;

    void Start()
    {
        if (eyesImage == null || mouthImage == null)
        {
            Debug.LogError("KurumiVisual: не назначены компоненты UI!");
            return;
        }

        StartCoroutine(IdleBlink());
        SetExpression("neutral");
        if (dialogBubble) dialogBubble.SetActive(false);
    }

    IEnumerator IdleBlink()
    {
        while (true)
        {
            yield return new WaitForSeconds(blinkInterval);
            if (eyeSprites != null && eyeSprites.Length >= 5)
                eyesImage.sprite = eyeSprites[4];
            yield return new WaitForSeconds(0.1f);
            SetExpression("neutral");
        }
    }

    public void SetExpression(string expr)
    {
        if (eyeSprites == null || mouthSprites == null) return;

        switch (expr)
        {
            case "happy":
                if (eyeSprites.Length > 1) eyesImage.sprite = eyeSprites[1];
                if (mouthSprites.Length > 2) mouthImage.sprite = mouthSprites[2];
                if (bodySprites != null && bodySprites.Length > 1) bodyImage.sprite = bodySprites[1];
                break;
            case "sad":
                if (eyeSprites.Length > 2) eyesImage.sprite = eyeSprites[2];
                if (mouthSprites.Length > 1) mouthImage.sprite = mouthSprites[1];
                if (bodySprites != null && bodySprites.Length > 2) bodyImage.sprite = bodySprites[2];
                break;
            case "surprise":
                if (eyeSprites.Length > 3) eyesImage.sprite = eyeSprites[3];
                if (mouthSprites.Length > 1) mouthImage.sprite = mouthSprites[1];
                break;
            default:
                if (eyeSprites.Length > 0) eyesImage.sprite = eyeSprites[0];
                if (mouthSprites.Length > 0) mouthImage.sprite = mouthSprites[0];
                if (bodySprites != null && bodySprites.Length > 0) bodyImage.sprite = bodySprites[0];
                break;
        }
    }

    public void ShowMessage(string text, float duration = 3f)
    {
        StartCoroutine(DisplayMessage(text, duration));
    }

    IEnumerator DisplayMessage(string text, float duration)
    {
        if (dialogBubble == null) yield break;
        dialogBubble.SetActive(true);
        if (dialogText) dialogText.text = text;
        yield return new WaitForSeconds(duration);
        dialogBubble.SetActive(false);
    }

    public void StartTalking(string fullText)
    {
        StartCoroutine(AnimateTalk(fullText));
    }

    IEnumerator AnimateTalk(string fullText)
    {
        if (isTalking || dialogBubble == null) yield break;
        isTalking = true;
        dialogBubble.SetActive(true);
        if (dialogText) dialogText.text = "";

        for (int i = 0; i <= fullText.Length; i++)
        {
            if (dialogText) dialogText.text = fullText.Substring(0, i);
            if (mouthSprites != null && mouthSprites.Length > 1)
            {
                mouthImage.sprite = mouthSprites[1];
                yield return new WaitForSeconds(talkSpeed / 2);
                mouthImage.sprite = mouthSprites[0];
                yield return new WaitForSeconds(talkSpeed / 2);
            }
            else yield return new WaitForSeconds(talkSpeed);
        }

        isTalking = false;
        yield return new WaitForSeconds(2f);
        dialogBubble.SetActive(false);
    }

    void OnMouseDown()
    {
        isDragging = true;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        dragOffset = transform.position - mousePos;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            transform.position = mousePos + dragOffset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }
}