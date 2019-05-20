//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class hpUpdate : MonoBehaviour
//{

//    GameObject unit;
//    GameUnit.GameUnit gameUnit;

//    // Use this for initialization
//    void Start()
//    {
//        unit = transform.parent.gameObject;
//        gameUnit = unit.GetComponent<GameUnit.GameUnit>();
//    }

//    // Update is called once per frame
//    public void UpdateHp()
//    {
//        float hpDivMaxHp = (float)gameUnit.hp / gameUnit.MaxHP * 100;
//        Debug.Log(hpDivMaxHp);
//        if (hpDivMaxHp <= 0.0f)
//        {
//            Destroy(unit);
//        }
//        else
//        {
//            var textHp = transform.GetComponent<Text>();
//            textHp.text = string.Format("Hp: {0}%", Mathf.Ceil(hpDivMaxHp));
//        }


//    }
//}
