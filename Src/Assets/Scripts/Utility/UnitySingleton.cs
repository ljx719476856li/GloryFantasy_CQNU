using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine
{
    /// <summary>
    /// 单例模板类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UnitySingleton<T> : MonoBehaviour
    where T : Component
        //约束T需要继承Component
    {
        //proteced保证了继承的子类可以访问
        //不过也导致同样继承了UnitySingleton的别的子类也可以访问，不过问题不大
        protected static T _instance;

        /// <summary>
        /// 单例的唯一引用
        /// </summary>
        /// <returns></returns>
        public static T Instance()
        {
            if (_instance == null)
            {
                //找到我们在场景里已经实例化的实例
                _instance = FindObjectOfType(typeof(T)) as T;
                //如果我们场景里没有实例化，就创建一个空物体增加脚本返回这个实例
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).ToString();
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

}