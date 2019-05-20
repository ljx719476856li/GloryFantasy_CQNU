using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using LitJson;
using System.IO;
using Unit = GameUnit.GameUnit;
using IMessage;
using UnityEngine.UI;
using GameUnit;
using System.Collections;

namespace BattleMap
{
    public class BattleMap : UnitySingleton<BattleMap>, MsgReceiver
    {

        private void Awake()
        {
            _unitsList = new List<Unit>();
            _instance = this;
            IsColor = false;
            MapNavigator = new MapNavigator();
            battleArea = new BattleArea();
            debuffBM = new DebuffBattleMapBlovk();
            encounter = new Encounter.EncounterData();
            BattleMapPath = "Assets/Scripts/BattleMap/";
        }

        private void Start()
        {
            InitMap();

            MsgDispatcher.RegisterMsg(
               this.GetMsgReceiver(),
               (int)MessageType.MPBegin,
               canDoMPAction,
               MpBegin,
               "Mp Begin Trigger"
           );

            MsgDispatcher.RegisterMsg(
               this.GetMsgReceiver(),
               (int)MessageType.MPEnd,
               canDoMPEndAction,
               MpEnd,
               "Mp End Trigger"
           );
        }


        /// <summary>
        /// 检测是否能进行主要阶段，现在暂时设定为永true,是主要阶段的condition函数
        /// </summary>
        /// <returns>根据实际情况确定是否能进入主要阶段</returns>
        public bool canDoMPAction()
        {
            return true;
        }       
        /// <summary>
        /// 检测是否能进行主要阶段，现在暂时设定为永true,是主要阶段的condition函数
        /// </summary>
        /// <returns>根据实际情况确定是否能进入主要阶段</returns>
        public bool canDoMPEndAction()
        {
            return true;
        }

        /// <summary>
        /// 主要阶段j开始
        /// </summary>
        public void MpBegin()
        {

        }
        /// <summary>
        /// 主要阶段结束
        /// </summary>
        public void MpEnd()
        {

        }

        public void InitMap()
        {
            //读取并存储遭遇
            encounter.InitEncounter();
            //初始化地图
            InitAndInstantiateMapBlocks();
        }

        //初始化地图的地址
        //更改地图数据位置则需修改此处路径
        private string BattleMapPath;
        // 获取战斗地图上的所有单位
        private List<Unit> _unitsList;//TODO考虑后面是否毁用到，暂留
        public List<Unit> UnitsList{get{return _unitsList;}}              
        private int columns;                 // 地图方块每列的数量
        private int rows;                    // 地图方块每行的数量
        public int Columns{get{return columns;}}
        public int Rows{get{return rows;}}                    
        public int BlockCount{get{return columns * rows;}}
        public bool IsColor { get; set; }//控制是否高亮战区
        private BattleMapBlock[,] _mapBlocks; 
        public GameObject normalMapBlocks;//实例地图块的prefab，普通的地图方块
        public Transform _tilesHolder;          // 存储所有地图单位引用的变量
        public MapNavigator MapNavigator;//寻路类
        public BattleArea battleArea;//战区类
        public DebuffBattleMapBlovk debuffBM;//异常地图快类
        public Encounter.EncounterData encounter;//持有遭遇类
        private string[][] nstrs;//存战斗地图的数组
        [SerializeField]
        private GameObject battlePanel;//战斗地图，用于初始战斗地图大小


        /// <summary>
        /// 初始战斗地图路径
        /// </summary>
        /// <param name="mapID">地图名字</param>
        public void  InitBattleMapPath(string mapID)
        {
            BattleMapPath = BattleMapPath + mapID + ".txt";
        }

