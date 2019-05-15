using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUnit
{
    /// <summary>
    /// 对象工厂
    /// </summary>
    public class GameUnitFactory : UnitySingleton<GameUnitFactory> {

        public GameObject EnemyMould;
        public GameObject FriendlyMould;
        public GameObject NeutralityMould;

        /// <summary>
        /// 根据单位id和所有者获得单位实例
        /// </summary>
        /// <param name="unitID"></param>
        /// <param name="owner"></param>
        /// <param name="damage"></param>
        /// <returns></returns>
        public GameObject GetGameUnit(string unitID, OwnerEnum owner, int damage = 0)
        {
            GameObject newUnit;
            //根据所有者选择不同的模板进行单位实例化
            switch (owner)
            {
                case (OwnerEnum.Enemy):
                    newUnit = Instantiate(EnemyMould);
                    break;
                case (OwnerEnum.Player):
                    newUnit = Instantiate(FriendlyMould);
                    break;
                default:
                    newUnit = Instantiate(NeutralityMould);
                    break;
            }

            //将生成的GameUnit传入数据库赋值初始化
            UnitDataBase.Instance().InitGameUnit(newUnit.GetComponent<GameUnit>(), unitID, owner, damage);

            return newUnit;
        }
    }
}