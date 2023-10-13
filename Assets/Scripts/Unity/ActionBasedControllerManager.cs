using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System.Collections;



//Following project is not set for any headgear currently since we are lacking any headgear, if the project 
//get set in future uncomment //#Debug
//#Debug using Unity.XRContent.Input;

namespace Unity.XRContent.Interaction
{
    /// <summary>
    /// Use this class to map input actions to each controller state (mode)
    /// and set up the transitions between controller states (modes).
    /// </summary>
    [AddComponentMenu("XR/Action Based Controller Manager")]
    [DefaultExecutionOrder(kControllerManagerUpdateOrder)]
    public class ActionBasedControllerManager : MonoBehaviour
    {
        public const int kControllerManagerUpdateOrder = 10;

        [Space]
        [Header("Interactors")]

        [SerializeField]
        [Tooltip("The GameObject containing the interactor used for direct manipulation.")]
        XRDirectInteractor m_DirectInteractor;

        [SerializeField]
        [Tooltip("The GameObject containing the interactor used for distant/ray manipulation.")]
        XRRayInteractor m_RayInteractor;

        [SerializeField]
        [Tooltip("The GameObject containing the interactor used for teleportation.")]
        XRRayInteractor m_TeleportInteractor;

        [Space]
        [Header("Controller Actions")]

        [SerializeField]
        [Tooltip("The reference to the action of selecting with this controller.")]
        InputActionReference m_Select;

        [SerializeField]
        [Tooltip("The reference to the action of moving an object closer or further away with the ray interactor")]
        InputActionReference m_AnchorTranslate;

        [SerializeField]
        [Tooltip("The reference to the action of rotating an object with the ray interactor")]
        InputActionReference m_AnchorRotate;

        // State transition actions
        [SerializeField]
        [Tooltip("The reference to the action of activating the teleport mode for this controller.")]
        InputActionReference m_TeleportModeActivate;

        [SerializeField]
        [Tooltip("The reference to the action of canceling the teleport mode for this controller.")]
        InputActionReference m_TeleportModeCancel;

        // Character movement actions
        [SerializeField]
        [Tooltip("The reference to the action of deciding to turn the XR rig with this controller.")]
        InputActionReference m_TurnSelect;

        [SerializeField]
        [Tooltip("The reference to the action of turning the XR rig with this controller.")]
        InputActionReference m_Turn;

        [SerializeField]
        [Tooltip("The reference to the action of snap turning the XR rig with this controller.")]
        InputActionReference m_SnapTurn;

        [SerializeField]
        [Tooltip("The reference to the action of moving the XR rig with this controller.")]
        InputActionReference m_Move;

        bool m_DirectHover = false;
        bool m_DirectSelect = false;
        bool m_Teleporting = false;

        // Backend variables controlled by the rig locomotion manager
        bool m_SmoothMotionEnabled = false;
        bool m_SmoothTurnEnabled = false;

        public bool SmoothMotionEnabled
        {
            get => m_SmoothMotionEnabled;
            set
            {
                m_SmoothMotionEnabled = value;
                UpdateLocomotionActions();
            }
        }

        public bool SmoothTurnEnabled
        {
            get => m_SmoothTurnEnabled;
            set
            {
                m_SmoothTurnEnabled = value;
                UpdateRotationActions();
            }
        }

