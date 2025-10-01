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

            // ��ʼ�����Ϊ����״̬
            panelRect.anchoredPosition = offScreenPosition;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0f;
        }

        /// <summary>
        /// ��廬�붯��
        /// </summary>
        private void SlideIn()
        {
            // ֹͣ�������ڽ��еĶ����������ͻ
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


            // �������߼�����ʾ
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;

            // ִ�л�������
            m_SlideInTweener = panelRect.DOAnchorPos(onScreenPosition, animationDuration)
                .SetEase(easeType)
                .SetLink(gameObject); // ���ӵ���ǰGameObject��������ʱ�Զ�ֹͣ
        }

        /// <summary>
        /// ��廬������
        /// </summary>
        private void SlideOut()
        {
            // ֹͣ�������ڽ��еĶ����������ͻ
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

            // ִ�л�������
            m_SlideOutTweener = panelRect.DOAnchorPos(offScreenPosition, animationDuration)
                .SetEase(easeType)
                .OnComplete(() =>
                {
                    // ������ɺ�������߼�������
                    canvasGroup.blocksRaycasts = false;
                    canvasGroup.alpha = 0f;
                })
                .SetLink(gameObject);
        }
    }
}
