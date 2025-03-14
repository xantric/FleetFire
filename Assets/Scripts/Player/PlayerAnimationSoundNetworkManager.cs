using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimationSoundNetworkManager : NetworkBehaviour
{
    public GameObject Clip_on_point;
    public AudioClip shoot_sound;
    public AudioClip silence_shoot_sound;
    public GameObject Shoot_start_point;

    public GameObject muzzle;

    [ServerRpc]
    public void PlayShootSoundServerRpc(bool suppressor_a_bool, bool suppressor_mac10_bool,
                                        bool suppressor_c_bool, bool suppressor_d_bool,
                                        Quaternion rotation, Vector3 Add_spread)
    {
        PlayShootSoundClientRpc(suppressor_a_bool, suppressor_mac10_bool,
                                suppressor_c_bool, suppressor_d_bool,
                                rotation, Add_spread);
        if (IsHost)
        {
            PlayShootSoundLocally(suppressor_a_bool, suppressor_mac10_bool, suppressor_c_bool, suppressor_d_bool, rotation);
        }
    }
    [ClientRpc]
    public void PlayShootSoundClientRpc(bool suppressor_a_bool, bool suppressor_mac10_bool, 
                                        bool suppressor_c_bool, bool suppressor_d_bool,
                                        Quaternion rotation, Vector3 Add_spread)
    {
        if (!suppressor_a_bool && !suppressor_mac10_bool && !suppressor_c_bool && !suppressor_d_bool)
        {
            Debug.Log("Sound Play");
            GameObject g = Instantiate(Clip_on_point, Shoot_start_point.transform.position, rotation);
            g.GetComponent<AudioSource>().clip = shoot_sound;
            g.GetComponent<AudioSource>().maxDistance = 100;
            g.GetComponent<AudioSource>().Play();
            g.transform.parent = Shoot_start_point.transform;
        }
        if (suppressor_a_bool || suppressor_mac10_bool || suppressor_c_bool || suppressor_d_bool)
        {
            GameObject g = Instantiate(Clip_on_point, Shoot_start_point.transform.position, rotation);
            g.GetComponent<AudioSource>().clip = silence_shoot_sound;
            g.GetComponent<AudioSource>().maxDistance = 100;
            g.GetComponent<AudioSource>().Play();
            g.transform.parent = Shoot_start_point.transform;

            //The Suppressor decreases the spread to 33 %
            Add_spread -= (Add_spread / 3);
        }
    }

    private void PlayShootSoundLocally(bool suppressor_a_bool, bool suppressor_mac10_bool,
                                   bool suppressor_c_bool, bool suppressor_d_bool,
                                   Quaternion rotation)
    {
        GameObject g = Instantiate(Clip_on_point, Shoot_start_point.transform.position, rotation);
        AudioSource audioSource = g.GetComponent<AudioSource>();

        if (!suppressor_a_bool && !suppressor_mac10_bool && !suppressor_c_bool && !suppressor_d_bool)
        {
            //Debug.Log("Playing Gunshot Sound Locally for Host");
            audioSource.clip = shoot_sound;
        }
        else
        {
            //Debug.Log("Playing Suppressed Gunshot Sound Locally for Host");
            audioSource.clip = silence_shoot_sound;
        }

        audioSource.maxDistance = 100;
        audioSource.Play();
        g.transform.parent = Shoot_start_point.transform;
    }


    [ServerRpc(RequireOwnership = false)]
    public void PlayMuzzleEffectServerRpc()
    {
        PlayMuzzleEffectClientRpc();
    }
    [ClientRpc]
    public void PlayMuzzleEffectClientRpc()
    {
        GameObject spawned_muzzle = Instantiate(muzzle, Shoot_start_point.transform.position, Shoot_start_point.transform.rotation);
        spawned_muzzle.GetComponent<muzzle_flash>().origin = Shoot_start_point;
    }


}

