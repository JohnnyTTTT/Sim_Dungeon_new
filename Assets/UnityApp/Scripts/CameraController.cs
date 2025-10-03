using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Johnny.SimDungeon
{
    public class CameraController : MonoBehaviour
    {
        // -------------------------------------------------------------
        // 单例部分保持不变
        // -------------------------------------------------------------



        [Header("Camera Settings")]
        public bool isValid = false;
        public CinemachineCamera cinemachineCamera;
        private Camera m_MainCamera;

        [Header("Rotation Settings")]
        [SerializeField] private float m_RotateSpeed = 5f;
        // **【新增】** 旋转阻尼时间：用于平滑停止旋转（建议 0.1f - 0.3f）
        [SerializeField] private float m_RotationSmoothTime = 0.2f;

        // SmoothDamp 追踪的速度变量
        private float m_RotationVelocityY;

        [Header("Pan Settings")]
        [SerializeField] private float m_PanSmoothness = 20f;
        [SerializeField] private float m_PlaneHeight = 0f; // 平面高度

        private Vector3 m_PanStartPoint;
        private bool m_IsPanning;
        private UnityEngine.Plane m_PanPlane;

        // **【关键】** 储存相机最终应该到达的位置（所有操作的目标）
        private Vector3 m_TargetPosition;

        [Header("Zoom Settings")]
        [SerializeField] private float m_ZoomSpeed = 0.5f;
        [SerializeField] private float m_ZoomSmoothness = 10f; // Lerp 平滑系数
        [SerializeField] private float m_MinZoomDistance = 3f; // 最小缩放距离
        [SerializeField] private float m_MaxZoomDistance = 50f; // 最大缩放距离

        // -------------------------------------------------------------

        private void Start()
        {
            m_MainCamera = CameraManager.Instance.MainCamera;
            m_PanPlane = new UnityEngine.Plane(Vector3.up, new Vector3(0f, m_PlaneHeight, 0f));
            m_TargetPosition = transform.position;
        }

        private void Update()
        {
            if (!isValid) return;

            HandlePanInput();
            HandleRotateInput();
            HandleZoomInput();

            SmoothMove();
        }

        /// <summary>
        /// 平滑地将相机移动到目标位置
        /// </summary>
        private void SmoothMove()
        {
            // **【位置平滑】** 保持原始稳定逻辑
            if (m_IsPanning)
            {
                transform.position = m_TargetPosition;
            }
            else
            {
                transform.position = Vector3.Lerp(
                    transform.position,
                    m_TargetPosition,
                    Time.deltaTime * m_ZoomSmoothness
                );
            }
        }

        // -------------------------------------------------------------

        #region Pan (平移) - 保持稳定
        private void HandlePanInput()
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
                    m_TargetPosition += delta;
                }
            }
        }

        private Vector3 GetMousePlaneIntersection(Vector2 screenPos)
        {
            if (m_MainCamera == null) return Vector3.zero;
            var ray = m_MainCamera.ScreenPointToRay(screenPos);
            if (m_PanPlane.Raycast(ray, out var enter))
            {
                return ray.GetPoint(enter);
            }
            return Vector3.zero;
        }
        #endregion

        // -------------------------------------------------------------

        #region Rotate (旋转) - 即时位置 + 修正方向的惯性 + 中断判断
        private void HandleRotateInput()
        {
            bool isRotating = Mouse.current.rightButton.isPressed;
            bool isPanningActive = Mouse.current.middleButton.isPressed;
            bool isZoomingActive = Mouse.current.scroll.ReadValue().y != 0;

            // ----------------------------------------------------------
            // 【核心中断逻辑】
            // 如果当前没有旋转输入，但有平移或缩放输入，则强制停止惯性，并退出。
            if (!isRotating && (isPanningActive || isZoomingActive))
            {
                m_RotationVelocityY = 0f;
                return;
            }
            // ----------------------------------------------------------

            // 鼠标右键按下：执行即时旋转
            if (isRotating)
            {
                var deltaX = Mouse.current.delta.ReadValue().x;
                var angle = deltaX * m_RotateSpeed * Time.deltaTime;

                var focusPoint = GetScreenCenterPlanePoint();

                // 【即时操作】保持原始稳定的 RotateAround 逻辑
                if (focusPoint != Vector3.zero)
                {
                    transform.RotateAround(focusPoint, Vector3.up, angle);
                }
                else
                {
                    transform.RotateAround(transform.position, Vector3.up, angle);
                }

                // 旋转后，同步 m_TargetPosition
                m_TargetPosition = transform.position;

                // 记录旋转速度，该速度包含方向信息
                m_RotationVelocityY = deltaX * m_RotateSpeed;
            }
            // 鼠标右键抬起/未按下：平滑停止 (惯性衰减)
            else if (Mathf.Abs(m_RotationVelocityY) > 0.001f)
            {
                // 1. **计算残余速度（SmoothDamp 负责衰减）**
                float currentVelocity = m_RotationVelocityY;

                // SmoothDamp 内部会平滑地将 currentVelocity 衰减到 0，并将衰减后的新值写回 m_RotationVelocityY
                float newVelocity = Mathf.SmoothDamp(
                    currentVelocity,
                    0f, // 目标速度是 0
                    ref m_RotationVelocityY,
                    m_RotationSmoothTime
                );

                // 2. **应用残余惯性**
                var focusPoint = GetScreenCenterPlanePoint();

                // 使用新的衰减后的速度和 Time.deltaTime 计算惯性角度
                float inertialAngle = newVelocity * Time.deltaTime;

                // **【关键】** 再次调用 RotateAround，使用残余惯性角度进行旋转。
                if (focusPoint != Vector3.zero)
                {
                    transform.RotateAround(focusPoint, Vector3.up, inertialAngle);
                }
                else
                {
                    transform.RotateAround(transform.position, Vector3.up, inertialAngle);
                }

                // 3. **同步位置**
                m_TargetPosition = transform.position;
            }
            else
            {
                // 完全停止后，确保速度归零
                m_RotationVelocityY = 0f;
            }
        }
        #endregion

        // -------------------------------------------------------------

        #region Zoom (缩放) - 保持平滑逻辑
        private void HandleZoomInput()
        {
            var scrollDelta = Mouse.current.scroll.ReadValue().y;
            if (scrollDelta == 0) return;

            var moveDistance = scrollDelta * m_ZoomSpeed;
            var zoomCenter = GetMousePlaneIntersection(Mouse.current.position.ReadValue());

            if (zoomCenter == Vector3.zero)
            {
                zoomCenter = GetScreenCenterPlanePoint();
            }

            Vector3 cameraToCenter = zoomCenter - m_TargetPosition;
            Vector3 newTargetPosition = m_TargetPosition + cameraToCenter.normalized * moveDistance;
            float newDistance = Vector3.Distance(newTargetPosition, zoomCenter);

            if (newDistance >= m_MinZoomDistance && newDistance <= m_MaxZoomDistance)
            {
                m_TargetPosition = newTargetPosition;
            }
        }
        #endregion

        // -------------------------------------------------------------

        private Vector3 GetScreenCenterPlanePoint()
        {
            if (m_MainCamera == null) return Vector3.zero;

            var screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            return GetMousePlaneIntersection(screenCenter);
        }
    }
}