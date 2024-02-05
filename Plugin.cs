using BepInEx;
using DoomInProxy.Doom.Unity;
using DoomInProxy.Patches;
using HarmonyLib;
using System;
using System.Collections;
using UIWindowPageFramework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Pages = UIWindowPageFramework.Framework;

namespace DoomInProxy
{
    internal class PluginInfo
    {
        internal const string Name = "DoomIn.VAProxy";
        internal const string GUID = "tairasoul.vaproxy.doom";
        internal const string Version = "1.0.0";
    }

    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance;
        internal bool init = false;
        internal static bool isRunning => cpu is not null && doom is not null;

        private static Coroutine? cpu;
        private static UnityDoom? doom;
        bool visible;
        CursorLockMode oldLock;
        private static RawImage? DoomImage;
        void Awake()
        {
            Instance = this;
            Logger.LogInfo($"Plugin {PluginInfo.GUID} ({PluginInfo.Name}) version {PluginInfo.Version} loaded.");
            new Harmony("doommod").PatchAll();
        }

        void Start() => Init();
        void OnDestroy() => Init();

        void Init()
        {
            if (init) return;
            init = true;
            StartCoroutine(StartWindow());
            Logger.LogInfo($"added doom to your game, have fun.");
        }

        IEnumerator StartWindow()
        {
            while (!Pages.Ready)
            {
                yield return null;
            }
            GameObject Page = Pages.CreateWindow("DOOM");
            GameObject DoomWindow = Page.AddObject("Window");
            RectTransform doomT = DoomWindow.AddComponent<RectTransform>();
            doomT.sizeDelta = new Vector2(700, 700);
            doomT.anchoredPosition3D = new Vector3(79, -93, 0);
            doomT.rotation = new Quaternion(0, 0, 0.7071f, -0.7071f);
            Pages.RegisterWindow(Page, (GameObject window) =>
            {
                GameObject win = window.Find("Window");
                DoomImage = win.GetComponent<RawImage>() ?? win.AddComponent<RawImage>();
                DoomImage.texture = null;
                GameObject StartButton = ComponentUtils.CreateButton("Start", "tairasoul.doom.start");
                Destroy(StartButton.GetComponent<LayoutElement>());
                StartButton.SetParent(window, true);
                RectTransform buttonTransform = StartButton.GetComponent<RectTransform>();
                buttonTransform.anchoredPosition3D = new Vector3(-520, -93, 0);
                buttonTransform.sizeDelta = new Vector2(100, 40);
                buttonTransform.localScale = new Vector3(1, 1, 1);
                RectTransform itemName = StartButton.Find("ItemName").GetComponent<RectTransform>();
                itemName.anchoredPosition = new Vector2(22, 0);
                StartButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    StartDoom();
                });
            });
        }

        void DestroyDoom()
        {
            if (!isRunning) return;

            StopCoroutine(cpu);
            cpu = null;

            doom?.Dispose();
            doom = null;
            GC.Collect();
            Logger.LogInfo("Destroyed Doom instance.");
            Cursor.visible = visible;
            Cursor.lockState = oldLock;
        }
        
        void StartDoom()
        {
           if (doom is null)
            {
                doom = new(new(Config, (700, 700)));
                cpu = StartCoroutine(RenderDoom());
                oldLock = Cursor.lockState;
                visible = Cursor.visible;
           }
        }

        void Update()
        {
            if (isRunning)
            {
                doom?.Input(Mouse.current, Keyboard.current);
            }
        }

        IEnumerator RenderDoom()
        {
            while (doom is not null)
            {
                KeyboardTrigger trigger = DoomImage.GetComponentInParent<KeyboardTrigger>();
                if (!doom.Render())
                {
                    DestroyDoom();
                    trigger.enabled = true;
                    yield break;
                }
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                trigger.enabled = false;
                if (DoomImage != null)
                {
                    DoomImage.texture = doom?.video?.Texture;
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
