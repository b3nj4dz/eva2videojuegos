using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicio : MonoBehaviour
{
    // Nombre exacto de tu escena principal de juego
    public string nombreEscenaJuego = "SampleScene"; 

    public void Jugar()
    {
        // Asegura que el tiempo no esté pausado
        Time.timeScale = 1f; 
        
        // Carga la escena del juego
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit(); // Cierra el juego (funciona en la Build compilada)
    }
}