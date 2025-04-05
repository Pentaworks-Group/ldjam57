using Assets.Scripts.Core;
using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Assets.Scripts.Scenes.GameScene
{
    public class WorldBehaviour : MonoBehaviour
    {


        public int widht;
        public int depth;
        private float xOffset;

        [SerializeField]
        private Camera mainCamera;


        [SerializeField]
        private GroundBehaviour TileTemplate;
        [SerializeField]
        private GameObject ShaftTemplate;
        [SerializeField]
        private SiteBehaviour SiteTemplate;
        [SerializeField]
        private ToolBehaviour ToolTemplate;
        [SerializeField]
        private GameObject TilesParent;



        //private List<TileBehaviour> Tiles = new();
        private Map<int, TileBehaviour> tileMap = new();

        private List<ShaftBehaviour> Shafts = new();


        private List<SiteBehaviour> Sites = new();



        private void Awake()
        {
            Base.Core.Game.ExecuteAfterInstantation(GenerateWorld);
        }


        public void ReplaceTile(TileBehaviour tile)
        {
            var newTile = GameObject.Instantiate(ShaftTemplate, tile.transform.position, tile.transform.rotation, TilesParent.transform);
            Point2 pos = tile.GetPosition();
            newTile.name = "Shaft_" + pos;
            var shaftBehaviour = newTile.GetComponent<ShaftBehaviour>();
            shaftBehaviour.Init(this, pos);
            Shafts.Add(shaftBehaviour);
            newTile.SetActive(true);
            tileMap[pos.X, pos.Y] = shaftBehaviour;
            GameObject.Destroy(tile.gameObject);
        }

        public void DisplayPosibleDigSites()
        {
            var miningTool = Base.Core.Game.State.AvailableMiningTools.First<MiningTool>();
            DisplayPosibleDigSites(miningTool);
        }

        public void DisplayPosibleDigSites(MiningTool miningTool)
        {
            ClearDigSites();
            foreach (var shaft in Shafts)
            {
                var beneath = GetTile(shaft.GetPosition(), 0, 1);
                if (beneath?.IsDigable() == true)
                {
                    if (miningTool.Size.X < 2)
                    {
                        var tile = GetTile(shaft.GetPosition(), -1, 0);
                        if (tile?.IsDigable() == true)
                        {
                            CreateSite(shaft.GetPosition(), miningTool, Direction.Left);
                        }
                        tile = GetTile(shaft.GetPosition(), 1, 0);
                        if (tile?.IsDigable() == true)
                        {
                            CreateSite(shaft.GetPosition(), miningTool, Direction.Right);
                        }
                        tile = GetTile(shaft.GetPosition(), 0, 1);
                        if (tile?.IsDigable() == true)
                        {
                            CreateSite(shaft.GetPosition(), miningTool, Direction.Down);
                        }
                    }
                }
            }
            for (var x = 0; x < widht; x++)
            {
                if (tileMap[x, 0].IsDigable())
                {
                    Point2 pos = new Point2(x, -1);
                    CreateSite(pos, miningTool, Direction.Down);
                }
            }
        }

        public void BuildDigSite(SiteBehaviour siteBehaviour)
        {
            var p = siteBehaviour.transform.position;
            var position = new UnityEngine.Vector3(p.x, p.y, ToolTemplate.transform.position.z);
            var newTool = GameObject.Instantiate(ToolTemplate, position, siteBehaviour.transform.rotation, TilesParent.transform);
            newTool.transform.localScale = siteBehaviour.transform.localScale;
            newTool.Init(this, siteBehaviour.GetMiningTool(), siteBehaviour.GetDirection(), siteBehaviour.GetPosition());
            newTool.gameObject.SetActive(true);
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
            if (direction == Direction.Down)
            {
                position = new UnityEngine.Vector3(pos.X - xOffset, -pos.Y - (1 - SiteTemplate.transform.localScale.y) / 2, SiteTemplate.transform.position.z);
                rotation = SiteTemplate.transform.rotation;
            }
            else if (direction == Direction.Left)
            {
                position = new UnityEngine.Vector3(pos.X - (1 - SiteTemplate.transform.localScale.y) / 2 - xOffset, -pos.Y, SiteTemplate.transform.position.z);
                rotation = SiteTemplate.transform.rotation;
                rotation *= Quaternion.Euler(0, 0, -90);
            }
            else if (direction == Direction.Right)
            {
                position = new UnityEngine.Vector3(pos.X + (1 - SiteTemplate.transform.localScale.y) / 2 - xOffset, -pos.Y, SiteTemplate.transform.position.z);
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
            xOffset = widht / 2f;
            var material = Base.Core.Game.State.GameMode.World.Minerals[0];

            for (int y = 0; y < depth; y++)
            {
                for (int x = 0; x < widht; x++)
                {
                    UnityEngine.Vector3 position = new UnityEngine.Vector3(-xOffset + x, -y, TileTemplate.transform.position.z);
                    var groundTile = GameObject.Instantiate(TileTemplate, position, TileTemplate.transform.rotation, TilesParent.transform);
                    var pos = new Point2(x, y);
                    groundTile.name = "Tile_" + pos;
                    groundTile.Init(this, pos, material);
                    groundTile.gameObject.SetActive(true);
                    tileMap[x, y] = groundTile;
                }
            }
        }




        public bool GetRelativePosition(Point2 pos, int x, int y, out int outX, out int outY)
        {
            int oldX = pos.X;
            int oldY = pos.Y;

            outX = x + oldX;
            if (outX < 0 || outX > widht)
            {
                outX = 0;
                outY = 0;
                return false;
            }

            outY = y + oldY;
            if (outY < 0 || outY > depth)
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

        internal TileBehaviour GetTile(Point2 pos, int x, int y)
        {
            if (!GetRelativePosition(pos, x, y, out int newX, out int newY))
            {
                return null;
            }
            return tileMap[newX, newY];
        }


    }
}
