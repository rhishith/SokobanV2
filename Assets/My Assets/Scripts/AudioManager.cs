using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmAudioSource, sfxAudioSource;
    [SerializeField] private AudioClip[] bgms;
    [SerializeField] private AudioClip playerMovementSfx, boxMoveSfx, goalSfx, reverseSfx;
    [SerializeField] private AudioMixer AudioMixer;

    public void SetSfxVolume(Slider sfxSlider)
    {
        float sfxVolume;
        AudioMixer.GetFloat("SFXVolume", out sfxVolume);
        sfxSlider.value = sfxVolume;
    }

    public void SetBgmVolume(Slider bgmSlider)
    {
        float bgmVolume;
        AudioMixer.GetFloat("BGMVolume", out bgmVolume);
        bgmSlider.value = bgmVolume;
    }

    public void ChangeBgmVolume(Slider slider)
    {
        AudioMixer.SetFloat("BGMVolume", slider.value);
    }

    public void ChangeSfxVolume(Slider slider)
    {
        AudioMixer.SetFloat("SFXVolume", slider.value);
    }

    #region Bgm
    internal void SwitchBackgroundMusic(int currentLevel)
    {
        bgmAudioSource.clip = bgms[currentLevel];
        bgmAudioSource.Play();
    }

    internal void StopBackgroundMusic()
    {
        bgmAudioSource.Stop();
    }
    #endregion

    #region Sfx
    internal void PlayPlayerMovementSfx()
    {
        sfxAudioSource.PlayOneShot(playerMovementSfx);
    }

    internal void PlayBoxMoveSfx()
    {
        sfxAudioSource.PlayOneShot(boxMoveSfx);
    }

    internal void PlayGoalSfx()
    {
        sfxAudioSource.PlayOneShot(goalSfx);
    }

    internal void PlayUndoSfx()
    {
        sfxAudioSource.PlayOneShot(reverseSfx);
    }
    #endregion
}
