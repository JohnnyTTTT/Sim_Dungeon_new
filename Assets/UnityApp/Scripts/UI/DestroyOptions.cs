using Loxodon.Framework.ViewModels;
using UnityEngine;

namespace Johnny.SimDungeon
{

    public class DestroyOptionsViewModel : ViewModelBase
    {
        public bool IsVisible
        {
            get
            {
                return m_IsVisible;
            }
            set
            {
                Set(ref m_IsVisible, value);
            }
        }
        private bool m_IsVisible;

    }

    public class DestroyOptions : AnimationUIView
    {

    }
}
