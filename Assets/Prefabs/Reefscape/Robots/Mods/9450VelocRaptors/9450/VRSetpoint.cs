using UnityEngine;

namespace Prefabs.Reefscape.Robots.Mods.TestingMod._9450
{
   [CreateAssetMenu(fileName = "Setpoint", menuName = "Robot/VR Setpoint", order = 0)]
   public class VRSetpoint : ScriptableObject
   {
       [Tooltip("Inches")] public float elevatorHeight;
       [Tooltip("Degrees")] public float elbowAngle;
       [Tooltip("Degrees")] public float diffyPitch;
       [Tooltip("Degrees")] public float diffyRoll;
   }
}
