using UnityEngine;

public class Consumible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SumarObjeto();
            }

            Destroy(gameObject);
        }
    }
}