using Assets.Scripts.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public static class Common
    {
        public static T GetObjectScritp<T>(string str)
        {
            var tempObject = GameObject.FindGameObjectWithTag(str);
            return tempObject.GetComponent<T>();
        } 
    }
}
