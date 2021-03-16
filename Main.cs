using MelonLoader;
using System;
using System.Reflection;
using UnhollowerRuntimeLib;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using VRC.Core;
using VRC.Animation;
using VRC.SDKBase;
using Harmony;

namespace ActionMenuUtils
{
    public static class ModInfo
    {
        public const string Name = "ActionMenuUtils";
        public const string Author = "gompo";
        public const string Version = "1.3.1";
        public const string DownloadLink = "https://github.com/gompocp/ActionMenuUtils/releases";
    }
    public class Main : MelonMod
    {
        private static AssetBundle iconsAssetBundle = null;
        private static Texture2D respawnIcon;
        private static Texture2D helpIcon;
        private static Texture2D goHomeIcon;
        private static Texture2D resetAvatarIcon;
        private static Texture2D rejoinInstanceIcon;
        private static ActionMenuAPI actionMenuApi;
        private static MelonMod Instance;
        public static HarmonyInstance HarmonyInstance => Instance.Harmony;


        public override void OnApplicationStart()
        {
            Instance = this;
            try
            {
                //Adapted from knah's JoinNotifier mod found here: https://github.com/knah/VRCMods/blob/master/JoinNotifier/JoinNotifierMod.cs 
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ActionMenuUtils.icons"))
                using (var tempStream = new MemoryStream((int)stream.Length))
                {
                    stream.CopyTo(tempStream);

                    iconsAssetBundle = AssetBundle.LoadFromMemory_Internal(tempStream.ToArray(), 0);
                    iconsAssetBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                }
                respawnIcon = iconsAssetBundle.LoadAsset_Internal("Assets/Resources/Refresh.png", Il2CppType.Of<Texture2D>()).Cast<Texture2D>();
                respawnIcon.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                helpIcon = iconsAssetBundle.LoadAsset_Internal("Assets/Resources/Help.png", Il2CppType.Of<Texture2D>()).Cast<Texture2D>();
                helpIcon.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                goHomeIcon = iconsAssetBundle.LoadAsset_Internal("Assets/Resources/Home.png", Il2CppType.Of<Texture2D>()).Cast<Texture2D>();
                goHomeIcon.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                resetAvatarIcon = iconsAssetBundle.LoadAsset_Internal("Assets/Resources/Avatar.png", Il2CppType.Of<Texture2D>()).Cast<Texture2D>();
                resetAvatarIcon.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                rejoinInstanceIcon = iconsAssetBundle.LoadAsset_Internal("Assets/Resources/Pin.png", Il2CppType.Of<Texture2D>()).Cast<Texture2D>();
                rejoinInstanceIcon.hideFlags |= HideFlags.DontUnloadUnusedAsset;

            }
            catch (Exception e) {
                MelonLogger.Warning("Consider checking for newer version as mod possibly no longer working, Exception occured OnAppStart(): " + e.Message);
            }
            // Creates new Api instance and patches all required methods if found 
            actionMenuApi = new ActionMenuAPI();
            ModSettings.RegisterSettings();
            ModSettings.Apply();
            SetupButtons();
        }

        public override void OnPreferencesLoaded() => ModSettings.Apply();
        public override void OnPreferencesSaved() => ModSettings.Apply();


        private static void SetupButtons()
        {
           
            actionMenuApi.AddPedalToExistingMenu(ActionMenuAPI.ActionMenuPageType.Options, delegate
            {
                actionMenuApi.CreateSubMenu(delegate {
                    AddRespawnButton();
                    AddGoHomeButton();
                    AddResetAvatarButton();
                    AddInstanceRejoinButton();
                });
            }, "Help", helpIcon, ActionMenuAPI.Insertion.Post);
        }

        private static void AddResetAvatarButton()
        {
            if (ModSettings.confirmAvatarReset)
            {
                actionMenuApi.AddPedalToCustomMenu(delegate
                {
                    actionMenuApi.CreateSubMenu(delegate
                    {
                        actionMenuApi.AddPedalToCustomMenu(delegate
                        {
                            ObjectPublicAbstractSealedApBoObApBoUnique.Method_Public_Static_Void_ApiAvatar_String_0(API.Fetch<ApiAvatar>("avtr_c38a1615-5bf5-42b4-84eb-a8b6c37cbd11"), "fallbackAvatar");
                            //VRC.User.prop_User_0.Method_Public_Void_ApiAvatar_String_0(API.Fetch<ApiAvatar>("avtr_c38a1615-5bf5-42b4-84eb-a8b6c37cbd11"), "fallbackAvatar");
                        }, "Confirm Reset Avatar", resetAvatarIcon);
                    });
                }, "Reset Avatar", resetAvatarIcon);
            }
            else
            {
                actionMenuApi.AddPedalToCustomMenu(delegate
                {
                    ObjectPublicAbstractSealedApBoObApBoUnique.Method_Public_Static_Void_ApiAvatar_String_0(API.Fetch<ApiAvatar>("avtr_c38a1615-5bf5-42b4-84eb-a8b6c37cbd11"), "fallbackAvatar");
                }, "Reset Avatar", resetAvatarIcon);
            }
        }

        private static void AddGoHomeButton()
        {
            if (ModSettings.confirmGoHome)
            {
                actionMenuApi.AddPedalToCustomMenu(delegate
                {
                    actionMenuApi.CreateSubMenu(delegate
                    {
                        actionMenuApi.AddPedalToCustomMenu(delegate
                        {
                            GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/GoHomeButton").GetComponent<Button>().onClick.Invoke();
                        }, "Confirm Go Home", goHomeIcon);
                    });
                }, "Go Home", goHomeIcon);
            }
            else
            {
                actionMenuApi.AddPedalToCustomMenu(delegate
                {
                    GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/GoHomeButton").GetComponent<Button>().onClick.Invoke();
                }, "Go Home", goHomeIcon);
            }
        }

        private static void AddRespawnButton()
        {
            if (ModSettings.confirmRespawn)
            {
                actionMenuApi.AddPedalToCustomMenu(delegate
                {
                    actionMenuApi.CreateSubMenu(delegate
                    {
                        actionMenuApi.AddPedalToCustomMenu(delegate
                        {
                            Respawn();
                        }, "Confirm Respawn", respawnIcon);
                    });
                }, "Respawn", respawnIcon);
            }
            else
            {
                actionMenuApi.AddPedalToCustomMenu(delegate
                {
                    Respawn();
                }, "Respawn", respawnIcon);

            }
        }

        private static void AddInstanceRejoinButton()
        {
            if (ModSettings.confirmInstanceRejoin)
            {
                actionMenuApi.AddPedalToCustomMenu(delegate
                {
                    actionMenuApi.CreateSubMenu(delegate
                    {
                        actionMenuApi.AddPedalToCustomMenu(delegate
                        {
                            RejoinInstance();
                        }, "Confirm Instance Rejoin", rejoinInstanceIcon);
                    });
                }, "Rejoin Instance", rejoinInstanceIcon);
            }
            else
            {
                actionMenuApi.AddPedalToCustomMenu(delegate
                {
                    RejoinInstance();
                }, "Rejoin Instance", rejoinInstanceIcon);

            }
        }


        private static void Respawn()
        {
            GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/RespawnButton").GetComponent<Button>().onClick.Invoke();
            VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<VRCMotionState>().Reset();
        }
        private static void RejoinInstance()
        {
            var instance = RoomManager.field_Internal_Static_ApiWorldInstance_0;
            Networking.GoToRoom($"{instance.instanceWorld.id}:{instance.idWithTags}");
        }
    }
}
