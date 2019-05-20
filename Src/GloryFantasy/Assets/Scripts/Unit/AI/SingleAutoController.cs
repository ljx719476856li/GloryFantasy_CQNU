using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unit = GameUnit.GameUnit;
using GamePlay.Input;
using BattleMap;

namespace AI
{
    public class SingleController
    { 
        public SingleController(Unit _gameUnit)
        {
            battleUnit = _gameUnit;
            targetBattleUnit = null;
            hatredRecorder = new HatredRecorder();
            toTargetPath = new List<Vector2>();
        }
        //所控制棋子
        protected Unit battleUnit;
        public Unit BattleUnit
        {
            get
            {
                return battleUnit;
            }
        }

        //目标单位
        protected Unit targetBattleUnit;
        //仇恨列表记录器
        public HatredRecorder hatredRecorder;
        //移动到目标的路径
        protected List<Vector2> toTargetPath;

        /// <summary>
        /// 路径长度
        /// </summary>
        protected int PathCount
        {
            get
            {
                return toTargetPath.Count;
            }
        }

        /// <summary>
        /// 起点
        /// </summary>
        protected Vector2 StartPos
        {
            get
            {
                return toTargetPath[PathCount - 1];
            }
        }

        /// <summary>
        /// 终点
        /// </summary>
        protected Vector2 EndPos
        {
            get
            {
                return toTargetPath[0];
            }
        }

        /// <summary>
        ///技能相关
        ///获取技能的停止移动距离
        /// </summary>
        protected int SkillStopDistance
        {
            get
            {
                return -1; //目前还未使用异能
            }
        }

        /// <summary>
        /// 自动行动
        /// </summary>
        /// <param name="battleUnit"></param>
        public virtual GameUnit.HeroActionState AutoAction()
        {
            //自动选取目标
            AutoSelectTarget();

            //找不到目标单位
            if (targetBattleUnit == null && battleUnit != null)
            {
                return GameUnit.HeroActionState.Warn; ;
            }

            UnitMoveAICommand unitMove;
            //需要移动
            if (battleUnit != null && PathCount > 0)
            {
                unitMove = new UnitMoveAICommand(battleUnit, toTargetPath, AutoUseAtk);
                Debug.Log("AI StartPos: " + StartPos + " EndPos: " + EndPos);
                unitMove.Excute(); //已经判断过距离
            }

            //TODO 战斗结束
            if (false)
                return GameUnit.HeroActionState.BattleEnd;
            else
                return GameUnit.HeroActionState.Normal;
        }

        /// <summary>
        /// 自动选择目标
        /// </summary>
        /// <param name="battleUnitAction"></param>
        protected virtual void AutoSelectTarget()
        {
        }

        /// <summary>
        /// 自动攻击
        /// </summary>
        protected virtual void AutoUseAtk()
        {
        }

        /// <summary>
        /// 获取攻击时的停止距离，近战，远程不同
        /// </summary>
        /// <returns>攻击范围</returns>
        protected int AtkStopDistance()
        {
            return battleUnit.rng;
        }

        protected int Distance(Unit unit1, Unit unit2)
        {
            return Mathf.Abs((int)unit1.CurPos.x - (int)unit2.CurPos.x) + Mathf.Abs((int)unit1.CurPos.y - (int)unit2.CurPos.y);
        }
    }
}


