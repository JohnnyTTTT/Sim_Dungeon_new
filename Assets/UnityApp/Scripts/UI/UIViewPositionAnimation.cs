using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Views;
using Loxodon.Framework.Views.Animations;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class UIViewPositionAnimation : UIAnimation
    {
        public Vector2 targetPosition = Vector2.zero;
        public float animationDuration = 0.3f;

        public Ease easeType = Ease.OutQuad;

        private RectTransform m_PanelRect;
        private CanvasGroup m_CanvasGroup;
        private TweenerCore<Vector2, Vector2, VectorOptions> m_Tweener;
        private IUIView m_View;
        private float endAlpha;
        private bool endblocksRaycasts;

        private void Awake()
        {
            m_PanelRect = GetComponent<RectTransform>();
            m_CanvasGroup = GetComponent<CanvasGroup>();
            this.m_View = this.GetComponent<IUIView>();
            switch (this.AnimationType)
            {
                case AnimationType.EnterAnimation:
                    this.m_View.EnterAnimation = this;
                    endAlpha = 1;
                    endblocksRaycasts = true;
                    break;
                case AnimationType.ExitAnimation:
                    this.m_View.ExitAnimation = this;
                    endAlpha = 0;
                    endblocksRaycasts = false;
                    Play();
                    break;
                case AnimationType.ActivationAnimation:
                    if (this.m_View is IWindowView)
                        (this.m_View as IWindowView).ActivationAnimation = this;
                    break;
                case AnimationType.PassivationAnimation:
                    if (this.m_View is IWindowView)
                        (this.m_View as IWindowView).PassivationAnimation = this;
                    break;
            }
        }

        public override IAnimation Play()
        {
            if (m_Tweener != null && m_Tweener.IsPlaying())
            {
                m_Tweener.Kill();
            }

            //if()

            m_Tweener = m_PanelRect
                .DOAnchorPos(targetPosition, animationDuration)
                .SetEase(easeType)
                .SetLink(gameObject)
                .SetAutoKill(true)
                .OnComplete(() =>
                    {
                        //m_CanvasGroup.alpha = endAlpha;
                        m_CanvasGroup.blocksRaycasts = endblocksRaycasts;
                    });

            return this;
        }

    }
}