        //初始战斗地图
        private void InitAndInstantiateMapBlocks()
        {
            encounter.InitEncounter("Forest_Shadow_1");//测试临时放在这里，对接后删除；
            encounter.InitBattlefield("Forest_Shadow_1");

            //读取战斗地图文件
            string[] strs = File.ReadAllLines(BattleMapPath);
            nstrs = new string[strs.Length][];
            for(int i = 0;i < nstrs.Length; i++)
            {
                nstrs[i] = strs[i].Split('/');
            }

            this.rows = nstrs.Length;
            this.columns = nstrs[0].Length;
            battlePanel.GetComponent<GridLayoutGroup>().constraintCount = this.columns;//初始化战斗地图大小（列数）
            _mapBlocks = new BattleMapBlock[columns, rows];
            GameObject instance = new GameObject();
            battleArea.GetAreas(nstrs);//存储战区id;
            //实例地图块
            for (int y = 0; y < nstrs.Length; y++)
            {
                for(int x = 0;x <nstrs[y].Length; x++)
                {
                    instance = GameObject.Instantiate(normalMapBlocks, new Vector3(x, y, 0f), Quaternion.identity);
                    instance.transform.SetParent(_tilesHolder);
                    instance.gameObject.AddComponent<BattleMapBlock>();
                    //初始化mapBlock成员
                    _mapBlocks[x, y] = instance.gameObject.GetComponent<BattleMapBlock>();
                    int area = int.Parse(nstrs[y][x].Split('-')[1]);
                    _mapBlocks[x, y].area = area;
                    _mapBlocks[x, y].x = x;
                    _mapBlocks[x, y].y = y;
                    _mapBlocks[x, y].blockType = EMapBlockType.normal;
                    //初始化地图块儿的collider组件
                    _mapBlocks[x, y].bmbCollider.init(_mapBlocks[x, y]);
                    GamePlay.Gameplay.Instance().bmbColliderManager.InitBMB(_mapBlocks[x, y].bmbCollider);

                    battleArea.StoreBattleArea(area, new Vector2(x,y));//存储战区
                }
            }

            InitAndInstantiateGameUnit("Forest_Shadow_1");//初始战斗地图上的单位
        }


        #region 有了新的地图读取后，可以删除，还没对接，暂时保留
        public string InitialMapDataPath = "/Scripts/BattleMap/eg1.json";//待删除
        private void InitAndInstantiateMapBlocks1()
        {
            JsonData mapData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + InitialMapDataPath));
            int mapDataCount = mapData.Count;
            this.columns = (int)mapData[mapDataCount - 1]["y"] + 1;
            this.rows = (int)mapData[mapDataCount - 1]["x"] + 1;
            _mapBlocks = new BattleMapBlock[rows, columns];
            GameObject instance = new GameObject();
            int x = 0;
            int y = 0;
            int area = 0;

            battleArea.GetAreas(mapData);

