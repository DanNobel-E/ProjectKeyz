using Aiv.Audio;
using Aiv.Fast2D;
using AIV_Exam_SimoneSantagati.Engine;
using AIV_Exam_SimoneSantagati.Engine.GameObjects.Items;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIV_Exam_SimoneSantagati
{
    abstract class PlayScene : Scene
    {

        public Player CurrentPlayer { get { return players[CurrentPlayerIndex]; } }
        public static Cursor MouseCursor { get; protected set; }
        public int CurrentPlayerIndex { get; protected set; }
        protected List<Player> players;
        protected Vector2 player1Position;
        protected Vector2 player2Position;

        public List<Player> Players { get { return players; } }

       
        protected string mapPath;
        public Vector2 MapInitPos { get; protected set; }

        public World World { get; protected set; }
        protected int busyWeight;

        public int WorldWidth { get; set; }
        public int WorldHeight { get; set; }

        public List<TileObj> TileObjs { get; protected set; }

        protected AudioSource skipSource;
        protected AudioClip skipClip;

        public static Door VictoryDoor { get; protected set; }



        public PlayScene(string map)
        {
            mapPath = map;

            //weight of uncrossable nodes
            busyWeight = 1000;

            //map window coords
            MapInitPos = Vector2.Zero;

            //Init players positions
            player1Position = Vector2.Zero;
            player2Position = Vector2.Zero;

            //Init skip turn audio source
            skipSource = new AudioSource();

        }

        public override void Start()
        {
            //load Assets
            LoadTextures();
            LoadClips();
            LoadFonts();

            if (World == null)
            {
                //instanciate map and tiles
                TilesInit(mapPath);

                //create node graph
                World = new World();
                World.Init(WorldHeight, WorldWidth);

                //instanciate customized mouse cursor
                MouseCursor = new Cursor();

            }

            base.Start();

            

        }

        protected void TilesInit(string mapPath)
        {
            //Parse tiles of mapFile, create tileset and tile instances
            TmxReader reader = new TmxReader(mapPath);
            TileSet ts = reader.TileSet;

            //Divide layers
            List<Layer> layers = reader.Layers;


            GfxMgr.AddTexture(ts.ImagePath, ts.ImagePath);
            TileObjs = new List<TileObj>();

            foreach (Layer each in layers)
            {
                AddTilesFor(each, TileObjs);
            }
        }


        private void AddTilesFor(Layer layer, List<TileObj> tileObjs)
        {
            //Set current layer, by parsing map's props
            DrawLayer engineLayer = DrawLayer.Playground;
            if (layer.Props.Has("drawLayer"))
            {
                string drawLayer = layer.Props.GetString("drawLayer");
                engineLayer = (DrawLayer)Enum.Parse(typeof(DrawLayer), drawLayer);
            }

            //Set current collisions layer, by parsing map's props
            CollisionLayer collisionLayer = CollisionLayer.Back;
            if (layer.Props.Has("collisionLayer"))
            {
                string collisionLayerString = layer.Props.GetString("collisionLayer");
                collisionLayer = (CollisionLayer)Enum.Parse(typeof(CollisionLayer), collisionLayerString);
            }

            for (uint i = 0; i < layer.Grid.Size(); i++)
            {
                TileInstance inst = layer.Grid.At(i);
                if (inst == null) continue;
                string texture = inst.Type.ImagePath;
                int tOffX = inst.Type.OffX;
                int tOffY = inst.Type.OffY;
                int width = inst.Type.Width;
                int height = inst.Type.Height;
                float posX = inst.PosX;
                float posY = inst.PosY;

                //Instanciate all tileobjects of current layer
                TileObj obj;

                //Instanciate Doors, by parsing tile instance props
                if (inst.Type.Props.Has("isDoor") && inst.Type.Props.GetBool("isDoor"))
                {
                    Door door = new Door(texture, tOffX, tOffY, posX, posY, width, height);

                    //Set if door is open or not
                    if (inst.Type.Props.Has("isOpened"))
                    {
                        if (inst.Type.Props.GetBool("isOpened"))
                        {
                            door.IsOpened = true;
                        }
                        else
                        {

                            door.IsOpened = false;

                        }



                    }

                    if (this is MainPlayScene)
                    {
                        //in Main PlayScene, initialize a secondary scene for each door
                        ((MainPlayScene)this).InitSecondaryMaps(inst,door);
                    }
                    else
                    {
                        //in Secondary PlayScene, set current door as scene spawn point
                        ((SecondaryPlayScene)this).SetSpawnPoint(door);

                    }

                    tileObjs.Add(door);
                    obj = door;
                }
                else if (inst.Type.Props.Has("isHideOut") && inst.Type.Props.GetBool("isHideOut"))
                 //Instanciate HideOuts, by parsing tile instance props
                {
                    HideOut hideOut = new HideOut(texture, tOffX, tOffY, posX, posY, width, height);
                    //Set HideOut as (secondary) scene hideout
                    ((SecondaryPlayScene)this).SetHideOut(hideOut);
                    tileObjs.Add(hideOut);
                    obj = hideOut;
                }
                else //Instanciate other common tileobjects and add them to tileObj list
                {
                    TileObj tile = new TileObj(texture, tOffX, tOffY, posX, posY, width, height);
                    tileObjs.Add(tile);
                    obj = tile;

                }


                //Set RigidBody of the current tile, if is collidable
                if (inst.Type.Props.Has("collidable") && inst.Type.Props.GetBool("collidable"))
                {
                    obj.IsCollidable = true;
                    obj.RigidBody = new RigidBody(obj);
                    obj.RigidBody.Collider = ColliderFactory.CreateBoxFor(obj.RigidBody, obj.Width * 0.8f, obj.Height * 0.8f);
                    obj.RigidBody.Type = RigidBodyType.TileObj;
                    obj.SetWeight(busyWeight);

                }

                //Parse tile instance props to verify if current tileObj is players init spawnpoint
                if (inst.Type.Props.Has("playerPos") && inst.Type.Props.GetBool("playerPos"))
                {
                    if (player1Position == Vector2.Zero)
                    {
                        player1Position = obj.Position;
                        player1Position.Y += Game.TILE_UNIT_SIZE;
                    }
                    else
                    {
                        player2Position = obj.Position;
                        player2Position.Y += Game.TILE_UNIT_SIZE;

                    }

                }



                obj.SetLayer(engineLayer);
                obj.SetCollisionLayer(collisionLayer);
     
                DrawMgr.AddItem(obj);

            }
        }

        protected virtual void LoadClips()
        {
            AudioMgr.AddClip("walkClip", "Assets/Sounds/footsteps.wav");
            AudioMgr.AddClip("pickKey", "Assets/Sounds/Pickup01.wav");
            AudioMgr.AddClip("pickItem1", "Assets/Sounds/pickItem.wav");
            AudioMgr.AddClip("pickItem2", "Assets/Sounds/pickItem2.wav");
            AudioMgr.AddClip("pickItem3", "Assets/Sounds/pickItem3.wav");
            //Add and set skip turn clip
            skipClip= AudioMgr.AddClip("skip", "Assets/Sounds/skipTurn.wav");

        }

        protected virtual void LoadTextures()
        {
            //Add players' movements sheets
            GfxMgr.AddTexture("player1IdleUp", "Assets/Sprites/HEROS8bit_Adventurer Idle U.png");
            GfxMgr.AddTexture("player1IdleSide", "Assets/Sprites/HEROS8bit_Adventurer Idle R.png");
            GfxMgr.AddTexture("player1IdleDown", "Assets/Sprites/HEROS8bit_Adventurer Idle D.png");
            GfxMgr.AddTexture("player1WalkUp", "Assets/Sprites/HEROS8bit_Adventurer Walk U.png");
            GfxMgr.AddTexture("player1WalkSide", "Assets/Sprites/HEROS8bit_Adventurer Walk R.png");
            GfxMgr.AddTexture("player1WalkDown", "Assets/Sprites/HEROS8bit_Adventurer Walk D.png");
            GfxMgr.AddTexture("player2IdleUp", "Assets/Sprites/HEROS8bit_Princess Idle U.png");
            GfxMgr.AddTexture("player2IdleSide", "Assets/Sprites/HEROS8bit_Princess Idle R.png");
            GfxMgr.AddTexture("player2IdleDown", "Assets/Sprites/HEROS8bit_Princess Idle D.png");
            GfxMgr.AddTexture("player2WalkUp", "Assets/Sprites/HEROS8bit_Princess Walk U.png");
            GfxMgr.AddTexture("player2WalkSide", "Assets/Sprites/HEROS8bit_Princess Walk R.png");
            GfxMgr.AddTexture("player2WalkDown", "Assets/Sprites/HEROS8bit_Princess Walk D.png");

            //Add Items
            GfxMgr.AddTexture("bananas", "Assets/Items/item8BIT_bananas.png");
            GfxMgr.AddTexture("bones", "Assets/Items/item8BIT_bones.png");
            GfxMgr.AddTexture("book", "Assets/Items/item8BIT_book.png");
            GfxMgr.AddTexture("coin", "Assets/Items/item8BIT_coin.png");
            GfxMgr.AddTexture("compass", "Assets/Items/item8BIT_compass.png");
            GfxMgr.AddTexture("crown", "Assets/Items/item8BIT_crown.png");
            GfxMgr.AddTexture("feather", "Assets/Items/item8BIT_feather.png");
            GfxMgr.AddTexture("gem", "Assets/Items/item8BIT_gem.png");
            GfxMgr.AddTexture("harp", "Assets/Items/item8BIT_harp.png");
            GfxMgr.AddTexture("key", "Assets/Items/item8BIT_key.png");
            GfxMgr.AddTexture("leaf", "Assets/Items/item8BIT_leaf.png");
            GfxMgr.AddTexture("mirror", "Assets/Items/item8BIT_mirror.png");
            GfxMgr.AddTexture("moneybag", "Assets/Items/item8BIT_moneybag.png");
            GfxMgr.AddTexture("ring", "Assets/Items/item8BIT_ring.png");
            GfxMgr.AddTexture("scroll", "Assets/Items/item8BIT_scroll.png");
            GfxMgr.AddTexture("seashell", "Assets/Items/item8BIT_seashell.png");
            GfxMgr.AddTexture("skullkey", "Assets/Items/item8BIT_skullkey.png");


            //Add doors
            GfxMgr.AddTexture("villageDoor", "Assets/Tilesets/Village_OpenedDoor.png");
            GfxMgr.AddTexture("caveDoor", "Assets/Tilesets/Cave_OpenedDoor.png");
            GfxMgr.AddTexture("victoryDoor", "Assets/Tilesets/Victory_OpenedDoor.png");


            //Add cursor
            GfxMgr.AddTexture("cursor", "Assets/GUI/mouseIcon.png");



        }

        protected virtual void LoadFonts()
        {
            FontMgr.AddFont("comic", "Assets/Fonts/comics.png", 10, 32, 61, 65);
            FontMgr.AddFont("stdFont", "Assets/Fonts/textSheet.png", 15, 32, 20, 20);
        }
        public override void Input()
        {
            //Update FSM of the current Player
            if (players[CurrentPlayerIndex].IsActive)
            {
                players[CurrentPlayerIndex].UpdateFSM();

            }
        }

        public override void Update()
        {

            PhysicsMgr.Update();
            UpdateMgr.Update();
            TurnMgr.Update(); //Manages turn alternation
            CameraMgr.Update();
            PhysicsMgr.CheckCollisions();

        }
        public override void Draw()
        {
            DrawMgr.Draw();

        }

        public  void NextPlayer()
        {
            //Called by TurnMgr, manages turn alternation

            //Change current player
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % players.Count;

            //Stabilize camera in case players are in diffrent scenes
            if (this is MainPlayScene && CurrentPlayer.CurrentScene is SecondaryPlayScene)
            {
                //moves camera to other player scene matching door, instead of his real current position
                CameraMgr.MoveCameraTo(PickSceneDoor((MainPlayScene)this, (SecondaryPlayScene)CurrentPlayer.CurrentScene).Position);
                CameraMgr.MainCamera.position = CurrentPlayer.Position;

            }
            else if(this is SecondaryPlayScene && CurrentPlayer.CurrentScene is MainPlayScene)
            {
                //makes camera movement begin from secondary scene door position, instead of player current position
                CameraMgr.MainCamera.position = PickSceneDoor(((SecondaryPlayScene)this).Owner, (SecondaryPlayScene)this).Position;
                CameraMgr.MoveCameraTo(CurrentPlayer.Position);

            }
            else //moves camera from one player to another when they're in the same scene
            {
                CameraMgr.MoveCameraTo(CurrentPlayer.Position);

            }

            //Targets current player
            CameraMgr.Target = CurrentPlayer;
            CurrentPlayer.Play();

            //Reset turn
            TurnMgr.ResetTimer();
            skipSource.Play(skipClip);

            //exits current scene if players are in diffrent scenes
            if (CurrentPlayer.CurrentScene != this)
            {
                NextScene = CurrentPlayer.CurrentScene;
                OnExit();
            }

        }
        public void SharePlayersList()
        {
            //Share player list between diffrent scenes
            if(PreviousScene is PlayScene)
            {
                players = ((PlayScene)PreviousScene).Players;
                CurrentPlayerIndex = ((PlayScene)PreviousScene).CurrentPlayerIndex;

            }

        }

        public Door PickSceneDoor(MainPlayScene mainScene, SecondaryPlayScene secondaryScene)
        {
           //Gain selected secondary scene matching door by searching in SecondaryScenes Dictionary
            foreach (var each in mainScene.SecondaryScenes)
            {
                if (each.Value == secondaryScene)
                {
                    return each.Key;
                }

            }

            return null;
        }

        public virtual void TargetPlayer()
        {
            CameraMgr.Target = CurrentPlayer;
            CurrentPlayer.SetCurrentScene(this);
            CheckActivePlayers();
            CurrentPlayer.Play();

        }

        public virtual void CheckActivePlayers()
        {
            //Sets as active only players in current scene
            for (int i = 0; i < players.Count; i++)
            {
                players[i].CheckActivePlayer();
            }

        }

        public virtual void Victory()
        {
            Player winner = null;

            //Select winner confronting their points
            for (int i = 0; i < Players.Count; i++)
            {
                if (winner == null)
                {
                    winner = players[i];
                }
                else
                {
                    if (players[i].Points > winner.Points)
                    {

                        winner = players[i];
                    }
                    else if (players[i]!=winner && players[i].Points == winner.Points)
                    {
                        winner = null;
                    }

                }


            }

            //callse matching victory scene, basing on the winner
            if (winner != null)
            {
                int id = winner.PlayerID % 2;

                if (id == 0)
                {
                    NextScene= Game.Player1VictoryScene;

                }
                else
                {
                    NextScene = Game.Player2VictoryScene;

                }

            }
            else
            {
                NextScene = Game.TieScene;

            }

            OnExit();


        }


        public override Scene OnExit()
        {
            //Set current scene's tile object ad inactive and clean assets managers
            TileObjs.ForEach( t => t.IsActive=false);
            GfxMgr.ClearAll();
            AudioMgr.ClearAll();
            FontMgr.ClearAll();
            GC.Collect();
            return base.OnExit();
        }
    }
}
