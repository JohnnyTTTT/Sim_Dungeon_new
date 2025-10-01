using Loxodon.Framework.Views;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class AnimationUIView : UIView
    {
        public override bool Visibility
        {
            get
            {
                return !this.IsDestroyed() && this.gameObject != null ? m_Visibility : false;
            }
            set
            {
                if (this.IsDestroyed() || this.gameObject == null)
                    return;
                if (this.m_Visibility != value)
                {
                    m_Visibility = value;
                    if (m_Visibility)
                    {
                        EnterAnimation.Play();
                    }
                    else
                    {
                        ExitAnimation.Play();
                    }
                }
            }
        }
        private bool m_Visibility;

    }
}
