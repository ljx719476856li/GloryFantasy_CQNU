using UnityEngine;
using System.Collections.Generic;
using Unit = GameUnit.GameUnit;

using GameUnit;

namespace GameGUI
{

    public class ShowRange : MonoBehaviour
    {
        private int columns;
        private int rows;
        private Unit unit;
        private bool unitMove;
        private void Awake()
        {
            this.unit = gameObject.GetComponent<Unit>();
        }

        private void Start()
        {

            columns = BattleMap.BattleMap.Instance().Columns;
            rows = BattleMap.BattleMap.Instance().Rows;
        }

        
        private List<Vector2> GetPositionsWithinCertainMd(Vector2 position, int ManhattanDistance)
        {
            List<Vector2> reslist = new List<Vector2>();
            if (unitMove)
            {
                RecrusiveBody((int)position.x, (int)position.y, ManhattanDistance, reslist);
                RemoveMapBlokHasUnit(reslist);
            }
            else
            {
                RecrusiveBody((int)position.x, (int)position.y, ManhattanDistance, reslist);
            }
            return reslist;

        }

        private void RecrusiveBody(int x, int y, int leftManhattanDistance, List<Vector2> reslist)
        {
            if (x < 0 || y < 0 || x >= columns || y >= rows) return;
            reslist.Add(new Vector2(x, y));
            if (leftManhattanDistance == 0)
                return;
            RecrusiveBody(x + 1, y, leftManhattanDistance - 1, reslist);
            RecrusiveBody(x - 1, y, leftManhattanDistance - 1, reslist);
            RecrusiveBody(x, y + 1, leftManhattanDistance - 1, reslist);
            RecrusiveBody(x, y - 1, leftManhattanDistance - 1, reslist);
        }

        private void RecrusiveBody2(int x, int y, int range, List<Vector2> reslist)
        {
            if (x < 0 || y < 0 || x >= columns || y >= rows) return;
            Vector2 centPosition = new Vector2(x, y);
            int tempRange = range%2 == 0 ? range / 2 - 1 : (range - 1) / 2 ;
            int starPosition_x =(int)centPosition.x - tempRange;
            int starPosition_y = (int)centPosition.y - tempRange;
            for(int i = 0;i < range; i++)
            {
                for(int j = 0; j < range; j++)
                {
                    reslist.Add(new Vector2(starPosition_x + j, starPosition_y + i));
                }
            }
        }


        //移动范围不显示有单位的地图块
        //TODO不显示无法到达的地图块
        private void RemoveMapBlokHasUnit(List<Vector2> reslist)
        {
            for (int i = 0; i < reslist.Count; i++)
            {
                for (int j = reslist.Count - 1; j > i; j--)
                {

                    if (reslist[i] == reslist[j])
                    {
                        reslist.RemoveAt(j);
                    }
                }
            }
            for (int i = 0; i < reslist.Count; i++)
            {
                if (BattleMap.BattleMap.Instance().CheckIfHasUnits(reslist[i]))
                {
                    reslist.Remove(reslist[i]);
                }
            }
        }

        private void RecrusiveBodyForSkill(int x,int y,int range,List<Vector2> reslist)
        {
            if (x < 0 || y < 0 || x >= columns || y >= rows) return;
            reslist.Add(new Vector2(x, y));
            if (range == 0) return;
            if(range == 2 || range == 4)
            {
                if(range == 2)
                {
                    range = range - 1;
                }
                else if(range == 4)
                {
                    range = range - 2;
                }
                RecrusiveBody(x, y, range, reslist);
            }
            if(range == 3 || range == 6)
            {
                if (range == 6) range = range - 1;
                RecrusiveBody2(x, y, range, reslist);
            }
            if(range == 5)
            {
                //TODO
                RecrusiveBody2(x, y, range, reslist);
            }
        }

        /// <summary>
        /// 返回技能范围内的所有地图快的坐标的列表
        /// </summary>
        /// <param name="position">中心坐标</param>
        /// <param name="range">范围（1-6）</param>
        /// <returns></returns>
        public List<Vector2> GetSkillRnage(Vector2 position, int range)
        {
            List<Vector2> reslist = new List<Vector2>();
            RecrusiveBodyForSkill((int)position.x, (int)position.y,range, reslist);
            return reslist;
        }

        /// <summary>
        /// 高亮单位移动范围
        /// </summary>
        /// <param name="target">单位坐标</param>
        public void MarkMoveRange(Vector2 target)
        {
            unitMove = true;
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                GetPositionsWithinCertainMd(target, unit.mov), Color.green);
        }

        /// <summary>
        /// 高亮单位攻击范围
        /// </summary>
        /// <param name="target">单位坐标</param>
        public void MarkAttackRange(Vector2 target)
        {
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                GetPositionsWithinCertainMd(target, unit.rng), Color.red);
        }

        /// <summary>
        /// 取消单位移动范围高亮
        /// </summary>
        /// <param name="target"></param>
        public void CancleMoveRangeMark(Vector2 target)
        {
            unitMove = false;
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                 GetPositionsWithinCertainMd(target, unit.mov), Color.white);   
        }

        /// <summary>
        /// 取消单位攻击范围高亮
        /// </summary>
        /// <param name="target"></param>
        public void CancleAttackRangeMark(Vector2 target)
        {
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                 GetPositionsWithinCertainMd(target, unit.rng), Color.white);
        }

        /// <summary>
        /// 高亮技能范围
        /// </summary>
        /// <param name="target">单位坐标</param>
        /// <param name="range">技能范围（范围等级（1-6））</param>
        public void MarkSkillRange(Vector2 target, int range)
        {
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                GetSkillRnage(target, range), Color.red);
        }

        /// <summary>
        /// 取消技能范围高亮
        /// </summary>
        /// <param name="target">单位坐标</param>
        /// <param name="range">技能范围（范围等级（1-6））</param>
        public void CancleSkillRangeMark(Vector2 target,int range)
        {
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                GetPositionsWithinCertainMd(target, range), Color.white);
        }
    }
}