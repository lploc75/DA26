using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Database
{
    public class SaveLoadUIManager : MonoBehaviour
    {
        public void OnClickSave()
        {
            DatabaseManager.Instance.SaveAll();
        }

        public void OnClickLoad()
        {
            DatabaseManager.Instance.LoadAll();
        }
    }
}
