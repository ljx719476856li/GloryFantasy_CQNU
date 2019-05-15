using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BattleMap;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GameGUI;
using GameUnit;
using GameCard;
using System;
using GamePlay.FSM;

namespace GamePlay.Input
{
    /// <summary>
    /// 游戏输入控制类
    /// </summary>
    public class GameplayInput
    {
        //public FiniteStateMachine FSM;
        public InputFSM InputFSM;

        public GameplayInput()
        {
            InputFSM = new InputFSM();
        }

        /// <summary>
        /// 标记是否已经选择了一张手牌,在召唤状态
        /// </summary>
        public bool IsSelectingCard
        {
            get
            {
                return InputFSM.CurrentState is InputFSMSummonState;
            }
        }

        /// <summary>
        /// 获取释放异能的选择目标
        /// </summary>
        public List<object> SelectingList
        {
            get
            {
                List<object> tmpList = new List<object>();
                foreach (Vector2 pos in InputFSM.TargetList)
                {
                    if (BattleMap.BattleMap.Instance().CheckIfHasUnits(pos))
                        tmpList.Add(BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(pos) as object);
                    else
                        tmpList.Add(BattleMap.BattleMap.Instance().GetSpecificMapBlock(pos) as object);
                }
                return tmpList;
            }
        }

        /// <summary>
        /// 处理地图方块的鼠标点击
        /// </summary>
        /// <param name="mapBlock"></param>
        /// <param name="eventData"></param>
        public void OnPointerDownBlock(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            InputFSM.OnPointerDownBlock(mapBlock, eventData);
        }
        /// <summary>
        /// 处理玩家单位的鼠标点击
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="eventData"></param>
        public void OnPointerDownFriendly(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            InputFSM.OnPointerDownFriendly(unit, eventData);
        }
        /// <summary>
        /// 处理敌人单位的鼠标点击
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="eventData"></param>
        public void OnPointerDownEnemy(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            this.InputFSM.OnPointerDownEnemy(unit, eventData);
        }
        /// <summary>
        /// 设置被选中的手牌槽
        /// </summary>
        /// <param name="currentItemUI"></param>
        internal void OnPointerDownUnitCard(GameObject currentItemInstance)
        {
            InputFSM.OnPointerDownUnitCard(currentItemInstance.GetComponent<BaseCard>());
        }
        /// <summary>
        /// 设置要使用的效果牌
        /// </summary>
        /// <param name="ability"></param>
        public void OnUseOrderCard(Ability.Ability ability)
        {
            InputFSM.OnCastCard(ability);
        }

        /// <summary>
        /// 处理地图方块的鼠标进入
        /// </summary>
        /// <param name="mapBlock"></param>
        /// <param name="eventData"></param>
        public void OnPointerEnter(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            InputFSM.OnPointerEnter(mapBlock, eventData);
        }
        /// <summary>
        /// 处理地图方块的鼠标移出
        /// </summary>
        /// <param name="mapBlock"></param>
        /// <param name="eventData"></param>
        public void OnPointerExit(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            InputFSM.OnPointerExit(mapBlock, eventData);
        }


        //技能可释放范围染色
        public void HandleSkillConfim(Vector2 target,int range)
        {
            BattleMap.BattleMap map = BattleMap.BattleMap.Instance();
            if (map.CheckIfHasUnits(target))
            {
                GameUnit.GameUnit unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(target);
                unit.GetComponent<ShowRange>().MarkSkillRange(target,range);
            }
        }

        //取消可释放技能范围染色
        public void HandleSkillCancel(Vector2 target,int range)
        {
            GameUnit.GameUnit unit = null;
            if (BattleMap.BattleMap.Instance().CheckIfHasUnits(target))
            {
                unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(target);
            }
            else
            {
                //unit = BeforeMoveGameUnits[0];
            }
            unit.GetComponent<ShowRange>().CancleSkillRangeMark(InputFSM.TargetList[0],range);
        }
        /// <summary>
        /// 单位回收
        /// </summary>
        /// <param name="deadUnit"></param>
        internal void UnitBackPool(GameUnit.GameUnit deadUnit)
        {
            //回收单位
            GameUnitPool.Instance().PushUnit(deadUnit.gameObject);
            //移除对应地图块儿下的死亡单位
            BattleMap.BattleMap.Instance().RemoveUnitOnBlock(deadUnit);
        }

