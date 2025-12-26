using Prefabs.Reefscape.Robots.Mods.TestingMod._9450;
using Games.Reefscape.Enums;
using Games.Reefscape.GamePieceSystem;
using Games.Reefscape.Robots;
using MoSimCore.BaseClasses.GameManagement;
using MoSimCore.Enums;
using MoSimLib;
using RobotFramework.Components;
using RobotFramework.Controllers.GamePieceSystem;
using RobotFramework.Controllers.PidSystems;
using RobotFramework.Enums;
using RobotFramework.GamePieceSystem;
using UnityEngine;

namespace Prefabs.Reefscape.Robots._9450
{
    public class VR : ReefscapeRobotBase
    {

        [Header("Components")]
        [SerializeField] private GenericElevator elevator;
        [SerializeField] private GenericJoint elbowArm;
        [SerializeField] private GenericJoint diffyPitch;
        [SerializeField] private GenericJoint diffyRoll;
        [SerializeField] private GenericJoint climberDropBar;
        [SerializeField] private GenericJoint climberMain;

        [SerializeField] private ReefscapeGamePieceIntake coralIntake;
        [SerializeField] private ReefscapeGamePieceIntake algaeIntake;

        [SerializeField] private GamePieceState coralStowState;
        [SerializeField] private GamePieceState algaeStowState;

        private RobotGamePieceController<ReefscapeGamePiece, ReefscapeGamePieceData>.GamePieceControllerNode _coralController;
        private RobotGamePieceController<ReefscapeGamePiece, ReefscapeGamePieceData>.GamePieceControllerNode _algaeController;


        [Header("PIDS")]
        [SerializeField] private PidConstants elbowArmPid;
        [SerializeField] private PidConstants diffyPitchPid;
        [SerializeField] private PidConstants diffyRollPid;
        [SerializeField] private PidConstants climberDropBarPid;
        [SerializeField] private PidConstants climberMainPid;

        [SerializeField] private VRSetpoint stow;
        [SerializeField] private VRSetpoint coralIntakePos;
        [SerializeField] private VRSetpoint algaeIntakePos;
        [SerializeField] private VRSetpoint lolipopCoralIntake;
        [SerializeField] private VRSetpoint lolipopAlgaeIntake;
        [SerializeField] private VRSetpoint l1;
        [SerializeField] private VRSetpoint l2;
        [SerializeField] private VRSetpoint l2Place;
        [SerializeField] private VRSetpoint l3;
        [SerializeField] private VRSetpoint l3Place;
        [SerializeField] private VRSetpoint l4;
        [SerializeField] private VRSetpoint l4Place;
        [SerializeField] private VRSetpoint procPlace;
        [SerializeField] private VRSetpoint lowAlgae;
        [SerializeField] private VRSetpoint highAlgae;
        [SerializeField] private VRSetpoint bargePlace;

        private float _elevatorTargetHeight;
        private float _elbowTargetAngle;
        private float _diffyPitchTargetAngle;
        private float _diffyRollTargetAngle;
        private float _climbDropBarTargetAngle;
        private float _climbMainTargetAngle;




        protected override void Start()
        {
            base.Start();

            elbowArm.SetPid(elbowArmPid);
            diffyPitch.SetPid(diffyPitchPid);
            diffyRoll.SetPid(diffyRollPid);
            climberDropBar.SetPid(climberDropBarPid);
            climberMain.SetPid(climberMainPid);

            _elevatorTargetHeight = 0;
            _elbowTargetAngle = 0;
            _diffyPitchTargetAngle = 0;
            _diffyRollTargetAngle = 0;
            _climbDropBarTargetAngle = 0;
            _climbMainTargetAngle = 180;

            RobotGamePieceController.SetPreload(coralStowState);
            _coralController = RobotGamePieceController.GetPieceByName(ReefscapeGamePieceType.Coral.ToString());
            _algaeController = RobotGamePieceController.GetPieceByName(ReefscapeGamePieceType.Algae.ToString());

            _coralController.gamePieceStates = new[]
            {
            coralStowState
            };
            _coralController.intakes.Add(coralIntake);

            _algaeController.gamePieceStates = new[] { algaeStowState };
            _algaeController.intakes.Add(algaeIntake);

        }

        private void LateUpdate()
        {
            elbowArm.UpdatePid(elbowArmPid);
            diffyPitch.UpdatePid(diffyPitchPid);
            diffyRoll.UpdatePid(diffyRollPid);
            climberDropBar.UpdatePid(climberDropBarPid);
            climberMain.UpdatePid(climberMainPid);
        }


