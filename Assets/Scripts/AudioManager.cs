using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header ("-------Audio source-------")]
[SerializeField] AudioSource musicSource;
[SerializeField] AudioSource SFXSource;

[Header("----Audio clip-----")]

public AudioClip Movement;
public AudioClip background;
public AudioClip death;
public AudioClip jump;
public AudioClip shooting;
public AudioClip Reload;


private void Start()
{
    musicSource.clip = background;
    musicSource.Play();
    }

    public void PlaySFX(AudioClip clip){
        SFXSource.PlayOneShot(clip);
    }

}
