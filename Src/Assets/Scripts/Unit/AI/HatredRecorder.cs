using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unit = GameUnit.GameUnit;

namespace AI
{
    public class HatredItem
    {
        /// <summary>
        /// 被仇恨的单位
        /// </summary>
        public Unit battleUnit;
        /// <summary>
        /// 仇恨值
        /// </summary>
        public int hatred;
    }


    public class HatredRecorder
        : IComparer<HatredItem>
    {
        /// <summary>
        /// 当前仇恨列表的拥有者
        /// </summary>
        private Unit host;
        /// <summary>
        /// 仇恨列表
        /// </summary>
        private List<HatredItem> hatredList = new List<HatredItem>(5);


        /// <summary>
        /// 重置仇恨列表
        /// </summary>
        /// <param name="hostUnit"></param>
        public void Reset(Unit hostUnit)
        {
            Clean();
            if (hostUnit == null)
            {
                Debug.Log("重置仇恨列表失败");
                return;
            }

            host = hostUnit;
        }

        /// <summary>
        /// 多个单位同时部署时
        /// </summary>
        /// <param name="enemyTeam">敌人队伍列表</param>
        public void AddHatredUnits(List<Unit> enemyTeam)
        {
            if(enemyTeam == null)
            {
                Debug.Log("重置仇恨列表失败");
                return;
            }

            for (int i = 0; i < enemyTeam.Count; i++) //遍历仇恨列表
            {
                hatredList.Add(new HatredItem()); //当敌人数量大于优先设置的仇恨列表总容量时，动态增加列表，以防数组越界
                hatredList[HatredCount - 1].battleUnit = enemyTeam[i];
                SetHartredByRace(enemyTeam[i].tag[0]);

                //hatredList[HatredCount - 1].hatred = 0; //gui 000.....
            }

        }

        /// <summary>
        /// 单个单位部署时
        /// </summary>
        /// <param name="enemyUnit"></param>
        public void AddHatred(Unit enemyUnit)
        {
            if (enemyUnit == null)
                return;

            hatredList.Add(new HatredItem());


            hatredList[HatredCount - 1].battleUnit = enemyUnit;
            SetHartredByRace(enemyUnit.tag[0]);

        }

        private void SetHartredByRace(string race)
        {
            switch (race)
            {
                case "英雄":
                    hatredList[HatredCount - 1].hatred = 0;
                    break;
                case "机甲":
                    hatredList[HatredCount - 1].hatred = 1;
                    break;
                case "骑兵":
                    hatredList[HatredCount - 1].hatred  = 2;
                    break;
                case "元素":
                    hatredList[HatredCount - 1].hatred = 3;
                    break;
                case "野兽":
                    hatredList[HatredCount - 1].hatred = 4;
                    break;
                case "叶族":
                    hatredList[HatredCount - 1].hatred = 8;
                    break;
                case "弓兵":
                    hatredList[HatredCount - 1].hatred = 12;
                    break;
                default:
                    hatredList[HatredCount - 1].hatred = 2;
                    break;
            }
        }

        public void RemoveDeadHartedItem()
        {
            foreach (HatredItem item in hatredList)
            {
                if (item.battleUnit.hp <= 0)
                    hatredList.Remove(item);
            }
        }

        /// <summary>
        /// 清空仇恨列表
        /// </summary>
        public void Clean()
        {
            host = null;
            for(int i= 0; i <hatredList.Count; i++)
            {
                hatredList[i].battleUnit = null;
                hatredList[i].hatred = 0;
            }

        }

        /// <summary>
        /// 排序仇恨列表
        /// </summary>
        private void SortHatred()
        {
            //简单的排序
            hatredList.Sort(delegate (HatredItem h1, HatredItem h2) { return h1.hatred.CompareTo(h2.hatred); });
            Debug.Log("sort ended");
        }

        /// <summary>
        ///记录仇恨列表
        ///对仇恨值做加法
        /// </summary>
        /// <param name="id">单位ID</param>
        ///id 因为Json获取下来的值类型为string
        /// <param name="hatredIncrease">仇恨增加值</param>
        public void RecordedHatred(string id, int hatredIncrease)
        {
            for(int i = 0; i < hatredList.Count; i++)
            {
                if(hatredList[i].battleUnit.id == id)
                {
                    //原始仇恨值
                    int originHatred = hatredList[i].hatred;
                    originHatred += hatredIncrease;
                    //仇恨值不能 ＜ 0
                    if (originHatred < 0)
                        originHatred = 0;
                    //记录新的仇恨值
                    hatredList[i].hatred = originHatred;
                    return; 
                }
            }
        }

        /// <summary>
        /// 记录仇恨的数量
        /// </summary>
        public int HatredCount
        {
            get
            {
                return hatredList.Count;
            }
        }

        /// <summary>
        /// 根据索引获得仇恨列表中的战斗单位
        /// </summary>
        /// <param name="index">仇恨列表的索引</param>
        /// <param name="isSort">是否需要排序</param>
        /// <returns></returns>
        public Unit GetHatredByIndex(int index, bool isSort)
        {
            if (hatredList.Count == 0)
                return null;

            if (isSort)
            {
                SortHatred();
                hatredList.Reverse();
            }

            //防止越界
            return hatredList[index > hatredList.Count - 1 ? hatredList.Count : index].battleUnit;
        }

        public int Compare(HatredItem x, HatredItem y)
        {
            return -1;
        }
    }

}



