using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Unity.XRContent.Interaction
{
    /// <summary>
    /// Use this class as a central manager to configure locomotion control schemes and configuration preferences.
    /// </summary>
    public class XRRigLocomotionManager : MonoBehaviour
    {
        /// <summary>
        /// Sets which movement control scheme to use.
        /// </summary>
        /// <seealso cref="moveScheme"/>
        public enum MoveScheme
        {
            /// <summary>
            /// Use teleport movement control scheme.
            /// </summary>
            Teleport,

            /// <summary>
            /// Use smooth (walk) movement control scheme.
            /// </summary>
            Smooth,
        }

        /// <summary>
        /// Sets which turn style of locomotion to use.
        /// </summary>
        /// <seealso cref="turnStyle"/>
        public enum TurnStyle
        {
            /// <summary>
            /// Use snap turning to rotate the direction you are facing by snapping by a specified angle.
            /// </summary>
            Snap,

            /// <summary>
            /// Use continuous turning to smoothly rotate the direction you are facing by a specified speed.
            /// </summary>
            Smooth,
        }

        [SerializeField]
        [Tooltip("Reference to the manager that mediates the left-hand controllers")]
        ActionBasedControllerManager m_LeftHandManager;

        [SerializeField]
        [Tooltip("Reference to the manager that mediates the right-hand controllers")]
        ActionBasedControllerManager m_RightHandManager;

        [SerializeField]
        [Tooltip("Controls which movement control scheme to use for the left hand.")]
        MoveScheme m_LeftHandMoveScheme;
        /// <summary>
        /// Controls which movement control scheme to use for the left hand.
        /// </summary>
        /// <seealso cref="MoveScheme"/>
        public MoveScheme LeftHandMoveScheme
        {
            get => m_LeftHandMoveScheme;
            set
            {
                SetMoveScheme(value, true);
                m_LeftHandMoveScheme = value;
            }
        }

        [SerializeField]
        [Tooltip("Controls which movement control scheme to use for the right hand.")]
        MoveScheme m_RightHandMoveScheme;
        /// <summary>
        /// Controls which movement control scheme to use for the left hand.
        /// </summary>
        /// <seealso cref="MoveScheme"/>
        public MoveScheme RightHandMoveScheme
        {
            get => m_RightHandMoveScheme;
            set
            {
                SetMoveScheme(value, false);
                m_RightHandMoveScheme = value;
            }
        }

        [SerializeField]
        [Tooltip("Controls which turn style of locomotion to use for the left hand")]
        TurnStyle m_LeftHandTurnStyle;
        /// <summary>
        /// Controls which turn style of locomotion to use for the left hand
        /// </summary>
        /// <seealso cref="TurnStyle"/>
        public TurnStyle LeftHandTurnStyle
        {
            get => m_LeftHandTurnStyle;
            set
            {
                SetTurnStyle(value, true);
                m_LeftHandTurnStyle = value;
            }
        }

        [SerializeField]
        [Tooltip("Controls which turn style of locomotion to use for the right hand")]
        TurnStyle m_RightHandTurnStyle;
        /// <summary>
        /// Controls which turn style of locomotion to use for the left hand
        /// </summary>
        /// <seealso cref="TurnStyle"/>
        public TurnStyle RightHandTurnStyle
        {
            get => m_RightHandTurnStyle;
            set
            {
                SetTurnStyle(value, false);
                m_RightHandTurnStyle = value;
            }
        }

        [SerializeField]
        [Tooltip("Input action assets associated with locomotion to affect when the active movement control scheme is set." +
            " Can use this list by itself or together with the Action Maps list to set control scheme masks by Asset or Map.")]
        List<InputActionAsset> m_ActionAssets;
        /// <summary>
        /// Input action assets associated with locomotion to affect when the active movement control scheme is set.
        /// Can use this list by itself or together with the Action Maps list to set control scheme masks by Asset or Map.
        /// </summary>
        /// <seealso cref="ActionMaps"/>
        public List<InputActionAsset> ActionAssets
        {
            get => m_ActionAssets;
            set => m_ActionAssets = value;
        }

        [SerializeField]
        [Tooltip("Input action maps associated with locomotion to affect when the active movement control scheme is set." +
            " Can use this list together with the Action Assets list to set control scheme masks by Map instead of the whole Asset.")]
        List<string> m_ActionMaps;
        /// <summary>
        /// Input action maps associated with locomotion to affect when the active movement control scheme is set.
        /// Can use this list together with the Action Assets list to set control scheme masks by Map instead of the whole Asset.
        /// </summary>
        /// <seealso cref="ActionAssets"/>
        public List<string> ActionMaps
        {
            get => m_ActionMaps;
            set => m_ActionMaps = value;
        }

        [SerializeField]
        [Tooltip("Input actions associated with locomotion to affect when the active movement control scheme is set." +
            " Can use this list to select exactly the actions to affect instead of setting control scheme masks by Asset or Map.")]
        List<InputActionReference> m_Actions;
        /// <summary>
        /// Input actions associated with locomotion that are affected by the active movement control scheme.
        /// Can use this list to select exactly the actions to affect instead of setting control scheme masks by Asset or Map.
        /// </summary>
        /// <seealso cref="ActionAssets"/>
        /// <seealso cref="ActionMaps"/>
        public List<InputActionReference> Actions
        {
            get => m_Actions;
            set => m_Actions = value;
        }

        [SerializeField]
        [Tooltip("Stores the locomotion provider for smooth (walk) movement.")]
        DynamicMoveProvider m_SmoothMoveProvider;
        /// <summary>
        /// Stores the locomotion provider for smooth movement.
        /// </summary>
        /// <seealso cref="DynamicMoveProvider"/>
        public DynamicMoveProvider SmoothMoveProvider
        {
            get => m_SmoothMoveProvider;
            set => m_SmoothMoveProvider = value;
        }

        [SerializeField]
        [Tooltip("Stores the locomotion provider for smooth turning.")]
        ContinuousTurnProviderBase m_SmoothTurnProvider;
        /// <summary>
        /// Stores the locomotion provider for smooth turning.
        /// </summary>
        /// <seealso cref="ContinuousTurnProviderBase"/>
        public ContinuousTurnProviderBase SmoothTurnProvider
        {
            get => m_SmoothTurnProvider;
            set => m_SmoothTurnProvider = value;
        }

        [SerializeField]
        [Tooltip("Stores the locomotion provider for snap turning.")]
        SnapTurnProviderBase m_SnapTurnProvider;
        /// <summary>
        /// Stores the locomotion provider for snap turning.
        /// </summary>
        /// <seealso cref="SnapTurnProviderBase"/>
        public SnapTurnProviderBase SnapTurnProvider
        {
            get => m_SnapTurnProvider;
            set => m_SnapTurnProvider = value;
        }

        void OnEnable()
        {
            SetMoveScheme(m_LeftHandMoveScheme, true);
            SetMoveScheme(m_RightHandMoveScheme, false);
            SetTurnStyle(m_LeftHandTurnStyle, true);
            SetTurnStyle(m_RightHandTurnStyle, false);
        }

        void SetMoveScheme(MoveScheme scheme, bool leftHand)
        {
            var targetHand = leftHand ? m_LeftHandManager : m_RightHandManager;
            targetHand.SmoothMotionEnabled = (scheme == MoveScheme.Smooth);
        }

        void SetTurnStyle(TurnStyle style, bool leftHand)
        {
            var targetHand = leftHand ? m_LeftHandManager : m_RightHandManager;
            targetHand.SmoothTurnEnabled = (style == TurnStyle.Smooth);
        }
    }
}
