using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Core;
using Assets.Scripts.Core.Model;
using Assets.Scripts.Core.Model.Inventories;
using Assets.Scripts.Prefabs.Menus;
using Assets.Scripts.Prefabs.Menus.Inventory;
using Assets.Scripts.UI;

using GameFrame.Core.Extensions;
using GameFrame.Core.Math;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Scenes.GameScene
{
    public class WorldBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera;
        [SerializeField]
        private CameraBehaviour CameraBehaviour;

        [SerializeField]
        private GroundBehaviour TileTemplate;
        [SerializeField]
        private ShaftBehaviour ShaftTemplate;
        [SerializeField]
        private ShaftBehaviour GroundLevelShaftTemplate;
        [SerializeField]
        private SiteBehaviour SiteTemplate;
        [SerializeField]
        private DiggerBehaviour DiggerTemplate;
        [SerializeField]
        private TransportSiteBehaviour TransportSiteTemplate;
        [SerializeField]
        private TransportBehaviour TransportTemplate;
        [SerializeField]
        private GameObject TilesParent;

        [SerializeField]
        private GameObject Headquarters;

        [SerializeField]
        private GameObject DepositoryTemplate;
        [SerializeField]
        private GameObject DepositoryContainer;

        [SerializeField]
        private InventoryMenuBehaviour inventoryMenuBehaviour;

        [SerializeField]
        private ShopMenuBehaviour shop;

        private readonly Map<Int32, TileBehaviour> tileMap = new Map<Int32, TileBehaviour>();
        private readonly Map<Int32, List<IStorage>> storages = new Map<Int32, List<IStorage>>();

        private readonly List<ShaftBehaviour> Shafts = new();

        private readonly List<SiteBehaviour> Sites = new();
        private readonly List<TransportSiteBehaviour> TransportSites = new();
        private readonly Dictionary<String, DepositoryBehaviour> depositories = new Dictionary<String, DepositoryBehaviour>();

        private TileGenerator tileGenerator;
        private TransportInventoryItem selectedTransport;

        private InputAction touchPressed;
        private InputAction touchPosition;

        private void Awake()
        {
            Base.Core.Game.ExecuteAfterInstantation(OnGameInitialized);

            touchPosition = InputSystem.actions.FindAction("TouchPosition");
            touchPressed = InputSystem.actions.FindAction("TouchPressed");
        }

        private void OnEnable()
        {
            HookActions();
        }

        private void OnDisable()
        {
            UnhookActions();
        }

        public void DisplayPosibleDigSites()
        {
            //var miningTool = Base.Core.Game.State.Inventory.MiningTools.FirstOrDefault();
            //DisplayPosibleDigSites(miningTool);
        }

        public Map<int, Digger> GenerateDiggerMap()
        {
            Map<int, Digger> diggerMap = new();

            foreach (var digger in Base.Core.Game.State.ActiveDiggers)
            {
                diggerMap[digger.Position.X, digger.Position.Y] = digger;
            }

            return diggerMap;
        }

        public void OnInventoryItemSelected(InventoryItem inventoryItem)
        {
            ClearTransportSites();
            ClearDigSites();

            switch (inventoryItem)
            {
                case MiningToolInventoryItem miningToolInventoryItem:
                    DisplayPosibleDigSites(miningToolInventoryItem.MiningTool);
                    break;

                case TransportInventoryItem transportInventoryItem:

                    if (transportInventoryItem.IsVertical)
                    {
                        DisplayPossibleVerticalTransportSites(transportInventoryItem);
                    }
                    else
                    {
                        DisplayPossibleHorizontalTransportSites(transportInventoryItem);
                    }

                    break;
            }
        }

        public void DisplayPosibleDigSites(MiningTool miningTool)
        {
            if (Sites.Count > 0)
            {
                ClearDigSites();
                return;
            }

            Map<int, Digger> diggerMap = GenerateDiggerMap();

            foreach (var shaft in Shafts)
            {
                if (GetTileRelative(shaft.GetPosition(), 0, 1, out var beneath))
                {
                    if (beneath.IsDigable() == true && !diggerMap.TryGetValue(shaft.GetPosition().X, shaft.GetPosition().Y, out _))
                    {
                        if (miningTool.Size.X < 2)
                        {
                            if (GetTileRelative(shaft.GetPosition(), -1, 0, out var leftTile))
                            {
                                if (leftTile.IsDigable() == true)
                                {
                                    CreateSite(shaft.GetPosition(), miningTool, Direction.Left);
                                }
                            }

                            if (GetTileRelative(shaft.GetPosition(), 1, 0, out var rightTile))
                            {
                                if (rightTile.IsDigable() == true)
                                {
                                    CreateSite(shaft.GetPosition(), miningTool, Direction.Right);
                                }
                            }

                            if (GetTileRelative(shaft.GetPosition(), 0, 1, out var bottomTile))
                            {
                                if (bottomTile.IsDigable() == true)
                                {
                                    CreateSite(shaft.GetPosition(), miningTool, Direction.Down);
                                }
                            }
                        }
                    }
                }
            }

            //for (var x = 0; x < Base.Core.Game.State.World.Width; x++)
            //{
            //    var tile = tileMap[x, 0];
            //    if (tile.IsDigable() && !diggerMap.TryGetValue(tile.GetPosition().X, tile.GetPosition().Y, out _))
            //    {
            //        Point2 pos = new Point2(x, -1);
            //        CreateSite(pos, miningTool, Direction.Down);
            //    }
            //}
        }

        public void AddDigSite(Digger digger)
        {
            var inventoryItem = Base.Core.Game.State.Inventory.MiningTools.FirstOrDefault(m => m.MiningTool.Reference == digger.MiningTool.Reference);

            if (inventoryItem != default)
            {
                inventoryItem.Amount -= 1;

                Base.Core.Game.State.ActiveDiggers.Add(digger);
                SpawnDigSite(digger);

                if (inventoryItem.Amount > 0)
                {
                    DisplayPosibleDigSites(digger.MiningTool);
                }
            }
        }

        public void SpawnDigSite(Digger digger)
        {
            var newDigger = GameObject.Instantiate(DiggerTemplate, TilesParent.transform);

            newDigger.Init(this, digger);
            newDigger.UpdatePosition();
            newDigger.gameObject.SetActive(true);

            ClearDigSites();
        }

        private void ClearDigSites()
        {
            foreach (var site in Sites)
            {
                GameObject.Destroy(site.gameObject);
            }

            Sites.Clear();
        }

        private void CreateSite(Point2 pos, MiningTool miningTool, Direction direction)
        {
            UnityEngine.Vector3 position;
            Quaternion rotation;

            switch (direction)
            {
                case Direction.Down:
                    position = GetUnityVector(pos, SiteTemplate.transform.position.z, yOffset: (1 - SiteTemplate.transform.localScale.y) / 2);
                    rotation = SiteTemplate.transform.rotation;
                    break;

                case Direction.Left:
                    position = GetUnityVector(pos, SiteTemplate.transform.position.z, xOffset: -(1 - SiteTemplate.transform.localScale.y) / 2);
                    rotation = SiteTemplate.transform.rotation;
                    rotation *= Quaternion.Euler(0, 0, -90);
                    break;

                case Direction.Right:
                    position = GetUnityVector(pos, SiteTemplate.transform.position.z, xOffset: (1 - SiteTemplate.transform.localScale.y) / 2);
                    rotation = SiteTemplate.transform.rotation;
                    rotation *= Quaternion.Euler(0, 0, 90);
                    break;
                default:
                    return;
            }

            var newSite = GameObject.Instantiate(SiteTemplate, position, rotation, TilesParent.transform);
            newSite.Init(this, pos, miningTool, direction);
            newSite.gameObject.SetActive(true);
            newSite.gameObject.name = "DigSite_" + pos.X + "," + pos.Y;

            Sites.Add(newSite);
        }

        public void DisplayPossibleHorizontalTransportSites()
        {
            var transport = Base.Core.Game.State.Inventory.HorizontalTransports.FirstOrDefault();
            DisplayPossibleHorizontalTransportSites(transport);
        }

        private void DisplayPossibleHorizontalTransportSites(TransportInventoryItem transport)
        {
            if (TransportSites.Count > 0)
            {
                ClearTransportSites();

                if (selectedTransport == transport)
                {
                    return;
                }
            }

            selectedTransport = transport;
            foreach (var shaft in Shafts)
            {
                if (shaft.HasTransport() 
                    || (GetTileRelative(shaft.GetPosition(), 0, 1, out var tile) == true && tile.IsDigable() == false))
                {
                    continue;
                }

                GenerateTransportSite(transport, shaft, Direction.Left);
                GenerateTransportSite(transport, shaft, Direction.Right);
            }
        }

        private void GenerateTransportSite(TransportInventoryItem transport, ShaftBehaviour shaft, Direction direction)
        {
            var newSite = GameObject.Instantiate(TransportSiteTemplate, TilesParent.transform);
            newSite.Init(this, transport.Transport, shaft, direction);
            var pos = shaft.GetPosition();
            newSite.gameObject.name = "TransportSite_" + pos.X + "," + pos.Y;
            newSite.gameObject.SetActive(true);
            TransportSites.Add(newSite);
        }

        public void DisplayPossibleVerticalTransportSites()
        {
            var transport = Base.Core.Game.State.Inventory.VerticalTransports.FirstOrDefault();
            DisplayPossibleVerticalTransportSites(transport);
        }

        private void DisplayPossibleVerticalTransportSites(TransportInventoryItem transport)
        {
            if (TransportSites.Count > 0)
            {
                ClearTransportSites();
                if (selectedTransport == transport)
                {
                    return;
                }
            }

            selectedTransport = transport;

            foreach (var shaft in Shafts)
            {
                if (shaft.HasTransport())
                {
                    continue;
                }
                var direction = Direction.Down;
                GenerateTransportSite(transport, shaft, direction);
            }
        }

        public TransportBehaviour GenerateTransportBehaviour(ShaftBehaviour shaftBehaviour, Transport transport, Direction direction)
        {
            var transportBehaviour = GameObject.Instantiate(TransportTemplate, TilesParent.transform);
            var p = shaftBehaviour.transform.position;
            transportBehaviour.transform.position = new UnityEngine.Vector3(p.x, p.y, TransportTemplate.transform.position.z);
            transportBehaviour.Init(this, shaftBehaviour, transport, direction);
            var pos = shaftBehaviour.GetPosition();
            transportBehaviour.gameObject.name = "Transport" + pos.X + "," + pos.Y;
            transportBehaviour.gameObject.SetActive(true);
            return transportBehaviour;
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                UnityEngine.Vector2 mousePosition = Mouse.current.position.ReadValue();
                //Debug.Log("OnClick: " + mousePosition);
                EvalutePress(mousePosition);
            }
        }


        private void OnTouched(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                UnityEngine.Vector2 position = touchPosition.ReadValue<UnityEngine.Vector2>();
                //Debug.Log("OnTouched: " + position);
                EvalutePress(position);
            }
        }
        private void EvalutePress(UnityEngine.Vector2 position)
        {
            Ray ray = mainCamera.ScreenPointToRay(position);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject clickedObject = hit.collider.gameObject;

                if (clickedObject.TryGetComponent<IClickable>(out var clickable))
                {
                    clickable?.OnClicked();
                }
            }
        }

        private void OnGameInitialized()
        {
            shop.OnShopToggled.AddListener(OnShopToggeled);

            inventoryMenuBehaviour.PointerEntered.AddListener(() => UnhookActions());
            inventoryMenuBehaviour.PointerExited.AddListener(() => HookActions());

            Base.Core.Game.OnPauseToggled.AddListener(OnPauseToggled);

            GenerateWorld();
        }

        private void OnShopToggeled(Boolean isOpen)
        {
            if (isOpen)
            {
                UnhookActions();
                this.inventoryMenuBehaviour.DisableButtons();
                this.CameraBehaviour.UnhookActions();
            }
            else
            {
                HookActions();
                this.inventoryMenuBehaviour.gameObject.SetActive(true);
                this.inventoryMenuBehaviour.EnableButtons();
                this.CameraBehaviour.HookActions();
            }
        }

        private void OnPauseToggled(Boolean isPaused)
        {
            if (isPaused)
            {
                UnhookActions();
                this.inventoryMenuBehaviour.DisableButtons();
                this.CameraBehaviour.UnhookActions();
            }
            else
            {
                HookActions();
                this.inventoryMenuBehaviour.gameObject.SetActive(true);
                this.inventoryMenuBehaviour.EnableButtons();
                this.CameraBehaviour.HookActions();
            }
        }

        private void GenerateWorld()
        {
            var gameState = Base.Core.Game.State;
            var world = gameState.World;

            tileGenerator = new TileGenerator(world);

            Headquarters.transform.position = world.Headquarters.Position.ToUnityVector3(Headquarters.transform.position.z);
            CameraBehaviour.GotoToPosition(Headquarters.transform.position);

            foreach (var tile in world.Tiles)
            {
                if (tile.DigingProgress >= 1)
                {
                    CreateShaft(tile.Position, ShaftTemplate);
                }
                else
                {
                    SpawnGround(tile);
                }
            }

            CameraBehaviour.OnBoundariesChanged(world.Width, world.MaxDepth);

            foreach (var depository in world.Depositories)
            {
                SpawnDepository(depository);
            }

            for (int i = 0; i < world.Width; i++)
            {
                CreateShaft(new Point2(i, -1), GroundLevelShaftTemplate);
            }

            foreach (var digger in gameState.ActiveDiggers)
            {
                SpawnDigSite(digger);
            }

            foreach (var transporter in gameState.ActiveTransporters)
            {

            }
        }

        private TileBehaviour SpawnGround(Tile tile)
        {
            var pos = tile.Position;

            var position = GetUnityVector(pos, TileTemplate.transform.position.z);

            var groundTile = GameObject.Instantiate(TileTemplate, position, TileTemplate.transform.rotation, TilesParent.transform);

            groundTile.name = "Tile_" + pos.X + "," + pos.Y;

            groundTile.Init(this, tile);

            groundTile.gameObject.SetActive(true);

            tileMap[pos.X, pos.Y] = groundTile;

            return groundTile;
        }

        public void ReplaceTile(TileBehaviour tile)
        {
            CreateShaft(tile.GetPosition(), ShaftTemplate);
            GameObject.Destroy(tile.gameObject);
        }

        private void CreateShaft(Point2 pos, ShaftBehaviour shaftTemplate)
        {
            var position = GetUnityVector(pos, shaftTemplate.transform.position.z);

            var shaftBehaviour = GameObject.Instantiate(shaftTemplate, position, shaftTemplate.transform.rotation, TilesParent.transform);

            shaftBehaviour.gameObject.name = shaftTemplate.gameObject.name + "_" + pos.X + "," + pos.Y; ;

            shaftBehaviour.Init(this, pos);

            Shafts.Add(shaftBehaviour);
            shaftBehaviour.gameObject.SetActive(true);

            tileMap[pos.X, pos.Y] = shaftBehaviour;
        }

        private void SpawnDepository(Depository depository)
        {
            var position = GetUnityVector(depository.Position, DepositoryTemplate.transform.position.z);

            var depositoryGameObject = Instantiate(DepositoryTemplate, position, DepositoryTemplate.transform.rotation, DepositoryContainer.transform);

            var depositoryBehaviour = depositoryGameObject.GetComponent<DepositoryBehaviour>();
            if (depositoryGameObject.transform.Find("PopupMenu").TryGetComponent<MouseEventBehaviour>(out var mouseEventBehaviour))
            {
                mouseEventBehaviour.PointerEntered.AddListener(UnhookActions);
                mouseEventBehaviour.PointerExited.AddListener(HookActions);
            }

            MoneyBehaviour sceneMoneyBehaviour = FindFirstObjectByType<MoneyBehaviour>();
            depositoryBehaviour.SetMoneyBehaviour(sceneMoneyBehaviour);

            depositoryBehaviour.Init(this, depository);

            depositories[depository.Mineral.Reference] = depositoryBehaviour;

            depositoryGameObject.SetActive(true);
        }

        public bool GetRelativePosition(Point2 pos, int x, int y, out int outX, out int outY)
        {
            int oldX = pos.X;
            int oldY = pos.Y;

            outX = x + oldX;
            if (outX < 0 || outX > Base.Core.Game.State.World.Width)
            {
                outX = 0;
                outY = 0;

                return false;
            }

            outY = y + oldY;
            if (outY < 0)
            {
                outX = 0;
                outY = 0;

                return false;
            }

            return true;
        }

        public bool GetRelativePosition(Point2 pos, int x, int y, out Point2 newPos)
        {
            if (GetRelativePosition(pos, x, y, out int outX, out int outY))
            {
                newPos = new Point2(outX, outY);
                return true;
            }

            newPos = new Point2();

            return false;
        }

        public UnityEngine.Vector3 GetUnityVector(Point2 position, float z, float xOffset = 0, float yOffset = 0)
        {
            return GetUnityVector(position.X, position.Y, z, xOffset, yOffset);
        }

        public UnityEngine.Vector3 GetUnityVector(float x, float y, float z, float xOffset = 0, float yOffset = 0)
        {
            return new UnityEngine.Vector3(x + xOffset, -(y + yOffset), z);
        }

        internal Boolean GetTileRelative(Point2 pos, int x, int y, out TileBehaviour tile)
        {
            tile = default;

            if (GetRelativePosition(pos, x, y, out int newX, out int newY))
            {
                if (this.tileMap.TryGetValue(newX, newY, out tile))
                {
                    return true;
                }
            }

            return false;
        }

        internal Boolean GetTileRelative<TTile>(Point2 pos, int x, int y, out TTile actualTîle) where TTile : TileBehaviour
        {
            actualTîle = default;

            if (GetTileRelative(pos, x, y, out var tile))
            {
                if (tile is TTile typedTile)
                {
                    actualTîle = typedTile;
                    return true;
                }
            }

            return false;
        }

        internal void DiggerMoved(DiggerBehaviour toolBehaviour)
        {
            var dir = toolBehaviour.GetDirection();
            var size = toolBehaviour.GetSize();
            List<(int x, int y)> pointList = new List<(int x, int y)>();

            switch (dir)
            {
                case Direction.Left:
                    {
                        var numPoints = size.Y + 2; //+4 for range -2 because we start lower
                        for (var i = -2; i < numPoints; i++)
                        {
                            pointList.Add((-2, i));
                        }

                        break;
                    }

                case Direction.Right:
                    {
                        var numPoints = size.Y + 2; //+4 for range -2 because we start lower
                        for (var i = -2; i < numPoints; i++)
                        {
                            pointList.Add((2, i));
                        }

                        break;
                    }

                case Direction.Down:
                    {
                        var numPoints = size.X + 2; //+4 for range -2 because we start lefter
                        for (var i = -2; i < numPoints; i++)
                        {
                            pointList.Add((i, 2));
                        }

                        break;
                    }

                default:
                    return;
            }

            var pos = toolBehaviour.GetPosition();

            foreach (var point in pointList)
            {
                var validPosition = GetRelativePosition(pos, point.x, point.y, out int x, out int y);

                if (validPosition && !tileMap.TryGetValue(x, y, out _))
                {
                    var tile = tileGenerator.GenerateTile(x, y);

                    SpawnGround(tile);
                }

                if (y > Base.Core.Game.State.World.MaxDepth)
                {
                    Base.Core.Game.State.World.MaxDepth = y;
                    CameraBehaviour.OnBoundariesChanged(Base.Core.Game.State.World.Width, y);
                }
            }
        }

        private void ClearTransportSites()
        {
            foreach (var site in TransportSites)
            {
                GameObject.Destroy(site.gameObject);
            }
            TransportSites.Clear();
        }

        internal void BuildTransporteSite(TransportSiteBehaviour transportSiteBehaviour)
        {
            ClearTransportSites();

            selectedTransport.Amount -= 1;

            if (transportSiteBehaviour.IsVertical())
            {
                DisplayPossibleVerticalTransportSites(selectedTransport);
            }
            else
            {

                DisplayPossibleHorizontalTransportSites(selectedTransport);
            }

            if (selectedTransport.Amount <= 0)
            {
                ClearTransportSites();
                selectedTransport = null;
            }
        }

        internal void RegisterStorage(IStorage storage)
        {
            var pos = storage.GetPosition();
            var storagesAtPoint = EnsureStoragesAtPoint(pos);
            storagesAtPoint.Add(storage);
            storagesAtPoint.OrderByDescending(s => s.Priority());
        }

        internal void UnRegisterStorage(IStorage storage)
        {
            var pos = storage.GetPosition();
            var storagesAtPoint = EnsureStoragesAtPoint(pos);
            storagesAtPoint.Remove(storage);
        }

        private List<IStorage> EnsureStoragesAtPoint(Point2 pos)
        {
            //storagesAtPoint
            if (!GetStoragesAtPoint(pos, out var storagesAtPoint))
            {
                storagesAtPoint = new();
                storages[pos.X, pos.Y] = storagesAtPoint;
            }
            return storagesAtPoint;
        }

        public bool GetStoragesAtPosition(int x, int y, out List<IStorage> storagesAtPoint)
        {
            bool v = storages.TryGetValue(x, y, out storagesAtPoint);
            //double r = 0;
            //if (v)
            //{
            //    r = storagesAtPoint.Count;
            //}
            //Debug.Log("Gettig Storages at: " + x + "," + y + "  " + r);
            return v;
        }

        public bool GetStoragesAtPoint(Point2 pos, out List<IStorage> storagesAtPoint)
        {
            return GetStoragesAtPosition(pos.X, pos.Y, out storagesAtPoint);
        }

        private void HookActions()
        {
            if (!shop.IsOpen)
            {
                var clickAction = InputSystem.actions.FindAction("Click");
                clickAction.performed += OnClick;
                touchPressed.performed += OnTouched;
            }
        }

        private void UnhookActions()
        {
            var clickAction = InputSystem.actions.FindAction("Click");
            clickAction.performed -= OnClick;
            touchPressed.performed -= OnTouched;
        }
    }
}
