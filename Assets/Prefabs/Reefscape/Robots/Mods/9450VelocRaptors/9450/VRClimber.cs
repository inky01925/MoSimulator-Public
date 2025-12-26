using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Prefabs.Reefscape.Robots._9450
{
    public class VRClimber : MonoBehaviour
    {

        [Header("Trigger Colliders")]
        public VRClimberHelper Trigger1;
        public VRClimberHelper Trigger2;

        [Header("Colliders to Enable")]
        public Collider[] hitBoxesToEnable;

        private bool _firstTriggered;
        private bool _secondTriggered;

        
        private void Start()
        {
            foreach (Collider elem in hitBoxesToEnable)
            {
                elem.enabled = false;
            }
        }

        public void NotifyTriggered(VRClimberHelper helper, bool isTriggered)
        {
            if (helper == Trigger1)
                _firstTriggered = isTriggered;

            if (helper == Trigger2)
                _secondTriggered = isTriggered;

            TryEnablingColliders();
        }

        private void TryEnablingColliders()
        {
            if (_firstTriggered && _secondTriggered) {
                foreach (Collider elem in hitBoxesToEnable)
                {
                    elem.enabled = true;
                }
            } else
            {
                foreach (Collider elem in hitBoxesToEnable)
                {
                    elem.enabled = false;
                }
            }
        }
    }
}
