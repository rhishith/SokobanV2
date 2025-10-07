using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmAudioSource, sfxAudioSource;
    [SerializeField] private AudioClip[] bgms;
    [SerializeField] private AudioClip playerMovementSfx, boxMoveSfx, goalSfx, reverseSfx;
    [SerializeField] private Slider sfxSlider, bgmSlider;
    [SerializeField] private AudioMixer AudioMixer;

    private void Start()
    {
        float bgmVolume, sfxVolume;
        AudioMixer.GetFloat("BGMVolume", out bgmVolume);
        AudioMixer.GetFloat("SFXVolume", out sfxVolume);
        bgmSlider.value = Mathf.Pow(10, bgmVolume);
        sfxSlider.value = Mathf.Pow(10, sfxVolume);
    }

    public void ChangeBgmVolume()
    {
        AudioMixer.SetFloat("BGMVolume", bgmSlider.value);
    }

    public void ChangeSfxVolume()
    {
        AudioMixer.SetFloat("SFXVolume", sfxSlider.value);
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
