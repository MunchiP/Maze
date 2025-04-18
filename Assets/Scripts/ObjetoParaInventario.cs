using UnityEngine;

public class ObjetoParaInventario : MonoBehaviour
{
    public string nombreObjeto; // Nombre del objeto para identificarlo en el inventario

    // Método para ser llamado cuando el objeto es guardado
    public void GuardarObjeto()
    {
        Debug.Log("Objeto guardado: " + nombreObjeto);
        // Aquí puedes añadir el objeto al inventario
        // Si quieres usar un inventario que ya has creado, aquí lo agregas
        Inventario.instance.AgregarObjeto(this.gameObject);  // Asegúrate de que 'Inventario' sea un singleton o accesible
    }
}