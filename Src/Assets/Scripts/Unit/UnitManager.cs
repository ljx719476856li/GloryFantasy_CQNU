using System;
using System.Collections;
using System.Collections.Generic;
using BattleMap;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using GamePlay;
using IMessage;

namespace GameUnit
{
    public enum HeroActionState
    {
        Normal,                  //正常
        WaitForPlayerChoose,     //等待玩家操作
        BattleEnd,               //战斗结束
        Error,                   //错误
        Warn,                    //警告(测试用)
    }

    /// <summary>
    /// 提供一些与Unit相关的方法
    /// </summary>
    public class UnitManager
    {
        //角色初始化到地图上
        public static void InstantiationUnit(string cardID, OwnerEnum owner, BattleMapBlock battleMapBlock)
        {
            //根据卡牌id生成单位
            GameObject temp = GameUnitPool.Instance().GetInst(cardID, owner);
            //获取GameUnit对象
            GameUnit gameUnit = temp.GetComponent<GameUnit>();

            //添加当前实例单位到UnitList中
            BattleMap.BattleMap.Instance().UnitsList.Add(gameUnit);
            //添加当前实例单位的所在地图块儿
            gameUnit.mapBlockBelow = battleMapBlock;

            //添加gameUnit到units_on_me上，且修改单位的父级对象
            battleMapBlock.AddUnit(gameUnit);
            //修改单位的本地坐标系坐标
            temp.transform.localPosition = Vector3.zero;
            //修改单位卡图的射线拦截设置
            temp.GetComponent<Image>().raycastTarget = true;

            //TODO 暂时用Text标识血量，以后改为slider
            var hpTest = temp.transform.GetChild(0);
            hpTest.gameObject.SetActive(true);
            float hp = (temp.GetComponent<GameUnit>().hp = temp.GetComponent<GameUnit>().hp);
            float hpDivMaxHp = hp / temp.GetComponent<GameUnit>().MaxHP * 100;
            //格式化血量的显示
            hpTest.GetComponent<Text>().text = string.Format("HP: {0}%", hpDivMaxHp);

            //单位部署相当于单位驻足地图块儿
            gameUnit.nextPos = gameUnit.CurPos;

            //挂载ShowRange脚本
            temp.AddComponent<GameGUI.ShowRange>();

            //部署成功
            Gameplay.Instance().bmbColliderManager.Fresh(gameUnit);
        }

        /// <summary>
        /// 获取地图块儿上的list 单位
        /// </summary>
        /// <param name="position">单位位置</param>
        /// <returns></returns>
        public static List<GameUnit> GetUnitFromBattleMapBlock(Vector2 position)
        {
            BattleMap.BattleMapBlock mapBlock =  BattleMap.BattleMap.Instance().GetSpecificMapBlock(position);
            return mapBlock.units_on_me;
        }
    }
}