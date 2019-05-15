using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;

namespace Ability
{
    public class LianJi : Ability
    {
        Trigger trigger;
        bool isActive = false;

        private void Awake()
        {
            //导入LianJi异能的参数
            InitialAbility("LianJi");
        }

        private void Start()
        {
            //创建Trigger实例，传入技能的发动者
            trigger = new TLianJi(GetComponent<GameUnit.GameUnit>().GetMsgReceiver());
            //注册Trigger进消息中心
            MsgDispatcher.RegisterMsg(trigger, "LianJi");
        }

        //这个技能被删除时要做反向操作
        //准确来说，应该是trigger启动即召唤之后删除技能才需要反向操作
        //不过也没差
        private void OnDestroy()
        {
            if (GetComponent<GameUnit.GameUnit>().priority.Count == 2)
                GetComponent<GameUnit.GameUnit>().priority.RemoveAt(2);
        }

    }

    public class TLianJi : Trigger
    {
        public TLianJi(MsgReceiver _speller)
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

                //让这只怪的priority队尾里增加一个-2的数值
                if (unit.priority.Count == 1)
                    unit.priority.Add(unit.priority[unit.priority.Count - 1] - 2);
            }
        }
    }
}