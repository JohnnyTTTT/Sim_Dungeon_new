//using Loxodon.Framework.Binding;
//using Loxodon.Framework.Contexts;
//using Loxodon.Framework.ViewModels;
//using Loxodon.Framework.Views;
//using UnityEngine;
//using static UnityEditor.Profiling.HierarchyFrameDataView;

//namespace Johnny.SimDungeon
//{


//    public class TestGroup : UIView
//    {
//        protected override void Start()
//        {
//            var viewModel = new SelectionViewModel();
//            this.SetDataContext(viewModel);

//            var serviceContainer = Context.GetApplicationContext().GetContainer();
//            serviceContainer.Register(viewModel);

//            var bindingSet = this.CreateBindingSet<TestGroup, SelectionViewModel>();


//            bindingSet.Build();
//        }
//    }



//}
