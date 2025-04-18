using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Object_Interact : MonoBehaviour
{
    [Header("Configuración")]
    public PlayerInput playerInput;
    public Transform posicionExamen;
    public float rango = 1.5f;
    public float sensibilidadRotacion = 0.1f;

    [Header("Control del jugador")]
    public MonoBehaviour firstPersonController;

    [Header("UI")]
    public TMP_Text textoInteractuar;
    public GameObject mira;

    private InputAction accionExaminar;
    private GameObject objetoActual;
    private bool examinando;
    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;
    private bool esperandoGuardar;

    void Awake()
    {
        accionExaminar = playerInput.actions["Examinar"];
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        accionExaminar.performed += ctx => OnExaminar();
        accionExaminar.Enable();
    }

    void OnDisable()
    {
        accionExaminar.performed -= ctx => OnExaminar();
        accionExaminar.Disable();
    }

    void Update()
    {
        if (!examinando)
        {
            VerificarObjeto();
        }
        else
        {
            RotarObjeto();
        }

        if (examinando && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelarExamen();
        }
    }

    void VerificarObjeto()
    {
        Ray rayo = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Debug.DrawRay(rayo.origin, rayo.direction * rango, Color.green, 0.1f);
        int layerMask = ~LayerMask.GetMask("Player");

        if (Physics.Raycast(rayo, out hit, rango, layerMask) && hit.collider.CompareTag("Interactuable"))
        {
            objetoActual = hit.collider.gameObject;
            textoInteractuar.text = "Pulsa E para examinar";
            if (mira != null) mira.SetActive(false);
        }
        else
        {
            objetoActual = null;
            textoInteractuar.text = "";
            if (mira != null) mira.SetActive(true);
        }
    }

    void OnExaminar()
    {
        if (objetoActual == null) return;

        if (!examinando)
        {
            // Empezar examen
            posicionOriginal = objetoActual.transform.position;
            rotacionOriginal = objetoActual.transform.rotation;

            objetoActual.transform.position = posicionExamen.position;
            objetoActual.transform.rotation = Quaternion.identity;

            if (firstPersonController != null) firstPersonController.enabled = false;
            if (mira != null) mira.SetActive(false);

            examinando = true;

            if (objetoActual.GetComponent<ObjetoParaInventario>() != null)
            {
                textoInteractuar.text = "Pulsa E para recoger el objeto";
                esperandoGuardar = true;
            }
            else
            {
                textoInteractuar.text = "Pulsa ESC para dejar de examinar";
                esperandoGuardar = false;
            }
        }
        else
        {
            // Si puede ser guardado, lo guarda
            if (esperandoGuardar && objetoActual.GetComponent<ObjetoParaInventario>() != null)
            {
                objetoActual.GetComponent<ObjetoParaInventario>().GuardarObjeto();
                objetoActual.SetActive(false);
                examinando = false;
                esperandoGuardar = false;
                if (firstPersonController != null) firstPersonController.enabled = true;
                if (mira != null) mira.SetActive(true);
                textoInteractuar.text = "";
            }
            // Si no se puede guardar, no hacer nada. Solo con ESC se sale.
        }
    }

    void CancelarExamen()
    {
        if (objetoActual != null)
        {
            objetoActual.transform.position = posicionOriginal;
            objetoActual.transform.rotation = rotacionOriginal;
        }

        examinando = false;
        esperandoGuardar = false;

        if (firstPersonController != null) firstPersonController.enabled = true;
        if (mira != null) mira.SetActive(true);

        textoInteractuar.text = "Pulsa E para examinar";
    }

    void RotarObjeto()
    {
        Vector2 movimientoMouse = Mouse.current.delta.ReadValue();
        objetoActual.transform.Rotate(Vector3.up, -movimientoMouse.x * sensibilidadRotacion, Space.World);
        objetoActual.transform.Rotate(Vector3.right, movimientoMouse.y * sensibilidadRotacion, Space.World);
    }
}
