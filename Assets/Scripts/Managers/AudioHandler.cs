using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] private SoundsSO soundsSO;
    [SerializeField] private AudioSource startAudioSource;
    public static AudioHandler Instance { get; private set; }

    public enum Sounds {
        start,
        snakeBit,
        ladderClimbed,
        energyEruption
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(this);
        }
    }

    private void Start() {
        //AudioSource.PlayClipAtPoint(soundsSO.bgm, Vector3.zero);
    }

    public float PlaySound(Sounds sounds) {
        float length = 0;
        switch (sounds) {
            case Sounds.start:
                length = Play(soundsSO.start); break;

            case Sounds.snakeBit:
                Debug.Log(soundsSO.snakeBit);
                length = Play(soundsSO.snakeBit); break;
            
            case Sounds.energyEruption:
                length = Play(soundsSO.energyEruption); break;
        }
        return length;
    }

    private float Play(AudioClip clip) {
        startAudioSource.clip = clip;
        startAudioSource.Play();
        return clip.length;
    }
}