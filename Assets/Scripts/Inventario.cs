using System.Collections.Generic;
using UnityEngine;

public class Inventario : MonoBehaviour
{
    public static Inventario instance; // Para acceder al inventario desde cualquier lugar
    private List<GameObject> objetosGuardados = new List<GameObject>();

    void Awake()
    {
        instance = this;
    }

    public void AgregarObjeto(GameObject objeto)
    {
        objetosGuardados.Add(objeto);
    }

    // Función para obtener los objetos guardados
    public List<GameObject> ObtenerObjetos()
    {
        return objetosGuardados;
    }
}