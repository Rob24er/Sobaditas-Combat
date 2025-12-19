using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    // Nombre de la escena a cargar
    public string nombreEscena;

    // Método que se llama desde el botón
    public void IrAEscena()
    {
        SceneManager.LoadScene(nombreEscena);
    }
}
