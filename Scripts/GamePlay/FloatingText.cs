using DG.Tweening;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshPro headerText;

    public void SetText(int value, Vector3 textPos, float duration = 1f)
    {
        transform.position = textPos;
        headerText.text = $"+{value}";
        headerText.alpha = 1;
        headerText.DOAlpha(0f, 1f).SetEase(Ease.InExpo);
        transform.DOMoveY(textPos.y + 0.5f, duration).OnComplete(ReturnToPool);
    }

    public void SetText(string text, Vector3 textPos, float duration = 1f)
    {
        transform.position = textPos;
        headerText.text = text;
        headerText.alpha = 1;
        headerText.DOAlpha(0f, 1f).SetEase(Ease.InExpo);
        transform.DOMoveY(textPos.y + 0.5f, duration).OnComplete(ReturnToPool);
    }

    private void ReturnToPool()
    {
        FloatingTextPool.Instance.Pool.Release(this);
    }
}