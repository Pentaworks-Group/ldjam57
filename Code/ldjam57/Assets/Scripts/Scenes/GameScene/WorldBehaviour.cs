using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Core;
using Assets.Scripts.Core.Model;

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
        private GameObject ShaftTemplate;
        [SerializeField]
        private SiteBehaviour SiteTemplate;
        [SerializeField]
        private DiggerBehaviour DiggerTemplate;
        [SerializeField]
        private TransportSiteBehaviour TransportSiteTemplate;
        [SerializeField]
        private GameObject TilesParent;

        [SerializeField]
        private GameObject Headquarters;

        [SerializeField]
        private GameObject DepositoryTemplate;
        [SerializeField]
        private GameObject DepositoryContainer;

        private Map<int, TileBehaviour> tileMap = new();

        private List<ShaftBehaviour> Shafts = new();

        private List<SiteBehaviour> Sites = new();
        private readonly List<DepositoryBehaviour> depositories = new List<DepositoryBehaviour>();

        private TileGenerator tileGenerator;

        private void Awake()
        {
            Base.Core.Game.ExecuteAfterInstantation(GenerateWorld);
        }

        private void OnEnable()
        {
            var clickAction = InputSystem.actions.FindAction("Click");
            clickAction.performed += OnClick;
        }

        private void OnDisable()
        {
            var clickAction = InputSystem.actions.FindAction("Click");
            clickAction.performed -= OnClick;
        }

        public void DisplayPosibleDigSites()
        {
            var miningTool = Base.Core.Game.State.AvailableMiningTools.First<MiningTool>();
            DisplayPosibleDigSites(miningTool);
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

        public void DisplayPosibleDigSites(MiningTool miningTool)
        {
            ClearDigSites();

            Map<int, Digger> diggerMap = GenerateDiggerMap();

            foreach (var shaft in Shafts)
            {
                var beneath = GetTileRelative(shaft.GetPosition(), 0, 1);

                if (beneath?.IsDigable() == true && !diggerMap.TryGetValue(shaft.GetPosition().X, shaft.GetPosition().Y, out _))
                {
                    if (miningTool.Size.X < 2)
                    {
                        var tile = GetTileRelative(shaft.GetPosition(), -1, 0);
                        if (tile?.IsDigable() == true)
                        {
                            CreateSite(shaft.GetPosition(), miningTool, Direction.Left);
                        }
                        tile = GetTileRelative(shaft.GetPosition(), 1, 0);
                        if (tile?.IsDigable() == true)
                        {
                            CreateSite(shaft.GetPosition(), miningTool, Direction.Right);
                        }
                        tile = GetTileRelative(shaft.GetPosition(), 0, 1);
                        if (tile?.IsDigable() == true)
                        {
                            CreateSite(shaft.GetPosition(), miningTool, Direction.Down);
                        }
                    }
                }
            }

            for (var x = 0; x < Base.Core.Game.State.World.Width; x++)
            {
                var tile = tileMap[x, 0];
                if (tile.IsDigable() && !diggerMap.TryGetValue(tile.GetPosition().X, tile.GetPosition().Y, out _))
                {
                    Point2 pos = new Point2(x, -1);
                    CreateSite(pos, miningTool, Direction.Down);
                }
            }
        }

        public void BuildDigSite(SiteBehaviour siteBehaviour)
        {
            var newDigger = GameObject.Instantiate(DiggerTemplate, TilesParent.transform);

            var digger = new Digger()
            {
                Direction = siteBehaviour.GetDirection(),
                MiningTool = siteBehaviour.GetMiningTool(),
                Position = siteBehaviour.GetPosition()
            };

            newDigger.Init(this, digger);
            newDigger.UpdatePosition();
            newDigger.gameObject.SetActive(true);

            ClearDigSites();

            Base.Core.Game.State.ActiveDiggers.Add(digger);
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
            if (direction == Direction.Down)
            {
                position = GetUnityVector(pos, SiteTemplate.transform.position.z, yOffset: (1 - SiteTemplate.transform.localScale.y) / 2);
                rotation = SiteTemplate.transform.rotation;
            }
            else if (direction == Direction.Left)
            {
                position = GetUnityVector(pos, SiteTemplate.transform.position.z, xOffset: -(1 - SiteTemplate.transform.localScale.y) / 2);
                rotation = SiteTemplate.transform.rotation;
                rotation *= Quaternion.Euler(0, 0, -90);
            }
            else if (direction == Direction.Right)
            {
                position = GetUnityVector(pos, SiteTemplate.transform.position.z, xOffset: (1 - SiteTemplate.transform.localScale.y) / 2);
                rotation = SiteTemplate.transform.rotation;
                rotation *= Quaternion.Euler(0, 0, 90);
            }
            else
            {
                return;
            }
            var newSite = GameObject.Instantiate(SiteTemplate, position, rotation, TilesParent.transform);
            //newSite.transform.localScale = new Vector3(miningTool.Size.X * 0.9f, miningTool.Size.Y * 0.9f, 1);
            newSite.Init(this, pos, miningTool, direction);
            newSite.gameObject.SetActive(true);
            Sites.Add(newSite);
        }

        public void DisplayPossibleVerticalTransportSites()
        {
            var transport = Base.Core.Game.State.AvailableVerticalTransports.First<Transport>();
            DisplayPossibleVerticalTransportSites(transport);
        }

        private void DisplayPossibleVerticalTransportSites(Transport transport)
        {
            var usedShafts = new HashSet<ShaftBehaviour>();
            var transportBehaviours = new List<TransportBehaviour>();
            foreach (var shaft in Shafts)
            {
                if (usedShafts.Contains(shaft))
                {
                    continue;
                }

                var points = new List<ShaftBehaviour>();
                points.Add(shaft);

                usedShafts.Add(shaft);
                GetValidShaftUp(shaft, points, usedShafts);
                GetValidShaftDown(shaft, points, usedShafts);

                foreach (var point in points)
                {
                    var newSite = GameObject.Instantiate(TransportSiteTemplate, TilesParent.transform);
                    newSite.Init(this, transport, points, point.GetPosition());
                    newSite.gameObject.SetActive(true);
                }
            }
        }

        private void GetValidShaftUp(ShaftBehaviour shaft, List<ShaftBehaviour> points, HashSet<ShaftBehaviour> usedShafts)
        {
            var tile = GetTileRelative(shaft.GetPosition(), 0, -1);
            if (tile is ShaftBehaviour)
            {
                ShaftBehaviour nextShaft = (ShaftBehaviour)tile;
                if (usedShafts.Contains(nextShaft))
                {
                    return;
                }
                usedShafts.Add(nextShaft);
                points.Add(nextShaft);
                GetValidShaftUp(nextShaft, points, usedShafts);
            }
        }

        private void GetValidShaftDown(ShaftBehaviour shaft, List<ShaftBehaviour> points, HashSet<ShaftBehaviour> usedShafts)
        {
            var tile = GetTileRelative(shaft.GetPosition(), 0, 1);
            if (tile is ShaftBehaviour)
            {
                ShaftBehaviour nextShaft = (ShaftBehaviour)tile; if (usedShafts.Contains(nextShaft))
                {
                    return;
                }
                usedShafts.Add(nextShaft);
                points.Add(nextShaft);
                GetValidShaftDown(nextShaft, points, usedShafts);
            }
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                UnityEngine.Vector2 mousePosition = Mouse.current.position.ReadValue();
                Ray ray = mainCamera.ScreenPointToRay(mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GameObject clickedObject = hit.collider.gameObject;
                    IClickable clickable = clickedObject.GetComponent<IClickable>();
                    Debug.Log("Clicked: " + clickedObject.name);
                    if (clickable != null)
                    {
                        clickable.OnClicked();
                    }
                }
            }
        }

        private void GenerateWorld()
        {
            var world = Base.Core.Game.State.World;

            tileGenerator = new TileGenerator(world);

            Headquarters.transform.position = world.Headquarters.Position.ToUnityVector3();

            foreach (var tile in world.Tiles)
            {
                if (tile.DigingProgress >= 1)
                {
                    CreateShaft(tile.Position);
                }
                else
                {
                    GenerateGround(tile);
                }
            }

            CameraBehaviour.OnBoundariesChanged(world.Width, world.MaxDepth);

            foreach (var depository in world.Depositories)
            {
                GenerateDepository(depository);
            }
        }

        private void GenerateGround(Tile tile)
        {
            var pos = tile.Position;

            var position = GetUnityVector(pos, TileTemplate.transform.position.z);

            var groundTile = GameObject.Instantiate(TileTemplate, position, TileTemplate.transform.rotation, TilesParent.transform);

            groundTile.name = "Tile_" + pos;

            groundTile.Init(this, tile);

            groundTile.gameObject.SetActive(true);

            tileMap[pos.X, pos.Y] = groundTile;
        }

        public void ReplaceTile(TileBehaviour tile)
        {
            CreateShaft(tile.GetPosition());
            GameObject.Destroy(tile.gameObject);
        }

        private void CreateShaft(Point2 pos)
        {
            var position = GetUnityVector(pos, ShaftTemplate.transform.position.z);
            var newTile = GameObject.Instantiate(ShaftTemplate, position, ShaftTemplate.transform.rotation, TilesParent.transform);

            newTile.name = "Shaft_" + pos;

            var shaftBehaviour = newTile.GetComponent<ShaftBehaviour>();

            shaftBehaviour.Init(this, pos);

            Shafts.Add(shaftBehaviour);

            newTile.SetActive(true);

            tileMap[pos.X, pos.Y] = shaftBehaviour;
        }

        private void GenerateDepository(Depository depository)
        {
            var depositoryGameObject = Instantiate(DepositoryTemplate, depository.Position.ToUnityVector3(), Quaternion.identity, DepositoryContainer.transform);

            var depositoryBehaviour = depositoryGameObject.GetComponent<DepositoryBehaviour>();

            depositoryBehaviour.Init(this, depository);

            depositories.Add(depositoryBehaviour);

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

        internal TileBehaviour GetTileRelative(Point2 pos, int x, int y)
        {
            if (!GetRelativePosition(pos, x, y, out int newX, out int newY))
            {
                return null;
            }
            return tileMap[newX, newY];
        }

        internal void DiggerMoved(DiggerBehaviour toolBehaviour)
        {
            var dir = toolBehaviour.GetDirection();
            var size = toolBehaviour.GetSize();
            List<(int x, int y)> pointList = new List<(int x, int y)>();
            if (dir == Direction.Left)
            {
                var numPoints = size.Y + 2; //+4 for range -2 because we start lower
                for (int i = -2; i < numPoints; i++)
                {
                    pointList.Add((-2, i));
                }

            }
            else if (dir == Direction.Right)
            {
                var numPoints = size.Y + 2; //+4 for range -2 because we start lower
                for (int i = -2; i < numPoints; i++)
                {
                    pointList.Add((2, i));
                }
            }
            else if (dir == Direction.Down)
            {
                var numPoints = size.X + 2; //+4 for range -2 because we start lefter
                for (int i = -2; i < numPoints; i++)
                {
                    pointList.Add((i, 2));
                }
            }
            else
            {
                return;
            }


            var pos = toolBehaviour.GetPosition();
            foreach (var point in pointList)
            {
                var validPosition = GetRelativePosition(pos, point.x, point.y, out int x, out int y);

                if (validPosition && !tileMap.TryGetValue(x, y, out _))
                {
                    var tile = tileGenerator.GenerateTile(x, y);

                    GenerateGround(tile);
                }

                if (y > Base.Core.Game.State.World.MaxDepth)
                {
                    Base.Core.Game.State.World.MaxDepth = y;
                    CameraBehaviour.OnBoundariesChanged(Base.Core.Game.State.World.Width, y);
                }
            }
        }
    }
}
