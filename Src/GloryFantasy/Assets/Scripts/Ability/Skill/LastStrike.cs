using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;

namespace Ability
{
    public class LastStrike : Ability
    {
        Trigger trigger;
        bool isActive = false;

        private void Awake()
        {
            //导入FirstStrike异能的参数
            InitialAbility("LastStrike");
        }

        private void Start()
        {
            //创建Trigger实例，传入技能的发动者
            trigger = new TLastStrike(GetComponent<GameUnit.GameUnit>().GetMsgReceiver());
            //注册Trigger进消息中心
            MsgDispatcher.RegisterMsg(trigger, "LastStrike");
        }

        //这个技能被删除时要做反向操作
        //准确来说，应该是trigger启动即召唤之后删除技能才需要反向操作
        //不过因为怪被summon之后才能被玩家看见，所以技能被删除时直接反向也没差
        private void OnDestroy()
        {
            GetComponent<GameUnit.GameUnit>().priSPD +=2;
        }

    }

    public class TLastStrike : Trigger
    {
        public TLastStrike(MsgReceiver _speller)
        {
            register = _speller;
            //响应时点是被召唤时
            msgName = (int)MessageType.Summon;
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            //获取召唤列表
            List<GameUnit.GameUnit> SummonUnits = this.GetSummonUnit();
            //循环查询有没有召唤的怪是这个技能的发动者
            for (int i = 0; i < SummonUnits.Count; i++)
            {
                if (SummonUnits[i].GetMsgReceiver() == register)
                    return true;
            }
            return false;
        }

        private void Action()
        {
            //获取发动这个技能的怪
            List<GameUnit.GameUnit> SummonUnits = this.GetSummonUnit();
            GameUnit.GameUnit unit = null;
            for (int i = 0; i < SummonUnits.Count; i++)
            {
                if (SummonUnits[i].GetMsgReceiver() == register)
                    unit = SummonUnits[i];

                //让这只怪的SPD修正值+2
                unit.priSPD -= 2;
            }
        }
    }
}

