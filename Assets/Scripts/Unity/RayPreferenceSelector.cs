using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Unity.XRContent.Interaction
{
    [RequireComponent(typeof(XRRayInteractor))]
    public class RayPreferenceSelector : MonoBehaviour
    {
        XRRayInteractor m_RayInteractor;
        Vector3 m_OriginalAttachPosition;
        Quaternion m_OriginalAttachRotation;

        void Start()
        {
            m_RayInteractor = GetComponent<XRRayInteractor>();
            if (m_RayInteractor == null)
                return;

            m_RayInteractor.selectEntered.AddListener(CheckRayPreference);
            m_OriginalAttachPosition = m_RayInteractor.attachTransform.localPosition;
            m_OriginalAttachRotation = m_RayInteractor.attachTransform.localRotation;
        }

        void OnDestroy()
        {
            if (m_RayInteractor == null)
                return;
            m_RayInteractor.selectEntered.RemoveListener(CheckRayPreference);
        }

        void CheckRayPreference(SelectEnterEventArgs args)
        {
            if (m_RayInteractor == null)
                return;

            // Check out the interactable for a preference component
            if (args.interactableObject.transform.GetComponent<PreferGrabInteraction>() == null)
                return;

            // Reset the transform to the hand position
            m_RayInteractor.attachTransform.localPosition = m_OriginalAttachPosition;
            m_RayInteractor.attachTransform.localRotation = m_OriginalAttachRotation;
        }
    }
}