            for (int i = 0; i < mapDataCount; i++)
            {
                x = (int)mapData[i]["x"];
                y = (int)mapData[i]["y"];
                area = (int)mapData[i]["area"];
                Vector2 mapPos = new Vector2(x, y);
                battleArea.StoreBattleArea(area, mapPos);

                //实例化地图块
                instance = GameObject.Instantiate(normalMapBlocks, new Vector3(x, y, 0f), Quaternion.identity);
                instance.transform.SetParent(_tilesHolder);
                instance.gameObject.AddComponent<BattleMapBlock>();
                //初始化mapBlock成员
                _mapBlocks[x, y] = instance.gameObject.GetComponent<BattleMapBlock>();
                _mapBlocks[x, y].area = area;
                _mapBlocks[x, y].x = x;
                _mapBlocks[x, y].y = y;
                _mapBlocks[x, y].blockType = EMapBlockType.normal;
                //初始化地图块儿的collider组件
                _mapBlocks[x, y].bmbCollider.init(_mapBlocks[x, y]);

                GamePlay.Gameplay.Instance().bmbColliderManager.InitBMB(_mapBlocks[x, y].bmbCollider);
                int tokenCount = mapData[i]["token"].Count;
                if (tokenCount == 1)
                {
                    Unit unit = InitAndInstantiateGameUnit(mapData[i]["token"][0], x, y);
                    unit.mapBlockBelow = _mapBlocks[x, y];
                    unit.gameObject.GetComponent<GameUnit.GameUnit>().owner = GameUnit.OwnerEnum.Enemy;

                    _unitsList.Add(unit);
                    _mapBlocks[x, y].AddUnit(unit);

                    AI.SingleController controller;
                    //初始化AI控制器与携带的仇恨列表
                    if (_unitsList.Count == 3 || _unitsList.Count == 5 || _unitsList.Count == 6)
                        controller = new AI.SingleAutoControllerAtker(unit); //无脑型
                    else
                        controller = new AI.SingleAutoControllerDefender(unit);//防守型
                    controller.hatredRecorder.Reset(unit);
                    GamePlay.Gameplay.Instance().autoController.singleControllers.Add(controller);

                }
            }

        }
        #endregion

        /// <summary>
        /// 获取传入寻路结点相邻的方块列表
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<BattleMapBlock> GetNeighbourBlock(Node node)
        {
            List<BattleMapBlock> neighbour = new List<BattleMapBlock>();
            int x = (int)node.position.x;
            int y = (int)node.position.y;
            if (GetSpecificMapBlock(x - 1, y) != null && GetSpecificMapBlock(x - 1, y).units_on_me.Count == 0)
            {
                neighbour.Add(GetSpecificMapBlock(x - 1, y));
            }
            if (GetSpecificMapBlock(x + 1, y) != null && GetSpecificMapBlock(x + 1, y).units_on_me.Count == 0)
            {
                neighbour.Add(GetSpecificMapBlock(x + 1, y));
            }
            if (GetSpecificMapBlock(x, y - 1) != null && GetSpecificMapBlock(x, y - 1).units_on_me.Count == 0)
            {
                neighbour.Add(GetSpecificMapBlock(x, y - 1));
            }
            if (GetSpecificMapBlock(x, y + 1) != null && GetSpecificMapBlock(x, y + 1).units_on_me.Count == 0)
            {
                neighbour.Add(GetSpecificMapBlock(x, y + 1));
            }
            return neighbour;
        }

        #region//初始地图单位，对接完成后删除，暂留
        private Unit InitAndInstantiateGameUnit(JsonData data, int x, int y)
        {
            Unit newUnit;
            GameObject _object;
            //TODO:怎么没有根据所有者分别做处理
            OwnerEnum owner;
            switch (data["Controler - Enemy, Friendly or Self"].ToString())
            {
                case ("Enemy"):
                    owner = OwnerEnum.Enemy; break;
                case ("Friendly"):
                    owner = OwnerEnum.Neutrality; break;
                case ("Self"):
                    owner = OwnerEnum.Player; break;
                default:
                    owner = OwnerEnum.Enemy;break;
            }
            //从对象池获取单位
            _object = GameUnitPool.Instance().GetInst(data["name"].ToString(), owner, (int)data["Damaged"]);     
            //修改单位对象的父级为地图方块
            _object.transform.SetParent(_mapBlocks[x, y].transform);
            _object.transform.localPosition = Vector3.zero;


            //TODO 血量显示 test版本, 此后用slider显示
            var TextHp = _object.transform.GetComponentInChildren<Text>();
            var gameUnit = _object.GetComponent<GameUnit.GameUnit>();
            float hp = gameUnit.hp/* - Random.Range(2, 6)*/;
            float maxHp = gameUnit.MaxHP;
            float hpDivMaxHp = hp / maxHp * 100;
            TextHp.text = string.Format("Hp: {0}%", hpDivMaxHp);              
            
            newUnit = _object.GetComponent<Unit>();
            return newUnit;
        }
        #endregion

        /// <summary>
        /// 初始战斗地图上的单位
        /// </summary>
        /// <param name="encounterID">遭遇id</param>
        private void InitAndInstantiateGameUnit(string encounterID)
        {
            JsonData data = encounter.GetEncounterDataByID(encounterID);
            if (data == null)
                return;

            JsonData unitData = data["UnitMessage"];
            int unitDataCount = unitData.Count;
            OwnerEnum owner;
            GameObject _object;
            for (int i = 0; i < unitDataCount; i++)
            {
                int x = (int)unitData[i]["Pos_X"];
                int y = (int)unitData[i]["Pos_Y"];
                //单位控制者:0为玩家，1为敌方AI_1,2为敌方AI_2，...
                switch (unitData[i]["UnitControler"].ToString())
                {
                    case ("0"):
                        owner = OwnerEnum.Player;break;
                    case ("1"):
                        owner = OwnerEnum.Enemy;break;
                    default:
                        owner = OwnerEnum.Enemy; break;
                }
                //从对象池获取单位
                _object = GameUnitPool.Instance().GetInst(unitData[i]["UnitID"].ToString(), owner);

                Unit unit = _object.GetComponent<Unit>();
                //修改单位对象的父级为地图方块
                _mapBlocks[x, y].AddUnit(unit);
                // _object.transform.SetParent(_mapBlocks[x, y].transform); 
                //_object.transform.localPosition = Vector3.zero;
                _unitsList.Add(unit);
                unit.mapBlockBelow = _mapBlocks[x, y];

                AI.SingleController controller;
                //初始化AI控制器与携带的仇恨列表
                if (_unitsList.Count == 0 || _unitsList.Count == 3 || _unitsList.Count == 5)
                    controller = new AI.SingleAutoControllerAtker(unit); //无脑型
                else
                    controller = new AI.SingleAutoControllerDefender(unit);//防守型
                controller.hatredRecorder.Reset(unit);
                GamePlay.Gameplay.Instance().autoController.singleControllers.Add(controller);


                //TODO 血量显示 test版本, 此后用slider显示
                var TextHp = _object.transform.GetComponentInChildren<Text>();
                var gameUnit = _object.GetComponent<GameUnit.GameUnit>();
                float hp = gameUnit.hp/* - Random.Range(2, 6)*/;
                float maxHp = gameUnit.MaxHP;
                float hpDivMaxHp = hp / maxHp * 100;
                TextHp.text = string.Format("Hp: {0}%", hpDivMaxHp);
            }
        }

        //TODO 根据坐标返回地图块儿 --> 在对应返回的地图块儿上抓取池内的对象，"投递上去"
        //TODO 相当于是召唤技能，可以与郑大佬的技能脚本产生联系
        //TODO 类似做一个召唤技能，通过UGUI的按钮实现

        //TODO 如何实现
        //1. 首先我们输入一个坐标 -> 传递给某个函数，此函数能够根据坐标获得地图块儿 -> 获取到地图块儿后便可以通过地图块儿，从池子中取出Unit “投递”到该地图块儿上
        //2. 完成
        //3. 转移成一个skill

        /// <summary>
        /// 将单位设置在MapBlock下
        /// </summary>
        public void SetUnitToMapBlock()
        {
            //TODO：完善一下
        }

        /// <summary>
        /// 传入坐标，获取对应的MapBlock。坐标不合法会返回null
        /// </summary>
        /// <returns></returns>
        public BattleMapBlock GetSpecificMapBlock(Vector2 pos)
        {
            return GetSpecificMapBlock((int)pos.x, (int)pos.y);
        }
        /// <summary>
        /// 传入坐标，获取对应的MapBlock。坐标不合法会返回null
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public BattleMapBlock GetSpecificMapBlock(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < columns && y < rows)
                return this._mapBlocks[x, y];
            return null;
        }
        /// <summary>
        /// 传入MapBlock，返回该MapBlock的坐标
        /// </summary>
        /// <param name="mapBlock"></param>
        /// <returns></returns>
        public Vector3 GetCoordinate(BattleMapBlock mapBlock)
        {
            for (int i = columns - 1; i >= 0; i--)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (_mapBlocks[i, j] == mapBlock)
                    {
                        return new Vector3(i, j, 0f);
                    }
                }
            }
            return new Vector3(-1, -1, 0f);
        }
        /// <summary>
        /// 确定给定坐标上是否含有单位，坐标不合法会返回false，其他依据实际情况返回值
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Boolean CheckIfHasUnits(Vector3 vector)
        {
            if (this._mapBlocks[(int)vector.x, (int)vector.y] != null && this._mapBlocks[(int)vector.x, (int)vector.y].transform.childCount != 0
                && this._mapBlocks[(int)vector.x, (int)vector.y].GetComponentInChildren<Unit>() != null &&
                this._mapBlocks[(int)vector.x, (int)vector.y].GetComponentInChildren<Unit>().id != "Obstacle"/*units_on_me.Count != 0*/)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 返回给定坐标上单位list，坐标不合法会返回null, 其他依据实际情况返回值
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Unit GetUnitsOnMapBlock(Vector3 vector)
        {
            if (this._mapBlocks[(int)vector.x, (int)vector.y] != null && this._mapBlocks[(int)vector.x, (int)vector.y].transform.childCount != 0)
            {
                return _mapBlocks[(int)vector.x, (int)vector.y].GetComponentInChildren<Unit>();
            }
            return null;
        }
        /// <summary>
        /// 传入坐标，返回该坐标的MapBlock的Type
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public EMapBlockType GetMapBlockType(Vector3 coordinate)
        {
            int x = (int)coordinate.x;
            int y = (int)coordinate.y;
            if (x < 0 || y < 0 || x >= columns || y >= rows)
            {
                // TODO: 添加异常坐标处理
            }

            return _mapBlocks[x, y].blockType;
        }
        /// <summary>
        /// 根据给定unit寻找其所处坐标，若找不到则会返回不合法坐标
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public Vector3 GetUnitCoordinate(Unit unit)
        {

            foreach (Unit gameUnit in _unitsList)
            {
                if (gameUnit == unit)
                {
                    return gameUnit.mapBlockBelow.GetCoordinate();
                }
            }

            return new Vector3(-1, -1, -1);
        }
        /// <summary>
        /// 传入unit和坐标，将Unit瞬间移动到该坐标（仅做坐标变更，不做其他处理）
        /// <param name="unit">移动的目标单位</param>
        /// <param name="gameobjectCoordinate">地图块儿自身的物体坐标</param>
        /// <returns></returns>
        /// </summary>
        public bool MoveUnitToCoordinate(Vector2 gameobjectCoordinate, Unit unit)
        {
            foreach (Unit gameUnit in _unitsList)
            {
                if (gameUnit == unit)
                {
                    unit.mapBlockBelow.RemoveUnit(unit);
                    if (_mapBlocks[(int)gameobjectCoordinate.x, (int)gameobjectCoordinate.y] == null)
                        return false;
                    unit.mapBlockBelow = _mapBlocks[(int)gameobjectCoordinate.x, (int)gameobjectCoordinate.y];
                    _mapBlocks[(int)gameobjectCoordinate.x, (int)gameobjectCoordinate.y].AddUnit(unit);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 传入unit和坐标，将Unit瞬间移动到该坐标（仅做坐标变更，不做其他处理）
        /// <param name="unit">移动的目标单位</param>
        /// <param name="gameobjectCoordinate">地图块儿自身的物体坐标</param>
        /// <returns></returns>
        /// </summary>
        public bool MoveUnitToCoordinate(Unit unit,  Vector2 gameobjectCoordinate)
        {
            foreach (Unit gameUnit in _unitsList)
            {
                if (gameUnit == unit)
                {
                    unit.mapBlockBelow.RemoveUnit(unit);
                    if (_mapBlocks[(int)gameobjectCoordinate.x, (int)gameobjectCoordinate.y] != null)
                    {
                        unit.mapBlockBelow = _mapBlocks[(int)gameobjectCoordinate.x, (int)gameobjectCoordinate.y];
                    }
                    StartCoroutine(MapNavigator.moveStepByStep(unit));                    
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// AI移动
        /// </summary>
        /// <param name="unit">目标单位</param>
        /// <param name="targetPosition">最优路径</param>
        /// <param name="callback">攻击回调</param>
        /// <returns></returns>
        public bool AIMoveUnitToCoordinate(Unit unit, List<Vector2> targetPosition, System.Action callback)
        {
            foreach (Unit gameUnit in _unitsList)
            {
                if (gameUnit == unit)
                {
                    unit.mapBlockBelow.RemoveUnit(unit);
                    if (_mapBlocks[(int)targetPosition[0].x, (int)targetPosition[0].y] != null)
                    {
                        unit.mapBlockBelow = _mapBlocks[(int)targetPosition[0].x, (int)targetPosition[0].y];
                    }
                    StartCoroutine(MapNavigator.moveStepByStepAI(unit, targetPosition, callback));                    
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 地图方块染色接口
        /// </summary>
        /// <param name="positions">染色的方块坐标</param>
        /// <param name="color">方块要被染的颜色</param>
        public void ColorMapBlocks(List<Vector2> positions, Color color)
        {
            foreach (Vector3 position in positions)
            {
                if (position.x < columns && position.y < rows && position.x >= 0 && position.y >= 0)
                {
                    _mapBlocks[(int)position.x, (int)position.y].gameObject.GetComponent<Image>().color = color;
                }
            }
        }

        #region 战区
        /// <summary>
        /// 战区所属权，传入一个坐标，判断该坐标所在的战区的所属权(胜利条件之一)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool WarZoneBelong(Vector3 position)
        {
            return battleArea.WarZoneBelong(position, _mapBlocks);
        }

        //战区胜利条件之一：守卫战区指定回合数
        public bool ProtectBattleZooe(int area, int curRounds, int targetRounds)
        {
            return battleArea.ProtectBattleZooe(area,curRounds,targetRounds);
        }

        //战区胜利条件之一：将某单位护送到指定战区/某敌人进入指定战区
        public int ProjectUnit(int area, Unit player = null, Unit enemy = null)
        {
            return battleArea.ProjectUnit(area,player,enemy);
        }

        //显示战区
        public void ShowBattleZooe(Vector3 position)
        {
            battleArea.ShowBattleZooe(position, _mapBlocks);
        }

        /// <summary>
        /// 用于确定给定坐标地图块所属的接口
        /// </summary>
        /// <param name="position">合法的坐标</param>
        /// <returns>若地图块拥有单位，则返回对应的单位所属，若无，则返回中立</returns>
        public GameUnit.OwnerEnum GetMapblockBelong(Vector3 position)
        {
            if (CheckIfHasUnits(position))
            {
                return _mapBlocks[(int) position.x, (int) position.y].GetComponentInChildren<GameUnit.GameUnit>().owner;
            }

            return OwnerEnum.Neutrality;
        }

        //隐藏战区
        public void HideBattleZooe(Vector2 position)
        {
            battleArea.HideBattleZooe(position, _mapBlocks);
        }
        /// <summary>
        /// 移除BattleBlock下的 unit
        /// </summary>
        public void RemoveUnitOnBlock(Unit deadUnit)
        {
            //获取死亡单位的Pos
            Vector2 unitPos = GetUnitCoordinate(deadUnit);
            //通过unitPos的坐标获取对应的地图块儿
            BattleMapBlock battleMap = GetSpecificMapBlock(unitPos);
            //移除对应地图块儿下的deadUnit
            battleMap.units_on_me.Remove(deadUnit);
        }
        #endregion 

        /// <summary>
        /// 返回我方所有单位
        /// </summary>
        /// <returns></returns>
        public List<Unit> GetFriendlyUnitsList()
        {
            List<Unit> friendlyUnits = new List<Unit>();
            foreach (Unit unit in _unitsList)
            {
                if(unit.owner == OwnerEnum.Player)
                {
                    friendlyUnits.Add(unit);
                }
            }

            return friendlyUnits;
        }


        T IMessage.MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }
    }
}