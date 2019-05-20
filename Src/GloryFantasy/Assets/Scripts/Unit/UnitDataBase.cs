using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using Unit = GameUnit.GameUnit;
using Random = UnityEngine.Random;

using Ability;

namespace GameUnit
{
    public class UnitDataBase : UnitySingleton<UnitDataBase>
    {
        private void Awake()
        {
            InitDictionary();
        }

        #region 变量
       [SerializeField]private string DataBasePath;
        private Dictionary<string, JsonData> _unitsData;
        private List<string> _unitsDataIDs;
        #endregion

        ///<summary>
        /// 初始化存储模板数据字典以及模板数据id列表
        ///</summary>
        private void InitDictionary()
        {
            this._unitsData = new Dictionary<string, JsonData>();
            this._unitsDataIDs = new List<string>();

            // 从制定路劲加载json文件并映射成字典
            JsonData unitsTemplate = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + DataBasePath));
            // 获取总模板数量
            int dataAmount = unitsTemplate.Count;
            // 依次添加数据到相应数据集中
            for (int i = 0; i < dataAmount; i++)
            {
                string id = unitsTemplate[i]["ID"].ToString();
                _unitsData.Add(id, unitsTemplate[i]);
                _unitsDataIDs.Add(id);
            }

            
        }

        /// <summary>
        /// 获取所有模板数据的ID列表
        /// </summary>
        /// <returns>string形式List</returns>
        public List<string> GetUnitsDataIDs()
        {
            return _unitsDataIDs;
        }

        /// <summary>
        /// 对GameUnit进行数值初始化
        /// </summary>
        /// <param name="unit">被初始化的持有GameUnit的GameObject</param>
        /// <param name="id">要初始化的Unit的数据的ID</param>
        /// <param name="damage">这个角色是否出场就受伤</param>
        public void InitGameUnit(GameObject unit, string id, OwnerEnum owner, int damage = 0)
        {
            if (unit.GetComponent<Unit>() != null)
            {
                InitGameUnit(unit.GetComponent<Unit>(), id, owner, damage);
            }
            else
            {
                Debug.Log("In UnitDataBase: " + unit.name + " doesn't have GameUnit.Can;t be Initial.");
            }
        }

        /// <summary>
        /// 对GameUnit进行数值初始化
        /// </summary>
        /// <param name="unit">被初始化的GameUnit引用</param>
        /// <param name="unitID">要初始化的Unit的数据的ID</param>
        /// <param name="damage">这个角色是否出场就受伤</param>
        public void InitGameUnit(Unit unit, string unitID, OwnerEnum owner, int damage = 0)
        {
            if (!_unitsData.ContainsKey(unitID))
            {
                Debug.Log("UnitDataBase is not contant " + unitID);
                return;
            }

            JsonData data = _unitsData[unitID];

            //先删除异能再初始化数值
            //初始化数值,记得和GameUnit的成员保持一致
            unit.owner = owner;
            unit.atk = int.Parse(data["Atk"].ToString());
            unit.id = data["CardID"].ToString();
            unit.Color = data["Color"][0].ToString();
            unit.Effort = data["Effort"].ToString();
            unit.CD = int.Parse(data["HasCD"].ToString());
            unit.MaxHP = int.Parse(data["Hp"].ToString()); unit.hp = unit.MaxHP - damage;
            unit.id = data["ID"].ToString();
            unit.mov = int.Parse(data["Mov"].ToString());
            unit.name = data["Name"].ToString();
            unit.priority = new List<int>();
            unit.priority.Add(int.Parse(data["Prt"].ToString()));
            unit.rng = int.Parse(data["Rng"].ToString());
            unit.tag = new List<string>();
            unit.events = new List<string>();

            int tagCount = data["Tag"].Count;
            int eventCount = data["Event"].Count;

            for (int i = 0; i < Mathf.Max(tagCount, eventCount); i++)
            {
                if(i < tagCount && i < eventCount)
                {
                    unit.tag.Add(data["Tag"][i].ToString());
                    unit.events.Add(data["Event"][i].ToString());
                }
                else if(i < tagCount)
                {
                    unit.tag.Add(data["Tag"][i].ToString());
                }
                else if(i < eventCount)
                {
                    unit.events.Add(data["Event"][i].ToString());
                    Debug.Log("event" + unit.events[i]);
                }
            }

            unit.priSPD = 0;
            unit.priDS = 0;
            unit.fly = false;
            //unit.damaged = "" //不知道这什么玩意儿
            unit.disarm = true;
            unit.restrain = true;
            unit.armorRestore = 0;
            unit.armor = 0;

            //最后初始化新异能
            AddGameUnitAbility(unit, data);
        }

        /// <summary>
        /// 将给定GameUnit的异能脚本全部删除
        /// </summary>
        /// <param name="unit"></param>
        public void DeleteGameUnitAbility(Unit unit)
        {
            //删除所有异能脚本
            foreach (Ability.Ability ability in unit.gameObject.GetComponents<Ability.Ability>())
            {
                Destroy(ability);
            }
        }

        /// <summary>
        /// 根据给定UnitID，添加GameUnit的异能脚本
        /// </summary>
        public void AddGameUnitAbility(Unit unit, JsonData unitJsonData)
        {
            unit.abilities = new List<string>();
            for (int i = 0; i < unitJsonData["Ability"].Count; i++)
            {
               if (unitJsonData["Ability"][i].ToString() == "") continue;
                unit.abilities.Add(unitJsonData["Ability"][i].ToString());
               Component ability = unit.gameObject.AddComponent(System.Type.GetType("Ability." + unitJsonData["Ability"][i].ToString()));
               if (ability != null)
                {
                    GameUtility.UtilityHelper.Log("添加异能 " + unitJsonData["Ability"][i].ToString() + " 成功", GameUtility.LogColor.RED);
                }
               else
                {
                    GameUtility.UtilityHelper.Log("添加异能 " + unitJsonData["Ability"][i].ToString() + " 失败", GameUtility.LogColor.RED);
                }
            }
        }
    }
}