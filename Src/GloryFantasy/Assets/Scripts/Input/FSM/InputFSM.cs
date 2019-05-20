using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BattleMap;
using UnityEngine.EventSystems;
using GameGUI;
using GameCard;
using GameUtility;

namespace GamePlay.FSM
{
    /// <summary>
    /// 输入控制状态机，用来控制玩家的UI操作输入
    /// </summary>
    public class InputFSM : FSMachine<InputFSMState>
    {
        public InputFSM()
        {
            this.PushState(new InputFSMIdleState(this));
        }

        /// <summary>
        /// 存储点击的对象坐标
        /// </summary>
        public List<Vector2> TargetList = new List<Vector2>();
        /// <summary>
        /// 存储点击的手牌
        /// </summary>
        public BaseCard selectedCard;
        /// <summary>
        /// 存储发动的指令牌的异能
        /// </summary>
        public Ability.Ability ability;

        /// <summary>
        /// 处理地图方块的鼠标点击
        /// </summary>
        /// <param name="mapBlock"></param>
        /// <param name="eventData"></param>
        public void OnPointerDownBlock(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            StateStack.Peek().OnPointerDownBlock(mapBlock, eventData);
            Debug.Log(BattleMap.BattleMap.Instance().GetCoordinate(mapBlock));
        }
        /// <summary>
        /// 处理玩家单位的鼠标点击
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="eventData"></param>
        public void OnPointerDownFriendly(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            StateStack.Peek().OnPointerDownFriendly(unit, eventData);
        }
        /// <summary>
        /// 处理敌人单位的鼠标点击
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="eventData"></param>
        public void OnPointerDownEnemy(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            StateStack.Peek().OnPointerDownEnemy(unit, eventData);
        }
        /// <summary>
        /// 处理地图方块的鼠标进入
        /// </summary>
        /// <param name="mapBlock"></param>
        /// <param name="eventData"></param>
        public void OnPointerEnter(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            if (BattleMap.BattleMap.Instance().IsColor == true)
            {
                BattleMap.BattleMap.Instance().ShowBattleZooe(mapBlock.GetSelfPosition());
            }
            //TODO显示技能伤害的范围
        }
        /// <summary>
        /// 处理地图方块的鼠标移出
        /// </summary>
        /// <param name="mapBlock"></param>
        /// <param name="eventData"></param>
        public void OnPointerExit(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            if (BattleMap.BattleMap.Instance().IsColor == true)
            {
                BattleMap.BattleMap.Instance().HideBattleZooe(mapBlock.GetSelfPosition());
            }
            //TODO
        }
        /// <summary>
        /// 处理单位牌的点击召唤
        /// </summary>
        /// <param name="unitCard"></param>
        public void OnPointerDownUnitCard(BaseCard unitCard)
        {
            selectedCard = unitCard;
            this.PushState(new InputFSMSummonState(this));
        }
        /// <summary>
        /// 处理指令牌的释放
        /// </summary>
        /// <param name="ability"></param>
        public void OnCastCard(Ability.Ability ability)
        {
            this.ability = ability;
            this.PushState(new InputFSMCastState(this));
            this.TargetList.Clear();
        }


        //移动范围染色
        public void HandleMovConfirm(Vector2 target)
        {
            BattleMap.BattleMap map = BattleMap.BattleMap.Instance();
            if (map.CheckIfHasUnits(target))
            {
                GameUnit.GameUnit unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(target);
                unit.GetComponent<ShowRange>().MarkMoveRange(target);
            }
        }

        public void HandleMovCancel(Vector2 target)
        {
            GameUnit.GameUnit unit = null;
            if (BattleMap.BattleMap.Instance().CheckIfHasUnits(target))
            {
                unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(target);
            }
            else
            {
                unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(TargetList[0]);
            }
            unit.GetComponent<ShowRange>().CancleMoveRangeMark(TargetList[0]);
        }

        //攻击范围染色
        public void HandleAtkConfirm(Vector2 target)
        {
            BattleMap.BattleMap map = BattleMap.BattleMap.Instance();
            if (map.CheckIfHasUnits(target))
            {
                GameUnit.GameUnit unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(target);
                unit.GetComponent<ShowRange>().MarkAttackRange(target);
            }
        }

        public void HandleAtkCancel(Vector2 target)
        {
            GameUnit.GameUnit unit = null;
            if (BattleMap.BattleMap.Instance().CheckIfHasUnits(target))
            {
                unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(target);
            }
            else
            {
                unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(TargetList[0]);
            }
            unit.GetComponent<ShowRange>().CancleAttackRangeMark(target);
        }
    }
}
