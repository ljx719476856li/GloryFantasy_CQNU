using System.Collections;
using System.Collections.Generic;
using BattleMap;
using GamePlay.Input;
using GameUnit;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay.FSM
{
    public class InputFSMMoveState : InputFSMState
    {
        //移动状态的构造函数，传入所属状态机，并且调用基类的的构造函数
        public InputFSMMoveState(InputFSM fsm) : base(fsm)
        {
                
        }

        public override void OnPointerDownBlock(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            base.OnPointerDownBlock(mapBlock, eventData);

            //获取第一个选择的对象
            GameUnit.GameUnit unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(FSM.TargetList[0]);
            //创建移动指令
            Vector2 startPos = FSM.TargetList[0];
            Vector2 endPos = mapBlock.position;
            UnitMoveCommand unitMove = new UnitMoveCommand(unit, startPos, endPos, mapBlock.GetSelfPosition());
            //如果移动指令合法
            if (unitMove.Judge())
            {
                //移动完毕关闭移动范围染色
                Vector2 pos = BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
                FSM.HandleMovCancel(pos);
                GameUtility.UtilityHelper.Log("移动完成，进入攻击状态，点击敌人进行攻击，右键点击角色取消攻击", GameUtility.LogColor.RED);
                unitMove.Excute();

                //清空对象列表
                //FSM.TargetList.Clear();
                FSM.TargetList.Add(endPos);
                //unit.restrain = true;

                FSM.PushState(new InputFSMAttackState(FSM));//状态机压入新的攻击状态
            }
            else
            {
                //如果不符合移动条件，什么都不做
            }
        }

        public override void OnPointerDownFriendly(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            base.OnPointerDownFriendly(unit, eventData);

            
            Vector2 pos = BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
            //如果两次都点在同一个角色身上，就从移动转为攻击
            if (FSM.TargetList.Count > 0 && FSM.TargetList[0] == pos)
            {
                GameUtility.UtilityHelper.Log("取消移动，进入攻击,再次点击角色取消攻击", GameUtility.LogColor.RED);
                FSM.HandleMovCancel(pos);//关闭移动范围染色
                FSM.HandleAtkConfirm(pos);//开启攻击范围染色
                unit.restrain = true;//横置单位
                FSM.PushState(new InputFSMAttackState(FSM));//状态机压入新的攻击状态
            }
            else
            {
                //点到其他单位什么都不做
            }
        }
    }
}