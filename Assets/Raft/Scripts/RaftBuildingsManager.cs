using System.Collections.Generic;
using System.Linq;
using Inventory;
using Player.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Raft.Scripts
{
    public sealed class RaftBuildingsManager : MonoBehaviour
    {
        private const int DefaultHealth = 20;
        
        private InputAction _buildModeAction;
        
        private InputAction _mouseLookAction;
        private InputAction _mouseLeftClickAction;
        private bool _mouseLeftClickRequested;

        private List<Building> _buildings;
        private List<Plane> _planes;
        private List<Plane> _planeBlueprints;
        private Plane _corePlain;
        
        [SerializeField] private EntityInventory inventory;
        
        [SerializeField] private Camera cam;
        
        [SerializeField] private GameObject planePrefab;
        [SerializeField] private GameObject planeBlueprintPrefab;
        [FormerlySerializedAs("mainCorePrefab")] [SerializeField] private GameObject corePlanePrefab;
        [Space]
        [SerializeField] private List<BuildingPrefabConfig> buildingPrefabs;

        private void Awake()
        {
            _buildings = new List<Building>();
            _planes = new List<Plane>();
            _planeBlueprints = new List<Plane>();
            
            _mouseLeftClickAction = InputSystem.actions.FindAction("Mouse LeftClick");
            _mouseLookAction = InputSystem.actions.FindAction("Look");
            
            _buildModeAction = InputSystem.actions.FindAction("BuildMode");
            
            PlayerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Default);
        }

        private void Start()
        {
            //Give start items
            inventory.AddItem("Wood", 9);
            
            //Place start plane
            var startPlane = PlacePlane(0, 0, isCorePlain: true);
            PlaceBlueprintsAroundPlane(startPlane);
            SetBlueprintsActive(false);
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
                    planeBlueprint.BuildPlane();
                }
                
                _mouseLeftClickRequested = false;
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private Plane PlacePlane(PlaneBlueprint blueprint, int maxHealth = DefaultHealth, int currentHealth = DefaultHealth, bool isCorePlain = false)
        {
            var xCoord = blueprint.xCoord;
            var yCoord = blueprint.yCoord;
            
            RemovePlaneBlueprintFromList(blueprint);
            Destroy(blueprint.gameObject);
            
            var planeGameObject = Instantiate(isCorePlain ? corePlanePrefab : planePrefab, new Vector3(xCoord, 0, yCoord) * 3, Quaternion.identity);
            planeGameObject.transform.SetParent(transform);
            planeGameObject.name = isCorePlain ?
                "CorePlane" :
                "Plane(" + xCoord + "," + yCoord + ")";
            
            var plane = planeGameObject.GetComponent<Plane>();
            plane.xCoord = xCoord;
            plane.yCoord = yCoord;
            plane.maxHealth = maxHealth;
            plane.CurrentHealth = currentHealth;
            plane.SetBuildingManager(this);
            plane.buildingType = isCorePlain ?
                BuildingType.CorePlane :
                BuildingType.Plane;
            
            _planes.Add(plane);
            _buildings.Add(plane);
            if (isCorePlain) _corePlain = plane;
            return plane;
        }

        private Plane PlacePlane(int xCoord, int yCoord, int maxHealth = DefaultHealth, int currentHealth = DefaultHealth, bool isCorePlain = false)
        {
            //If Blueprint with it coords exists
            foreach (var blueprint in _planeBlueprints)
            {
                if (blueprint.xCoord == xCoord && blueprint.yCoord == yCoord)
                {
                    return PlacePlane(
                        blueprint.GetComponent<PlaneBlueprint>(),
                        maxHealth,
                        currentHealth,
                        isCorePlain);
                }
            }
            
            //Else
            var planeBlueprint = PlacePlaneBlueprint(xCoord, yCoord);
            
            return PlacePlane(
                planeBlueprint,
                maxHealth,
                currentHealth,
                isCorePlain);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void PlaceBlueprintsAroundPlane(Plane plane)
        {
            HashSet<(int, int)> occupiedCells = new();
            foreach (var checkingPlane in CheckNeighbourPlanes(plane))
            {
                occupiedCells.Add((checkingPlane.xCoord, checkingPlane.yCoord));
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

                    PlacePlaneBlueprint(worldPlaneXCoord, worldPlaneYCoord);
                }
            }
        }

        public PlaneBlueprint PlacePlaneBlueprint(int xCoord, int yCoord)
        {
            var planeBlueprintGameObject =
                Instantiate(
                    planeBlueprintPrefab,
                    new Vector3(xCoord, 0, yCoord) * 3,
                    Quaternion.identity);
                    
            planeBlueprintGameObject.transform.SetParent(transform);
            planeBlueprintGameObject.name = "PlaneBlueprint(" + xCoord + "," + yCoord + ")";
            
            var planeBlueprint = planeBlueprintGameObject.GetComponent<PlaneBlueprint>();
            planeBlueprint.xCoord = xCoord;
            planeBlueprint.yCoord = yCoord;
            planeBlueprint.SetBuildingManager(this);
            planeBlueprint.buildingType = BuildingType.PlaneBlueprint;
            planeBlueprint.maxHealth = int.MaxValue;
            planeBlueprint.CurrentHealth = int.MaxValue;
                    
            _planeBlueprints.Add(planeBlueprint);
            _buildings.Add(planeBlueprint);
            
            return planeBlueprint;
        }

        private List<Plane> CheckNeighbourPlanes(Plane plane)
        {
            List<Plane> neighbourCells = new();
            
            foreach (Plane placedPlane in _planes)
            {
                if (placedPlane.xCoord >= plane.xCoord - 1 && placedPlane.xCoord <= plane.xCoord + 1 &&
                    placedPlane.yCoord >= plane.yCoord - 1 && placedPlane.yCoord <= plane.yCoord + 1)
                    neighbourCells.Add(placedPlane);
            }
            foreach (Plane placedPlaneBlueprint in _planeBlueprints)
            {
                if (placedPlaneBlueprint.xCoord >= plane.xCoord - 1 && placedPlaneBlueprint.xCoord <= plane.xCoord + 1 &&
                    placedPlaneBlueprint.yCoord >= plane.yCoord - 1 && placedPlaneBlueprint.yCoord <= plane.yCoord + 1)
                    neighbourCells.Add(placedPlaneBlueprint);
            }
            
            foreach (Plane neighbour in neighbourCells.ToList())
            {
                var x = neighbour.xCoord - plane.xCoord;
                var y = neighbour.yCoord - plane.yCoord;

                if (Mathf.Approximately(Mathf.Abs(x), Mathf.Abs(y)))
                {
                    neighbourCells.Remove(neighbour);
                }
            }
            
            return neighbourCells;
        }

        public void BuildPlane(PlaneBlueprint blueprint, int maxHealth = DefaultHealth, int currentHealth = DefaultHealth)
        {
            if (inventory.GetItemCount("Wood") >= 4)
            {
                PlaceBlueprintsAroundPlane(PlacePlane(
                    blueprint,
                    maxHealth,
                    currentHealth));
                inventory.RemoveItem("Wood", 4);
            }
        }

        public void DestroyPlane(Plane plane)
        {
            DestroyPlaneNeighbourBlueprints(plane);
            
            var neighbourPlanes = CheckNeighbourPlanes(plane);
            bool isToFullDestory = true;
            foreach (var neighbourPlane in neighbourPlanes)
            {
                if (!(neighbourPlane is PlaneBlueprint)) isToFullDestory = false;
            }

            RemovePlaneFromList(plane);
            Destroy(plane.gameObject);
            if (!isToFullDestory) PlacePlaneBlueprint(plane.xCoord, plane.yCoord);
        }

        private void DestroyPlaneNeighbourBlueprints(Plane plane)
        {
            var neighbourPlanes = CheckNeighbourPlanes(plane);

            foreach (var neighbourPlane in neighbourPlanes)
            {
                if (neighbourPlane is PlaneBlueprint)
                {
                    var blueprintNeighbours = CheckNeighbourPlanes(neighbourPlane);
                    var isBlueprintedToDestroy = true;
                    foreach (var blueprintNeighbour in blueprintNeighbours)
                    {
                        if (blueprintNeighbour == plane) continue;
                        if (blueprintNeighbour is PlaneBlueprint) continue;
                        isBlueprintedToDestroy = false;
                        break;
                    }

                    if (isBlueprintedToDestroy)
                    {
                        Destroy(neighbourPlane.gameObject);
                    }
                }
            }
        }

        public void RemovePlaneFromList(Plane plane)
        {
            _planes.Remove(plane);
            _buildings.Remove(plane);
        }

        public void RemovePlaneBlueprintFromList(PlaneBlueprint blueprint)
        {
            _planeBlueprints.Remove(blueprint);
            _buildings.Remove(blueprint);
        }

        public void RemoveBuildingFromList(Building building)
            => _buildings.Remove(building);
    }
}
