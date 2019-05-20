using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using BattleMap;

namespace GamePlay.Input
{
    //指令类，在UI下面真正执行游戏逻辑的是这些Command类
    //也就是说 玩家输入鼠标 → GUI → Command → GamePlay，BattaleMap等游戏类

    public class Command : GameplayTool
    {
        virtual public void Excute() { }
    }

    public class SelectUnitCommand : Command
    {
        public SelectUnitCommand(GameUnit.GameUnit unit)
        {
            _unit = unit;
        }

        public override void Excute()
        {
            Gameplay.Info.SelectingUnit = _unit;
        }

        private GameUnit.GameUnit _unit;
    }
    public class BattleDispositionCommand : Command
    {
        public BattleDispositionCommand(List<GameUnit.GameUnit> units)
        {
            _units = units;
        }

        public override void Excute()
        {
            //更新本此召唤的怪物（覆盖方式）
            this.SetSummonUnit(_units);

            //更新仇恨列表
            Gameplay.Instance().autoController.UpdateAllHatredList(null, _units);
        }

        private List<GameUnit.GameUnit> _units;
    }
    public class SkillJumpCommand : Command
    {
        public SkillJumpCommand(List<GameUnit.GameUnit> units, Vector2 targetPos)
        {
            _units = units;
            _targetPos = targetPos;
        }

        public override void Excute()
        {
            BattleMap.BattleMap.Instance().MoveUnitToCoordinate(_targetPos, _units[0]);
        }

        private List<GameUnit.GameUnit> _units;
        private Vector2 _targetPos;
    }

    public class UnitMoveAICommand : Command
    {
        /// <summary>
        /// AI移动类的构造函数
        /// </summary>
        /// <param name="unit">单位</param>
        /// <param name="_toTargetPath">最优路径</param>
        /// <param name="_callback">攻击回调</param>
        public UnitMoveAICommand(GameUnit.GameUnit unit, List<Vector2> _toTargetPath, System.Action _callback)
        {
            _unit = unit;
            toTargetPath = _toTargetPath;
            callback = _callback;
        }

        public override void Excute()
        {
            Debug.Log("Moving Command excusing");
            BattleMap.BattleMap.Instance().AIMoveUnitToCoordinate(_unit, toTargetPath, callback);
        }


        private System.Action callback;
        private GameUnit.GameUnit _unit;
        //移动到目标的路径
        private List<Vector2> toTargetPath;
    }

    public class UnitMoveCommand : Command
    {
        public UnitMoveCommand(GameUnit.GameUnit unit, Vector2 unitPositon, Vector2 targetPosion, Vector2 destination)
        {
            _unit = unit;
            _unitPosition = unitPositon;
            _targetPosition = targetPosion;
            _destination = destination;
        }

        public bool Judge()
        {
            Vector2 unit1 = _unitPosition;
            Vector2 unit2 = _targetPosition;
            int MAN_HA_DUN = Mathf.Abs((int)unit1.x - (int)unit2.x) + Mathf.Abs((int)unit1.y - (int)unit2.y);
            if (MAN_HA_DUN <= _unit.mov)
                return true;
            //BattleMap.BattleMap.Instance().MapNavigator
            return false;
        }

        public override void Excute()
        {
            Debug.Log("Moving Command excusing");
            if (BattleMap.BattleMap.Instance().MapNavigator.PathSearch(_unitPosition, _targetPosition))
            {
                //TODO 产生移动变化，检测
                BattleMap.BattleMap.Instance().MoveUnitToCoordinate(_unit, _targetPosition);
            }

        }

        private GameUnit.GameUnit _unit;
        private Vector2 _destination;
        private Vector2 _unitPosition;
        private Vector2 _targetPosition;
    }

    public class UnitAttackCommand : Command
    {
        public UnitAttackCommand(GameUnit.GameUnit Attacker, GameUnit.GameUnit AttackedUnit)
        {
            _Attacker = Attacker; this.SetAttacker(Attacker);
            _AttackedUnit = AttackedUnit; this.SetAttackedUnit(AttackedUnit);
        }

