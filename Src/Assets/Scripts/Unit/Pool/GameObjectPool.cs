//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

///// <summary>
///// 资源池
///// </summary>
//[Serializable]
//public class GameObjectPool
//{
//    [SerializeField]
//    private string name;
//    [SerializeField]
//    private string id;
//    [SerializeField]
//    private GameObject prefabs;
//    [SerializeField]
//    private int capacity;


//    public string Name
//    {
//        get
//        {
//            return name;
//        }
//    }
//    public string ID
//    {
//        get
//        {
//            return id;
//        }
//    }

//    //TODO 池子的实现
//    //1. 通过goListStatusTrue / goListStatusFalse 两种List 对池子资源进行保存
//    //2. 测试..

//    [NonSerialized]
//    public static List<GameObject> goListStatusTrue = new List<GameObject>();
//    [NonSerialized]
//    public static List<GameObject> goListStatusFalse = new List<GameObject>();

//    private bool flag = true;

//    private void Init()
//    {
//        flag = false;
//    }

//    public GameObject GetInst()
//    {
//        //先从goListStatusFalse中选取资源返回
//        foreach (GameObject go in goListStatusFalse)
//        {
//            go.SetActive(true);

//            goListStatusTrue.Add(go);
//            goListStatusFalse.RemoveAt(0);
//            return go;
//        }

//        int count = goListStatusTrue.Count + goListStatusFalse.Count;   //总量

//        //超出数量后，从池子当中释放一个资源对象
//        if (count >= capacity)
//        {
//            //优先考虑goListStatusFalse中的资源
//            if (goListStatusFalse.Count != 0)
//            {
//                GameObject.Destroy(goListStatusFalse[0]);
//                goListStatusFalse.RemoveAt(0);
//            }
//            else
//            {
//                //优先考虑goListStatusTrue中，使用 “最久” 的资源
//                GameObject.Destroy(goListStatusTrue[0]);
//                goListStatusTrue.RemoveAt(0);
//            }
//        }
//        else if (count == 0 && flag)
//            Init();

//        //实例化出go对象
//        GameObject temp = GameObject.Instantiate(prefabs) as GameObject;
//        goListStatusTrue.Add(temp);
//        //Debug.Log(goListStatusTrue.Count);


//        //TODO 血量显示 test版本, 此后用slider显示
//        var TextHp = temp.transform.GetComponentInChildren<Text>();
//        var gameUnit = temp.GetComponent<GameUnit.GameUnit>();
//        float hp = gameUnit.UnitAttribute.HP/* - Random.Range(2, 6)*/;
//        float maxHp = gameUnit.UnitAttribute.MaxHp;
//        float hpDivMaxHp = hp / maxHp * 100;

//        TextHp.text = string.Format("Hp: {0}%", hpDivMaxHp);

//        return temp;
//        //TODO 让temp产生到指定地图块儿上

//    }




//}
