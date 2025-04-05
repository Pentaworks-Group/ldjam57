using Assets.Scripts.Core.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


namespace Assets.Scripts.Scenes.GameScene
{
    public class WorldBehaviour : MonoBehaviour
    {


        public int widht;
        public int depth;


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



        private List<TileBehaviour> Tiles = new();
        private List<ShaftBehaviour> Shafts = new();


        private List<SiteBehaviour> Sites = new();



        private void Awake()
        {
            Base.Core.Game.ExecuteAfterInstantation(GenerateWorld);
        }


        public void ReplaceTile(TileBehaviour tile)
        {
            var newTile = GameObject.Instantiate(ShaftTemplate, tile.transform.position, tile.transform.rotation, TilesParent.transform);
            int pos = tile.GetPosition();
            newTile.name = "Shaft_" + pos;
            var shaftBehaviour = newTile.GetComponent<ShaftBehaviour>();
            shaftBehaviour.Init(this, pos);
            Shafts.Add(shaftBehaviour);
            newTile.SetActive(true);
            Tiles[pos] = shaftBehaviour;
            GameObject.Destroy(tile.gameObject);
            Debug.Log("Replaced Tile " + pos);
        }

        public void DisplayPosibleDigSites()
        {
            var miningTool = Base.Core.Game.State.AvailableMiningTools.First<MiningTool>();
            DisplayPosibleDigSites(miningTool);
        }

        public void DisplayPosibleDigSites(MiningTool miningTool)
        {
            foreach (var shaft in Shafts)
            {
                if (miningTool.Size.X < 2)
                {
                    var tile = GetTile(shaft.GetPosition(), -1, 0);
                    if (tile?.IsDigable() == true)
                    {
                        var position = new Vector3(shaft.transform.position.x - 0.1f, shaft.transform.position.y, shaft.transform.position.z);
                        CreateSite(shaft.GetPosition(), miningTool, position, Direction.Left);
                    }
                    tile = GetTile(shaft.GetPosition(), 1, 0);
                    if (tile?.IsDigable() == true)
                    {
                        var position = new Vector3(shaft.transform.position.x + 0.1f, shaft.transform.position.y, shaft.transform.position.z);
                        CreateSite(shaft.GetPosition(), miningTool, position, Direction.Right);
                    }
                    tile = GetTile(shaft.GetPosition(), 0, 1);
                    if (tile?.IsDigable() == true)
                    {
                        var position = new Vector3(shaft.transform.position.x, shaft.transform.position.y - 0.1f, shaft.transform.position.z);
                        CreateSite(shaft.GetPosition(), miningTool, position, Direction.Down);
                    }
                }
            }
            for (var x = 0; x < widht; x++)
            {
                if (Tiles[x].IsDigable())
                {
                    var position = new Vector3(Tiles[x].transform.position.x, Tiles[x].transform.position.y + 0.9f, Tiles[x].transform.position.z);
                    int pos = GetPosition(x, -1);
                    CreateSite(pos, miningTool, position, Direction.Down);
                }
            }
        }

        public void BuildDigSite(SiteBehaviour siteBehaviour)
        {
            var newTool = GameObject.Instantiate(ToolTemplate, siteBehaviour.transform.position, ToolTemplate.transform.rotation, TilesParent.transform);
            newTool.transform.localScale = siteBehaviour.transform.localScale;
            newTool.Init(this, siteBehaviour.GetMiningTool(), siteBehaviour.GetDirection(), siteBehaviour.GetPosition());
            newTool.gameObject.SetActive(true);
            foreach (var site in Sites)
            {
                GameObject.Destroy(site.gameObject);
            }
            Sites.Clear();
        }


        private void CreateSite(int pos, MiningTool miningTool, Vector3 position, Direction direction)
        {
            var newSite = GameObject.Instantiate(SiteTemplate, position, SiteTemplate.transform.rotation, TilesParent.transform);
            newSite.transform.localScale = new Vector3(miningTool.Size.X * 0.9f, miningTool.Size.Y * 0.9f, 1);
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
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                Ray ray = mainCamera.ScreenPointToRay(mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GameObject clickedObject = hit.collider.gameObject;
                    IClickable clickable = clickedObject.GetComponent<IClickable>();
                    if (clickable != null)
                    {
                        clickable.OnClicked();
                    }
                }
            }
        }

        private void GenerateWorld()
        {
            var widhtHalf = widht / 2f;
            var material = Base.Core.Game.State.GameMode.World.Materials[0];

            for (int y = 0; y < depth; y++)
            {
                for (int x = 0; x < widht; x++)
                {
                    Vector3 position = new Vector3(-widhtHalf + x, -y, TileTemplate.transform.position.z);
                    var groundTile = GameObject.Instantiate(TileTemplate, position, TileTemplate.transform.rotation, TilesParent.transform);
                    var pos = GetPosition(x, y);
                    groundTile.name = "Tile_" + pos;
                    groundTile.Init(this, pos, material);
                    groundTile.gameObject.SetActive(true);
                    Tiles.Add(groundTile);
                }
            }
        }

        private int GetPosition(int x, int y)
        {
            return x + y * widht;
        }


        public int GetRelativePosition(int pos, int x, int y)
        {
            int oldX = pos % widht;
            if (oldX < 0)
            {
                oldX += widht;
                y -= 1;
            }
            int oldY = pos / widht;

            int newX = x + oldX;
            if (newX < 0 || newX > widht)
            {
                return -1;
            }

            int newY = y + oldY;
            if (newY < 0 || newY > depth)
            {
                return -1;
            }
            return GetPosition(newX, newY);
        }

        internal TileBehaviour GetTile(int pos, int x, int y)
        {
            int targetPos = GetRelativePosition(pos, x, y);
            if (targetPos < 0)
            {
                return null;
            }
            return Tiles[targetPos];
        }
    }
}
