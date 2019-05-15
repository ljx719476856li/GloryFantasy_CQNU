using System.Collections;
using System.Collections.Generic;
using GameUnit;
using UnityEngine;

using Unit = GameUnit.GameUnit;
using GamePlay.Input;
using BattleMap;


namespace AI
{
    public class SingleAutoControllerAtker : SingleController
    {
        bool canAtk; //未到达目的地前，为false

        public SingleAutoControllerAtker(Unit _gameUnit) : base(_gameUnit) { }

        /// <summary>
        /// 自动行动
        /// </summary>
        /// <param name="battleUnit"></param>
        public override HeroActionState AutoAction()
        {
            return base.AutoAction();
        }

        /// <summary>
        /// 自动选择目标
        /// </summary>
        /// <param name="battleUnitAction"></param>
        protected override void AutoSelectTarget()
        {
            int stopDistance = AtkStopDistance();
            //从仇恨列表中确定目标
            Unit hatredUnit = null;
            //地图导航
            BattleMap.MapNavigator mapNavigator = BattleMap.BattleMap.Instance().MapNavigator;

            for (int i = 0; i < hatredRecorder.HatredCount; i++)
            {
                hatredUnit = hatredRecorder.GetHatredByIndex(i, i == 0);
                if (hatredUnit.IsDead())
                {
                    //已经排序过，且无法找到还能够行动的单位，就表示场上没有存活的敌方单位了
                    hatredUnit = null;
                    continue;
                }

                //判断这个单位是否可以到达
                bool catched = false;

                //如果这个单位就在攻击范围内，即身边
                if (Distance(battleUnit, hatredUnit) <= stopDistance)
                {
                    toTargetPath.Clear();
                    targetBattleUnit = hatredUnit;
                    canAtk = true;
                    AutoUseAtk();
                    catched = true;
                }
                else
                {
                    //if (catched = mapNavigator.PathSearch(battleUnit.CurPos, new Vector2(hatredUnit.CurPos.x, hatredUnit.CurPos.y + 1)))
                    //    toTargetPath = mapNavigator.Paths;
                    //TODO 把被仇恨单位作为起点
                    //遍历4个相邻地图块儿，把对于当前单位最近的地图块儿作为终点
                    Node nodeStart = new Node(hatredUnit.CurPos, hatredUnit.CurPos);
                    //获得A的周边MapBlock
                    List<BattleMapBlock> neighbourBlock = BattleMap.BattleMap.Instance().GetNeighbourBlock(nodeStart);
                    int prevPathCount = int.MaxValue;
                    BattleMapBlock preBattleMapBlock = null;

                    //TODO 待优化
                    foreach (BattleMapBlock battleMapBlock in neighbourBlock)
                    {
                        if (mapNavigator.PathSearchForAI(battleUnit.CurPos, battleMapBlock.position, this is SingleAutoControllerAtker))
                        {
                            //找到对于ai单位的最短路径
                            if (prevPathCount > mapNavigator.Paths.Count)
                            {
                                //更新最优路径
                                toTargetPath = mapNavigator.Paths;
                                prevPathCount = mapNavigator.Paths.Count;
                                toTargetPath = AtkPatternPathRangeMove(); //剪切多余的路径

                                //可以攻击
                                if (canAtk)
                                {
                                    if (preBattleMapBlock != null)
                                        preBattleMapBlock.RemoveUnit(battleUnit);
                                    battleMapBlock.units_on_me.Add(battleUnit);
                                    preBattleMapBlock = battleMapBlock;
                                }
                                else
                                {
                                    if (preBattleMapBlock != null)
                                        preBattleMapBlock.RemoveUnit(battleUnit);
                                    BattleMap.BattleMap.Instance().GetSpecificMapBlock(toTargetPath[0]).units_on_me.Add(battleUnit);
                                    preBattleMapBlock = BattleMap.BattleMap.Instance().GetSpecificMapBlock(toTargetPath[0]);
                                }
                                catched = true;
                            }
                        }
                    }


                }

                //寻路不可达
                if (!catched)
                {
                    hatredUnit = null;
                    continue;
                }
                else //找到了
                {
                    break;
                }
            }

            //没有目标
            if (hatredUnit == null)
            {
                targetBattleUnit = null;
                return;
            }

            if (battleUnit != null && !hatredUnit.Equals(targetBattleUnit))
            {
                targetBattleUnit = hatredUnit;
            }
        }
        /// <summary>
        /// 自动攻击
        /// 当行动单位到达目的地才能够攻击
        /// </summary>
        protected override void AutoUseAtk()
        {
            if (canAtk == false)
                return;

            //TODO 异能引入后进行修改

            //异能为引入前版本
            //获取攻击者和被攻击者
            GameUnit.GameUnit Attacker = battleUnit;
            GameUnit.GameUnit AttackedUnit = targetBattleUnit;
            //创建攻击指令
            UnitAttackCommand unitAtk = new UnitAttackCommand(Attacker, AttackedUnit);

            unitAtk.Excute();//已经判断过距离，放心攻击
        }


        /// <summary>
        /// 移动单位最远移动距离且距离目标单位最近的路径
        /// </summary>
        public List<Vector2> AtkPatternPathRangeMove()
        {
            if (toTargetPath == null)
            {
                canAtk = false;
                return null;
            }
            else if (PathCount <= battleUnit.mov)
            {
                canAtk = true;
                return toTargetPath;
            }


            toTargetPath = toTargetPath.GetRange(toTargetPath.Count - battleUnit.mov, battleUnit.mov);
            canAtk = false;
            return toTargetPath;
        }



    }

}
