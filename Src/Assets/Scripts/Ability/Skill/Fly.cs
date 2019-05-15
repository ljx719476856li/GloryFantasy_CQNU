using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;

namespace Ability
{
    public class Fly : Ability
    {
        Trigger trigger;

        private void Awake()
        {
            //导入Fly异能的参数
            InitialAbility("Fly");
        }

        private void Start()
        {
            //让这个技能的单位获得飞行
            this.GetUnit(this).fly = true;
        }

        //这个技能被删除时要做反向操作
        private void OnDestroy()
        {
            this.GetUnit(this).fly = false;
        }

    }
}