        // For our input mediation, we are enforcing a few rules between direct, ray, and teleportation interaction:
        // 1. If the Teleportation Ray is engaged, the Direct and Ray interactors are disabled
        // 2. If the Direct interactor is not idle (hovering or select), the ray interactor is disabled
        // 3. If the Ray interactor is selecting, all locomotion controls are disabled (teleport ray and snap controls) to prevent input collision
        void SetupInteractorEvents()
        {
            UpdateLocomotionActions();
            UpdateRotationActions();

            if (m_DirectInteractor != null)
            {
                m_DirectInteractor.hoverEntered.AddListener(DirectHoverEntered);
                m_DirectInteractor.hoverExited.AddListener(DirectHoverExited);
                m_DirectInteractor.selectEntered.AddListener(DirectSelectEntered);
                m_DirectInteractor.selectExited.AddListener(DirectSelectExited);
            }

            if (m_RayInteractor != null)
            {
                m_RayInteractor.selectEntered.AddListener(RaySelectEntered);
                m_RayInteractor.selectExited.AddListener(RaySelectExited);
            }

            if (m_TeleportModeActivate != null && m_TeleportModeCancel != null)
            {
                var teleportModeAction = GetInputAction(m_TeleportModeActivate);
                var cancelTeleportModeAction = GetInputAction(m_TeleportModeCancel);
                teleportModeAction.performed += StartTeleport;
                teleportModeAction.canceled += CancelTeleport;
                cancelTeleportModeAction.performed += CancelTeleport;
            }
        }

        void TeardownInteractorEvents()
        {
            if (m_DirectInteractor != null)
            {
                m_DirectInteractor.hoverEntered.RemoveListener(DirectHoverEntered);
                m_DirectInteractor.hoverExited.RemoveListener(DirectHoverExited);
                m_DirectInteractor.selectEntered.RemoveListener(DirectSelectEntered);
                m_DirectInteractor.selectExited.RemoveListener(DirectSelectExited);
            }

            if (m_RayInteractor != null)
            {
                m_RayInteractor.selectEntered.RemoveListener(RaySelectEntered);
                m_RayInteractor.selectExited.RemoveListener(RaySelectExited);
            }

            if (m_TeleportModeActivate != null && m_TeleportModeCancel != null)
            {
                var teleportModeAction = GetInputAction(m_TeleportModeActivate);
                var cancelTeleportModeAction = GetInputAction(m_TeleportModeCancel);
                teleportModeAction.performed -= StartTeleport;
                teleportModeAction.canceled -= CancelTeleport;
                cancelTeleportModeAction.performed -= CancelTeleport;
            }
        }

        void StartTeleport(InputAction.CallbackContext obj)
        {
            m_Teleporting = true;
            m_TeleportInteractor.gameObject.SetActive(true);
            RayInteractorUpdate();

            //#Debug InputMediator.ConsumeControl(GetInputAction(m_TeleportModeActivate), false);
            //#Debug InputMediator.ConsumeControl(GetInputAction(m_TeleportModeCancel), false);
        }

        void CancelTeleport(InputAction.CallbackContext obj)
        {
            m_Teleporting = false;
            // We delay turning off the teleport interactor in this callback so that it has a chance to complete the teleport
            RayInteractorUpdate();

            //#Debug InputMediator.ReleaseControl(GetInputAction(m_TeleportModeActivate));
            //#Debug InputMediator.ReleaseControl(GetInputAction(m_TeleportModeCancel));
        }

        void DirectHoverEntered(HoverEnterEventArgs args)
        {
            m_DirectHover = true;
            DirectInteractorUpdate();
        }

        void DirectHoverExited(HoverExitEventArgs args)
        {
            m_DirectHover = false;
            DirectInteractorUpdate();
        }

        void DirectSelectEntered(SelectEnterEventArgs args)
        {
            m_DirectSelect = true;
            DirectInteractorUpdate();
        }

        void DirectSelectExited(SelectExitEventArgs args)
        {
            m_DirectSelect = false;
            DirectInteractorUpdate();
        }

        void DirectInteractorUpdate()
        {
            RayInteractorUpdate();
            //#DebugInputMediator.ConsumeControl(m_Select.action, true, true);
        }

        void RayInteractorUpdate()
        {
            if (m_RayInteractor != null)
                m_RayInteractor.gameObject.SetActive(!(m_DirectHover || m_DirectSelect || m_Teleporting));
        }

