using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;

namespace Ability
{
    public class CuringWind : Ability
    {
        Trigger trigger;
        bool isActive = false;

        private void Awake()
        {
            //导入Wit异能的参数
            InitialAbility("CuringWind");
        }

        private void Start()
        {
            //创建Trigger实例，传入技能的发动者
            trigger = new TCuringWind(this.GetCardReceiver(this));
            //注册Trigger进消息中心
            MsgDispatcher.RegisterMsg(trigger, "CuringWind");
        }


    }

    public class TCuringWind : Trigger
    {
        public TCuringWind(MsgReceiver _speller)
        {
            register = _speller;
            //响应时点是发动卡牌
            msgName = (int)MessageType.CastCard;
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            return true;
        }

        private void Action()
        {
            List<GameUnit.GameUnit> gameUnits = BattleMap.BattleMap.Instance().GetFriendlyUnitsList();
            foreach (GameUnit.GameUnit unit in gameUnits)
            {
                unit.hp += 5;
            }
        }
    }
}