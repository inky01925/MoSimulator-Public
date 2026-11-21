using UnityEngine;
using UnityEngine.Serialization;

namespace Prefabs.Reefscape.Robots._9450
{
    public class VRClimberHelper : MonoBehaviour
    {

        public VRClimber mainClimber;        
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Cage") {
                mainClimber.NotifyTriggered(this, true);
            } else
            {
                mainClimber.NotifyTriggered(this, false);
            }
        }

        private void OnTriggerExit(Collider other) {
            mainClimber.NotifyTriggered(this, false);
        }
    }
}
