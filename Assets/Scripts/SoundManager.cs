using UnityEngine;
using System;
using System.Collections;

public enum SoundType
{
    DOOR,
    WALK,
    RUN,
    LIGHT, 
    DISABLE,
    SENSE,
    PARANOIA,
    DEATH,
    MWALK,
    MRUN,
    METALDOOR,
    PTREMBLE,
    PFALL
}

[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds { get => sounds; }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList;
    private static SoundManager instance;
    private AudioSource sfxSource;
    private AudioSource heartbeatSource;
    private AudioSource disableSource;
    private Coroutine paranoiaLoopCoroutine;
    private static bool _blockGameplaySounds = false;

    private void Awake()
    {
        instance = this;

        var sources = GetComponents<AudioSource>();        
        sfxSource = GetComponent<AudioSource>();

        if (sources.Length < 2)
        {
            heartbeatSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            heartbeatSource = sources[1];
        }

        if (sources.Length < 3)
        {
            disableSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            disableSource = sources[2];
        }

        heartbeatSource.playOnAwake = false;
        heartbeatSource.loop = true;
        heartbeatSource.clip = null;
    }

    //private void RebuildHeartbeatSource()
    //{
    //    if (heartbeatSource != null)
    //    {
    //        DestroyImmediate(heartbeatSource);
    //    }

    //    heartbeatSource = gameObject.AddComponent<AudioSource>();
    //    heartbeatSource.playOnAwake = false;
    //    heartbeatSource.loop = true;
    //    heartbeatSource.clip = null;
    //    heartbeatSource.volume = 1f;
    //    heartbeatSource.pitch = 1f;
    //}

    public static void PlaySound(SoundType sound, float volume = .7f)
    {
        if (_blockGameplaySounds && sound != SoundType.DEATH) return;

        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        instance.sfxSource.pitch = UnityEngine.Random.Range(0.98f, 1.02f);
        instance.sfxSource.PlayOneShot(randomClip, volume);
    }

    public static void PlaySpatialSound(SoundType sound, Vector3 position, float volume = .7f, float maxDistance = 20f)
    {
        if (_blockGameplaySounds && sound != SoundType.DEATH) return;

        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];

        GameObject go = new GameObject("OneShotAudio");
        go.transform.position = position;

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 1f;
        source.maxDistance = maxDistance;
        source.rolloffMode = AudioRolloffMode.Logarithmic; 
        source.dopplerLevel = 0f;
        source.Play();
        Destroy(go, clip.length);
    }

    public static void PlaySoundWithPitch(SoundType sound, float volume = .7f, float pitch = 1f)
    {
        if (GameManager.Instance != null && GameManager.Instance.isDead && sound != SoundType.DEATH)
        {
            return;
        }
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];

        instance.sfxSource.pitch = pitch;
        instance.sfxSource.PlayOneShot(clip, volume);
    }

    public static void BlockNonDeathSounds()
    {
        _blockGameplaySounds = true;
    }

    public static void Unmute()
    {
        _blockGameplaySounds = false;
    }

    public static void StartHeartbeat()
    {
        if (instance == null) return;

        SoundList heartbeatList = instance.soundList[(int)SoundType.SENSE];
        if (heartbeatList.Sounds.Length == 0 || heartbeatList.Sounds[0] == null)
        {
            return;
        }

        instance.heartbeatSource.clip = heartbeatList.Sounds[0];
        instance.heartbeatSource.playOnAwake = true;
        instance.heartbeatSource.loop = true;
        instance.heartbeatSource.volume = 1f;
        instance.heartbeatSource.pitch = 1f;
        instance.heartbeatSource.Play();
    }

    public static void StopHeartbeat()
    {
        if (instance == null) return;
        instance.heartbeatSource.Stop();
    }

    public static void UpdateHeartbeat(
        float distance,
        float minDistance = 3f,
        float maxDistance = 20f,
        float minVolume = 0.4f,
        float maxVolume = 1f,
        float minPitch = 1f,
        float maxPitch = 2f)
    {
        if (instance == null || !instance.heartbeatSource.isPlaying) return;

        float clampedDist = Mathf.Clamp(distance, minDistance, maxDistance);
        float normalized = (maxDistance - clampedDist) / (maxDistance - minDistance);
        normalized = Mathf.Pow(normalized, 0.7f);

        instance.heartbeatSource.volume = Mathf.Lerp(minVolume, maxVolume, normalized);
        instance.heartbeatSource.pitch = Mathf.Lerp(minPitch, maxPitch, normalized);
    }

    public static void StartParanoiaSounds()
    {
        if (instance == null) return;

        StopParanoiaSounds();
        instance.paranoiaLoopCoroutine = instance.StartCoroutine(instance.ParanoiaLoop());
    }

    public static void StopParanoiaSounds()
    {
        if (instance != null && instance.paranoiaLoopCoroutine != null)
        {
            instance.StopCoroutine(instance.paranoiaLoopCoroutine);
            instance.paranoiaLoopCoroutine = null;
        }
    }

    private IEnumerator ParanoiaLoop()
    {
        while (true)
        {
            AudioClip[] clips = instance.soundList[(int)SoundType.PARANOIA].Sounds;
            if (clips == null || clips.Length == 0)
            {
                yield break;
            }

            AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];

            instance.sfxSource.pitch = UnityEngine.Random.Range(0.98f, 1.02f);
            float tempVolume = UnityEngine.Random.Range(0.6f, 0.8f); 

            instance.sfxSource.PlayOneShot(clip, tempVolume);

            float waitTime = UnityEngine.Random.Range(15f, 35f);
            yield return new WaitForSeconds(waitTime);
        }
    }

    public static void PlayDisableStart()
    {
        if (instance == null) return;

        SoundList disableList = instance.soundList[(int)SoundType.DISABLE];
        if (disableList.Sounds.Length == 0) return;

        AudioClip clip = disableList.Sounds[0];
        instance.disableSource.clip = clip;
        instance.disableSource.volume = 0.7f;
        instance.disableSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        instance.disableSource.Play();
    }

    public static void StopDisableSound()
    {
        if (instance != null && instance.disableSource.isPlaying)
        {
            instance.disableSource.Stop();
        }
    }

    public static void PlayFootstep(SoundType type, float pitchMultiplier = 1f)
    {
        if (type == SoundType.WALK || type == SoundType.RUN)
        {
            AudioClip[] clips = instance.soundList[(int)SoundType.WALK].Sounds;
            if (clips.Length == 0) return;

            AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];

            instance.sfxSource.pitch = 1f * pitchMultiplier;
            instance.sfxSource.PlayOneShot(clip);
            instance.sfxSource.pitch = 1f;
        }
    }


#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }
#endif
}
