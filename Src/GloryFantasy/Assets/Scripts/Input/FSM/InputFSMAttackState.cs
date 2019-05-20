using System.Collections;
using System.Collections.Generic;
using GamePlay.Input;
using GameUnit;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay.FSM
{
    public class InputFSMAttackState : InputFSMState
    {
        public InputFSMAttackState(InputFSM fsm) : base(fsm)
        {

        }

        public override void OnPointerDownEnemy(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            base.OnPointerDownEnemy(unit, eventData);

            //获取攻击者和被攻击者
            
            GameUnit.GameUnit Attacker = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(FSM.TargetList[FSM.TargetList.Count - 1]);
            GameUnit.GameUnit AttackedUnit = unit;
            //创建攻击指令
            UnitAttackCommand unitAtk = new UnitAttackCommand(Attacker, AttackedUnit);
            //如果攻击指令符合条件则执行
            if (unitAtk.Judge())
            {
                GameUtility.UtilityHelper.Log("触发攻击", GameUtility.LogColor.RED);
                FSM.HandleAtkCancel(BattleMap.BattleMap.Instance().GetUnitCoordinate(Attacker));////攻击完工攻击范围隐藏  
                unitAtk.Excute();
                Attacker.disarm = true; //单位横置不能攻击
                FSM.TargetList.Clear(); //清空对象列表
                FSM.PushState(new InputFSMIdleState(FSM)); //状态机压入静止状态
            }
            else
            {
                //如果攻击指令不符合条件就什么都不做
            }
        }

        public override void OnPointerDownFriendly(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            base.OnPointerDownFriendly(unit, eventData);
            //鼠标右键取消攻击
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                GameUtility.UtilityHelper.Log("取消攻击", GameUtility.LogColor.RED);
                FSM.HandleAtkCancel(BattleMap.BattleMap.Instance().GetUnitCoordinate(unit));
                unit.restrain = true;
                unit.disarm = true;
                FSM.TargetList.Clear();
                FSM.PushState(new InputFSMIdleState(FSM));
            }
        }
    }
}