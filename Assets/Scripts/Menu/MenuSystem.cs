using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escenas

public class MenuSystem : MonoBehaviour
{
    // Función para el botón de Jugar
    public void PlayGame()
    {
        // Carga la escena principal del juego. 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Función para el botón de Salir
    public void QuitGame()
    {
        // Como Application.Quit() no hace nada dentro del Editor de Unity,
        // ponemos este mensaje en la consola para confirmar que el botón funciona.
        Debug.Log("Salidendo del juego, adios ");
        Application.Quit();  
    }
}
