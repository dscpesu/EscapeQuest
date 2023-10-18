using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Unity.XRContent.Interaction
{
    public class DynamicMoveProvider : ActionBasedContinuousMoveProvider
    {

        [SerializeField] bool m_HeadDrivesMotion = true;
        [SerializeField] Transform m_HeadTransform;

        [SerializeField] Transform m_LeftControllerTransform;
        [SerializeField] Transform m_RightControllerTransform;


        Transform m_CombinedTransform;

        public bool HeadDrivesMotion { get => m_HeadDrivesMotion; set => m_HeadDrivesMotion = value; }

        protected override void Awake()
        {
            m_CombinedTransform = new GameObject("DirectionGuide").transform;
            m_CombinedTransform.parent = transform;
            m_CombinedTransform.gameObject.hideFlags = HideFlags.HideAndDontSave;

            forwardSource = m_CombinedTransform;
        }

        protected override Vector2 ReadInput()
        {
            if (m_HeadDrivesMotion)
            {
                if (m_HeadTransform != null)
                {
                    m_CombinedTransform.position = m_HeadTransform.position;
                    m_CombinedTransform.rotation = m_HeadTransform.rotation;
                }
            }
            else
            {
                if (m_LeftControllerTransform != null && m_RightControllerTransform != null)
                {
                    var leftHandValue = leftHandMoveAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
                    var rightHandValue = rightHandMoveAction.action?.ReadValue<Vector2>() ?? Vector2.zero;

                    var totalValue = (leftHandValue.magnitude + rightHandValue.magnitude);
                    var leftHandBlend = 0.5f;

                    if (totalValue > Mathf.Epsilon)
                    {
                        leftHandBlend = leftHandValue.magnitude / totalValue;
                    }
                    m_CombinedTransform.position = Vector3.Lerp(m_RightControllerTransform.position, m_LeftControllerTransform.position, leftHandBlend);
                    m_CombinedTransform.rotation = Quaternion.Slerp(m_RightControllerTransform.rotation, m_LeftControllerTransform.rotation, leftHandBlend);
                }
            }
            return base.ReadInput();
        }


    }
}




// using UnityEngine;
// using UnityEngine.XR.Interaction.Toolkit;

// namespace Unity.XRContent.Interaction
// {
//     public class DynamicMoveProvider : ActionBasedContinuousMoveProvider
//     {
//         [SerializeField] bool m_HeadDrivesMotion = true;
//         [SerializeField] Transform m_HeadTransform;
//         [SerializeField] Transform m_LeftControllerTransform;
//         [SerializeField] Transform m_RightControllerTransform;
//         [SerializeField] Transform m_BodyControllerTransform;

//         Transform m_CombinedTransform;

//         public bool HeadDrivesMotion { get => m_HeadDrivesMotion; set => m_HeadDrivesMotion = value; }

//         protected override void Awake()
//         {
//             m_CombinedTransform = new GameObject("DirectionGuide").transform;
//             m_CombinedTransform.parent = transform;
//             m_CombinedTransform.gameObject.hideFlags = HideFlags.HideAndDontSave;

//             forwardSource = m_CombinedTransform;
//         }

//         protected override Vector2 ReadInput()
//         {
//             if (m_HeadDrivesMotion)
//             {
//                 if (m_HeadTransform != null)
//                 {
//                     m_CombinedTransform.position = m_HeadTransform.position;
//                     m_CombinedTransform.rotation = m_HeadTransform.rotation;
//                 }
//             }
//             else
//             {
//                 if (m_LeftControllerTransform != null && m_RightControllerTransform != null)
//                 {
//                     var leftHandValue = leftHandMoveAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
//                     var rightHandValue = rightHandMoveAction.action?.ReadValue<Vector2>() ?? Vector2.zero;

//                     var totalValue = (leftHandValue.magnitude + rightHandValue.magnitude);
//                     var leftHandBlend = 0.5f;

//                     if (totalValue > Mathf.Epsilon)
//                     {
//                         leftHandBlend = leftHandValue.magnitude / totalValue;
//                     }

//                     // Interpolate the position and rotation of the body
//                     m_CombinedTransform.position = Vector3.Lerp(m_RightControllerTransform.position, m_LeftControllerTransform.position, leftHandBlend);
//                     m_CombinedTransform.rotation = Quaternion.Slerp(m_RightControllerTransform.rotation, m_LeftControllerTransform.rotation, leftHandBlend);

//                     // Move the body to the new position
//                     m_BodyControllerTransform.position = m_CombinedTransform.position;
//                     m_BodyControllerTransform.rotation = m_CombinedTransform.rotation;
//                 }
//             }

//             return base.ReadInput();
//         }
//     }
// }
