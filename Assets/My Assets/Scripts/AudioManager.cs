using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmAudioSource,sfxAudioSource;
    [SerializeField] private AudioClip[] bgms;
    [SerializeField] private AudioClip playerMovementSfx, boxMoveSfx, goalSfx, reverseSfx;

    public void SwitchBackgroundMusic(int currentLevel)
    {
        bgmAudioSource.clip = bgms[currentLevel];
        bgmAudioSource.Play();
    }

    public void StopBackgroundMusic()
    {
        bgmAudioSource.Stop();
    }

    #region Sfx
    public void PlayPlayerMovementSfx()
    {
        sfxAudioSource.PlayOneShot(playerMovementSfx);
    }

    public void PlayBoxMoveSfx()
    {
        sfxAudioSource.PlayOneShot(boxMoveSfx);
    }

    public void PlayGoalSfx()
    {
        sfxAudioSource.PlayOneShot(goalSfx);
    }

    public void PlayUndoSfx()
    {
        sfxAudioSource.PlayOneShot(reverseSfx);
    }
    #endregion
}
