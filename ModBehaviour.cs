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
    /// <summary>
    /// 启用 Running 屏蔽。
    /// </summary>
    [HarmonyPatch(typeof(CharacterMainControl), "Trigger")]
    public class CharacterMainControlTriggerPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(CharacterMainControl __instance, bool trigger, ref bool triggerThisFrame, bool releaseThisFrame)
        {
            if (!ModEnabled) {
                resetCache();
                return true;
            }

            if (!configToggle2Run || !backgroundRunInputBuffer)
            {
                resetCache();
                return true;
            }

            // 在奔跑状态下延迟播放序列，进入非奔跑状态后重新播放信号。序列要求：<TTF>,<TFF>,<TFF>...
            if (!cacheTriggerThisFrame)
            {
                // 等待序列头
                if (!(__instance.Running && checkSequenceHead(trigger, triggerThisFrame, releaseThisFrame)))
                {
                    resetCache(); // 只有 cacheTriggerThisFrame 一个标志时，等效于无效操作
                    return true;
                }
                cacheTriggerThisFrame = true;
                return true;
            }
            else
            {
                // 处理序列体
                // 严格校验准入规则
                if (!checkValidSequenceBody(trigger, triggerThisFrame, releaseThisFrame))
                {
                    resetCache();
                    return true;
                }

                if (__instance.Running)
                {
                    return true;
                }

                // 进入非奔跑模式，重新播放 triggerThisFrame
                triggerThisFrame = true;
                resetCache();
                return true;
            }
        }

        private static bool cacheTriggerThisFrame = false;

        /// <summary>
        /// 重置为正常状态。
        /// </summary>
        private static void resetCache()
        {
            cacheTriggerThisFrame = false;
        }

        private static bool checkSequenceHead(bool trigger, bool triggerThisFrame, bool releaseThisFrame)
        {
            return trigger && triggerThisFrame && !releaseThisFrame;
        }

        private static bool checkValidSequenceBody(bool trigger, bool triggerThisFrame, bool releaseThisFrame)
        {
            return trigger && !triggerThisFrame && !releaseThisFrame;
        }
    }

    # endregion
    # endregion
}
