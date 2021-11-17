using Aiv.Audio;
using Aiv.Fast2D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIV_Exam_SimoneSantagati
{

    class Player : Actor
    {
        public int PlayerID { get; protected set; }
        protected Controller controller;
        public PlayScene CurrentScene { get; protected set; }

        public World WorldGrid { get { return ((PlayScene)Game.CurrentScene).World; } }
        protected List<PointedItem> itemInventory;
        protected List<Key> keyInventory;

        protected Vector2 GUIItemOffset;
        protected Vector2 GUIKeyOffset;


        protected List<string[]> textureSheetNames;

        protected Animation walkAnimation;
        protected AudioSource walkSource;
        protected AudioClip walkClip;


        protected int frameWidth;
        protected int frameHeight;
        
        protected Vector2 forward;


        private float lerpMultiplier;
        protected StateMachine stateMachine;
        protected StateKey currentState;
     
        public NodePath CurrentPath { get; protected set; }
        protected NodeInfo nextNode;
        private float walkCounter;

        private float stepDuration;
        private Vector2 endNodePosition;
        private Vector2 currentNodePosition;


        protected TextObject text;
        protected TextObject nameText;
        protected Vector2 textOffset;

        protected int points;
        public int Points
        {
            get { return points; }
            set { points = value; text.Text = points.ToString(); }
        }

        public Node LastNode
        {
            get
            {
                if (CurrentPath != null && CurrentPath.Length() > 0)
                {
                    return CurrentPath.At(CurrentPath.Length() - 1);
                }

                return null;
            }

        }

        protected bool isMouseLeftPressed;
        private bool isSearchPressed;
        private bool isMouseRightPressed;

        public Player(Vector2 position, Controller ctrl, int id = 0) : base("playerIdleDown", DrawLayer.Playground, Game.TILE_UNIT_SIZE, Game.TILE_UNIT_SIZE)
        {
            controller = ctrl;
            PlayerID = id;
            Position = position;

            TextureOffsetX = 0;
            TextureOffsetY = 0;
            frameWidth = 16;
            frameHeight = 16;

            //Set GUI Offsets
            float itemOffsetX;
            float keyOffsetX;
            float nameOffsetX;
            float textOffsetX;

            if (PlayerID % 2 == 0)
            {
                itemOffsetX = 0.7f;
                keyOffsetX = 1f;
                nameOffsetX = 2f;
                textOffsetX = 5;
            }
            else
            {
                itemOffsetX = Game.Win.OrthoWidth*0.5f + 2f;
                keyOffsetX = Game.Win.OrthoWidth * 0.5f + 2.5f;
                nameOffsetX = Game.Win.OrthoWidth * 0.5f + 3f;
                textOffsetX = Game.Win.OrthoWidth-3f;

            }
            
            GUIItemOffset = new Vector2(itemOffsetX, 1.5f);
            GUIKeyOffset = new Vector2(keyOffsetX, Game.Win.OrthoHeight - 1.5f);
            textOffset = new Vector2(textOffsetX, 0.5f);
            text = new TextObject(textOffset, "", FontMgr.GetFont("stdFont"), 0.1f, 2, 2);

            Vector2 nameTextPosition = new Vector2(nameOffsetX,textOffset.Y);
            nameText = new TextObject(nameTextPosition, $"Player {PlayerID+1}", FontMgr.GetFont("stdFont"), 0.1f, 2, 2);

            //Set points when game starts
            Points = 0;

            //Set collisions
            RigidBody.Type = RigidBodyType.Player;
            RigidBody.Collider = ColliderFactory.CreateBoxFor(RigidBody, Width * 0.8f, Height * 0.8f);
            RigidBody.AddCollisionType((uint)RigidBodyType.TileObj);
            collisionLayer = CollisionLayer.Play;

            //Set speeds
            speed = 3.5f;
            walkCounter = 0;
            stepDuration = 0.1f;
            lerpMultiplier = 10;
            forward = new Vector2(0, 1);
            CurrentPath = new NodePath();

            //Instanciate animation texture by player movements direction (SheetDirection)
            textureSheetNames = new List<string[]>();
            for (int i = 0; i < (int)StateKey.Last; i++)
            {
                string[] textureNames = new string[(int)SheetDirection.Last];
                textureNames[(int)SheetDirection.Up] = $"player{PlayerID+1}" + ((StateKey)i).ToString() + "Up";
                textureNames[(int)SheetDirection.Side] = $"player{PlayerID + 1}" + ((StateKey)i).ToString() + "Side";
                textureNames[(int)SheetDirection.Down] = $"player{PlayerID + 1}" + ((StateKey)i).ToString() + "Down";
                textureSheetNames.Add(textureNames);

            }

            //Instanciate animation
            walkAnimation = new Animation(this, frameWidth, frameHeight, 4, 10);
            Components.Add("walkAnimation",walkAnimation);

            //Instanciate audio
            walkSource = new AudioSource();
            walkSource.Pitch *= 3f;
            walkSource.Volume *= 0.2f;
            walkSource.Velocity *= new Vector3(4,4,4);
            walkClip = AudioMgr.GetClip("walkClip");

            //Instanciate inventories
            itemInventory = new List<PointedItem>();
            keyInventory = new List<Key>();


            //Instanciate state machine
            stateMachine = new StateMachine();
            stateMachine.AddState(StateKey.Idle, new IdleState(this));
            stateMachine.AddState(StateKey.Wait, new WaitState(this));
            stateMachine.AddState(StateKey.Walk, new WalkState(this));
            stateMachine.GoTo(StateKey.Idle);


            IsActive = true;

        }

        #region Input

        public void Input()
        {
            //Input to centrate Camera to player's position
            if (currentState == StateKey.Idle)
            {
                if (Game.Win.mouseRight)
                {
                    if (!isMouseRightPressed)
                    {
                        CameraMgr.Behaviour = FollowBehaviour.FollowTarget;
                        isMouseRightPressed = true;

                    }

                }
                else if (isMouseRightPressed)
                {
                    isMouseRightPressed = false;
                }

            }


            //Avoid clicking outside world's limits
            if (IsMouseOverWorldLimits())
            {
                return;
            }


            //Input to initialize pathfinding
            if (Game.Win.mouseLeft)
            {
                if (!isMouseLeftPressed)
                {

                    Node endNode = WorldGrid.GetNodeAtPosition(Game.CurrentMousePosition);

                    //Inizialize pathfinding toward non collidable objects
                    if (!WorldGrid.GetTileObj(endNode).IsCollidable)
                    {

                        InitPath(endNode);

                    }
                    else //Inizialize pathfinding toward doors and hideouts
                    {

                        TileObj possibleDoor = WorldGrid.GetTileAtPosition(Game.CurrentMousePosition);
                        if (possibleDoor is Door || possibleDoor is HideOut)
                        {

                            InitPath(endNode);
                        }

                    }

                    isMouseLeftPressed = true;
                }

            }
            else if (isMouseLeftPressed)
            {
                isMouseLeftPressed = false;
            }



            //Input to interact with the enviroment
            if (controller.Search())
            {
                if (!isSearchPressed)
                {
                    if (Game.CurrentScene is MainPlayScene)
                    {
                        OpenDoor(); ;

                    }
                    else
                    {
                        SearchItem(); ;

                    }
                    isSearchPressed = true;
                }

            }
            else if (isSearchPressed)
            {

                isSearchPressed = false;

            }

        }

        public bool IsMouseOverWorldLimits()
        {
            if (Game.CurrentMousePosition.X < 0 || Game.CurrentMousePosition.X >= ((PlayScene)Game.CurrentScene).WorldWidth * Game.TILE_UNIT_SIZE
                            || Game.CurrentMousePosition.Y < 0 || Game.CurrentMousePosition.Y >= ((PlayScene)Game.CurrentScene).WorldHeight * Game.TILE_UNIT_SIZE)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region PathFinding

        public void InitPath(Node endNode)
        {

            BuildPathTo(endNode);

            //Centrate camera to player before switching to walk state
            if (currentState != StateKey.Walk)
            {
                stateMachine.GoTo(StateKey.Wait);

            }
        }

        public virtual bool BuildPathTo(Node endNode)
        {
            //Build shortest path to end node

            Node startNode = WorldGrid.GetNodeAtPosition(Position);

            //Update current path
            CurrentPath = GreedyAlgo.AStar_ShortestPath(startNode, endNode);

            if (CurrentPath == null || CurrentPath.Length() == 0)
            {
                return false;
            }


            if (CurrentPath.Length() > 1)
            {
                nextNode.SetNode(CurrentPath.At(1), 1);

            }
            else
            {
                nextNode.SetNode(CurrentPath.At(0), 0);

            }




            return true;
        }

        public virtual bool FollowPathDuration()
        {

            if (CurrentPath != null && CurrentPath.Length() > 0)
            {
                //Avoid walking on collidable tiles
                if (WorldGrid.GetTileObj(nextNode.Node).IsCollidable)
                {
                    return true;
                }

                endNodePosition = nextNode.Position;

                //Change player's forward
                if (Position != endNodePosition)
                {
                    forward = (endNodePosition - Position).Normalized();

                }

                return MoveTo();



            }

            return false;
        }



        public bool MoveTo()
        {
            //Walk towards end node within given time

            walkCounter += Game.DeltaTime;

            if (walkCounter >= stepDuration)
            {
                Position = endNodePosition;

                //When arrived set destination to next node
                if (nextNode.Index < CurrentPath.Length() - 1)
                {
                    currentNodePosition = Position;
                    int newIndex = nextNode.Index + 1;
                    nextNode.SetNode(CurrentPath.At(newIndex), newIndex);
                    walkCounter = 0;
                }
                else
                {
                    walkCounter = 0;
                    return true;
                }
            }
            else
            {
                //Lerps position
                Position = Vector2.Lerp(currentNodePosition, endNodePosition, walkCounter / stepDuration);
            }

            return false;

        }

        #endregion

        #region Interactions

        public bool OpenDoor()
        {

            TileObj positionTile = WorldGrid.GetTileAtPosition(Position);

            //Find out if there are doors around player's position
            foreach (TileObj each in WorldGrid.GetTilesAround(positionTile))
            {
                if (each is Door)
                {
                    Door door = (Door)each;

                    if (!door.IsOpened)
                    {

                        for (int i = 0; i < keyInventory.Count; i++)
                        {
                            //Unlock door if matching key is in inventory
                            if (keyInventory[i].Unlock(door))
                            {
                                RemoveFromInventory(keyInventory[i]);
                                return true;
                            }

                        }


                    }
                }


            }



            return false;


        }

        public void SearchItem()
        {

            HideOut hideOut = ((SecondaryPlayScene)Game.CurrentScene).ItemHideOut;
            TileObj positionTile = WorldGrid.GetTileAtPosition(Position);
            SecondaryPlayScene scene = (SecondaryPlayScene)Game.CurrentScene;

            //Find out if there are hideouts around player's position
            foreach (TileObj each in WorldGrid.GetTilesAround(positionTile))
            {

                if (each.Contains(hideOut.Position))
                {
                    //Get key or item, dipending on hideout's item
                    if (scene.HasKey)
                    {



                        scene.GetKey(this);




                    }
                    else if (scene.HasItem)
                    {

                        Points += scene.GetItem(this);


                    }


                }


            }




        }

        public virtual void AddToInventory(Key key)
        {
            if (key != null)
            {
                key.IsActive = true;
                keyInventory.Add(key);
                SortInventories(keyInventory);
            }
        }
        public virtual void AddToInventory(PointedItem item)
        {
            if (item != null)
            {
                item.IsActive = true;
                itemInventory.Add(item);
                SortInventories(itemInventory);
            }
        }

        public virtual void RemoveFromInventory(Key key)
        {
            if (key != null)
            {


                key.IsActive = false;

                if (keyInventory.Contains(key))
                {

                    keyInventory.Remove(key);
                    SortInventories(keyInventory);

                }

            }
        }

        public virtual void RemoveFromInventory(PointedItem item)
        {
            if (item != null)
            {
                item.IsActive = false;
                if (itemInventory.Contains(item))
                {
                    itemInventory.Remove(item);
                    SortInventories(itemInventory);

                }

            }
        }

        public virtual void SortInventories(List<Key> inventory)
        {
            //Sort inventory items' position in GUI
            for (int i = 0; i < inventory.Count; i++)
            {

                inventory[i].Position = new Vector2(GUIKeyOffset.X + i, GUIKeyOffset.Y);

            }

        }

        public virtual void SortInventories(List<PointedItem> inventory)
        {

            //Sort inventory items' position in GUI
            for (int i = 0; i < inventory.Count; i++)
            {
                float offsetY = GUIItemOffset.Y;
                float offsetX = i;

                if (i > 10)
                {
                    offsetY += Game.TILE_UNIT_SIZE + 0.2f;
                    offsetX = i % 11;
                }

                inventory[i].Position = new Vector2(GUIItemOffset.X + (offsetX * 0.6f), offsetY);

            }


        }

        #endregion

        #region Layers Management & Collisions

        public void ChangeCollisionLayer()
        {

            collisionLayer = CheckCurrentLevel();

        }

        public CollisionLayer CheckCurrentLevel()
        {
            //Check current collision layer

            CollisionLayer currentLayer = CollisionLayer;

            //Iterate tile objects in current scene
            foreach (TileObj each in ((PlayScene)Game.CurrentScene).TileObjs)
            {
                if (each.IsActive)
                {
                    
                    if (each.Contains(Position))
                    {
                        //if current tile object's layer equals player's one, player's layer is set up of one level
                        if (each.CollisionLayer == CollisionLayer)
                        {

                            ChangeDrawLayer(each.layer);
                            currentLayer = (CollisionLayer)((int)each.CollisionLayer + 1);
                            return currentLayer;

                        }
                        else if (each.CollisionLayer == CollisionLayer - 2) //if current tile object's layer is lower than two level, deacrease player's one by one level
                        {

                            ChangeDrawLayer(each.layer + 1);
                            currentLayer -= 1;
                            return currentLayer;

                        }


                    }

                }



            }

            return currentLayer;

        }

        public void ChangeDrawLayer(DrawLayer layer)
        {
            //remove and replace player from draw manager after changing his draw layer
            DrawMgr.RemoveItem(this);
            Layer = layer;
            DrawMgr.AddItem(this);

        }

        public void ChangeDirection(StateKey state)
        {
            //Change sprite texture according to forward direction
            //Right
            if (forward.Normalized().X > 0.5f)
            {
                ChangeTexture(textureSheetNames[(int)state][(int)SheetDirection.Side]);
                sprite.FlipX = false;

            }//Left
            else if (forward.Normalized().X < -0.5f)
            {
                ChangeTexture(textureSheetNames[(int)state][(int)SheetDirection.Side]);
                sprite.FlipX = true;


            }//Up
            else if (forward.Normalized().Y < -0.5f)
            {
                ChangeTexture(textureSheetNames[(int)state][(int)SheetDirection.Up]);

            }//Down
            else if (forward.Normalized().Y > 0.5f)
            {
                ChangeTexture(textureSheetNames[(int)state][(int)SheetDirection.Down]);

            }

        }

        public void ChangeTexture(string textureName)
        {
            //Change texture according to given texture name
            Texture t = GfxMgr.GetTexture(textureName);

            if (t != null && t != texture)
            {
                texture = t;

            }
        }

        public override void OnCollide(Collision collisionInfo)
        {
            if (collisionInfo.Collider is TileObj)
            {

                if (collisionInfo.Collider.CollisionLayer == CollisionLayer || collisionInfo.Collider.CollisionLayer == CollisionLayer - 1)
                {
                    OnWallCollide(collisionInfo);
                }

            }

        }

        protected void OnWallCollide(Collision collisionInfo)
        {
            if (collisionInfo.Delta.X < collisionInfo.Delta.Y)
            {
                //horizontal collision
                if (Position.X < collisionInfo.Collider.Position.X)
                {
                    collisionInfo.Delta.X = -collisionInfo.Delta.X;
                }

                Position = new Vector2(Position.X + collisionInfo.Delta.X, Position.Y);
                RigidBody.Velocity.X = 0;
            }
            else
            {
                //vertical collision


                if (Position.Y < collisionInfo.Collider.Position.Y)
                {
                    collisionInfo.Delta.Y = -collisionInfo.Delta.Y;
                }

                Position = new Vector2(Position.X, Position.Y + collisionInfo.Delta.Y);
                RigidBody.Velocity.Y = 0;

            }
        }

        #endregion

        #region Animation & Audio
        public void PlayWalkClip()
        {
           
            if (!walkSource.IsPlaying)
            {
                walkSource.Play(walkClip);

            }

        }

        public void StopWalkClip()
        {
            walkSource.Stop();
        }

        public virtual void StartAnimation()
        {
            walkAnimation.Play();
            walkAnimation.IsActive = true;
        }

        public virtual void StopAnimation()
        {
            walkAnimation.Stop();
            TextureOffsetX = 0;
            walkAnimation.IsActive = false;
        }

        #endregion

        public void UpdateFSM()
        {
            stateMachine.Update();

        }

        public void SetStartPos(Vector2 position)
        {
            currentNodePosition = position;
        }

        public void SetCurrentState(StateKey state)
        {
            currentState = state;
        }


        public void SetCurrentScene(PlayScene scene)
        {
            if (scene != null && scene!=CurrentScene)
            {
                CurrentScene = scene;
            }
        }

        public void CheckActivePlayer()
        {
            IsActive = CurrentScene == Game.CurrentScene;

        }

        public virtual void Play()
        {
            stateMachine.GoTo(StateKey.Idle);
        }


        public virtual void Reset()
        {
            IsActive = true;
        }


        public override void Draw()
        {
            if (IsActive)
            {
                sprite.DrawTexture(texture, TextureOffsetX, TextureOffsetY, frameWidth, frameHeight);

            }

        }

    }
}
