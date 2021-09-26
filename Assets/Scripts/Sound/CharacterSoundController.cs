using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundController : MonoBehaviour
{
    public AudioClip Jump;

    private AudioSource _audioPlayer;
    // Start is called before the first frame update
    private void Start()
    {
        _audioPlayer = GetComponent<AudioSource>();
    }

    public void PlayJumpSound()
    {
        _audioPlayer.PlayOneShot(Jump);
    }
}
