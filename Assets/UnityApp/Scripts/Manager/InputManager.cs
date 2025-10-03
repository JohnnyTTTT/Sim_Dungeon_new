using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Johnny.SimDungeon
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActionsAsset;
        [SerializeField] private List<string> actionsToBlockWhenPointerOverUI;

        private InputActionMap coreGridActions;
        private InputActionMap buildActions;
        private InputActionMap destroyActions;
        private InputActionMap moveActions;

        private const string CORE_GRID_ACTIONS = "Core Grid Actions";
        private const string BUILD_ACTIONS = "Build Actions";
        private const string DESTROY_ACTIONS = "Destroy Actions";
        private const string MOVE_ACTIONS = "Move Actions";

        private const string GRID_MODE_RESET = "Grid Mode Reset";

        private const string BUILD_MODE_ACTIVATION = "Build Mode Activation";
        private const string BUILD = "Build";
        private const string BUILDABLE_ROTATE_CLOCKWISE = "Buildable Rotate Clockwise";
        private const string BUILDABLE_ROTATE_COUNTER_CLOCKWISE = "Buildable Rotate Counter Clockwise";

        private const string DESTROY_MODE_ACTIVATION = "Destroy Mode Activation";
        private const string DESTROY = "Destroy";

        private const string MOVE_MODE_ACTIVATION = "Move Mode Activation";
        private const string MOVE = "Move";

        private List<(InputAction.CallbackContext context, Action action)> pendingActions;
        private bool isApplicationFocused;
        private const float FOCUSED_APPLICATION_INPUT_ENABLE_DELAY = 0.1f;

        private float doubleTapTime = 0.3f;
        private float buildLastTapTime = -1f;
        private bool isBuildSecondTapInProgress = false;

        private float destroyLastTapTime = -1f;                 // Time of the last tap for destroying
        private bool isDestroySecondTapInProgress = false;

        private float moveLastTapTime = -1f;                    // Time of the last tap for moving
        private bool isMoveSecondTapInProgress = false;

        private void Awake()
        {
            coreGridActions = inputActionsAsset.FindActionMap(CORE_GRID_ACTIONS);
            buildActions = inputActionsAsset.FindActionMap(BUILD_ACTIONS);
            destroyActions = inputActionsAsset.FindActionMap(DESTROY_ACTIONS);
            //selectActions = inputActionsAsset.FindActionMap(SELECT_ACTIONS);
            moveActions = inputActionsAsset.FindActionMap(MOVE_ACTIONS);
            //heatMapActions = inputActionsAsset.FindActionMap(HEAT_MAP_ACTIONS);
        }

        private void OnEnable()
        {
            inputActionsAsset.Enable();
            InvokeSubscribeInputActionEvents();
        }

        private void Start()
        {
            pendingActions = new List<(InputAction.CallbackContext, Action)>();
            EasyGridBuilderPro.OnGridSystemCreated += OnGridSystemCreated;

            foreach (EasyGridBuilderPro easyGridBuilderPro in GridManager.Instance.GetEasyGridBuilderProSystemsList())
            {
                SetEasyGridBuilderProList(easyGridBuilderPro);
            }
        }

        private void OnDestroy()
        {
            EasyGridBuilderPro.OnGridSystemCreated -= OnGridSystemCreated;
        }

        private void LateUpdate()
        {
            foreach ((InputAction.CallbackContext context, Action action) in pendingActions)
            {
                if (isApplicationFocused && (!EventSystem.current.IsPointerOverGameObject() || !actionsToBlockWhenPointerOverUI.Contains(context.action.name))) action?.Invoke();
            }
            pendingActions.Clear();
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus) StartCoroutine(DelayInputProcessing());
            else isApplicationFocused = false;
        }

        #region Application Focus Validation Functions Start:
        private IEnumerator DelayInputProcessing()
        {
            // Wait for 0.1 seconds before enabling input processing
            yield return new WaitForSeconds(FOCUSED_APPLICATION_INPUT_ENABLE_DELAY);
            isApplicationFocused = true;
        }
        #endregion Application Focus Validation Functions End:

        private void OnGridSystemCreated(EasyGridBuilderPro easyGridBuilderPro)
        {
            SetEasyGridBuilderProList(easyGridBuilderPro);
        }

        private void SetEasyGridBuilderProList(EasyGridBuilderPro easyGridBuilderPro)
        {
            easyGridBuilderPro.SetInputGridModeVariables(true, true, false, true);
        }

        private void InvokeSubscribeInputActionEvents()
        {
            SubscribeInputActionEvents(
                (coreGridActions.FindAction(GRID_MODE_RESET), null, GridModeResetActionPerformed, null),

                (buildActions.FindAction(BUILD_MODE_ACTIVATION), null, BuildModeActivationActionPerformed, null),
                (buildActions.FindAction(BUILD), BuildActionStarted, BuildActionPerformed, BuildActionCancelled),
                (buildActions.FindAction(BUILDABLE_ROTATE_CLOCKWISE), null, BuildableRotateClockwiseActionPerformed, BuildableRotateClockwiseActionCancelled),
                (buildActions.FindAction(BUILDABLE_ROTATE_COUNTER_CLOCKWISE), null, BuildableRotateCounterClockwiseActionPerformed, BuildableRotateCounterClockwiseActionCancelled),

                (destroyActions.FindAction(DESTROY_MODE_ACTIVATION), null, DestroyModeActivationActionPerformed, null),
                (destroyActions.FindAction(DESTROY), DestroyActionStarted, DestroyActionPerformed, DestroyActionCancelled),


                (moveActions.FindAction(MOVE_MODE_ACTIVATION), null, MoveModeActivationActionPerformed, null),
                (moveActions.FindAction(MOVE), MoveActionStarted, MoveActionPerformed, MoveActionCancelled)
            );
        }

        private void SubscribeInputActionEvents(params (InputAction inputAction, Action<InputAction.CallbackContext> startedAction, Action<InputAction.CallbackContext> performedAction, Action<InputAction.CallbackContext> cancelledAction)[] actions)
        {
            foreach ((InputAction inputAction, Action<InputAction.CallbackContext> startedAction, Action<InputAction.CallbackContext> performedAction, Action<InputAction.CallbackContext> cancelledAction) actionTuple in actions)
            {
                if (actionTuple.inputAction != null)
                {
                    if (actionTuple.startedAction != null) actionTuple.inputAction.started += actionTuple.startedAction;
                    if (actionTuple.performedAction != null) actionTuple.inputAction.performed += actionTuple.performedAction;
                    if (actionTuple.cancelledAction != null) actionTuple.inputAction.canceled += actionTuple.cancelledAction;
                }
            }
        }


        #region Grid Mode Reset Actions Start:
        private void GridModeResetActionPerformed(InputAction.CallbackContext context)
        {
            EnqueueAction(context, () =>
            {
                SpawnManager.Instance.GridModeReset();
            });
        }
        #endregion Grid Mode Reset Actions End:

        #region Build Mode Activation Actions Start:
        private void BuildModeActivationActionPerformed(InputAction.CallbackContext context)
        {
            EnqueueAction(context, () => SpawnManager.Instance.SetActiveGridModeInAllGrids(GridMode.BuildMode));
        }
        #endregion Build Mode Activation Actions End:

        #region Build Actions Start:
        private void BuildActionStarted(InputAction.CallbackContext context)
        {
            var currentTime = Time.time;

            // Check for double-tap timing
            if (currentTime - buildLastTapTime <= doubleTapTime) isBuildSecondTapInProgress = true; // The second tap is now in progress
            else isBuildSecondTapInProgress = false; // Reset if it's not within double-tap time

            buildLastTapTime = currentTime;
        }

        private void BuildActionPerformed(InputAction.CallbackContext context)
        {
            foreach (EasyGridBuilderPro easyGridBuilderPro in GridManager.Instance.GetEasyGridBuilderProSystemsList())
            {
                EnqueueAction(context, () => easyGridBuilderPro.SetInputBuildableObjectPlacement());
            }
        }
        [SerializeField] private bool placeMovingObjectOnInputRelease = false;
        private void BuildActionCancelled(InputAction.CallbackContext context)
        {
            foreach (EasyGridBuilderPro easyGridBuilderPro in GridManager.Instance.GetEasyGridBuilderProSystemsList())
            {
                EnqueueAction(context, () => easyGridBuilderPro.SetInputBuildableObjectPlacementComplete(placeMovingObjectOnInputRelease));
            }
        }
        #endregion Build Actions End:

        #region Buildable Rotate Clockwise Actions Start:
        private void BuildableRotateClockwiseActionPerformed(InputAction.CallbackContext context)
        {
            EnqueueAction(context, () =>
            {
                SpawnManager.Instance.SetInputBuildableObjectClockwiseRotation();
            });
        }

        private void BuildableRotateClockwiseActionCancelled(InputAction.CallbackContext context)
        {
            EnqueueAction(context, () =>
            {
                SpawnManager.Instance.SetInputBuildableObjectClockwiseRotationComplete();
            });
        }
        #endregion Buildable Rotate Clockwise Actions End:

        #region Buildable Rotate Counter Clockwise Actions Start:
        private void BuildableRotateCounterClockwiseActionPerformed(InputAction.CallbackContext context)
        {
            EnqueueAction(context, () =>
            {
                SpawnManager.Instance.SetInputBuildableObjectCounterClockwiseRotation();
            });
        }

        private void BuildableRotateCounterClockwiseActionCancelled(InputAction.CallbackContext context)
        {
            EnqueueAction(context, () =>
            {
                SpawnManager.Instance.SetInputBuildableObjectCounterClockwiseRotationComplete();
            });
        }
        #endregion Buildable Rotate Counter Clockwise Actions End:



        #region Destroy Mode Activation Actions Start:
        private void DestroyModeActivationActionPerformed(InputAction.CallbackContext context)
        {
            if (GridManager.Instance.GetEasyGridBuilderProSystemsList().Count <= 0) return;
            EnqueueAction(context, () => SpawnManager.Instance.SetDestroyModeInAllGrids(DestroyMode.Entity));
        }
        #endregion Destroy Mode Activation Actions End:

        #region Destroy Actions Start:
        private void DestroyActionStarted(InputAction.CallbackContext context)
        {
            if (GridManager.Instance.GetEasyGridBuilderProSystemsList().Count <= 0) return;

            var currentTime = Time.time;

            // Check for double-tap timing
            if (currentTime - destroyLastTapTime <= doubleTapTime) isDestroySecondTapInProgress = true; // The second tap is now in progress
            else isDestroySecondTapInProgress = false; // Reset if it's not within double-tap time

            destroyLastTapTime = currentTime;
        }

        private void DestroyActionPerformed(InputAction.CallbackContext context)
        {
            if (GridManager.Instance.GetEasyGridBuilderProSystemsList().Count <= 0) return;
            EnqueueAction(context, () =>
            {
                if (GridManager.Instance.TryGetBuildableObjectDestroyer(out BuildableObjectDestroyer buildableObjectDestroyer)) buildableObjectDestroyer.SetInputDestroyBuildableObject();
            });
        }

        private void DestroyActionCancelled(InputAction.CallbackContext context)
        {
            if (GridManager.Instance.GetEasyGridBuilderProSystemsList().Count <= 0) return;
            EnqueueAction(context, () =>
            {
                if (GridManager.Instance.TryGetBuildableObjectDestroyer(out BuildableObjectDestroyer buildableObjectDestroyer)) buildableObjectDestroyer.SetInputDestroyBuildableObjectComplete();
            });
        }
        #endregion Destroy Actions End:



        #region Move Mode Activation Actions Start:
        private void MoveModeActivationActionPerformed(InputAction.CallbackContext context)
        {
            if (GridManager.Instance.GetEasyGridBuilderProSystemsList().Count <= 0) return;
            EnqueueAction(context, () => SpawnManager.Instance.SetActiveGridModeInAllGrids(GridMode.MoveMode));
        }
        #endregion Move Mode Activation Actions End:

        #region Move Action Start:
        private void MoveActionStarted(InputAction.CallbackContext context)
        {
            if (GridManager.Instance.GetEasyGridBuilderProSystemsList().Count <= 0) return;

            float currentTime = Time.time;

            // Check for double-tap timing
            if (currentTime - moveLastTapTime <= doubleTapTime) isMoveSecondTapInProgress = true; // The second tap is now in progress
            else isMoveSecondTapInProgress = false; // Reset if it's not within double-tap time

            moveLastTapTime = currentTime;
        }

        private void MoveActionPerformed(InputAction.CallbackContext context)
        {
            if (GridManager.Instance.GetEasyGridBuilderProSystemsList().Count <= 0) return;
            EnqueueAction(context, () =>
            {
                if (GridManager.Instance.TryGetBuildableObjectMover(out BuildableObjectMover buildableObjectMover)) buildableObjectMover.SetInputStartMoveBuildableObject();
            });
        }

        private void MoveActionCancelled(InputAction.CallbackContext context)
        {
            if (GridManager.Instance.GetEasyGridBuilderProSystemsList().Count <= 0) return;
            //if (IsTouchInput(context)) isMoveSecondTapInProgress = false; // Ensure it's mobile touch input
        }
        #endregion Move Action End:


        #region Late Update Functions Start:
        private void EnqueueAction(InputAction.CallbackContext context, System.Action action)
        {
            // Add to pending actions to process in LateUpdate
            pendingActions.Add((context, action));
        }
        #endregion Late Update Functions End:
    }
}
