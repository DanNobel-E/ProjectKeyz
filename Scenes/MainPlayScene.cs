using Aiv.Fast2D;
using AIV_Exam_SimoneSantagati.Engine;
using AIV_Exam_SimoneSantagati.Engine.GameObjects.Items;
using OpenTK;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AIV_Exam_SimoneSantagati
{

    class MainPlayScene : PlayScene
    {
      

        public Dictionary<Door, SecondaryPlayScene> SecondaryScenes;
        public Dictionary<MapType, Queue<Tuple<int, string>>> SecondaryMaps;
        public static Dictionary<MapType, string> DoorTextures;
        public static Dictionary<int, Type> Items;
        public static List<Key> Keys;


        protected int villageSecondaryMaps;
        protected int caveSecondaryMaps;
        protected int citySecondaryMaps;
        protected int swampSecondaryMaps;
        protected int castleSecondaryMaps;



        public MainPlayScene(string mapPath) : base(mapPath)
        {
            //Set map init position
            MapInitPos = new Vector2(Game.TILE_UNIT_SIZE * 0.5f, Game.TILE_UNIT_SIZE * 0.5f);

            //Instanciate Dictionaries
            SecondaryScenes = new Dictionary<Door, SecondaryPlayScene>();
            SecondaryMaps = new Dictionary<MapType, Queue<Tuple<int,string>>>();
            DoorTextures = new Dictionary<MapType, string>();
            Items = new Dictionary<int, Type>();

            //set number of secondary maps for each mapType
            villageSecondaryMaps = 5;
            caveSecondaryMaps = 4;
            citySecondaryMaps = 10;
            swampSecondaryMaps = 5;
            castleSecondaryMaps = 7;

            //Fill dictionaries
            LoadSecondaryMaps();
            LoadDoorTextures();
            LoadItems();

            //Init Managers

            CameraMgr.Init(player1Position, new Vector2(Game.Win.OrthoWidth * 0.5f, Game.Win.OrthoHeight * 0.5f));
            CameraMgr.Behaviour = FollowBehaviour.FollowTarget;
            CameraMgr.AddCamera("GUI", new Camera());

            FontMgr.Init();
            LoadFonts();
            
            TurnMgr.Init();
            TurnMgr.ResetTimer();

            Game.IsRunning = true;

        }

        public override void Start()
        {

            base.Start();

            //Fill Keys Dictionary
            if (Keys == null)
            {
                Keys = new List<Key>();
                foreach (TileObj each in TileObjs)
                {
                    if(each is Door)
                    {
                        Door door = (Door)each;  

                        if (!door.IsOpened) //Creates a key for each closed door
                        {
                            //Finds a random secondary scene to hide key
                            SecondaryPlayScene hideOutScene = FindRandomScene();

                            Key key; 
                            //instanciate and hide victory key
                            if (door == VictoryDoor)
                            {
                                key  = new VictoryKey(hideOutScene);

                            }
                            else //instanciate other keys
                            {
                                key = new Key(hideOutScene, door);


                            }

                            Keys.Add(key);

                            //Set that the selected scene has a key
                            hideOutScene.ContainsKey();




                        }

                    }
                   
                }

            }

           
            //instanciate player list and players
            if (players == null)
            {
                players = new List<Player>();
                Player player1 = new Player(player1Position, Game.GetController(0));
                players.Add(player1);
                Player player2 = new Player(player2Position, Game.GetController(0), 1);
                players.Add(player2);
                CurrentPlayerIndex = 0;
                for (int i = 0; i < players.Count; i++)
                {
                    players[i].SetCurrentScene(this);
                }


            }
            else //gain player list from previous scene
            {
                SharePlayersList();
            }

            //Target a player before starting the scene;
            TargetPlayer();


        }

        public void InitSecondaryMaps(TileInstance inst, Door door)
        {
            //Parse mapType to know secondary scene's mapType
            if (inst.Type.Props.Has("mapType"))
            {
                string type = inst.Type.Props.GetString("mapType");
                MapType mapType = (MapType)Enum.Parse(typeof(MapType), type);
                //set doorType
                door.SetDoorType(mapType);
                //Update secondary maps Dictionary, which links secondary maps with matching doors
                SetSecondaryMaps(mapType, door);
            }
            else if (inst.Type.Props.Has("victoryDoor"))
            {
                VictoryDoor = door;
                VictoryDoor.SetDoorType(MapType.Last);
            }



        }

        public void SetSecondaryMaps(MapType mapType, Door doorToScene)
        {
            //links an index to each mapPath string
            Tuple<int, string> mapInfo = SecondaryMaps[mapType].Dequeue(); //Deqeue first secondary map's mapPath of selected mapType
            int mapIndex = mapInfo.Item1;
            string mapString = mapInfo.Item2;
            SecondaryMaps[mapType].Enqueue(mapInfo); //Enqueue back to mantain mapPath order

            //instanciate secondary play scene, linking it with matching door
            SecondaryScenes[doorToScene] = new SecondaryPlayScene(this, mapString, mapIndex, mapType);



        }


        protected void LoadSecondaryMaps()
        {

            for (int i = 0; i < (int)MapType.Last; i++)
            {
                //Link all mapPaths references to each mapType
                SecondaryMaps[(MapType)i] = new Queue<Tuple<int, string>>();
                LoadMapList((MapType)i, SecondaryMaps[(MapType)i]);
            }


        }

        private void LoadMapList(MapType type, Queue<Tuple<int,string>> mapsPaths)
        {
            //Basing on mapTypes, Enqueue Tuples containing mapPath string and map index
            switch (type)
            {
                case MapType.Village:
                    for (int i = 0; i < villageSecondaryMaps; i++)
                    {
                        mapsPaths.Enqueue(new Tuple<int,string>(i + 1,$"Assets/Tilesets/Maps/AIV_Exam_SecondaryMap_Village_{i+1}.tmx"));
                    }
                    break;
                case MapType.Cave:
                    for (int i = 0; i < caveSecondaryMaps; i++)
                    {
                        mapsPaths.Enqueue(new Tuple<int, string>(i + 1,$"Assets/Tilesets/Maps/AIV_Exam_SecondaryMap_Cave_{i + 1}.tmx"));
                    }
                    break;
                case MapType.Swamp:
                    for (int i = 0; i < swampSecondaryMaps; i++)
                    {
                        mapsPaths.Enqueue(new Tuple<int, string>(i + 1,$"Assets/Tilesets/Maps/AIV_Exam_SecondaryMap_Swamp_{i + 1}.tmx"));
                    }
                    break;
                case MapType.City:
                    for (int i = 0; i < citySecondaryMaps; i++)
                    {
                        mapsPaths.Enqueue(new Tuple<int, string>(i + 1,$"Assets/Tilesets/Maps/AIV_Exam_SecondaryMap_City_{i + 1}.tmx"));
                    }
                    break;
                case MapType.Castle:
                    for (int i = 0; i < castleSecondaryMaps; i++)
                    {
                        mapsPaths.Enqueue(new Tuple<int, string>(i + 1,$"Assets/Tilesets/Maps/AIV_Exam_SecondaryMap_Castle_{i + 1}.tmx"));
                    }
                    break; 
            }

        }


        private void LoadDoorTextures()
        {
            //Matching textures to closed doors, basing on door type
            DoorTextures[MapType.Cave] = "caveDoor";
            DoorTextures[MapType.Village] = "villageDoor";
            DoorTextures[MapType.Last] = "victoryDoor";


        }

        private void LoadItems()
        {
            //Links Items type to an index
            Items[0] = typeof(Bananas);
            Items[1] = typeof(Bones);
            Items[2] = typeof(Book);
            Items[3] = typeof(Coin);
            Items[4] = typeof(Compass);
            Items[5] = typeof(Crown);
            Items[6] = typeof(Feather);
            Items[7] = typeof(Gem);
            Items[8] = typeof(Harp);
            Items[9] = typeof(Leaf);
            Items[10] = typeof(Mirror);
            Items[11] = typeof(MoneyBag);
            Items[12] = typeof(Ring);
            Items[13] = typeof(Scroll);
            Items[14] = typeof(Seashell);


        }

        private SecondaryPlayScene FindRandomScene()
        {

            SecondaryPlayScene result = null;

            //randomize mapType selection
            int randomMapType = RandomGenerator.GetRandomInt(0, (int)MapType.Last);
            int mapsCount = SecondaryMaps[(MapType)randomMapType].Count;
            //randomize scene selection, basing on the number of scenes of the selected mapType;
            int randomSceneIndex = RandomGenerator.GetRandomInt(0, mapsCount) + 1;


            do
            {

                foreach (var each in SecondaryScenes)
                {
                    //for each secondary scene confront mapType
                    if (each.Value.Type == (MapType)randomMapType)
                    {
                        //find secondary scene with selected index
                        if (each.Value.SceneIndex == randomSceneIndex)
                        {

                            //Iterate process until is found a secondary scene with no key and whose door is open
                            if (each.Value.HasKey || !each.Key.IsOpened)
                            {
                                randomSceneIndex = RandomGenerator.GetRandomInt(0, mapsCount) + 1;
                                break;
                            }
                            else
                            {
                                result = each.Value; ;

                            }
                        }


                    }
                }

            } while (result == null);

            return result;

        }


    }
}
