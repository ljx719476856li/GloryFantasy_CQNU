using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LitJson;
using System.IO;

namespace Ability
{
    public class AbilityDatabase : MonoBehaviour
    {
        //技能表存储对象
        private Dictionary<string, AbilityFormat> _abilityData;
        //Json文件的路径
        public string JsonFilePath = "/Scripts/Ability/AbilityList.json";

        protected static AbilityDatabase _instance = null;

        public static AbilityDatabase GetInstance()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(AbilityDatabase)) as AbilityDatabase;
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(AbilityDatabase).ToString();
                    _instance = obj.AddComponent<AbilityDatabase>();
                }
            }
            return _instance;
        }

        private void Awake()
        {
            InitAbilityDatabase();
        }

        /// <summary>
        /// 初始化异能数据库
        /// </summary>
        private void InitAbilityDatabase()
        {
            _abilityData = new Dictionary<string, AbilityFormat>();

            JsonData abilitiesJsonData =
                JsonMapper.ToObject(File.ReadAllText(Application.dataPath + JsonFilePath));

            for (int i = 0; i < abilitiesJsonData.Count; i++)
            {
                //如果数据库里还没有这个异能
                if (!_abilityData.ContainsKey(abilitiesJsonData[i]["AbilityID"].ToString()))
                {
                    JsonData tmp = abilitiesJsonData[i];
                    AbilityFormat newAbility = new AbilityFormat(tmp["AbilityID"].ToString());

                    //Debug.Log(tmp["AbilityID"]);

                    if ((int)tmp["TargetCount"] > 0)
                        newAbility.AbilityTargetList.Add(new AbilityTarget(tmp["TargetType"].ToString(), ((int)tmp["IsSpeller"]>0), ((int)tmp["IsTarget"]>0)));
                    FullAbilityVariable(newAbility.AbilityVariable, tmp);
                    newAbility.Group = (int)tmp["Group"];
                    newAbility.AbilityName = tmp["AbilityName"].ToString();
                    newAbility.Description = tmp["Description"].ToString();
                    newAbility.TriggerID = tmp["TriggerID"].ToString();

                    //数据库中加入这个异能
                    _abilityData.Add(tmp["AbilityID"].ToString(), newAbility);
                }
                else
                {
                    //如果已经有这个异能，直接加入对象即可
                    JsonData tmp = abilitiesJsonData[i];

                    _abilityData[tmp["AbilityID"].ToString()].AbilityTargetList.Add(new AbilityTarget(tmp["TargetType"].ToString(), ((int)tmp["IsSpeller"] > 0), ((int)tmp["IsTarget"] > 0)));
                }
            }
        }

        /// <summary>
        /// 传入AbiltiyVariable和对应json，进行数据填充
        /// </summary>
        /// <param name="abilityVariable">被填充的AbilityVariabile</param>
        /// <param name="jsonData">拥有填充数据的jsonData</param>
        private void FullAbilityVariable(AbilityVariable abilityVariable, JsonData jsonData)
        {
            if (jsonData["Range"].ToString() != "")
                abilityVariable.Range = int.Parse(jsonData["Range"].ToString());
            if (jsonData["Damage"].ToString() != "")
                abilityVariable.Damage = int.Parse(jsonData["Damage"].ToString());
            if (jsonData["Amount"].ToString() != "")
                abilityVariable.Amount = int.Parse(jsonData["Amount"].ToString());
            if (jsonData["Draws"].ToString() != "")
                abilityVariable.Draws =  int.Parse(jsonData["Draws"].ToString());
            if (jsonData["Turns"].ToString() != "")
                abilityVariable.Turns = int.Parse(jsonData["Turns"].ToString());
            if (jsonData["Area"].ToString() != "")
            {
                string[] area = jsonData["Area"].ToString().Split('*');
                abilityVariable.Area = new Vector2(int.Parse(area[0]), int.Parse(area[1]));
            }
            if (jsonData["Curing"].ToString() != "")
                abilityVariable.Curing = int.Parse(jsonData["Curing"].ToString());
        }

        /// <summary>
        /// 传入AbilityID获得对应的AbilityFormat
        /// </summary>
        /// <param name="_abilityID"></param>
        /// <returns></returns>
        public AbilityFormat GetAbilityFormat(string _abilityID)
        {
            if (_abilityData.ContainsKey(_abilityID))
                return _abilityData[_abilityID];
            else
                Debug.Log("Ability Database doesn't have _abilityID information.");
            return null;
        }
    }
}
