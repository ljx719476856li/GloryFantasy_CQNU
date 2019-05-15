using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BattleMap;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GameGUI;
using GameUnit;
using GameCard;
using GameUtility;

namespace GamePlay.FSM
{
    /// <summary>
    /// 输入状态机的状态的基类，和InputFSM配套使用
    /// </summary>
    public class InputFSMState : FSMState
    {
        /// <summary>
        /// 自己所属状态机的引用，在构造函数更新
        /// </summary>
        protected InputFSM FSM;

        //构造函数,protected是不想被外部类new
        protected InputFSMState(InputFSM fsm)
        {
            this.FSM = fsm;
        }

        /// <summary>
        /// 处理地图方块的鼠标点击
        /// </summary>
        /// <param name="mapBlock"></param>
        /// <param name="eventData"></param>
        virtual public void OnPointerDownBlock(BattleMapBlock mapBlock, PointerEventData eventData)
        {

        }
        /// <summary>
        /// 处理玩家单位的鼠标点击
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="eventData"></param>
        virtual public void OnPointerDownFriendly(GameUnit.GameUnit unit, PointerEventData eventData)
        {

        }
        /// <summary>
        /// 处理敌人单位的鼠标点击
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="eventData"></param>
        virtual public void OnPointerDownEnemy(GameUnit.GameUnit unit, PointerEventData eventData)
        {

        }
        /// <summary>
        /// 处理单位卡的鼠标点击，召唤行为
        /// </summary>
        /// <param name="unitCard"></param>
        /// <param name="eventData"></param>
        virtual public void OnPointerDownUnitCard(BaseCard unitCard, PointerEventData eventData)
        {

        }
        /// <summary>
        /// 处理地图方块的鼠标进入
        /// </summary>
        /// <param name="mapBlock"></param>
        /// <param name="eventData"></param>
        virtual public void OnPointerEnter(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            if (BattleMap.BattleMap.Instance().IsColor == true)
            {
                BattleMap.BattleMap.Instance().ShowBattleZooe(mapBlock.GetSelfPosition());
            }
        }
        /// <summary>
        /// 处理地图方块的鼠标移出
        /// </summary>
        /// <param name="mapBlock"></param>
        /// <param name="eventData"></param>
        virtual public void OnPointerExit(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            if (BattleMap.BattleMap.Instance().IsColor == true)
            {
                BattleMap.BattleMap.Instance().HideBattleZooe(mapBlock.GetSelfPosition());
            }
        }
    }
}