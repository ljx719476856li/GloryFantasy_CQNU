using System;
using GamePlay.Round;
using IMessage;
using System.Collections.Generic;
using BattleMap;
using GameUnit;
using UnityEngine;

namespace GamePlay
{
    public class GameJudge : UnitySingleton<GameJudge>, MsgReceiver
    {
        private Dictionary<int, List<Vector2>> _battleAreaDictionary;
        
        private void Start()
        {
            _battleAreaDictionary =
                BattleMap.BattleMap.Instance().battleArea.BattleAreaDic;
            
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.Dead,
                NeedToDoJudge,
                CountEnemyControllingArea,
                "Game Judge CBA trigger"
            );
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.Encounter,
                InEncounter,
                EncounterJudge,
                "Encounter Game Judge CBA trigger"
            );   
        }

        /// <summary>
        /// 判断是否处于遭遇战状态，决定是否用遭遇战的获胜条件
        /// 先默认返回false，因为还没有弄遭遇战，所以以下遭遇战规则不能用，用了也报错。。。
        /// </summary>
        /// <returns></returns>
        public bool InEncounter()
        {
            return false;
        }
        
        
        /// <summary>
        /// 用于判断是否需要进行judge的工作，若已处于结果状态则不需要再响应处理请求
        /// </summary>
        /// <returns>若已经在结果状态返回false，否则返回true</returns>
        public bool NeedToDoJudge()
        {
            if (Gameplay.Instance().roundProcessController.IsResultState())
                return false;
            return true;
        }

        /// <summary>
        /// 本函数本意是根据敌方控制战区数量情况确定胜负
        /// 若敌方控制所有战区，判定失败，敌方无控制战区，判定胜利
        /// </summary>
        public void CountEnemyControllingArea()
        {
            int enemyCountrolAreaAmount = 0;
            int allBattleAreaAmount = 0;
            
            foreach (int areaID in _battleAreaDictionary.Keys)
            {
                int enemyAmount = 0;
                int playerUnitAmount = 0;
                allBattleAreaAmount++;
                foreach (Vector2 pos in _battleAreaDictionary[areaID])
                {
                    OwnerEnum ownerEnum = BattleMap.BattleMap.Instance().GetMapblockBelong(pos);
                    if (ownerEnum == OwnerEnum.Player)
                    {
                        playerUnitAmount++;
                    } else if (ownerEnum == OwnerEnum.Enemy)
                    {
                        enemyAmount++;
                    }
                }
                
                // 若战区内敌我单位数量一致
                if (playerUnitAmount == enemyAmount)
                {
                    // 若都为0，那就是中立战区，不影响战区所属判定，继续下一个判断
                    if(enemyAmount == 0)
                        continue;
                    
                    // 若均不为0，说明处于争夺状态，则该战区属于敌方
                    enemyCountrolAreaAmount++;
                }
                // 我方单位数量比敌方少，该战区属于敌方单位
                else if (playerUnitAmount < enemyAmount)
                {
                    enemyCountrolAreaAmount++;
                }
                
                // 剩下的情况不影响结果，不检测了
                // if playerUnitAmount > enemyAmount
            }
            
            // 若敌方控制所有战区，则游戏失败，通知状态机
            if (allBattleAreaAmount == enemyCountrolAreaAmount)
            {
                Gameplay.Instance().roundProcessController.Lose();
                return;
            }
                
            // 若敌方无控制战区，则游戏胜利，通知状态机
            if(enemyCountrolAreaAmount == 0)
                Gameplay.Instance().roundProcessController.Win();
        }


        /// <summary>
        /// 注册遭遇战胜负判断消息
        /// </summary>
        public void EncounterJudge()
        {
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.Dead,
                NeedToDoJudge,
                KillSpecifyUnit,
                "Game Judge KillUnit trigger"
            );
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.RoundsEnd,
                NeedToDoJudge,
                ProtectArea,
                "Game Judge ProtectArea trigger"
            );MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.Aftermove,
                NeedToDoJudge,
                EscortUnitToArea,
                "Game Judge ProtectUnitToArea trigger"
            );
        }
        
        //TODO: 添加判断是否处于遭遇战以使用遭遇战胜利条件判断
        //TODO: 等待胜利条件指定单位及战区接口

        #region 遭遇战胜利条件

        // 胜利条件先这样写着吧，胜利条件看的有点迷迷糊糊
        // 里面的指定单位和战区都是遭遇战里面的吧，先空着等待接口
        
        /// <summary>
        /// 护送某单位到指定战区
        /// 判断单位位置是否在指定战区范围内
        /// 己方单位先到战区则胜利
        /// 敌方单位先到战区则失败
        /// 都没到但是己方单位死亡也失败
        /// </summary>
        public void EscortUnitToArea()
        {
            //TODO: 获取进入遭遇战后指定单位接口
            const int success = 0;
            const int failure = 1;
            const int notUnit = -1;
            GameUnit.GameUnit player = null;               // 被护送单位
            GameUnit.GameUnit enemy = null;                // 敌方单位
            int AreaID = 0;                                // 指定战区id
            BattleMap.BattleMap battleMap = BattleMap.BattleMap.Instance();

//            if (_battleAreaDictionary[AreaID].Contains(currentPos)) // 到达指定战区
//            {
//                Gameplay.Instance().roundProcessController.Win();
//            }
//            else if(player.IsDead())                                    // 未到达指定战区且死亡则失败
//            {
//                Gameplay.Instance().roundProcessController.Lose();
//            }
            if (battleMap.ProjectUnit(AreaID, player, enemy) == success)
            {
                Gameplay.Instance().roundProcessController.Win();
            }
            else if (battleMap.ProjectUnit(AreaID, player, enemy) == failure)
            {
                Gameplay.Instance().roundProcessController.Lose();
            }

            if (battleMap.ProjectUnit(AreaID, player, enemy) == notUnit && player.IsDead())
            {
                Gameplay.Instance().roundProcessController.Lose();
            }
        }


        /// <summary>
        /// 守卫战区存活指定回合数
        /// </summary>
        public void ProtectArea()
        {
            //TODO: 获取
            int areaID = 0;
            int curRounds = 0;
            int targetRounds = 0;
            if (!BattleMap.BattleMap.Instance().ProtectBattleZooe(areaID, curRounds, targetRounds))
            {
                Gameplay.Instance().roundProcessController.Lose();
            }
            else if(targetRounds == 0)
            {
                Gameplay.Instance().roundProcessController.Win();
            }
        }


        /// <summary>
        /// 击杀指定单位，指定单位死亡则胜利，否则失败
        /// </summary>
        public void KillSpecifyUnit()
        {
            GameUnit.GameUnit BeKilledUnit = null;                   // 指定被击杀单位
            //TODO：获取指定的单位的引用，等待接口
            if (BeKilledUnit.IsDead())
            {
                Gameplay.Instance().roundProcessController.Win();
            }
            else
            {
                Gameplay.Instance().roundProcessController.Lose();
            }
        }
        
        #endregion

        
        /// <summary>
        /// 仿照主程写的写的接口
        /// </summary>
        T MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }
    }
}