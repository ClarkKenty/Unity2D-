﻿//该脚本控制音效的播放
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;
    public AudioSource efxSource;
    public AudioSource musicSource;
    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;
    public void PlaySingle(AudioClip clip){
        efxSource.clip = clip;
        efxSource.Play();
    }

    public void RandomizeSfx(params AudioClip[] clips){
        int randomIndex = Random.Range(0,clips.Length);
        float randomPitch = Random.Range(lowPitchRange,highPitchRange);
        efxSource.clip = clips[randomIndex];
        efxSource.pitch = randomPitch;
        efxSource.Play();
    }
    void Awake(){
        if(instance == null){
            instance = this;
        }else if(instance!=this){
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
