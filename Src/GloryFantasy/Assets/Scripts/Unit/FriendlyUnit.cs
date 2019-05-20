using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using GamePlay;

namespace GameUnit
{
    public class FriendlyUnit : GameUnit, IPointerDownHandler
    {

        public void OnPointerDown(PointerEventData eventData)
        {
            //Gameplay.Instance().gamePlayInput.OnPointerDownFriendly(this, eventData);
            Gameplay.Instance().gamePlayInput.OnPointerDownFriendly(this, eventData);
        }



    }
}


