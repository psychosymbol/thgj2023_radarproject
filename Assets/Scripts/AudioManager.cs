using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject); // game only has 1 scene. no need to dontDestroyOnload
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        
    }


    public AudioSource bgmSource;
    public AudioSource ambientSource;

    public List<AudioSource> sfxSources = new List<AudioSource>();


    public List<AudioClip> clips = new List<AudioClip>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayBGM()
    {
        bgmSource.Play();
    }

    
    public void PlayAmbient(string clipname)
    {
        AudioClip clip = null;
        for (int i = 0; i < clips.Count; i++)
        {
            if (clips[i].name == clipname)
            {
                clip = clips[i];
            }
        }

        if (clip != null)
        {
            if (ambientSource.isPlaying)
            {
                ambientSource.Stop();
            }

            ambientSource.clip = clip;
            ambientSource.Play();
        }
    }

    public void PlaySound(string clipname, Chanel chanel)
    {
        AudioClip clip = null;

        var _chanel = (int)chanel;

        for (int i = 0; i < clips.Count; i++)
        {
            if (clips[i].name == clipname)
            {
                clip = clips[i];
            }
        }

        if (clip != null)
        {
            if (sfxSources[_chanel].isPlaying)
            {
                sfxSources[_chanel].Stop();
            }

            sfxSources[_chanel].clip = clip;
            sfxSources[_chanel].Play();
        }
    }
    public void StopSound(Chanel chanel)
    {
        if (sfxSources[(int)chanel].isPlaying)
        {
            sfxSources[(int)chanel].Stop();
        }
    }

    public enum Chanel
    {
        SFX_1 = 0,
        SFX_2,
        SONAR,
        ELEVATOR_LOOP
    }


}
