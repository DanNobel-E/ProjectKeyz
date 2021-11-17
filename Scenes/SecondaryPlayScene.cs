using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AIV_Exam_SimoneSantagati
{
    enum MapType { Village, Cave, Swamp, City, Castle, Last}
    class SecondaryPlayScene : PlayScene
    {

        public MainPlayScene Owner { get; protected set; }

        public int SceneIndex { get; protected set; }
        public Door SpawnPoint { get; protected set; }

        public MapType Type { get; protected set; }
        public bool HasKey { get; protected set; }
        public bool HasItem { get; protected set; }
        public HideOut ItemHideOut { get; protected set; }



        public SecondaryPlayScene (MainPlayScene aOwner, string map, int index, MapType type) : base(map)
        {
            //Set Main PlayScene ad owner
            Owner = aOwner;
            PreviousScene = Owner;
            SceneIndex = index;
            Type = type;
            HasItem = true;

            //Set map init position
            MapInitPos = new Vector2(Game.TILE_UNIT_SIZE * 0.5f, Game.TILE_UNIT_SIZE * 0.5f);


        }


        public override void Start()
        {

            base.Start();

            //if the scene has a key, hides it in his hideout
            if (HasKey)
            {
                if (ItemHideOut.HiddenItem == null)
                {
                    for (int i = 0; i < MainPlayScene.Keys.Count; i++)
                    {
                        if(MainPlayScene.Keys[i].HideOutScene == this)
                        {
                            ItemHideOut.SetHiddenItem (MainPlayScene.Keys[i]);
                        }
                    }
                }

            }
            else //Reset hideout when no more has a key
            {
                if(ItemHideOut.HiddenItem is Key)
                {
                    ItemHideOut.ResetHideOut();

                }
            }

            //Set position of hidden item
            if (ItemHideOut.HiddenItem != null)
            {
                ItemHideOut.HiddenItem.Position = ItemHideOut.Position;

            }

            //If the scene hasn't got a key, randomize hideout's item 
            if (HasItem)
            {
                if (ItemHideOut.HiddenItem == null)
                {
                    ItemHideOut.SetHiddenItem(PickRandomItem());
                }

            }

            //gain player list from previous scene
            SharePlayersList();
            SpawnPlayer();
            TargetPlayer();



        }

        public void SetSpawnPoint(Door spawnPoint)
        {
            SpawnPoint = spawnPoint;
        }

        private void SpawnPlayer()
        {
            //Set player position in scene's spawn point
            if (CurrentPlayer.CurrentScene != this)
            {
                CurrentPlayer.Position = new Vector2(SpawnPoint.Position.X, SpawnPoint.Position.Y - Game.TILE_UNIT_SIZE);

            }
        }

        public void SetHideOut(HideOut hideOut)
        {
            if (hideOut != null)
            {
                ItemHideOut = hideOut;
            }
        }

        public void ContainsKey()
        {
            //if this scene has a key, avoid items randomization
            HasKey = true;
            HasItem = false;
        }

        public Key GetKey(Player player)
        {
            //Get hideout's hidden key
            if (ItemHideOut.HiddenItem is Key)
            {

                Key key = (Key)ItemHideOut.HiddenItem;
                key.PlayItemClip();
                HasKey = false;

                //Set player as owner
                key.SetOwner(player);
                player.AddToInventory(key);
                return key;

            }

            return null;
        }

        public int GetItem(Player player)
        {
            //Get hideout's hidden item
            if (ItemHideOut.HiddenItem is PointedItem)
            {
                PointedItem item = (PointedItem)ItemHideOut.HiddenItem;
                item.PlayItemClip();
                ItemHideOut.ResetHideOut();
                HasItem = false;
                int points = item.GetPoints();

                //Set player as owner and update his points
                item.SetOwner(player);
                player.AddToInventory(item);
                return points;

            }

            return 0;
        }


        private PointedItem PickRandomItem()
        {
            //Reference to scene door
            Door door = PickSceneDoor(Owner, this);
            PointedItem item;
            bool wasClosed = false;

            //Search if the door was matching door of a key
            for (int i = 0; i < MainPlayScene.Keys.Count; i++)
            {
                if (MainPlayScene.Keys[i].MatchingDoor == door)
                {
                    wasClosed = true;
                }
            }

            //if door was closed, instanciate a random preciuos item
            if (wasClosed)
            {
                do
                {

                    int random = RandomGenerator.GetRandomInt(0, MainPlayScene.Items.Count);
                    item = (PointedItem)(Activator.CreateInstance(MainPlayScene.Items[random]));

                } while (!item.IsPrecious);

                return item;
            }
            else //if door was opened, instanciate a random common item
            {
                
                do
                {

                    int random = RandomGenerator.GetRandomInt(0, MainPlayScene.Items.Count);
                    item = (PointedItem)(Activator.CreateInstance(MainPlayScene.Items[random]));

                } while (item.IsPrecious);

                return item;

            }
        }

        public override Scene OnExit()
        {
            NextScene.PreviousScene = this;
            World = null;
            TileObjs.ForEach(t => t.IsActive = false);
            TileObjs.Clear();
            return base.OnExit();
        }
    }
}
