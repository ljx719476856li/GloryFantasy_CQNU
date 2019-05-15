using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;

namespace Ability
{
    public class Ponder : Ability
    {
        Trigger trigger;

        private void Awake()
        {
            //导入Ponder异能的参数
            InitialAbility("Ponder");
        }

        private void Start()
        {
            //创建Trigger实例，传入技能的发动者
            trigger = new TInstantIdea(this.GetUnitReceiver(this));
            //注册Trigger进消息中心
            MsgDispatcher.RegisterMsg(trigger, "Ponder");
        }
    }

    public class TPonder : Trigger
    {
        int Amount = 2;

        public TPonder(MsgReceiver speller)
        {
            register = speller;
            //初始化响应时点,为卡片使用时
            msgName = (int)MessageType.CastCard;
            //初始化条件函数和行为函数
            action = Action;
            condition = Condition;
        }

        private bool Condition()
        {
            return true;
        }

        private void Action()
        {

            //TODO抽两张牌
            GameCard.CardManager.Instance().ExtractCards(3);
            //TODO选择手牌

            //TODO将选择的冷却两回合
        }
    }
}
