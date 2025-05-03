using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.UI;


public class ManualReload : Script
{
    public ManualReload()
    {
        Tick += OnTick;
        KeyUp += OnKeyUp;
        KeyDown += OnKeyDown;

        //Interval = 50;
    }


    bool bAddedBullet = false;
    public List<int> IsGunEmpty = new List<int>();
    public int soundCounter = 0;


    public void OnTick(object sender, EventArgs e)
    {
        /* debugging
        foreach (int value in IsGunEmpty)
        {
            Notification.PostTicker(IsGunEmpty.Count.ToString(), false);
        }
        */

        // turn off when in vehicle (forgot, oops)
        if (!Game.Player.Character.IsInVehicle())
        {
            // if player is holding a proper gun and bullet hasnt been added yet add bullet
            if (Game.Player.Character.Weapons.Current.MaxAmmoInClip > 2 && bAddedBullet == false)
            {
                if (Game.Player.Character.Weapons.Current.AmmoInClip == 1)
                {
                    Game.Player.Character.Weapons.Current.AmmoInClip = 2;
                    bAddedBullet = true;
                }
            } // else if players gun is empty and bullet has been added disable attack
            else if (Game.Player.Character.Weapons.Current.AmmoInClip == 1 && bAddedBullet == true)
            {
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, GTA.Control.Attack, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, GTA.Control.Attack2, true);
                // if gun has not been added to the emptygun list add it
                if (!IsGunEmpty.Contains((int)Game.Player.Character.Weapons.Current.Hash))
                {
                    IsGunEmpty.Add((int)Game.Player.Character.Weapons.Current.Hash);
                }
            }

            // if the gun is in the emptygun list and player isnt reloading set the ammo to 1
            if (IsGunEmpty.Contains((int)Game.Player.Character.Weapons.Current.Hash) && !Game.Player.Character.IsReloading)
            {
                Game.Player.Character.Weapons.Current.AmmoInClip = 1;
            }

            // if gun has reloaded reset the bullet pool and remove it from emptygun list
            if (Game.IsControlJustReleased(GTA.Control.Reload))
            {
                IsGunEmpty.Remove((int)Game.Player.Character.Weapons.Current.Hash);
                bAddedBullet = false;
            }


            // play dry fire sound on empty mag
            if (Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 1, GTA.Control.Attack2) && Game.Player.Character.Weapons.Current.AmmoInClip == 1 && Game.Player.Character.IsAiming)
            {
                if (soundCounter > 17)
                {
                    PlaySound();
                    soundCounter = 0;
                }
            }
            soundCounter++;

        }

        // if gun is full reset the added bullet bool and remove from list (fix for exiting vehicles with full gun)
        if (Game.Player.Character.Weapons.Current.AmmoInClip == Game.Player.Character.Weapons.Current.MaxAmmoInClip)
        {
            bAddedBullet = false;
            IsGunEmpty.Remove((int)Game.Player.Character.Weapons.Current.Hash);
        }
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
    }

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
    }

    // load sound files
    System.Media.SoundPlayer rifle = new System.Media.SoundPlayer($"{Application.StartupPath}\\scripts\\ManualReload\\rifle.wav");
    System.Media.SoundPlayer shotgun = new System.Media.SoundPlayer($"{Application.StartupPath}\\scripts\\ManualReload\\shotgun.wav");
    System.Media.SoundPlayer pistol = new System.Media.SoundPlayer($"{Application.StartupPath}\\scripts\\ManualReload\\pistol.wav");
    System.Media.SoundPlayer smg = new System.Media.SoundPlayer($"{Application.StartupPath}\\scripts\\ManualReload\\smg.wav");
    System.Media.SoundPlayer sniper = new System.Media.SoundPlayer($"{Application.StartupPath}\\scripts\\ManualReload\\sniper.wav");
    System.Media.SoundPlayer other = new System.Media.SoundPlayer($"{Application.StartupPath}\\scripts\\ManualReload\\other.wav");

    // audio
    private void PlaySound()
    {
        int WeaponType = Function.Call<int>(Hash.GET_WEAPONTYPE_GROUP, (int)Game.Player.Character.Weapons.Current.Hash);

        try
        {
            if (WeaponType == 416676503)
            {
                pistol.Play();
            }
            else if (WeaponType == 970310034)
            {
                rifle.Play();
            }
            else if (WeaponType == 860033945)
            {
                shotgun.Play();
            }
            else if (WeaponType == -957766203)
            {
                smg.Play();
            }
            else if (WeaponType == -1212426201)
            {
                sniper.Play();
            }
            else
            {
                other.Play();
            }
        }
        catch (Exception)
        {
            Notification.PostTicker($"ERROR: Couldn't find sound file in {Application.StartupPath}\\scripts\\ManualReload", true, true);
        }
    }
}