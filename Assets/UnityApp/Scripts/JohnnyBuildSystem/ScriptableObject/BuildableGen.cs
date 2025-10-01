using SoulGames.EasyGridBuilderPro;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public enum BuildCategory
    {
        None,
        God,
        Structure,
        Placement,
    }

    [CreateAssetMenu(menuName = "Johnny/Build System/Buildable Gen", order = 10)]
    public class BuildableGen : ScriptableObject
    {
        public BuildableObjectSO buildableObjectSO;
        public BuildCategory buildCategory;
        public GridType gridType;
        public string description;

    }
}
