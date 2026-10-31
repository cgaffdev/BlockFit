using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple Audio Queue Player
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickSound;
    [SerializeField] private AudioClip placeSound;
    [SerializeField] private AudioClip rotateSound;
    [SerializeField] private AudioClip clearSound;

    void Awake(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    public void PlayPickSound(){
        audioSource.PlayOneShot(pickSound);
    }

    public void PlayPlaceSound(){
        audioSource.PlayOneShot(placeSound);
    }

    public void PlayRotateSound(){
        audioSource.PlayOneShot(rotateSound);
    }

    public void PlayClearSound(){
        audioSource.PlayOneShot(clearSound);
    }

}
