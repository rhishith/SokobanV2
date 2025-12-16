using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] bgms;
    [SerializeField] private AudioClip playerMovementSfx;
    [SerializeField] private AudioClip boxMoveSfx;
    [SerializeField] private AudioClip goalSfx;
    [SerializeField] private AudioClip reverseSfx;

    [Header("Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    private const string BGM_VOLUME_PARAM = "BGMVolume";
    private const string SFX_VOLUME_PARAM = "SFXVolume";

    public void SetSfxVolume(Slider sfxSlider)
    {
        if (audioMixer.GetFloat(SFX_VOLUME_PARAM, out float sfxVolume))
        {
            sfxSlider.value = sfxVolume;
        }
    }

    public void SetBgmVolume(Slider bgmSlider)
    {
        if (audioMixer.GetFloat(BGM_VOLUME_PARAM, out float bgmVolume))
        {
            bgmSlider.value = bgmVolume;
        }
    }

    public void ChangeBgmVolume(Slider slider)
    {
        audioMixer.SetFloat(BGM_VOLUME_PARAM, slider.value);
    }

    public void ChangeSfxVolume(Slider slider)
    {
        audioMixer.SetFloat(SFX_VOLUME_PARAM, slider.value);
    }

    #region Bgm
    public void SwitchBackgroundMusic(int currentLevel)
    {
        if (currentLevel >= 0 && currentLevel < bgms.Length)
        {
            bgmAudioSource.clip = bgms[currentLevel];
            bgmAudioSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music for level {currentLevel} not found.");
        }
    }

    public void StopBackgroundMusic()
    {
        bgmAudioSource.Stop();
    }
    #endregion

    #region Sfx
    public void PlayPlayerMovementSfx() => PlaySfx(playerMovementSfx);
    public void PlayBoxMoveSfx() => PlaySfx(boxMoveSfx);
    public void PlayGoalSfx() => PlaySfx(goalSfx);
    public void PlayUndoSfx() => PlaySfx(reverseSfx);

    private void PlaySfx(AudioClip clip)
    {
        if (clip != null)
        {
            sfxAudioSource.PlayOneShot(clip);
        }
    }
    #endregion
}
