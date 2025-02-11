using System.Collections.Generic;
using Player.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Raft.Scripts
{
    public sealed class RaftBuildingsManager : MonoBehaviour
    {
        private InputAction _buildModeAction;
        
        private InputAction _mouseLookAction;
        private InputAction _mouseLeftClickAction;
        private bool _mouseLeftClickRequested;
        
        private readonly List<Plane> _planes = new();
        private readonly List<Plane> _planeBlueprints = new();
        
        [SerializeField] private Camera cam;
        
        [SerializeField] private GameObject planePrefab;
        [SerializeField] private GameObject planeBlueprintPrefab;

        private void Awake()
        {
            _mouseLeftClickAction = InputSystem.actions.FindAction("Mouse LeftClick");
            _mouseLookAction = InputSystem.actions.FindAction("Look");
            
            _buildModeAction = InputSystem.actions.FindAction("BuildMode");
            
            var startPlane = PlacePlane(0, 0);
            PlaceBlueprintsAroundPlane(startPlane);
            
            PlayerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Default);
        }

        private void FixedUpdate()
        {
            InteractByMouseLeftClick();
        }

        private void Update()
        {
            BuildModeInput();
            LeftMouseInput();
        }

        private void BuildModeInput()
        {
            if (_buildModeAction.WasPressedThisFrame())
            {
                if (PlayerStateMachine.State != PlayerStateMachine.PlayerState.BuildMode)
                    PlayerStateMachine.ChangeState(PlayerStateMachine.PlayerState.BuildMode);
                else
                    PlayerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Default);
            }
        }

        private void LeftMouseInput()
        {
            if (_mouseLeftClickAction.IsPressed())
                _mouseLeftClickRequested = true;
        }

        private void InteractByMouseLeftClick()
        {
            if (_mouseLeftClickRequested)
            {
                var ray = cam.ScreenPointToRay(_mouseLookAction.ReadValue<Vector2>());
                if (!Physics.Raycast(ray, out var hit)) return;

                if (PlayerStateMachine.State == PlayerStateMachine.PlayerState.BuildMode  &&
                    hit.collider.TryGetComponent(out PlaneBlueprint planeBlueprint))
                {
                    planeBlueprint.BuildPlane(this);
                }
                
                _mouseLeftClickRequested = false;
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private Plane PlacePlane(int xCord, int yCord)
        {
            var planeGameObject = Instantiate(planePrefab, new Vector3(xCord, 0, yCord) * 3, Quaternion.identity);
            planeGameObject.transform.SetParent(transform);
            planeGameObject.name = "Plane(" + xCord + "," + yCord + ")";
            
            var plane = planeGameObject.GetComponent<Plane>();
            plane.xCord = xCord;
            plane.yCord = yCord;
            
            _planes.Add(plane);
            return plane;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void PlaceBlueprintsAroundPlane(Plane plane)
        {
            HashSet<(int, int)> occupiedCells = new();
            foreach (Plane placedPlane in _planes)
            {
                if (placedPlane.xCord >= plane.xCord - 1 && placedPlane.xCord <= plane.xCord + 1 &&
                    placedPlane.yCord >= plane.yCord - 1 && placedPlane.yCord <= plane.yCord + 1)
                    occupiedCells.Add((placedPlane.xCord, placedPlane.yCord));
            }
            foreach (Plane planeBlueprint in _planeBlueprints)
            {
                if (planeBlueprint.xCord >= plane.xCord - 1 && planeBlueprint.xCord <= plane.xCord + 1 &&
                    planeBlueprint.yCord >= plane.yCord - 1 && planeBlueprint.yCord <= plane.yCord + 1)
                    occupiedCells.Add((planeBlueprint.xCord, planeBlueprint.yCord));
            }

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    
                    int worldPlaneXCord = plane.xCord + x;
                    int worldPlaneYCord = plane.yCord + y;
                    
                    if (occupiedCells.Contains((worldPlaneXCord, worldPlaneYCord))) continue;
                    
                    var planeBlueprintGameObject =
                        Instantiate(
                            planeBlueprintPrefab,
                            new Vector3(worldPlaneXCord, 0, worldPlaneYCord) * 3,
                            Quaternion.identity);
                    
                    planeBlueprintGameObject.transform.SetParent(transform);
                    planeBlueprintGameObject.name = "PlaneBlueprint(" + worldPlaneXCord + "," + worldPlaneYCord + ")";
            
                    var planeBlueprint = planeBlueprintGameObject.GetComponent<Plane>();
                    planeBlueprint.xCord = worldPlaneXCord;
                    planeBlueprint.yCord = worldPlaneYCord;
                    
                    _planeBlueprints.Add(planeBlueprint);
                }
            }
        }

        public void BuildPlane(int xCord, int yCord)
        {
            PlaceBlueprintsAroundPlane(PlacePlane(xCord, yCord));
        }
    }
}
