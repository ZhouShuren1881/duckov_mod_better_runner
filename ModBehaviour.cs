using UnityEngine;

namespace ModDemo
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        void Awake()
        {
            Debug.Log("ModDemo Loaded!!!");

            // 注册所有子Mod
            gameObject.AddComponent<DisplayItemValue.ModBehaviour>();
            gameObject.AddComponent<ShowConsole.ModBehaviour>();
        }
    }
}
