using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战区
/// </summary>
/// 
namespace BattleMap
{
    public class BattleArea
    {
        private List<int> areas;//地图块所属战区区域id
        private Dictionary<int, List<Vector2>> battleAreaDic;//战区id与战区相对应的字典
        public  Dictionary<int, List<Vector2>> BattleAreaDic{ get{return battleAreaDic;}}
        public Dictionary<int, string> battleAreaBelong;//战区归属,

        public BattleArea()
        {
            areas = new List<int>();
            battleAreaDic = new Dictionary<int, List<Vector2>>();
        }

        //获取地图块所属战区
        public void GetAreas(JsonData mapData)
        {
            int mapDataCount = mapData.Count;
            for (int i = 0; i < mapDataCount; i++)
            {
                int area = (int)mapData[i]["area"];
                areas.Add(area);
            }
            //移除重复元素
            for (int i = 0; i < areas.Count; i++)
            {
                for (int j = areas.Count - 1; j > i; j--)
                {

                    if (areas[i] == areas[j])
                    {
                        areas.RemoveAt(j);
                    }
                }
            }

            //动态增加战区数量,战区序号从-1开始
            for (int i = -1; i < areas.Count - 1; i++)
            {
                List<Vector2> battleArea = new List<Vector2>();//同一个战区上的所有地图块坐标
                battleAreaDic.Add(i, battleArea);
            }
        }

        //存储战区
        public void StoreBattleArea(int area,Vector2 mapPos)
        {
            battleAreaDic[area].Add(mapPos);
        }

        //显示战区
        public void ShowBattleZooe(Vector2 position,BattleMapBlock[,] mapBlock)
        {
            int area = mapBlock[(int)position.x, (int)position.y].area;
            List<Vector2> battleAreas = null;
            battleAreaDic.TryGetValue(area, out battleAreas);
            foreach (Vector2 pos in battleAreas)
            {
                if (!WarZoneBelong(position, mapBlock))
                {
                    mapBlock[(int)pos.x, (int)pos.y].gameObject.GetComponent<Image>().color = Color.red;
                }
                else
                {
                    mapBlock[(int)pos.x, (int)pos.y].gameObject.GetComponent<Image>().color = Color.yellow;
                }
                
            }
        }

        //隐藏战区
        public void HideBattleZooe(Vector2 position, BattleMapBlock[,] mapBlock)
        {
            int area = mapBlock[(int)position.x, (int)position.y].area;
            List<Vector2> battleAreas = null;
            battleAreaDic.TryGetValue(area, out battleAreas);
            foreach (Vector2 pos in battleAreas)
            {
                mapBlock[(int)pos.x, (int)pos.y].gameObject.GetComponent<Image>().color = Color.white;
            }
        }

        //战斗胜利条件之一：战区所属权
        public bool WarZoneBelong(Vector3 position, BattleMapBlock[,] mapBlock)
        {
            int unitAmout = 0;//战区上单位的数量
            int enemyAmout = 0;//战区上敌方单位数量
            int area = mapBlock[(int)position.x, (int)position.y].area;
            List<Vector2> battleAreas = null;
            battleAreaDic.TryGetValue(area, out battleAreas);
            foreach (Vector2 pos in battleAreas)
            {
                int x = (int)pos.x;
                int y = (int)pos.y;
                if (BattleMap.Instance().CheckIfHasUnits(pos))
                {
                    unitAmout++;
                    GameUnit.GameUnit unit = mapBlock[x, y].GetComponentInChildren<GameUnit.GameUnit>();
                    if (unit.owner == GameUnit.OwnerEnum.Enemy)
                        enemyAmout++;   
                }
            }
            if(unitAmout == 0)
            {
                //该战区没有所属权
                return true;
            }
            else if(enemyAmout == unitAmout)
            {
                //该战区被敌方控制
                return false;
            }
            else if(enemyAmout < unitAmout && enemyAmout != 0)
            {
                //该战区处于争夺状态
                return false;
            }
            else
            {
                //该战区被玩家控制
                return true;
            }
        }

        /// <summary>
        ///  战斗胜利条件之一：守卫某战区存活指定回合数
        /// </summary>
        /// <param name="area">要守卫的战区</param>
        /// <param name="curRounds">当前回合数</param>
        /// <param name="targetRounds">要守卫的目标回合数</param>
        /// <param name="mapBlock"></param>
        /// <returns></returns>
        public bool ProtectBattleZooe(int area,int curRounds,int targetRounds)
        {
            int unitAmout = 0;//该战区上我方单位数量
            List<Vector2> battleAreas = null;
            battleAreaDic.TryGetValue(area, out battleAreas);
            foreach (Vector2 pos in battleAreas)
            {
                if (BattleMap.Instance().CheckIfHasUnits(pos))
                {
                    GameUnit.GameUnit unit = BattleMap.Instance().GetUnitsOnMapBlock(pos);
                    if(unit.owner == GameUnit.OwnerEnum.Player&& curRounds <= targetRounds)
                    {
                        unitAmout++;
                    }
                }
            }
            if(unitAmout == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 战斗胜利条件与失败之一:将某单位护送到指定战区/某敌人进入指定战区
        /// </summary>
        /// <param name="area">玩家单位或敌方单位要到到达的目标战区（同一战区）</param>
        /// <param name="player">哪个玩家的单位到达</param>
        /// /// <param name="enemy">哪个敌方单位到达</param>
        /// <returns></returns>
        public int ProjectUnit(int area,GameUnit.GameUnit player,GameUnit.GameUnit enemy)//不好直接返回bool值，万一都还没见进入这个战区该返回什么？暂时就这样吧
        {
            List<Vector2> battleAreas = null;
            battleAreaDic.TryGetValue(area, out battleAreas);
            foreach (Vector2 pos in battleAreas)
            {
                if (BattleMap.Instance().CheckIfHasUnits(pos))
                {
                    GameUnit.GameUnit tempUnit = BattleMap.Instance().GetUnitsOnMapBlock(pos);
                    if(player != null&& tempUnit.id == player.id)
                    {
                            return 0;//胜利
                    }
                    
                    else if(enemy!=null && tempUnit.id == enemy.id)
                    {
                        return 1;//失败
                    }    
                }
            }
            return -1;//都还没进入指定战区
        }
    }
}
