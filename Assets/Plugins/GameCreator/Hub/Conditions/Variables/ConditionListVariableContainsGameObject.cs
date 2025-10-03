using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace GVL.GameCreator.Runtime.VisualScripting
{
    [Version(2, 0, 5)]
    [Title("List Variable Contains Game Object")]
    [Description(
        "Checks if a GameObject exists in Local or Global List Variable and return true if exists, false if it doesn't")]
    [Image(typeof(IconInstructions), ColorTheme.Type.Blue, typeof(OverlayListVariable))]
    [Category("Variables/List Variable Contains GameObject")]
    [Parameter("List Variable", "Local List or Global List which elements are iterated")]
    [Parameter("Target", "The GameObject to check for")]
    [Keywords("GameObject", "Contains", "Char", "Iterate", "Cycle", "Every", "All", "Stack", "List", "Text", "Search",
        "Scan")]

    [Serializable]
    public class ConditionListVariableContainsGameObject : Condition
    {

        // MEMBERS: -------------------------------------------------------------------------------
        [SerializeField] private CollectorListVariable m_ListVariable = new CollectorListVariable();
        [SerializeField] private PropertyGetGameObject m_Target = new PropertyGetGameObject();

        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string Summary => $"{this.m_ListVariable} contains {this.m_Target}";

        // RUN METHOD: ----------------------------------------------------------------------------
        protected override bool Run(Args args)
        {

            List<object> source = this.m_ListVariable.Get(args);
            var target = this.m_Target.Get(args);

            if (source == null || target == null) return false;

            for (int i = 0; i < source.Count; ++i)
            {
                var listItem = source[i] as GameObject;
                // if (listItemText == null) continue;
                if (listItem == target) return true;
            }

            return false;
        }
    }
}