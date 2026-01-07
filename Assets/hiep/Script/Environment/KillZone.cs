using UnityEngine;
using UnityEngine.SceneManagement;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Load lại màn hiện tại
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}