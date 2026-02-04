using UnityEngine;
using DG.Tweening;

public class ButtonDotWeen : MonoBehaviour
{
    public float appearDuration = 0.3f;
    public Ease appearEase = Ease.OutBack;

    public float bounceScale = 1.08f;
    public float bounceDuration = 0.15f;
    public int bounceCount = 3;

    public float disappearDuration = 0.2f;
    public Ease disappearEase = Ease.InBack;

    [SerializeField] private Vector3 originScale = Vector3.one;

    void Awake()
    {
        originScale = transform.localScale == Vector3.zero ? Vector3.one : transform.localScale;
    }

    void OnEnable()
    {
        transform.DOKill(true);
        transform.localScale = Vector3.zero;

        transform.DOScale(Vector3.one, 0.3f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true); // 🔥 CỰC KỲ QUAN TRỌNG
    }


    public void PlayAppear()
    {
        transform.DOKill(true);
        transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);

        seq.Append(transform.DOScale(originScale, appearDuration).SetEase(appearEase));
        seq.Append(transform.DOScale(originScale * bounceScale, bounceDuration)
            .SetLoops(bounceCount * 2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine));
        seq.Append(transform.DOScale(originScale, 0.05f));
    }

    public void PlayDisappear()
    {
        Time.timeScale = 1f;
        transform.DOKill(true);

        transform.DOScale(Vector3.zero, disappearDuration)
            .SetEase(disappearEase)
            .SetUpdate(true)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
