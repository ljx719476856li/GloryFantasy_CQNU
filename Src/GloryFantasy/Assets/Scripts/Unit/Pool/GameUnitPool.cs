using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUnit
{

    /// <summary>
    /// 单位对象池
    /// </summary>
    public class GameUnitPool : UnitySingleton<GameUnitPool>
    {
        #region 无用代码
        ////配置文件的路径
        //[SerializeField] private static string poolConfigPathPrefix = "Assets/Resources/ScriptableObjects/GameObjectPool/";
        ////配置文件的名字与文件格式
        //[SerializeField] private const string poolConfigPathMiddle = "gameobjectpool";
        //[SerializeField] private const string poolConfigPathPostfix = ".asset";
        ///// <summary>
        ///// 获取对象池配置文件的完整路径
        ///// </summary>
        //public static string PoolConfigPath
        //{
        //    get
        //    {
        //        return poolConfigPathPrefix + poolConfigPathMiddle + poolConfigPathPostfix;
        //    }
        //}
        #endregion

        //龟龟，这原本写的都什么东西……
        //这不是对象池啊……这是带数量监控的对象工厂啊大哥……

        //对象池队列
        private List<GameObject> m_pool = new List<GameObject>();

        void Awake()
        {
            m_pool = new List<GameObject>();
        }


        /// <summary>
        /// 实现返回GameObject的函数，GetInst(string id)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <param name="Damage"></param>
        /// <returns></returns>
        public GameObject GetInst(string unitId, OwnerEnum owner, int Damage = 0)
        {
            foreach (GameObject _unit in m_pool)
            {
                ///根据所有者不同选择对象，因为三种对象（友军，敌军，中立）身上的脚本可能不一样，这样可以节约一点写代码的时间
                ///注意这里其实比较危险，如果搜不到GameUnit的脚本是会报错中断的，不过我们可以保证他一定有这个脚本，所以在这里可以这么写
                if (_unit.GetComponent<GameUnit>().owner == owner)
                {
                    m_pool.Remove(_unit);
                    _unit.SetActive(true);
                    //从单位数据库将新单位初始化
                    UnitDataBase.Instance().InitGameUnit(_unit, unitId, owner, Damage);
                    return _unit;
                }
            }
           
            //如果没有空余的对应类型的对象，就
            //从单位生成车间取得新单位实例
            return GameUnitFactory.Instance().GetGameUnit(unitId, owner, Damage);
        }

        public void PushUnit(GameObject unit)
        {
            unit.SetActive(false);
            m_pool.Add(unit);
        }
    }

}