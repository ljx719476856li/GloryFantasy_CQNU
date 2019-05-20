 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unit = GameUnit.GameUnit;

namespace AI
{
    public class AIBattleController
      : UnitySingleton<AIBattleController>
    {

        public IEnumerator PlayBattleByCoroutine(System.Action callback)
        {
            foreach (Unit unit in BattleMap.BattleMap.Instance().UnitsList)
            {
                if (unit.owner != GameUnit.OwnerEnum.Enemy)
                    yield return null; //TODO 调用人物行为对应的函数
            }

            if (callback != null)
                callback();
        }

        //播放战斗(异步的方式)
        public void PlayBattle(System.Action callback)
        {
            StartCoroutine(PlayBattleByCoroutine(callback));
        }
    }
}



