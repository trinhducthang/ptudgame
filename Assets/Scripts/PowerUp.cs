using UnityEngine;

public class PowerUp : MonoBehaviour
{   
    private AudioManager audioManager;
    public enum Type
    {
        Coin,
        ExtraLife,
        MagicMushroom,
        Starpower,
    }

    public Type type;
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            Collect(other.gameObject);
        }
    }

    private void Collect(GameObject player)
    {
        switch (type)
        {
            case Type.Coin:
                GameManager.Instance.AddCoin();
                break;

            case Type.ExtraLife:
                GameManager.Instance.AddLife();
                break;

            case Type.MagicMushroom:
                player.GetComponent<Player>().Grow();
                audioManager.PlaySFX(audioManager.jumpClip);
                break;

            case Type.Starpower:
                player.GetComponent<Player>().Starpower();
                audioManager.PlaySFX(audioManager.starPowerClip);
                break;
        }

        Destroy(gameObject);
    }

}
