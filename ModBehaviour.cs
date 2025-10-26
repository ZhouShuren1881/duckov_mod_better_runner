using System.IO;
using System.Reflection;
using UnityEngine;
using HarmonyLib;

namespace BetterRunner;
public class ModBehaviour : Duckov.Modding.ModBehaviour
{
    # region 生命周期管理
    // private Harmony? harmonyInstance;

    void Awake()
    {
        Debug.Log("BetterRunner Loaded!!!");
        Load0Harmony();
    }

    void OnEnable()
    {
        // Do nothing
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
    }

    void ApplyPatch()
    {
        new Harmony("BetterRunner.ModBehaviour")
        .PatchAll(Assembly.GetExecutingAssembly());
    }

    const string HARMONY_DLL_NAME = "BetterRunner.0Harmony.dll";

    void Load0Harmony()
    {
        // Load0HarmonyLocal();
    }

    void Load0HarmonyFromResource()
    {
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        using Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(HARMONY_DLL_NAME);
        using MemoryStream memoryStream = new MemoryStream();
        manifestResourceStream?.CopyTo(memoryStream);
        Assembly _ = Assembly.Load(memoryStream.ToArray());
    }

    void Load0HarmonyLocal()
    {
        string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
        string dllPath = Path.Combine(basePath, HARMONY_DLL_NAME);
        Debug.Log("Loading BetterRunner 0Harmony from local: " + dllPath);
        Assembly _ = Assembly.LoadFrom(dllPath);
    }
    # endregion

    #region 逻辑层
    /// <summary>
    /// Harmony Patch：用于修改子弹类型切换时的行为
    /// </summary>
    [HarmonyPatch(typeof(InputManager), "Update")]
    public class InputManagerPatch
    {
        /// <summary>
        /// 更好的 RunInputBuffer。只会受 Shift 键影响，并且在 adsInput 失效后，会自动切换回原来的输入方式。
        /// </summary>
        private static bool betterRunInputBuffer = false;

        /// <summary>
        /// 是否需要在 postfix 中恢复原来的输入方式。
        /// </summary>
        private static bool flagRestore = false;

        private static readonly FieldInfo fiRunInputBuffer = AccessTools.Field(typeof(InputManager), "runInputBuffer");
        private static readonly FieldInfo fiRunInput = AccessTools.Field(typeof(InputManager), "runInput");
        // 此处 runInptutThisFrame 存在语法错误
        private static readonly FieldInfo fiRunInptutThisFrame = AccessTools.Field(typeof(InputManager), "runInptutThisFrame");
        private static readonly FieldInfo fiMoveAxisInput = AccessTools.Field(typeof(InputManager), "moveAxisInput");
        private static readonly FieldInfo fiAdsInput = AccessTools.Field(typeof(InputManager), "adsInput");

        [HarmonyPrefix]
        public static bool Prefix(InputManager __instance)
        {
            if (flagRestore)
            {
                fiRunInputBuffer.SetValue(__instance, betterRunInputBuffer);
            }
            betterRunInputBuffer = (bool)fiRunInputBuffer.GetValue(__instance);
            flagRestore = false;

            if ((bool)fiRunInput.GetValue(__instance))
            {
                if ((bool)fiRunInptutThisFrame.GetValue(__instance))
                {
                    // 在用户主动切换后不恢复原样
                    flagRestore = false;
                }
            }
            else
            {
                Vector2 moveAxisInput = (Vector2)fiMoveAxisInput.GetValue(__instance);
                if (moveAxisInput.magnitude < 0.1f)
                {
                    // 无视手柄操作
                    flagRestore = true;
                }
                else if ((bool)fiAdsInput.GetValue(__instance))
                {
                    // 在用户主动切换后恢复原样
                    flagRestore = true;
                }
                flagRestore = true;
            }
            return true;
        }
    }
    # endregion
}
