using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;
using GamePlay.Input;
using GameGUI;

namespace Ability
{
    public class Arrowrain : Ability
    {
        Trigger trigger;
        private AbilityFormat arrowrRain;
        private int range;
        private int damage;

        private void Awake()
        {
            //导入Arrowrain异能的参数
            arrowrRain = AbilityDatabase.GetInstance().GetAbilityFormat("Arrowrain");
            InitialAbility("Arrowrain");
        }

        private void Start()
        {
            arrowrRain = AbilityDatabase.GetInstance().GetAbilityFormat("Arrowrain");
            range = (int)arrowrRain.AbilityVariable.Range;
            damage = (int)arrowrRain.AbilityVariable.Damage;
            //创建Trigger实例，传入技能的发动者
            trigger = new TArrowrain(this.GetCardReceiver(this));
            //注册Trigger进消息中心
            MsgDispatcher.RegisterMsg(trigger, "Arrowrains");
        }

        public void ReleaseSkill(GameUnit.GameUnit skillMaker,int skillRange,Vector2 skillMakerPosition, Vector2 targetPositon)
        {
            ReleaseSkillCommand releaseSkillCommand = new ReleaseSkillCommand(skillMaker, skillRange, skillMakerPosition, targetPositon);
            if (releaseSkillCommand.Judge())
            {
                Debug.Log("箭雨发动");
                //TODO技能伤害范围染色
                Gameplay.Instance().gamePlayInput.HandleSkillCancel(BattleMap.BattleMap.Instance().GetUnitCoordinate(skillMaker), skillRange);//取消技能可释放范围染色

                List<Vector2> vector2s = new List<Vector2>();
                GameUnit.GameUnit unit = (GameUnit.GameUnit)this.GetSelectingUnits()[0];
                Vector3 unitCoordinate = BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
                vector2s = unit.GetComponent<ShowRange>().GetSkillRnage(unitCoordinate, range);
                foreach(Vector2 vector2 in vector2s)
                {
                    if (BattleMap.BattleMap.Instance().CheckIfHasUnits(vector2))
                    {
                        GameUnit.GameUnit gameUnit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(vector2);
                        gameUnit.hp -= damage;
                    }
                }
                //TODO取消技能伤害范围染色
                //GamePlay.Gameplay.Instance().gamePlayInput.isSkill = false;//箭雨释放完毕;
            }
        }
    }

    public class TArrowrain : Trigger
    {
        public TArrowrain(MsgReceiver speller)
        {
            register = speller;
            //初始化响应时点,为卡片使用时
            msgName = (int)MessageType.CastCard;
            //初始化条件函数和行为函数
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            //判断发动的卡是不是这个技能的注册者，并且这张卡是不是箭雨
            if (this.GetCastingCard().GetMsgReceiver() == register && this.GetCastingCard().id == "WArrowrain_1")
                return true;
            else
                return false;
        }

        private void Action()
        {
            GameUnit.GameUnit unit = (GameUnit.GameUnit)this.GetSelectingUnits()[0];
            Vector3 unitCoordinate =  BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
            AbilityFormat ability = AbilityDatabase.GetInstance().GetAbilityFormat("Arrowrain");
            Gameplay.Instance().gamePlayInput.HandleSkillConfim(unitCoordinate,(int)ability.AbilityVariable.Range);//显示技能可释放范围（攻击范围)
            //GamePlay.Gameplay.Instance().gamePlayInput.isSkill = true;//可以发动技能
        }
    }
}
