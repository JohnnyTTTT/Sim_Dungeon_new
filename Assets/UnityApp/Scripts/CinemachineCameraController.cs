using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Johnny.SimDungeon
{
    public class CinemachineCameraController : MonoBehaviour
    {
        [SerializeField] private Camera m_Camera;
        [SerializeField] private Transform target;
        public bool isUnderground;


        private CinemachineCamera m_CinemachineCamera;
        private CinemachineOrbitalFollow m_OrbitalFollow;
        private CinemachineInputAxisController m_CinemachineInputAxisController;

        [SerializeField] private float m_ChangeModeDuration = 0f; // 平面高度
        private bool m_IsPanning;
        private Vector3 m_PanStartPoint;
        private Plane m_PanPlane;
        private bool lockInput;
        private TweenerCore<Vector3, Vector3, VectorOptions> m_Tweener;

        private void Awake()
        {
            m_CinemachineCamera = GetComponent<CinemachineCamera>();
            m_OrbitalFollow = m_CinemachineCamera.GetComponent<CinemachineOrbitalFollow>();
            m_CinemachineInputAxisController = m_CinemachineCamera.GetComponent<CinemachineInputAxisController>();
            m_PanPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
        }

        public void ChangeCamera()
        {
            if (m_Tweener != null && m_Tweener.IsPlaying())
            {
                m_Tweener.Kill();
            }

            lockInput = true;
            isUnderground = !isUnderground;

            var endPosition = Vector3.zero;
            if (isUnderground)
            {
                endPosition = new Vector3(target.position.x, target.position.y - 50f, target.position.z);
                m_PanPlane = new Plane(Vector3.up, new Vector3(0f, -50f, 0f));
            }
            else
            {
                endPosition = new Vector3(target.position.x, target.position.y + 50f, target.position.z);
                m_PanPlane = new Plane(Vector3.up, new Vector3(0f, -0f, 0f));
            }

            m_Tweener = target.DOMove(endPosition, m_ChangeModeDuration)
                .SetEase(Ease.InOutSine)
                .SetAutoKill(true)
                .OnComplete(() => { lockInput = false; });
            Debug.Log(endPosition);
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.vKey.wasPressedThisFrame)
            {
                ChangeCamera();
            }

            if (!lockInput)
            {
                if (Mouse.current.middleButton.wasPressedThisFrame)
                {
                    m_IsPanning = true;
                    m_PanStartPoint = GetMousePlaneIntersection(Mouse.current.position.ReadValue());
                }
                else if (Mouse.current.middleButton.wasReleasedThisFrame)
                {
                    m_IsPanning = false;
                }

                if (m_IsPanning)
                {

                    var currentPoint = GetMousePlaneIntersection(Mouse.current.position.ReadValue());
                    if (currentPoint != Vector3.zero)
                    {
                        var delta = m_PanStartPoint - currentPoint;
                        target.position += delta;
                    }
                }
            }

        }

        private Vector3 GetMousePlaneIntersection(Vector2 screenPos)
        {
            var ray = m_Camera.ScreenPointToRay(screenPos);
            if (m_PanPlane.Raycast(ray, out var enter))
            {
                return ray.GetPoint(enter);
            }
            return Vector3.zero;
        }

    }
}
