using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;

namespace Ability
{
    public class Regeneration : Ability
    {
        Trigger trigger;

        private void Awake()
        {
            //导入Regeneration异能的参数
            InitialAbility("Regeneration");
        }

        private void Start()
        {
            //创建Trigger实例，传入技能的发动者
            trigger = new TRegeneration(GetComponent<GameUnit.GameUnit>().GetMsgReceiver());
            //注册Trigger进消息中心
            MsgDispatcher.RegisterMsg(trigger, "Regeneration");
        }

    }

    public class TRegeneration : Trigger
    {
        public TRegeneration(MsgReceiver speller)
        {
            register = speller;
            //初始化响应时点
            msgName = (int)MessageType.Dead;
            //初始化条件函数和行为函数
            condition = Condition;
            action = Action;
        }
        private void Action()
        {
            //保存死掉的怪
            GameUnit.GameUnit deadUnit = this.GetDead();
            //复活死掉的怪并保存
            GameUnit.GameUnit newUnit = this.Regenerate(deadUnit.id, this.GetUnitPosition(deadUnit));
            //修改这只怪的血量
            newUnit.hp -= newUnit.hp / 2;
            //删除这只怪的复活技能
            this.DeleteUnitAbility(newUnit, "Regeneration");
        }

        private bool Condition()
        {
            //判断死掉的怪是不是这个复活技能的注册者
            if (this.GetDead().GetMsgReceiver() == register)
                return true;
            else
                return false;
        }
    }
}