using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FGUIStarter
{
    public class AllImageButton : Button, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Press Effect")]
        [SerializeField] private float pressOffset = 10f;

        private RectTransform rect;
        private Vector2 originalPos;

        protected override void Awake()
        {
            base.Awake();
            rect = GetComponent<RectTransform>();
            originalPos = rect.anchoredPosition;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            ApplyPressedVisual();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            ApplyNormalVisual();
        }

        private void ApplyPressedVisual()
        {
            rect.anchoredPosition = originalPos - new Vector2(0, pressOffset);
        }

        private void ApplyNormalVisual()
        {
            rect.anchoredPosition = originalPos;
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            if (state == SelectionState.Pressed)
                ApplyPressedVisual();
            else
                ApplyNormalVisual();
        }
    }
}
