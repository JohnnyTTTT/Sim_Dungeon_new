using SoulGames.EasyGridBuilderPro;
using UnityEngine;

namespace Johnny.SimDungeon
{
    [CreateAssetMenu(menuName = "Johnny/Build System/Buildable Gen Assets", order = 1)]
    public class BuildableGenAssets : ScriptableObject
    {
        public BuildableGen[] allAssets;
    }
}
