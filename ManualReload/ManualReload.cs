using GTA;
using GTA.Native;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Windows.Forms;



public class ManualReload : Script
{
    public ManualReload()
    {
        Tick += OnTick;
        KeyUp += OnKeyUp;
        KeyDown += OnKeyDown;

        
        //Interval = 10;
    }


    bool bAddedBullet = false;
    public List<int> IsGunEmpty = new List<int>();
    public int soundCounter = 0;
    private bool _SettingsLoaded = false;
    bool PlayEmptyGunSound;
    float AudioLevel;
    bool SoundPlayed;
    public void LoadSettings()
    {
        if (!_SettingsLoaded)
        {
            PlayEmptyGunSound = Settings.GetValue("Settings", "PlayEmptySound", true);
            AudioLevel = Settings.GetValue("Settings", "AudioLevel", 1.0f);
            _SettingsLoaded = true;
        }
    }

    public void OnTick(object sender, EventArgs e)
    {

        LoadSettings();    

        if (PlayEmptyGunSound && !SoundPlayed && Game.Player.Character.Weapons.Current.AmmoInClip == 1 && bAddedBullet)
        {
            SoundPlayed = true;
            _outputDevice = new WaveOutEvent();
            _audioFile = new AudioFileReader($"{Application.StartupPath}\\scripts\\ManualReload\\empty.wav");
            _outputDevice.Init(_audioFile);
            _audioFile.Volume = AudioLevel;
            _outputDevice.Play();
        }

        // turn on when
        if (!Game.Player.Character.IsInVehicle() && Game.Player.Character.Weapons.Current.Hash != WeaponHash.Unarmed)
        {
            // if player is holding a proper gun and bullet hasnt been added yet add bullet && player isnt reloading
            if (Game.Player.Character.Weapons.Current.MaxAmmoInClip > 2 && bAddedBullet == false && !Game.Player.Character.IsReloading)
            {
                if (Game.Player.Character.Weapons.Current.AmmoInClip == 1)
                {
                    Game.Player.Character.Weapons.Current.AmmoInClip = 2;
                    bAddedBullet = true;
                    SoundPlayed = false;
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

            // if gun has reloaded reset the bullet pool and remove it from emptygun list and if player has ammo
            if (Game.IsControlJustReleased(GTA.Control.Reload) && Game.Player.Character.Weapons.Current.Ammo > 1)
            {
                IsGunEmpty.Remove((int)Game.Player.Character.Weapons.Current.Hash);
                bAddedBullet = false;
                SoundPlayed = false;
            }


            // play dry fire sound on empty mag
            if (Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 1, GTA.Control.Attack2) && Game.Player.Character.Weapons.Current.AmmoInClip == 1 && Game.Player.Character.IsAiming)
            {
                if (soundCounter > 25)
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

    private IWavePlayer _outputDevice;
    private AudioFileReader _audioFile;
    private void PlaySound()
    {
        int WeaponType = Function.Call<int>(Hash.GET_WEAPONTYPE_GROUP, (int)Game.Player.Character.Weapons.Current.Hash);

        try
        {
            switch (WeaponType)
            {
                case 416676503:
                    _outputDevice = new WaveOutEvent();
                    _audioFile = new AudioFileReader($"{Application.StartupPath}\\scripts\\ManualReload\\pistol.wav");
                    _outputDevice.Init(_audioFile);
                    _audioFile.Volume = AudioLevel;
                    _outputDevice.Play();
                    break;
                case 970310034:
                    _outputDevice = new WaveOutEvent();
                    _audioFile = new AudioFileReader($"{Application.StartupPath}\\scripts\\ManualReload\\rifle.wav");
                    _outputDevice.Init(_audioFile);
                    _audioFile.Volume = AudioLevel;
                    _outputDevice.Play();
                    break;
                case 860033945:
                    _outputDevice = new WaveOutEvent();
                    _audioFile = new AudioFileReader($"{Application.StartupPath}\\scripts\\ManualReload\\shotgun.wav");
                    _outputDevice.Init(_audioFile);
                    _audioFile.Volume = AudioLevel;
                    _outputDevice.Play();
                    break;
                case -957766203:
                    _outputDevice = new WaveOutEvent();
                    _audioFile = new AudioFileReader($"{Application.StartupPath}\\scripts\\ManualReload\\smg.wav");
                    _outputDevice.Init(_audioFile);
                    _audioFile.Volume = AudioLevel;
                    _outputDevice.Play();
                    break;
                case -1212426201:
                    _outputDevice = new WaveOutEvent();
                    _audioFile = new AudioFileReader($"{Application.StartupPath}\\scripts\\ManualReload\\sniper.wav");
                    _outputDevice.Init(_audioFile);
                    _audioFile.Volume = AudioLevel;
                    _outputDevice.Play();
                    break;
                default:
                    _outputDevice = new WaveOutEvent();
                    _audioFile = new AudioFileReader($"{Application.StartupPath}\\scripts\\ManualReload\\other.wav");
                    _outputDevice.Init(_audioFile);
                    _audioFile.Volume = AudioLevel;
                    _outputDevice.Play();
                break;
            }
        }
        catch (Exception)
        {
            GTA.UI.Screen.ShowSubtitle($"ERROR: Couldn't find sound file in {Application.StartupPath}\\scripts\\ManualReload", 3000);
        }
    }
}