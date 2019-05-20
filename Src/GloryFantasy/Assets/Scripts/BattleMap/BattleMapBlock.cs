using UnityEngine;
using System;
using System.Collections.Generic;
using Unit = GameUnit.GameUnit;
using UnityEngine.EventSystems;

using GamePlay;


//TODO 通过 自身 this.transform.position与第一块儿地图块儿坐标(298.8)的差，的几倍关系得到具体为(0, 0) -> (7, 7)得物体坐标

namespace BattleMap
{
    public enum AStarState
    {
        free,
        isInOpenList,
        isInCloseList
    }


    public enum EMapBlockType
    {
        normal,   //普通地图块儿
        Burnning, //灼烧块儿
        Retire,   //滞留块儿
        aStarPath   //A星路径
    }


    public class BattleMapBlock : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {

        private bool _showUnitMsg = false;        //判断能否展示单位信息
        private Unit _unit;                        //地图上单位
        private FGUIInterfaces _fguiInterfaces;
        private void Awake()
        {
            setMapBlackPosition();
            
            _fguiInterfaces = FGUIInterfaces.Instance();
        }

        /// <summary>
        /// 向方块上增加GameUnit
        /// </summary>
        /// <param name="unit"></param>
        public void AddUnit(Unit unit, bool isSetUnitsOnMe = true)
        {
            //Debug.Log("MapBlocks--Added unit:" + unit.ToString());
            if(isSetUnitsOnMe)
                units_on_me.Add(unit);
            //在Hierarchy中，还需要把单位添加到Block下
            //修改单位的父级对象
                unit.gameObject.transform.SetParent(this.transform);
        }
        /// <summary>
        /// 向方块上增加GameUnit
        /// </summary>
        /// <param name="units"></param>
        public void AddUnits(Unit[] units)
        {
            //Debug.Log("MapBlocks--Adding Units");
            foreach (Unit gameUnit in units)
            {
                units_on_me.Add(gameUnit);
                //在Hierarchy中，还需要把单位添加到Block下
                //修改单位的父级对象
                gameUnit.gameObject.transform.SetParent(this.transform);
            }
        }
        /// <summary>
        /// 方块上移除GameUnit
        /// </summary>
        /// <param name="unit"></param>
        public void RemoveUnit(Unit unit)
        {
            //Debug.Log("MapBlocks--Removed unit:" + unit.ToString());
            units_on_me.Remove(unit);
        }
        /// <summary>
        /// 获得这个地图块的虚拟坐标(从地图Json读取出来的坐标)
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCoordinate()
        {
            return new Vector3(this.x, this.y, 0f);
        }
        /// <summary>
        /// 获取该地图块自身的世界坐标
        /// </summary>
        /// <returns></returns>
        public Vector3 GetSelfPosition()
        {
            return coordinate;
        }
        /// <summary>
        /// update地图块的世界坐标
        /// </summary>
        private void setMapBlackPosition()
        {
            coordinate = new Vector3((int)transform.position.x, (int)transform.position.y, 0.0f);
        }

        //处理地图块点击事件
        public void OnPointerDown(PointerEventData eventData)
        {
            if(Gameplay.Instance().roundProcessController.IsPlayerRound())
                Gameplay.Instance().gamePlayInput.OnPointerDownBlock(this, eventData);
        }

