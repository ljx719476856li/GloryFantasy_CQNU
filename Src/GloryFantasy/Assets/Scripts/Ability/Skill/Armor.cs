using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;

namespace Ability
{
    //护甲1
    public class Armor1 : Ability
    {
        Trigger trigger;
        public int armor = 1;

        private void Awake()
        {
            //导入Armor异能的参数
            InitialAbility("Armor");
        }

        private void Start()
        {
            //获得这个技能的持有者，并修改其护甲恢复值
            GetComponent<GameUnit.GameUnit>().armorRestore = armor;
            //创建Trigger实例，传入技能的发动者和护甲恢复值
            trigger = new TArmor(GetComponent<GameUnit.GameUnit>().GetMsgReceiver());
            //注册Trigger进消息中心
            MsgDispatcher.RegisterMsg(trigger, "Armor");
        }

    }

    public class TArmor : Trigger
    {

        public TArmor(MsgReceiver _speller)
        {
            register = _speller;
            msgName = (int)MessageType.BP;
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            return true;
        }

        private void Action()
        {
            //获取发动这个技能的怪
            GameUnit.GameUnit unit = register.GetUnit<GameUnit.GameUnit>();
            //用护甲恢复值去修正护甲值
            if (unit.armor < unit.armorRestore)
                unit.armor = unit.armorRestore;
        }
    }
}