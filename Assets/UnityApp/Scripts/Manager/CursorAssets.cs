using UnityEngine;

namespace Johnny.SimDungeon
{
    [System.Serializable]
    public class CursorAsset
    {
        public CursorType cursorMode;
        public Texture2D texture;
        public Vector2 hotspot;
    }

    [CreateAssetMenu(menuName = "SIm Dungeon/System/CursorAssets", order = 100)]
    public class CursorAssets : ScriptableObject
    {
        public CursorAsset[] cursorAssets;
    }
}
