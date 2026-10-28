using UnityEngine;

public class ZonaTransicion : MonoBehaviour
{
    [Header("Tipo de Zona")]
    public bool esVictoriaFinal = false;

    [Header("Requisito de Objetos")]
    public int objetosRequeridos = 10; // Cantidad necesaria para habilitar este TP

    [Header("Teletransporte")]
    public Transform puntoSiguienteNivel;

    [Header("Efectos de Audio")]
    public AudioClip sonidoTP;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                // Verifica si el jugador tiene la cantidad necesaria de Pokéballs
                if (player.objetosConsumidos >= objetosRequeridos)
                {
                    if (esVictoriaFinal)
                    {
                        // Victoria final en el Nivel 3
                        player.AlcanzarMeta();
                    }
                    else if (puntoSiguienteNivel != null)
                    {
                        // Reproduce sonido de TP
                        if (sonidoTP != null && player.audioSourceEfectos != null)
                        {
                            player.audioSourceEfectos.PlayOneShot(sonidoTP);
                        }

                        // Teletransporta al siguiente nivel
                        player.MoverASiguienteNivel(puntoSiguienteNivel.position);
                    }
                }
                else
                {
                    // Opcional: Mensaje en consola si no tiene suficientes objetos
                    Debug.Log("¡Aún te faltan objetos! Llevas: " + player.objetosConsumidos + "/" + objetosRequeridos);
                }
            }
        }
    }
}