        void RaySelectEntered(SelectEnterEventArgs args)
        {
            // Disable direct selection
            if (m_DirectInteractor != null)
                m_DirectInteractor.gameObject.SetActive(false);

            // Consume all the control motion events
            //#Debug InputMediator.ConsumeControl(m_AnchorTranslate.action, false, true, m_AnchorRotate.action);
            //#Debug InputMediator.ConsumeControl(m_AnchorRotate.action, false, false);
        }

        void RaySelectExited(SelectExitEventArgs args)
        {
            // Enable direct selection
            if (m_DirectInteractor != null)
                m_DirectInteractor.gameObject.SetActive(true);

            // Stop consuming motion control events
            //#Debug InputMediator.ReleaseControl(m_AnchorTranslate.action, false);
            //#Debug InputMediator.ReleaseControl(m_AnchorRotate.action, false);
        }

        protected IEnumerator Start()
        {
            yield return null;
            if (enabled)
            {
                UpdateLocomotionActions();
                UpdateRotationActions();
            }
        }

        protected void OnEnable()
        {
            SetupInteractorEvents();
            if (m_TeleportInteractor != null)
                m_TeleportInteractor.gameObject.SetActive(false);
        }

        protected void OnDisable()
        {
            TeardownInteractorEvents();
        }

        void Update()
        {
            if (!m_Teleporting && m_TeleportInteractor.gameObject.activeInHierarchy)
                m_TeleportInteractor.gameObject.SetActive(false);

            // Preferably there would just be event handlers in the move and snap turn select actions,
            // but move does not trigger 'canceled' reliably so we have to do it in the update instead

            var turnSelectAction = GetInputAction(m_TurnSelect);
            var snapTurnAction = GetInputAction(m_SnapTurn);
            var turnAction = GetInputAction(m_Turn);

            if (turnSelectAction != null)
            {
                if (turnSelectAction.phase == InputActionPhase.Started || turnSelectAction.phase == InputActionPhase.Performed)
                    //#Debug InputMediator.ConsumeControl(turnSelectAction, false, false, snapTurnAction, turnAction);

                    if (turnSelectAction.phase == InputActionPhase.Canceled || turnSelectAction.phase == InputActionPhase.Waiting)
                    {
                        //#DebugInputMediator.ReleaseControl(turnSelectAction, false);
                    }
            }

            var moveAction = GetInputAction(m_Move);
            if (moveAction != null)
            {
                if (moveAction.phase == InputActionPhase.Started || moveAction.phase == InputActionPhase.Performed)
                    //#Debug InputMediator.ConsumeControl(moveAction, false);

                    if (moveAction.phase == InputActionPhase.Canceled || moveAction.phase == InputActionPhase.Waiting)
                    {
                    //#Debug InputMediator.ReleaseControl(moveAction, false);
                    }
            }
        }

        void UpdateLocomotionActions()
        {
            if (m_SmoothMotionEnabled)
            {
                EnableAction(m_Move);
                DisableAction(m_TeleportModeActivate);
                DisableAction(m_TeleportModeCancel);
            }
            else
            {
                DisableAction(m_Move);
                EnableAction(m_TeleportModeActivate);
                EnableAction(m_TeleportModeCancel);
            }
        }

        void UpdateRotationActions()
        {
            if (m_SmoothTurnEnabled)
            {
                EnableAction(m_Turn);
                DisableAction(m_SnapTurn);
            }
            else
            {
                DisableAction(m_Turn);
                EnableAction(m_SnapTurn);
            }
        }

        static void EnableAction(InputActionReference actionReference)
        {
            var action = GetInputAction(actionReference);
            if (action != null && !action.enabled)
                action.Enable();
        }

        static void DisableAction(InputActionReference actionReference)
        {
            var action = GetInputAction(actionReference);
            if (action != null && action.enabled)
                action.Disable();
        }

        static InputAction GetInputAction(InputActionReference actionReference)
        {
#pragma warning disable IDE0031 // Use null propagation -- Do not use for UnityEngine.Object types
            return actionReference != null ? actionReference.action : null;
#pragma warning restore IDE0031
        }
    }
}
