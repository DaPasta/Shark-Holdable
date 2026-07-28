using BepInEx;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Mod1
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private AssetBundle sharkBundle;
        private GameObject sharkPrefab;
        private GameObject sharkObject;
        private bool initialized;

        private void Start()
        {
            HarmonyPatches.ApplyHarmonyPatches();
        }

        private void Update()
        {
            if (initialized || GorillaTagger.Instance == null)
                return;

            initialized = true;
            SetupShark();
        }

        private void OnDisable()
        {
            HarmonyPatches.RemoveHarmonyPatches();

            if (sharkObject != null)
                Destroy(sharkObject);

            if (sharkBundle != null)
                sharkBundle.Unload(false);
        }

        private void SetupShark()
        {
            sharkBundle = LoadAssetBundle("Mod1.Assets.sharky");
            if (sharkBundle == null)
                return;

            sharkPrefab = sharkBundle.LoadAsset<GameObject>("akula");
            if (sharkPrefab == null)
            {
                foreach (string asset in sharkBundle.GetAllAssetNames())
                    Debug.Log(asset);
                return;
            }

            sharkObject = Instantiate(sharkPrefab);
            sharkObject.name = "Akula_Holdable";
            sharkObject.transform.localScale = Vector3.one;

            AttachToHand();
        }

        private void AttachToHand()
        {
            if (sharkObject == null)
                return;

            Transform hand = GorillaTagger.Instance
                .offlineVRRig
                .transform
                .Find("rig/hand.R");

            if (hand == null)
            {
                return;
            }

            sharkObject.transform.SetParent(hand, false);
            sharkObject.transform.localPosition = new Vector3(0.08f, 0.05f, 0.02f);
            sharkObject.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
            sharkObject.transform.localScale = Vector3.one * 0.7f;
        }

        private AssetBundle LoadAssetBundle(string resourceName)
        {
            Stream stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                foreach (string resource in Assembly.GetExecutingAssembly()
                    .GetManifestResourceNames())
                    Debug.Log(resource);
                return null;
            }

            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }
    }
}
