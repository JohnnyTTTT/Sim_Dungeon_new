using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class AnimationPanel : MonoBehaviour
    {
        private RectTransform panelRect;
        private CanvasGroup canvasGroup;

        public Vector2 onScreenPosition = Vector2.zero;
        public Vector2 offScreenPosition = new Vector2(1000f, 0f);
        public float animationDuration = 0.3f;
        public Ease easeType = Ease.OutQuad;

        public bool Show
        {
            get
            {
                return m_Show;
            }
            set
            {
                m_Show = value;
                if (m_Show)
                {
                    SlideIn();
                }
                else
                {
                    SlideOut();
                }
            }
        }
        private bool m_Show;

        private TweenerCore<Vector2, Vector2, VectorOptions> m_SlideInTweener;
        private TweenerCore<Vector2, Vector2, VectorOptions> m_SlideOutTweener;

        private void Awake()
        {
            panelRect = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();

            // 初始化面板为隐藏状态
            panelRect.anchoredPosition = offScreenPosition;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0f;
        }

        /// <summary>
        /// 面板滑入动画
        /// </summary>
        private void SlideIn()
        {
            // 停止可能正在进行的动画，避免冲突
            //panelRect.DOComplete();
            //canvasGroup.DOComplete();

            if (m_SlideInTweener != null && m_SlideInTweener.IsPlaying())
            {
                m_SlideInTweener.Kill();
            }
            if (m_SlideOutTweener != null && m_SlideOutTweener.IsPlaying())
            {
                m_SlideOutTweener.Kill();
            }


            // 启用射线检测和显示
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;

            // 执行滑动动画
            m_SlideInTweener = panelRect.DOAnchorPos(onScreenPosition, animationDuration)
                .SetEase(easeType)
                .SetLink(gameObject); // 链接到当前GameObject，在销毁时自动停止
        }

        /// <summary>
        /// 面板滑出动画
        /// </summary>
        private void SlideOut()
        {
            // 停止可能正在进行的动画，避免冲突
            //panelRect.DOComplete();
            //canvasGroup.DOComplete();

            if (m_SlideInTweener != null && m_SlideInTweener.IsPlaying())
            {
                m_SlideInTweener.Kill();
            }
            if (m_SlideOutTweener != null && m_SlideOutTweener.IsPlaying())
            {
                m_SlideOutTweener.Kill();
            }

            // 执行滑动动画
            m_SlideOutTweener = panelRect.DOAnchorPos(offScreenPosition, animationDuration)
                .SetEase(easeType)
                .OnComplete(() =>
                {
                    // 动画完成后禁用射线检测和隐藏
                    canvasGroup.blocksRaycasts = false;
                    canvasGroup.alpha = 0f;
                })
                .SetLink(gameObject);
        }
    }
}