        private void FixedUpdate()
        {
            bool hasAlgae = _algaeController.HasPiece();
            bool hasCoral = _coralController.HasPiece();

            _algaeController.SetTargetState(algaeStowState);
            _coralController.SetTargetState(coralStowState);



            UpdateSetpoints();

            switch (CurrentSetpoint)
            {
                case ReefscapeSetpoints.Stow:
                    SetSetpoint(stow);
                    break;
                case ReefscapeSetpoints.Intake:
                    _algaeController.RequestIntake(algaeIntake, CurrentRobotMode == ReefscapeRobotMode.Algae && !hasAlgae);
                    _coralController.RequestIntake(coralIntake, CurrentRobotMode == ReefscapeRobotMode.Coral && !hasCoral);

                    if (CurrentRobotMode == ReefscapeRobotMode.Coral) {
                        SetSetpoint(coralIntakePos);
                    } else {
                        SetSetpoint(algaeIntakePos);
                    }   
                    break;
                case ReefscapeSetpoints.Place:
                    PlacePiece();
                    break;
                case ReefscapeSetpoints.L1:
                    SetSetpoint(l1);
                    break;

                case ReefscapeSetpoints.Stack:
                    _algaeController.RequestIntake(algaeIntake,!hasCoral && !hasAlgae);
                    _coralController.RequestIntake(coralIntake, false);

                    SetSetpoint(lolipopAlgaeIntake);
                    break;
                case ReefscapeSetpoints.L2:
                    SetSetpoint(l2);
                    break;
                case ReefscapeSetpoints.LowAlgae:
                    _algaeController.RequestIntake(algaeIntake, !hasCoral && !hasAlgae);
                    _coralController.RequestIntake(coralIntake, false);

                    SetSetpoint(lowAlgae);
                    break;
                case ReefscapeSetpoints.L3:
                    SetSetpoint(l3);
                    break;
                case ReefscapeSetpoints.HighAlgae:
                    _algaeController.RequestIntake(algaeIntake, !hasCoral && !hasAlgae);
                    _coralController.RequestIntake(coralIntake, false);

                    SetSetpoint(highAlgae);
                    break;
                case ReefscapeSetpoints.L4:
                    SetSetpoint(l4);
                    break;
                case ReefscapeSetpoints.Processor:
                    SetSetpoint(procPlace);
                    break;
                case ReefscapeSetpoints.Barge:
                    SetSetpoint(bargePlace);
                    break;
                case ReefscapeSetpoints.RobotSpecial:
                        _algaeController.RequestIntake(algaeIntake, false);
                        _coralController.RequestIntake(coralIntake, !hasCoral && !hasAlgae);

                        SetSetpoint(lolipopCoralIntake);
                    break;
                case ReefscapeSetpoints.Climb:
                    SetSetpoint(stow);
                    _climbDropBarTargetAngle = -50;
                    _climbMainTargetAngle = 70;
                    break;
                case ReefscapeSetpoints.Climbed:
                    SetSetpoint(stow);
                    _climbMainTargetAngle = 165;
                    break;
                default:
                    SetSetpoint(stow);
                    break;
            }
        }

        private void PlacePiece()
        {
            if (_algaeController.HasPiece()) {
                if (LastSetpoint == ReefscapeSetpoints.Barge) {
                    _algaeController.ReleaseGamePieceWithForce(new Vector3(0, -4.5f, 0));
                } else
                {
                    _algaeController.ReleaseGamePieceWithForce(new Vector3(0, -3f, 0));
                }
            } else {
                switch (LastSetpoint)
                {
                    case ReefscapeSetpoints.L1:
                        _coralController.ReleaseGamePieceWithForce(new Vector3(0, -1.25f, 0));
                        break;

                    case ReefscapeSetpoints.RobotSpecial:
                        _coralController.ReleaseGamePieceWithForce(new Vector3(0, -2f, 0));
                        break;

                    case ReefscapeSetpoints.L2:
                        SetSetpoint(l2Place);
                        _coralController.ReleaseGamePieceWithForce(new Vector3(0, 0, 2f));
                        break;

                    case ReefscapeSetpoints.L3:
                        SetSetpoint(l3Place);
                        _coralController.ReleaseGamePieceWithForce(new Vector3(0, 0, 2f));
                        break;

                    case ReefscapeSetpoints.L4:
                        SetSetpoint(l4Place);
                        _coralController.ReleaseGamePieceWithForce(new Vector3(0, 0, 10f));
                        break;
                }
            }
        }


        private void SetSetpoint(VRSetpoint setpoint)
        {
            _elevatorTargetHeight = setpoint.elevatorHeight;
            _elbowTargetAngle = setpoint.elbowAngle;
            _diffyPitchTargetAngle = setpoint.diffyPitch;
            _diffyRollTargetAngle = setpoint.diffyRoll;
        }

        private void UpdateSetpoints()
        {
            elevator.SetTarget(_elevatorTargetHeight);
            elbowArm.SetTargetAngle(_elbowTargetAngle).withAxis(JointAxis.X);
            diffyPitch.SetTargetAngle(_diffyPitchTargetAngle).withAxis(JointAxis.X);
            diffyRoll.SetTargetAngle(_diffyRollTargetAngle).withAxis(JointAxis.Y);
            climberDropBar.SetTargetAngle(_climbDropBarTargetAngle).withAxis(JointAxis.X);
            climberMain.SetTargetAngle(_climbMainTargetAngle).withAxis(JointAxis.X);
        }




    }
}
