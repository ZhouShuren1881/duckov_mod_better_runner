using System;
using System.Reflection;
using UnityEngine;
using HarmonyLib;

namespace BetterRunner;
public class ModBehaviour : Duckov.Modding.ModBehaviour
{
    # region 生命周期管理
    static bool ModEnabled = false;

    void Awake()
    {
        Debug.Log("BetterRunner Loaded!!!");
    }

    void OnEnable()
    {
        ModEnabled = true;
    }

    /// <summary>
    /// 初始化 harmony 并 patch 所有方法，确保只调用一次
    /// </summary>
    void Start()
    {
        ApplyPatch();
    }

    void OnDisable()
    {
        Debug.Log("Restart game to disable mod: BetterRunner");
        ModEnabled = false;
    }

    void ApplyPatch()
    {
        new Harmony("BetterRunner.ModBehaviour")
        .PatchAll(Assembly.GetExecutingAssembly());
    }
    #endregion

    #region 逻辑层
    /// <summary>
    /// Config: 奔跑切换模式
    /// </summary>
    static bool configToggle2Run = false;

    /// <summary>
    /// 更好的 RunInputBuffer。只会受 Shift 键影响，并且在 adsInput 失效后，会自动切换回原来的输入方式。
    /// </summary>
    private static bool backgroundRunInputBuffer = false;

    /// <summary>
    /// 在开火时，暂停跑步状态。
    /// </summary>
    static bool triggerInput = false;

    /// <summary>
    /// 获取开火键的输入状态。
    /// </summary>
    [HarmonyPatch(typeof(InputManager), "SetTrigger")]
    public class InputManagerSetTriggerPatch
    {

        [HarmonyPrefix]
        public static bool Prefix(InputManager __instance, bool trigger, bool triggerThisFrame, bool releaseThisFrame)
        {
            triggerInput = trigger || triggerThisFrame;
            return true;
        }
    }

    /// <summary>
    /// 根据背景状态，更新 runInputBuffer 的值。
    /// </summary>
    [HarmonyPatch(typeof(InputManager), "Update")]
    public class InputManagerUpdatePatch
    {
        private static readonly FieldInfo fiUseRunInputBuffer = AccessTools.Field(typeof(InputManager), "useRunInputBuffer");
        private static readonly FieldInfo fiRunInputBuffer = AccessTools.Field(typeof(InputManager), "runInputBuffer");
        private static readonly FieldInfo fiRunInput = AccessTools.Field(typeof(InputManager), "runInput");
        // 此处 runInptutThisFrame 存在语法错误
        private static readonly FieldInfo fiRunInptutThisFrame = AccessTools.Field(typeof(InputManager), "runInptutThisFrame");

        [HarmonyPrefix]
        public static bool Prefix(InputManager __instance)
        {
            if (!ModEnabled)
            {
                // 模组未启用时禁用功能。
                return true;
            }

            configToggle2Run = (bool)fiUseRunInputBuffer.GetValue(__instance);

            if (!triggerInput && backgroundRunInputBuffer)
            {
                // 恢复跑步状态。
                fiRunInputBuffer.SetValue(__instance, backgroundRunInputBuffer);
            }

            if ((bool)fiRunInput.GetValue(__instance))
            {
                if ((bool)fiRunInptutThisFrame.GetValue(__instance))
                {
                    // 仅在用户主动切换时，切换跑步状态。
                    backgroundRunInputBuffer = !backgroundRunInputBuffer;
                }
            }
            return true;
        }
    }

    #region 保证栓动和半自动武器正常开火
    /*/// <summary>
    /// 启用 Running 屏蔽。
    /// </summary>
    [HarmonyPatch(typeof(CharacterMainControl), "Trigger")]
    public class CharacterMainControlTriggerPatch
    {

        private static FieldInfo fiDisableTriggerTimer = AccessTools.Field(typeof(CharacterMainControl), "disableTriggerTimer");
        private static FieldInfo fiOnTriggerInputUpdateEvent = AccessTools.Field(typeof(CharacterMainControl), "OnTriggerInputUpdateEvent");

        [HarmonyPrefix]
        public static bool Prefix(CharacterMainControl __instance, bool trigger, bool triggerThisFrame, bool releaseThisFrame)
        {
            if (!configToggle2Run || !backgroundRunInputBuffer)
            {
                return true;
            }

            // if (Running || disableTriggerTimer > 0f)
            // {
            //     trigger = false;
            //     triggerThisFrame = false;
            // }
            // else if (trigger && CharacterMoveability > 0.5f)
            // {
            //     movementControl.ForceSetAimDirectionToAimPoint();
            // }
            // this.OnTriggerInputUpdateEvent?.Invoke(trigger, triggerThisFrame, releaseThisFrame);
            // agentHolder.SetTrigger(trigger, triggerThisFrame, releaseThisFrame);

            if ((float)fiDisableTriggerTimer.GetValue(__instance) > 0f)
            {
                trigger = false;
                triggerThisFrame = false;
            }
            else if (trigger && __instance.CharacterMoveability > 0.5f)
            {
                __instance.movementControl.ForceSetAimDirectionToAimPoint();
            }

            var onTriggerInputUpdateEvent = (Action<bool, bool, bool>)fiOnTriggerInputUpdateEvent.GetValue(__instance);
            onTriggerInputUpdateEvent?.Invoke(trigger, triggerThisFrame, releaseThisFrame);
            __instance.agentHolder.SetTrigger(trigger, triggerThisFrame, releaseThisFrame);

            return false;
        }
    }*/

    # endregion
    # endregion
}