        /// <summary>
        /// 更新血条HP
        /// </summary>
        /// <param name="attackedUnit">受攻击单位</param>
       internal void UpdateHp(GameUnit.GameUnit attackedUnit)
        {
            float hpDivMaxHp = (float)attackedUnit.hp / attackedUnit.MaxHP * 100;
            var textHp = attackedUnit.transform.GetComponentInChildren<Text>();
            textHp.text = string.Format("Hp: {0}%", Mathf.Ceil(hpDivMaxHp));
        }

        #region 玩家的UI操作输入检测，状态机前版本，现已注释
        ///// <summary>
        ///// 处理地图方块的鼠标点击
        ///// </summary>
        ///// <param name="mapBlock"></param>
        ///// <param name="eventData"></param>
        //public void OnPointerDown(BattleMapBlock mapBlock, PointerEventData eventData)
        //{
        //    if (IsMoving)
        //    {
        //        GameUnit.GameUnit unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(TargetList[0]);
        //        Vector2 startPos = TargetList[0];
        //        Vector2 endPos = mapBlock.position;
        //        UnitMoveCommand unitMove = new UnitMoveCommand(unit, startPos, endPos,  mapBlock.GetSelfPosition() );
        //        if (unitMove.Judge())
        //        {
        //            GameUtility.UtilityHelper.Log("移动完成，进入攻击状态，点击敌人进行攻击，右键点击角色取消攻击", GameUtility.LogColor.RED);
        //            unitMove.Excute();
        //            SetMovingIsFalse(unit);//并清空targetList
        //            IsAttacking = true;
        //            unit.restrain = true;
        //            unit.disarm = true;
        //        }
        //        else
        //        {
        //            //如果不符合移动条件，什么都不做
        //        }
        //    }
        //    //如果已经选中了一张手牌
        //    else if (IsSelectingCard)
        //    {
        //        //如果不是自己的战区，则无操作
        //        //if (!BattleMap.BattleMap.Instance().WarZoneBelong(mapBlock.GetSelfPosition())) return;
        //        //做个判断，如果选中的手牌不是单位卡则返回不操作
        //        if (_selectedCardInstance.GetComponent<BaseCard>().type != "Unit") return;
        //        //在对应MapBlock生成单位
        //        UnitManager.InstantiationUnit(_selectedCardInstance.GetComponent<BaseCard>().id , OwnerEnum.Player, mapBlock);
        //        //把这张手牌从手牌里删掉
        //        CardManager.Instance().RemoveCardToMapList(_selectedCardInstance);
        //        // 扣除消耗的Ap值
        //        Player.Instance().ConsumeAp(_selectedCardInstance.GetComponent<BaseCard>().cost);
        //        //删掉对应手牌槽的引用
        //        _selectedCardInstance = null;
        //        //关闭鼠标所在战区的高光显示
        //        BattleMap.BattleMap.Instance().IsColor = false;
        //        BattleMap.BattleMap.Instance().HideBattleZooe(mapBlock.GetSelfPosition());