        /// <summary>
        /// 计算攻击距离是否大于曼哈顿
        /// </summary>
        /// <returns></returns>
        public bool Judge()
        {
            Vector2 unit1 = BattleMap.BattleMap.Instance().GetUnitCoordinate(_Attacker);
            Vector2 unit2 = BattleMap.BattleMap.Instance().GetUnitCoordinate(_AttackedUnit);
            int MAN_HA_DUN = Mathf.Abs((int)unit1.x - (int)unit2.x) + Mathf.Abs((int)unit1.y - (int)unit2.y);
            if (MAN_HA_DUN <= _Attacker.rng)
                return true;

            return false;
        }
        /// <summary>
        /// 计算反击距离是否大于曼哈顿
        /// </summary>
        public bool JudgeStrikeBack()
        {
            Vector2 unit1 = BattleMap.BattleMap.Instance().GetUnitCoordinate(_AttackedUnit);
            Vector2 unit2 = BattleMap.BattleMap.Instance().GetUnitCoordinate(_Attacker); 
            int MAN_HA_DUN = Mathf.Abs((int)unit1.x - (int)unit2.x) + Mathf.Abs((int)unit1.y - (int)unit2.y);
            if (MAN_HA_DUN <= _AttackedUnit.rng)
                return true;

            return false;
        }

        public override void Excute()
        {
            MsgDispatcher.SendMsg((int)MessageType.AnnounceAttack);
            //根据伤害优先级对伤害请求排序
            DamageRequestList = DamageRequest.CaculateDamageRequestList(_Attacker, _AttackedUnit);

            for (int i = 0; i < DamageRequestList.Count; i++)
            {
                //优先级相同并且两方互打的伤害请求作为同时处理
                if (i != DamageRequestList.Count - 1 && DamageRequestList[i].priority == DamageRequestList[i + 1].priority
                    && DamageRequestList[i]._attacker == DamageRequestList[i + 1]._attackedUnit
                    && DamageRequestList[i]._attackedUnit == DamageRequestList[i + 1]._attacker)
                {
                    //判断被攻击者的反击距离
                    if (JudgeStrikeBack())
                        DamageRequestList[i].ExcuteSameTime();
                    else
                        DamageRequestList[i].Excute(); //距离不够，无法进行反击

                    i++;
                }
                else if (!_AttackedUnit.IsDead() && !_Attacker.IsDead() && JudgeStrikeBack()) //符合反击要求
                {
                    DamageRequestList[i].Excute();
                }
                else if(!_AttackedUnit.IsDead() && !_Attacker.IsDead() && !JudgeStrikeBack()) //距离不够，无法进行反击
                {
                    DamageRequestList[i].Excute();
                    i++;
                }
            }
        }

        //TODO 攻击制作
        //1. 通过变量_Attacker _AttackedUnit 保存宣言攻击者和被攻击者
        //2. 通过DamageRequestList  —> Damange类中
        //3. 通过Damage类与Command类来执行攻击环节，注意细节修改

        private List<DamageRequest> DamageRequestList;
        private GameUnit.GameUnit _Attacker; //宣言攻击者
        private GameUnit.GameUnit _AttackedUnit; //被攻击者
    }

    public class ReleaseSkillCommand : Command
    {
        public ReleaseSkillCommand(GameUnit.GameUnit skillMaker, int range,Vector2 makerPosition,Vector2 targetPosition)
        {
            _skillMaker = skillMaker;
            _range = range;
            _targetPosition = targetPosition;
            _makerPosition = makerPosition;
        }

        //判断技能释放范围目标是否超出释放范围
        public bool Judge()
        {
            Vector2 skillMaker = _makerPosition;
            Vector2 target = _targetPosition;
            int MAN_HA_DUN = Mathf.Abs((int)skillMaker.x - (int)target.x) + Mathf.Abs((int)skillMaker.y - (int)target.y);
            if (MAN_HA_DUN <= _range)
                return true;
            return false;
        }

        public override void Excute()
        {

        }
        private GameUnit.GameUnit _skillMaker;//技能释放者
        private int _range;//技能范围
        private Vector2 _targetPosition;//释放技能的目标点(中心点)
        private Vector2 _makerPosition;//释放者坐标
    }
}