        //显示战区
        public void OnPointerEnter(PointerEventData eventData)
        {
            Gameplay.Instance().gamePlayInput.OnPointerEnter(this, eventData);
            Show();
            if (_unit != null)
            {
                _fguiInterfaces.setDescribeWindowShow();        // 显示
            }
        }
        //隐藏战区
        public void OnPointerExit(PointerEventData eventData)
        {
            Gameplay.Instance().gamePlayInput.OnPointerExit(this, eventData);
            //show();
            if (_unit != null)
            {
                _fguiInterfaces.setDescribeWindowHide();        // 隐藏
            }
        }

        
        /// <summary>
        /// 获取单位即信息，用于fgui显示
        /// </summary>
        private void Show()
        {
            // 分隔符
            string separator = " / ";
            
            _unit = BattleMap.Instance().GetUnitsOnMapBlock(GetSelfPosition());
            if(_unit == null)
                return;
            // 多个tag, 一起显示，/ 隔开
            string tagInTotal = "";
            if (_unit.tag.Count != 0)
            {
                for (int i = 0; i < _unit.tag.Count; i++)
                {
                    tagInTotal += _unit.tag[i];
                    tagInTotal += separator;
                }
                tagInTotal = tagInTotal.Substring(0, tagInTotal.Length - separator.Length);    // 删掉最后一个 /
            }
                     
            // 多个priority, 一起显示，/ 隔开
            string priorityInTotal = "";
            if (_unit.priority.Count != 0)
            {
                for (int i = 0; i < _unit.priority.Count; i++)
                {
                    priorityInTotal += _unit.priority[i].ToString();
                    priorityInTotal += separator;
                }
                priorityInTotal = priorityInTotal.Substring(0, priorityInTotal.Length - separator.Length);  // 删掉最后一个 /
            }
           
            // 单位基础信息
            string valueInfo = "颜色： " + _unit.Color + "    生命：  " + _unit.hp + "\n攻击： " + _unit.atk 
                               + "    范围： " + _unit.rng + "\n移动： " + _unit.mov + "    优先级： " + priorityInTotal;
            
            // 标签及效果信息，可能过长显示截断
            string effectInfo = tagInTotal + "  " + _unit.Effort;
            
            _fguiInterfaces.setDescribeWindowContentText(_unit.name, valueInfo, effectInfo);
        }

        #region UGUI显示，暂时无用代码
//        private void OnGUI()
//        {
//            if (_showUnitMsg)
//            {
//                string tagInTotal = "";
//                if(_unit == null)
//                    return;
//                if (_unit.tag.Count != 0)
//                {
//                    for (int i = 0; i < _unit.tag.Count; i++)
//                    {
//                        tagInTotal += _unit.tag[i];
//                    }
//                }  
//                     
//                string priorityInToal = "";
//                if (_unit.priority.Count != 0)
//                {
//                    for (int i = 0; i < _unit.priority.Count; i++)
//                    {
//                        priorityInToal += _unit.priority[i].ToString();
//                        priorityInToal += "/";
//                    }
//                }
//
//                priorityInToal = priorityInToal.Substring(0, priorityInToal.Length - 1);
//                //GUILayout.BeginArea(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 300, 350));
//                GUILayout.BeginArea(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 330, 900));
//                GUILayout.BeginHorizontal("Box");
//                GUILayout.BeginVertical(GUILayout.Width(50));
//                GUILayout.Label("name:");
//                GUILayout.Label("color:");
//                GUILayout.Label("attack:");
//                GUILayout.Label("HP:");
//                GUILayout.Label("range:");
//                GUILayout.Label("move:");
//                GUILayout.Label("priority:");
//                GUILayout.Label("tag:");
//                GUILayout.Label("effect:");
//                GUILayout.EndVertical();
//                
//                GUILayout.BeginVertical("Box", GUILayout.Width(900));
//                GUILayout.TextField(_unit.name);
//                GUILayout.TextField(_unit.Color);
//                GUILayout.TextField(_unit.atk.ToString());
//                GUILayout.TextField(_unit.hp.ToString());
//                GUILayout.TextField(_unit.rng.ToString());
//                GUILayout.TextField(_unit.mov.ToString());
//                GUILayout.TextField(priorityInToal);
//                GUILayout.TextField(tagInTotal);
//                GUILayout.TextField(_unit.Effort);
//                
//                GUILayout.EndVertical();
//                GUILayout.EndHorizontal();
//                GUILayout.EndArea();
//            }
//        }
        #endregion


        private Vector3 coordinate;//该地图块的世界坐标
        public int area { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string type { get; set; }
        public List<Unit> units_on_me = new List<Unit>();
        public EMapBlockType blockType { get; set; }
        public Vector2 position//该地图块的虚拟坐标(从地图Json读取出来的坐标)
        {
            get { return new Vector2(x, y); } 
        }

        /// <summary>
        /// collider组件
        /// </summary>
        public BMBCollider bmbCollider = new BMBCollider();


    }
}