        //        //创建部署指令并执行
        //        BattleDispositionCommand unitDispose = new BattleDispositionCommand(mapBlock.units_on_me);
        //        unitDispose.Excute();
        //    }
        //    //如果正在释放指令牌，就视为正在选择目标
        //    else if (IsCasting)
        //    {
        //        if (this.CastingCard.AbilityTargetList[SelectingList.Count].TargetType == Ability.TargetType.Field  ||
        //            this.CastingCard.AbilityTargetList[SelectingList.Count].TargetType == Ability.TargetType.All)
        //        {
        //            SelectingList.Add(mapBlock);
        //        }
        //        //如果已经选够了目标就发动卡片
        //        //这里应该让Card那边写个发动卡片的函数，写在Input里不科学
        //        if (SelectingList.Count == this.CastingCard.AbilityTargetList.Count)
        //        {
        //            Gameplay.Info.CastingCard = this.CastingCard.GetComponent<OrderCard>();
        //            // 消耗Ap值
        //            Player.Instance().ConsumeAp(Gameplay.Info.CastingCard.cost);
        //            IMessage.MsgDispatcher.SendMsg((int)IMessage.MessageType.CastCard);
        //        }
        //    }
        //}
        ///// <summary>
        ///// 处理单位的鼠标点击
        ///// </summary>
        ///// <param name="unit"></param>
        ///// <param name="eventData"></param>
        //public void OnPointerDown(GameUnit.GameUnit unit, PointerEventData eventData)
        //{
        //    //鼠标右键取消攻击
        //    if (IsAttacking == true && eventData.button == PointerEventData.InputButton.Right)
        //    {
        //        GameUtility.UtilityHelper.Log("取消攻击", GameUtility.LogColor.RED);
        //        HandleAtkCancel(BattleMap.BattleMap.Instance().GetUnitCoordinate(unit));
        //        IsAttacking = false;
        //        unit.restrain = true;
        //        unit.disarm = false;
        //        IsMoving = false;
        //        BeforeMoveGameUnits.Clear();
        //        TargetList.Clear();
        //    }
        //    else if (IsAttacking)
        //    {
        //        if (unit.owner == OwnerEnum.Enemy)
        //        {
        //            //获取攻击者和被攻击者
        //            Debug.Log(BeforeMoveGameUnits[0]);
        //            GameUnit.GameUnit Attacker = BeforeMoveGameUnits[0];
        //            GameUnit.GameUnit AttackedUnit = unit;
        //            //创建攻击指令
        //            UnitAttackCommand unitAtk = new UnitAttackCommand(Attacker, AttackedUnit);
        //            //如果攻击指令符合条件则执行
        //            if (unitAtk.Judge())
        //            {
        //                GameUtility.UtilityHelper.Log("触发攻击", GameUtility.LogColor.RED);
        //                unitAtk.Excute();
        //                IsAttacking = false;
        //                BeforeMoveGameUnits[0].restrain = true;
        //                IsMoving = false;
        //                unit.disarm = true;
        //                HandleAtkCancel(BattleMap.BattleMap.Instance().GetUnitCoordinate(BeforeMoveGameUnits[0]));////攻击完工攻击范围隐藏  
        //                BeforeMoveGameUnits.Clear();
        //                TargetList.Clear();
        //            }
        //            else
        //            {
        //                //如果攻击指令不符合条件就什么都不做
        //            }
        //        }
        //    }
        //    else if (IsMoving)
        //    {
        //        //如果移动两次都选择同一个单位，就进行一次待机
        //        Vector2 pos = BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
        //        if (TargetList[0] == pos)
        //        {
        //            GameUtility.UtilityHelper.Log("取消移动，进入攻击,再次点击角色取消攻击", GameUtility.LogColor.RED);
        //            SetMovingIsFalse(unit);
        //            HandleAtkConfirm(BattleMap.BattleMap.Instance().GetUnitCoordinate(unit));
        //            unit.restrain = true;
        //            IsAttacking = true;
        //        }
        //        else
        //        {
        //            //点到其他单位什么都不做
        //        }
        //    }
        //    else if (IsCasting)
        //    {
        //        if ((this.CastingCard.AbilityTargetList[SelectingList.Count].TargetType == Ability.TargetType.Enemy && unit.owner == OwnerEnum.Enemy) ||
        //            (this.CastingCard.AbilityTargetList[SelectingList.Count].TargetType == Ability.TargetType.Friendly && unit.owner == OwnerEnum.Player) ||
        //            (this.CastingCard.AbilityTargetList[SelectingList.Count].TargetType == Ability.TargetType.Field) ||
        //            (this.CastingCard.AbilityTargetList[SelectingList.Count].TargetType == Ability.TargetType.All))
        //        {
        //            SelectingList.Add(unit);
        //        }
        //        //如果已经选够了目标就发动卡片
        //        //这里应该让Card那边写个发动卡片的函数，写在Input里不科学
        //        if (SelectingList.Count == this.CastingCard.AbilityTargetList.Count)
        //        {
        //            Gameplay.Info.CastingCard = this.CastingCard.GetComponent<OrderCard>();
        //            // 消耗Ap值
        //            Player.Instance().ConsumeAp(Gameplay.Info.CastingCard.cost);
        //            IMessage.MsgDispatcher.SendMsg((int)IMessage.MessageType.CastCard);
        //        }
        //    }
        //    //如果单位可以移动
        //    else if (unit.restrain == false && unit.owner == OwnerEnum.Player)
        //    {
        //        GameUtility.UtilityHelper.Log("准备移动，再次点击角色取消移动进入攻击", GameUtility.LogColor.RED);
        //        SetMovingIsTrue(unit);
        //    }
        //    //如果单位已经不能移动，但是可以攻击
        //    else if (unit.restrain == true && unit.disarm == false)
        //    {
        //        BeforeMoveGameUnits.Add(unit);
        //        GameUtility.UtilityHelper.Log("准备攻击，右键取消攻击", GameUtility.LogColor.RED);
        //        IsAttacking = true;
        //        TargetList.Add(BattleMap.BattleMap.Instance().GetUnitCoordinate(unit));
        //    }
        //}
        #endregion
    }
}
