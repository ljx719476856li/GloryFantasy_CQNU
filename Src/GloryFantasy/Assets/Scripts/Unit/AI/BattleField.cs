using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unit = GameUnit.GameUnit;

namespace AI
{
    //战斗状态
    public enum BattleState
    {
        Prepare,        //准备中
        Ready,          //准备就绪
        Fighting,       //战斗中
        WaitForPlayer,  //等待玩家
        End,            //战斗结束
        Exception,      //战斗状态异常
    }

    public class BattleField
    {
        public BattleState battleState = BattleState.Prepare;

        /// <summary>
        /// 准备战斗
        /// </summary>
        private void Prepare()
        {
            battleState = BattleState.Ready;

            AIBattleController.Instance().PlayBattle(Fight);
        }

        /// <summary>
        /// 开始战斗
        /// </summary>
        private void Fight()
        {
            battleState = BattleState.Fighting;

            foreach (Unit unit in BattleMap.BattleMap.Instance().UnitsList)
            {
                //只获取敌人
                if (unit.owner != GameUnit.OwnerEnum.Enemy && battleState == BattleState.End)
                    break;

                if (!unit.IsDead())
                {
                    AI.SingleController controller = GamePlay.Gameplay.Instance().autoController.GetSingleControllerByID(unit.CurPos);
                    if(controller != null)
                    {
                        GameUnit.HeroActionState state = controller.AutoAction();

                        //TODO 状态切换
                        //目前没有切换，之后添加
                        switch (state)
                        {
                            case GameUnit.HeroActionState.WaitForPlayerChoose:
                                battleState = BattleState.WaitForPlayer;
                                break;
                            case GameUnit.HeroActionState.BattleEnd:
                                battleState = BattleState.End;
                                break;
                            case GameUnit.HeroActionState.Error:
                                battleState = BattleState.Exception;
                                break;
                            case GameUnit.HeroActionState.Warn:
                                battleState = BattleState.Exception;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 运行
        /// </summary>
        public void Run()
        {
            switch (battleState)
            {
                case BattleState.Prepare:
                    Prepare();
                    break;

                case BattleState.Ready:
                    Fight();
                    break;

                case BattleState.Fighting:
                    Fight();
                    break;

                case BattleState.WaitForPlayer:
                    break;

                case BattleState.End:
                    break;

                case BattleState.Exception:
                    break;

                default:
                    break;
            }
        }
    }
}


