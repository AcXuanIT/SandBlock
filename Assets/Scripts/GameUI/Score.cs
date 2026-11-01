using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textScore;
    [SerializeField] private RectTransform rec;
    [SerializeField] private Canvas canvas;
    public float duration = 0.5f;
    public float liftOffset = 0.5f;
    public float moveUp = 100f;

    private bool active = false;

    public void OnScore(int score, Vector3 worldPos)
    {
        if (active) return;
        active = true;

        if (rec == null) rec = GetComponent<RectTransform>();
        if (canvas == null) canvas = GetComponentInParent<Canvas>();

        textScore.text = "+" + score;

        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            rec.position = screenPoint;
        }
        else
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rec, screenPoint, canvas.worldCamera, out Vector3 worldUiPos);
            rec.position = worldUiPos;
        }

        Vector2 startPos = rec.anchoredPosition;

        gameObject.SetActive(true);
        textScore.color = new Color(textScore.color.r, textScore.color.g, textScore.color.b, 1f);

        DOTween.Kill(this);
        Sequence seq = DOTween.Sequence().SetId(this);
        seq.Join(rec.DOAnchorPosY(startPos.y + moveUp, duration).SetEase(Ease.OutQuad));
        seq.Join(textScore.DOFade(0f, duration));
        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
            active = false;
        });
    }
   
}
