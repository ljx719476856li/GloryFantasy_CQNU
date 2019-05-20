using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Ability;
using Ability.Buff;
using IMessage;
using GamePlay;
using GameCard;
using GamePlay.Input;
using GamePlay.Round;

namespace IMessage
{
    //TODO 关于Info的问题
    //每次攻击都会更新他的ATK部分吗？
    //会不会因为在同一回合中攻击太频繁导致出现BUG

    //更啊，又不会同时发生攻击，怕什么bug

    public class Info
    {
        public PlayerEnum RoundOwned; //回合所属
        public PlayerEnum Caster; //施放者
        public BaseCard CastingCard; //施放的牌
        public PlayerEnum SummonersController; //召唤单位的控制者
        public List<GameUnit.GameUnit> SummonUnit; //召唤的单位
        public PlayerEnum Drawer; //抓牌者
        public List<BaseCard> CaughtCard; //抓的牌
        public PlayerEnum HandAdder; //加手者
        public List<BaseCard> AddingCard; //加手的牌
        public GameUnit.GameUnit AbilitySpeller; //发动异能者

        #region ATK部分
        public Ability.Ability SpellingAbility; //发动的异能
        public GameUnit.GameUnit Attacker; //宣言攻击者
        public GameUnit.GameUnit AttackedUnit; //被攻击者
        public GameUnit.GameUnit Injurer; //伤害者
        public GameUnit.GameUnit InjuredUnit; //被伤害者
        public Damage damage; //伤害
        public GameUnit.GameUnit Killer; //击杀者
        public GameUnit.GameUnit KilledUnit; //被杀者
        public GameUnit.GameUnit Dead; //死者
        public int ATKDistance; //攻击者与被攻击者之间的距离
        #endregion

        /// <summary>
        /// 被UI选定的Unit单位
        /// </summary>
        public GameUnit.GameUnit SelectingUnit;

        public Info Clone()
        {
            Info other = new Info();
            other.RoundOwned = this.RoundOwned;
            other.Caster = this.Caster;
            other.CastingCard = this.CastingCard;
            other.SummonersController = this.SummonersController;
            other.SummonUnit = this.SummonUnit;
            other.Drawer = this.Drawer;
            other.CaughtCard = this.CaughtCard;
            other.HandAdder = this.HandAdder;
            other.AddingCard = this.AddingCard;
            other.Attacker = this.Attacker;
            other.AttackedUnit = this.AttackedUnit;
            other.AbilitySpeller = this.AbilitySpeller;
            other.SpellingAbility = this.SpellingAbility;
            other.Injurer = this.Injurer;
            other.InjuredUnit = this.InjuredUnit;
            other.damage = this.damage;
            other.Killer = this.Killer;
            other.KilledUnit = this.KilledUnit;
            other.Dead = this.Dead;

            return other;
        }
    }
}

namespace GamePlay
{
    

    public class Gameplay : UnitySingleton<Gameplay>
    {

        public void Awake()
        {
            roundProcessController = new RoundProcessController();
            gamePlayInput = new GameplayInput();
            bmbColliderManager = new BMBColliderManager();
            buffManager = new BuffManager();
            autoController = new AI.AutoController();
            singleBattle = new AI.BattleField();

            _phaseNameText = GameObject.Find("phaseNameText").GetComponentInChildren<Text>();
            _phaseNameText.color = Color.red;
            _phaseNameText.text = roundProcessController.State.ToString();
        }
        private void Update()
        {
        }

        public static Info Info = new Info();
        public RoundProcessController roundProcessController; 
        public GameplayInput gamePlayInput;
        public BMBColliderManager bmbColliderManager;
        public BuffManager buffManager;
        public  AI.BattleField singleBattle;
        public AI.AutoController autoController;

        private Text _phaseNameText;
        
        /// <summary>
        /// 提供给场景中阶段切换的按钮
        /// </summary>
        public void switchPhaseHandler()
        {
            roundProcessController.StepIntoNextState();
            
        }

        private void LateUpdate()
        {
            _phaseNameText.text = roundProcessController.State.ToString();
        }
    }
}