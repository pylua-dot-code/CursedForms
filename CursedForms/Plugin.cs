using BepInEx;
using CursedForms;
using System;
using UnityEngine;
using UnityEngine.XR;
using Utilla;
using System.ComponentModel;
//Nothing to fix here

namespace CursedForms
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.10")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    //fixed utilia version
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;

        bool RightTrigger;
        bool RightGrip;
        bool RightPrimary;

        bool LeftTrigger;
        bool LeftGrip;
        bool LeftPrimary;

        public GameObject RightPlatform = null;
        public GameObject LeftPlatform = null;

        GameObject rightHandPos;
        GameObject leftHandPos;

        Quaternion resetRotation = new Quaternion(0, 0, 0, 0);

        void Start()
        {
            /* A lot of Gorilla Tag systems will not be set up when start is called /*
			/* Put code in OnGameInitialized to avoid null references */

            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnEnable()
        {
            /* Set up your mod here */
            /* Code here runs at the start and whenever your mod is enabled*/
            GameObject BasePlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BasePlatform.transform.localScale = new Vector3(0.3f, 0.1f, 0.3f);
            Debug.Log("base platform created");

            RightPlatform = Instantiate(BasePlatform);
            LeftPlatform = Instantiate(BasePlatform);

            GameObject.Destroy(BasePlatform);

            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            /* Undo mod setup here */
            /* This provides support for toggling mods with ComputerInterface, please implement it :) */
            /* Code here runs whenever your mod is disabled (including if it disabled on startup)*/
            GameObject.Destroy(LeftPlatform);
            GameObject.Destroy(RightPlatform);

            HarmonyPatches.RemoveHarmonyPatches();
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            /* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */

            GameObject BasePlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BasePlatform.transform.localScale = new Vector3(0.3f, 0.1f, 0.3f);
            Debug.Log("base platform created");

            RightPlatform = Instantiate(BasePlatform);
            LeftPlatform = Instantiate(BasePlatform);

            rightHandPos = GorillaTagger.Instance.rightHandTransform.transform.localPosition;
            leftHandPos = GorillaTagger.Instance.leftHandTransform.transform.localPosition;

            GameObject.Destroy(BasePlatform);
        }

        void Update(ControllerInputPoller controlinputpoll)
        {
            /* Code here runs every frame when the mod is enabled */
            if (inRoom)
            {
                if (RightPlatform != null && LeftPlatform != null)
                {
                    RightGrip = controlinputpoll.rightGrab;
                    LeftGrip = controlinputpoll.leftGrab;
                    RightTrigger = controlinputpoll.rightControllerIndexFloat;
                    LeftTrigger = controlinputpoll.leftControllerIndexFloat;

                    if (!RightGrip && !RightTrigger)
                    {
                        RightPlatform.transform.SetParent(rightHandPos.transform);
                        RightPlatform.transform.localPosition = new Vector3(0.1f, 0, 0);
                    }
                    else if (RightGrip || RightTrigger)
                    {
                        RightPlatform.transform.localRotation = resetRotation;
                        RightPlatform.transform.SetParent(null);
                        RightPlatform.transform.position = Vector3.zero;
                    }

                    if (!LeftGrip && !LeftTrigger)
                    {
                        LeftPlatform.transform.SetParent(leftHandPos.transform);
                        LeftPlatform.transform.localPosition = new Vector3(-0.1f, 0, 0);
                    }
                    else if (LeftGrip || LeftTrigger)
                    {
                        LeftPlatform.transform.localRotation = resetRotation;
                        LeftPlatform.transform.SetParent(null);
                        LeftPlatform.transform.position = Vector3.zero;
                    }
                }
            }
        }

        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            /* Activate your mod here */
            /* This code will run regardless of if the mod is enabled*/
            GameObject BasePlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BasePlatform.transform.localScale = new Vector3(0.3f, 0.1f, 0.3f);
            if (RightPlatform == null)
            {
                RightPlatform = Instantiate(BasePlatform);
            }
            if (LeftPlatform == null)
            {
                LeftPlatform = Instantiate(BasePlatform);
            }
            GameObject.Destroy(BasePlatform);
            inRoom = true;
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            /* Deactivate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            if (RightPlatform != null)
            {
                GameObject.Destroy(RightPlatform);
            }
            if (LeftPlatform != null)
            {
                GameObject.Destroy(LeftPlatform);
            }
            inRoom = false;
        }
    }
}
