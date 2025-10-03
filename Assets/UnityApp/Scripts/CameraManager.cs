using UnityEngine;
using UnityEngine.InputSystem;

namespace Johnny.SimDungeon
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<CameraManager>();
                }
                return s_Instance;
            }
        }
        private static CameraManager s_Instance;

        public Camera MainCamera;

    }
}
