using Loxodon.Framework.Binding;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Services;
using Loxodon.Framework.ViewModels;
using SoulGames.EasyGridBuilderPro;
using System;
using UnityEngine;

namespace Johnny.SimDungeon
{
    [DefaultExecutionOrder(-100)]
    public class BindingService : MonoBehaviour
    {
        public static BindingService Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<BindingService>();
                }
                return s_Instance;
            }
        }
        private static BindingService s_Instance;

        private BindingServiceBundle m_BindingServiceBundle;

        private void Awake()
        {
            Debug.Log($"[------ BindingService ------ ] : Init");
            var context = Context.GetApplicationContext();
            var serviceContainer = context.GetContainer();
            m_BindingServiceBundle = new BindingServiceBundle(serviceContainer);
            m_BindingServiceBundle.Start();

            var mainGameViewModel = new MainGameViewModel();
            serviceContainer.Register(mainGameViewModel);
            //SelectionViewModel = new SelectionViewModel();
            //BuildableObjectsPanelViewModel = new BuildableObjectsPanelViewModel();
            //CategoryObjectsPanelViewModel = new CategoryObjectsPanelViewModel();
            //FixedToolbarViewModel = new FixedToolbarViewModel();

        }

        private void OnDestroy()
        {
            m_BindingServiceBundle.Stop();
            Debug.Log($"[------ BindingService ------ ] : Stop");
        }
    }
}
