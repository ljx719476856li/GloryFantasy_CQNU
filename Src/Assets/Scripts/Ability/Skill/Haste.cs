using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;

namespace Ability
{
    public class Haste : Ability
    {
        Trigger trigger;

        private void Awake()
        {
            //导入Haste异能的参数
            InitialAbility("Haste");
        }

        private void Start()
        {
            trigger = new THaste(GetComponent<GameUnit.GameUnit>().GetMsgReceiver());
            MsgDispatcher.RegisterMsg(trigger, "Haste");
        }

    }

    public class THaste : Trigger
    {
        public THaste(MsgReceiver _speller)
        {
            register = _speller;
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

                //让这只怪部署后可以移动
                unit.restrain = false;
                //让这只怪部署后可以攻击
                unit.disarm = false;
            }
        }
    }
}