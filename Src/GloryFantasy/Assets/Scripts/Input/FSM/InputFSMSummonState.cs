using System.Collections;
using System.Collections.Generic;
using BattleMap;
using GameCard;
using GamePlay.Input;
using GameUnit;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay.FSM
{
    public class InputFSMSummonState : InputFSMState
    {
        public InputFSMSummonState(InputFSM fsm) : base(fsm)
        { }

        public override void OnPointerDownBlock(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            base.OnPointerDownBlock(mapBlock, eventData);

            //在对应MapBlock生成单位
            UnitManager.InstantiationUnit(FSM.selectedCard.id, OwnerEnum.Player, mapBlock);
            //把这张手牌从手牌里删掉
            CardManager.Instance().RemoveCardToMapList(FSM.selectedCard.gameObject);
            // 扣除消耗的Ap值
            Player.Instance().ConsumeAp(FSM.selectedCard.GetComponent<BaseCard>().cost);
            //删掉对应手牌槽的引用
            FSM.selectedCard = null;
            //关闭鼠标所在战区的高光显示
            BattleMap.BattleMap.Instance().IsColor = false;
            BattleMap.BattleMap.Instance().HideBattleZooe(mapBlock.GetSelfPosition());
            //创建部署指令并执行
            BattleDispositionCommand unitDispose = new BattleDispositionCommand(mapBlock.units_on_me);
            unitDispose.Excute();
            //状态机压入静止状态
            this.FSM.PushState(new InputFSMIdleState(FSM));
        }
    }
}