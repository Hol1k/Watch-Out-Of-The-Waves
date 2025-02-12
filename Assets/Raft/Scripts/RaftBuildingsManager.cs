using System;
using System.Collections.Generic;
using Inventory;
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
        
        [SerializeField] private EntityInventory inventory;
        
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
            SetBlueprintsActive(false);
        }

        private void Start()
        {
            inventory.AddItem("Wood", 9);
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
                {
                    PlayerStateMachine.ChangeState(PlayerStateMachine.PlayerState.BuildMode);
                    SetBlueprintsActive(true);
                }
                else
                {
                    PlayerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Default);
                    SetBlueprintsActive(false);
                }
            }
        }

        private void SetBlueprintsActive(bool value)
        {
            foreach (var blueprint in _planeBlueprints)
            {
                blueprint.gameObject.SetActive(value);
            }
        }

        private void LeftMouseInput()
        {
            if (_mouseLeftClickAction.WasPerformedThisFrame())
                _mouseLeftClickRequested = true;
        }

        private void InteractByMouseLeftClick()
        {
            if (_mouseLeftClickRequested)
            {
                var ray = cam.ScreenPointToRay(_mouseLookAction.ReadValue<Vector2>());
                if (!Physics.Raycast(ray, out var hit))
                {
                    _mouseLeftClickRequested = false;
                    return;
                }

                if (PlayerStateMachine.State == PlayerStateMachine.PlayerState.BuildMode &&
                    hit.collider.TryGetComponent(out PlaneBlueprint planeBlueprint))
                {
                    planeBlueprint.BuildPlane(this);
                }
                
                _mouseLeftClickRequested = false;
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private Plane PlacePlane(PlaneBlueprint blueprint)
        {
            var xCoord = blueprint.xCoord;
            var yCoord = blueprint.yCoord;
            
            _planeBlueprints.Remove(blueprint);
            Destroy(blueprint.gameObject);
            
            var planeGameObject = Instantiate(planePrefab, new Vector3(xCoord, 0, yCoord) * 3, Quaternion.identity);
            planeGameObject.transform.SetParent(transform);
            planeGameObject.name = "Plane(" + xCoord + "," + yCoord + ")";
            
            var plane = planeGameObject.GetComponent<Plane>();
            plane.xCoord = xCoord;
            plane.yCoord = yCoord;
            
            _planes.Add(plane);
            return plane;
        }

        private Plane PlacePlane(int xCoord, int yCoord)
        {
            //If Blueprint with it coords exists
            foreach (var blueprint in _planeBlueprints)
            {
                if (blueprint.xCoord == xCoord && blueprint.yCoord == yCoord)
                {
                    return PlacePlane(blueprint.GetComponent<PlaneBlueprint>());
                }
            }
            
            //Else
            var planeGameObject = Instantiate(planeBlueprintPrefab, new Vector3(xCoord, 0, yCoord) * 3, Quaternion.identity);
            planeGameObject.transform.SetParent(transform);
            
            return PlacePlane(planeGameObject.GetComponent<PlaneBlueprint>());
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void PlaceBlueprintsAroundPlane(Plane plane)
        {
            HashSet<(int, int)> occupiedCells = new();
            foreach (Plane placedPlane in _planes)
            {
                if (placedPlane.xCoord >= plane.xCoord - 1 && placedPlane.xCoord <= plane.xCoord + 1 &&
                    placedPlane.yCoord >= plane.yCoord - 1 && placedPlane.yCoord <= plane.yCoord + 1)
                    occupiedCells.Add((placedPlane.xCoord, placedPlane.yCoord));
            }
            foreach (Plane planeBlueprint in _planeBlueprints)
            {
                if (planeBlueprint.xCoord >= plane.xCoord - 1 && planeBlueprint.xCoord <= plane.xCoord + 1 &&
                    planeBlueprint.yCoord >= plane.yCoord - 1 && planeBlueprint.yCoord <= plane.yCoord + 1)
                    occupiedCells.Add((planeBlueprint.xCoord, planeBlueprint.yCoord));
            }

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    if (Mathf.Approximately(Mathf.Abs(x), Mathf.Abs(y))) continue;
                    
                    int worldPlaneXCoord = plane.xCoord + x;
                    int worldPlaneYCoord = plane.yCoord + y;
                    
                    if (occupiedCells.Contains((worldPlaneXCoord, worldPlaneYCoord))) continue;
                    
                    var planeBlueprintGameObject =
                        Instantiate(
                            planeBlueprintPrefab,
                            new Vector3(worldPlaneXCoord, 0, worldPlaneYCoord) * 3,
                            Quaternion.identity);
                    
                    planeBlueprintGameObject.transform.SetParent(transform);
                    planeBlueprintGameObject.name = "PlaneBlueprint(" + worldPlaneXCoord + "," + worldPlaneYCoord + ")";
            
                    var planeBlueprint = planeBlueprintGameObject.GetComponent<Plane>();
                    planeBlueprint.xCoord = worldPlaneXCoord;
                    planeBlueprint.yCoord = worldPlaneYCoord;
                    
                    _planeBlueprints.Add(planeBlueprint);
                }
            }
        }

        public void BuildPlane(PlaneBlueprint blueprint)
        {
            if (inventory.GetItemCount("Wood") >= 4)
            {
                PlaceBlueprintsAroundPlane(PlacePlane(blueprint));
                inventory.RemoveItem("Wood", 4);
            }
        }
    }
}
