using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameGUI;
using System.IO;
using UnityEngine.EventSystems;
using UnityEditor;

namespace MainMap
{
    /// <summary>管理所有地格上元素，在这里初始化并生成地格上元素（怪物，事件，道具等）
    /// 
    /// </summary>
    public class MapElementManager : UnitySingleton<MapElementManager>
    {
    /// <summary>这里挂一个测试用的文本，后期我会加获取文件夹里文件位置的方法，
    /// 
    /// </summary>
        public TextAsset MapElementAsset;
        private void Awake()
        {
            
        }
    /// <summary>根据传入的参数生成地图上层元素
    /// 
    /// </summary>
    /// <param name="elementtype"></param>
    /// <param name="mapunit"></param>
        public void InstalizeElement(string elementtype, GameObject mapunit)
        {
//#if UNITY_EDITOR
            switch (elementtype)
            {
                case "monster":
                    Debug.Log("生成怪物");
                    GameObject monster = (GameObject)Instantiate(Resources.Load("MMtestPrefab/monster", typeof(GameObject)));
                    ElementSet(monster,mapunit);
                    monster.AddComponent<Monster>();
                    break;
                case "randomevent":
                    Debug.Log("生成随机事件");
                    GameObject randomevent = (GameObject)Instantiate(Resources.Load("MMtestPrefab/randomevent", typeof(GameObject)));
                    ElementSet(randomevent,mapunit);
                    randomevent.AddComponent<RandomEvent>();
                    break;
                default:
                    Debug.Log("地格上层元素读取错误");
                    break;
            }
//#endif
        }
        /// <summary>设置传入的地图上层元素的父节点
        /// 
        /// </summary>
        /// <param name="mapelement"></param>
        /// <param name="mapunit"></param>
        public void ElementSet(GameObject mapelement,GameObject mapunit)
        {
            mapelement.transform.parent = mapunit.transform;
            mapelement.transform.position = mapunit.transform.position;

        }
    }
    /// <summary>地图元素抽象类
    /// 
    /// </summary>
    public abstract class MapElement:MonoBehaviour
    {
        /// <summary>有啥用？？不知道
        /// 
        /// </summary>
        public HexVector site = new HexVector();
        /// <summary>这是上层所在元素的mapunit,暂时还用不上，就注掉了
        /// 
        /// </summary>
      //  private MapUnit ParentUnit;
        protected virtual void Awake()
        {
         
            
        }
        protected void instalize()
        {
        //  ParentUnit = gameObject.GetComponentInParent<MapUnit>();
            Button btn = gameObject.GetComponentInParent<Button>();
            btn.onClick.AddListener(ElementOnClick);
        }
        public abstract void ElementOnClick();
    }
    /// <summary>怪物
    /// 
    /// </summary>
    public class Monster:MapElement
    {
        protected override void Awake()
        {
            Debug.Log("怪物初始化");
            instalize();
        }
        public override void ElementOnClick()
        {
            //if(mapManager.charactor.aroundlist.ContainsValue(this))
            Debug.Log("怪物被点击");
        }
    }
    /// <summary>随机事件
    /// 
    /// </summary>
    public class RandomEvent : MapElement
    {
        protected override void Awake()
        {
            Debug.Log("随机事件初始化");
            instalize();
        }
        public override void ElementOnClick()
        {
            Debug.Log("随机事件被点击");
        }
    }

}

