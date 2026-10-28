using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento Horizontal")]
    public float speed = 5f;

    [Header("Contador y Consumibles")]
    public int objetosConsumidos = 0;
    public TextMeshProUGUI textoContador;
    public GameObject panelVictoria;

    [Header("Sistema de Vidas y Derrota")]
    public int vidasMaximas = 3;
    public int vidas = 3;
    public Image[] imagenesCorazones;
    public GameObject panelDerrota;

    [Header("Sistema de Pausa")]
    public GameObject panelPausa;
    private bool juegoPausado = false;

    [Header("Salto y Suelo")]
    public float jumpForce = 4f;
    public float groundCheckDistance = 1.5f;
    public LayerMask groundLayer;
    public bool isGrounded = true;

    [Header("Efectos y Música de Audio")]
    public AudioSource audioSourceEfectos;
    public AudioSource audioSourcePiso;
    public AudioSource audioSourceMusica;
    public AudioClip sonidoSalto;
    public AudioClip sonidoConsumible;
    public AudioClip sonidoDano;
    public AudioClip sonidoVictoria;
    public AudioClip sonidoDerrota;
    public AudioClip sonidoPiso;
    public AudioClip musicaFondo;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;
    private Vector3 puntoDeRespawnNivelActual;
    private bool juegoTerminado = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        puntoDeRespawnNivelActual = transform.position;

        if (panelVictoria != null) panelVictoria.SetActive(false);
        if (panelDerrota != null) panelDerrota.SetActive(false);
        if (panelPausa != null) panelPausa.SetActive(false);

        if (audioSourcePiso != null && sonidoPiso != null)
        {
            audioSourcePiso.clip = sonidoPiso;
            audioSourcePiso.loop = true;
            audioSourcePiso.playOnAwake = false;
        }

        if (audioSourceMusica != null && musicaFondo != null)
        {
            audioSourceMusica.clip = musicaFondo;
            audioSourceMusica.loop = true;
            audioSourceMusica.playOnAwake = true;
            audioSourceMusica.Play();
        }

        ActualizarUI();
    }

    void Update()
    {
        if (!juegoTerminado && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)))
        {
            AlternarPausa();
        }

        if (juegoTerminado || juegoPausado)
        {
            DetenerSonidoPiso();
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");

        bool isMoving = Mathf.Abs(movement.x) > 0;
        if (animator != null)
        {
            animator.SetBool("isRunning", isMoving);
        }

        if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movement.x > 0)
        {
            spriteRenderer.flipX = false;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;

        if (isGrounded && isMoving)
        {
            if (audioSourcePiso != null && !audioSourcePiso.isPlaying)
            {
                audioSourcePiso.Play();
            }
        }
        else
        {
            DetenerSonidoPiso();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            ReproducirSonidoEfecto(sonidoSalto);
        }
    }

    void FixedUpdate()
    {
        if (juegoTerminado || juegoPausado) return;
        rb.linearVelocity = new Vector2(movement.x * speed, rb.linearVelocity.y);
    }

    public void AlternarPausa()
    {
        juegoPausado = !juegoPausado;

        if (panelPausa != null)
        {
            panelPausa.SetActive(juegoPausado);
        }

        if (juegoPausado)
        {
            Time.timeScale = 0f;
            DetenerSonidoPiso();
            if (audioSourceMusica != null) audioSourceMusica.Pause();
        }
        else
        {
            Time.timeScale = 1f;
            if (audioSourceMusica != null) audioSourceMusica.UnPause();
        }
    }

    private void DetenerSonidoPiso()
    {
        if (audioSourcePiso != null && audioSourcePiso.isPlaying)
        {
            audioSourcePiso.Stop();
        }
    }

    private void DetenerMusica()
    {
        if (audioSourceMusica != null && audioSourceMusica.isPlaying)
        {
            audioSourceMusica.Stop();
        }
    }

    public void SumarObjeto()
    {
        objetosConsumidos++;
        ReproducirSonidoEfecto(sonidoConsumible);
        ActualizarUI();
    }

    public void MoverASiguienteNivel(Vector3 nuevaPosicion)
    {
        transform.position = nuevaPosicion;
        puntoDeRespawnNivelActual = nuevaPosicion;
        rb.linearVelocity = Vector2.zero;
    }

    public void AlcanzarMeta()
    {
        if (!juegoTerminado)
        {
            StartCoroutine(GanarJuegoConRetraso());
        }
    }

    private IEnumerator GanarJuegoConRetraso()
    {
        juegoTerminado = true;
        rb.linearVelocity = Vector2.zero;
        DetenerSonidoPiso();
        DetenerMusica();

        yield return new WaitForSeconds(0.4f);

        ReproducirSonidoEfecto(sonidoVictoria);

        yield return new WaitForSeconds(1.2f);

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void RecibirDano()
    {
        if (juegoTerminado) return;

        vidas--;
        ReproducirSonidoEfecto(sonidoDano);
        DetenerSonidoPiso();
        ActualizarUI();

        if (vidas > 0)
        {
            transform.position = puntoDeRespawnNivelActual;
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            StartCoroutine(PerderJuegoConRetraso());
        }
    }

    private IEnumerator PerderJuegoConRetraso()
    {
        juegoTerminado = true;
        rb.linearVelocity = Vector2.zero;
        DetenerSonidoPiso();
        DetenerMusica();

        yield return new WaitForSeconds(0.8f);

        ReproducirSonidoEfecto(sonidoDerrota);

        yield return new WaitForSeconds(1.2f);

        if (panelDerrota != null)
        {
            panelDerrota.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        juegoPausado = false;

        transform.position = puntoDeRespawnNivelActual;
        rb.linearVelocity = Vector2.zero;

        if (panelPausa != null) panelPausa.SetActive(false);

        // Reanudar música
        if (audioSourceMusica != null && musicaFondo != null)
        {
            if (!audioSourceMusica.isPlaying)
            {
                audioSourceMusica.clip = musicaFondo;
                audioSourceMusica.Play();
            }
        }

        ActualizarUI();
    }

    public void ReiniciarJuegoCompleto()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuInicio");
    }

    private void ReproducirSonidoEfecto(AudioClip clip)
    {
        if (audioSourceEfectos != null && clip != null)
        {
            audioSourceEfectos.PlayOneShot(clip);
        }
    }

    private void ActualizarUI()
    {
        if (textoContador != null)
        {
            textoContador.text = "Pokeballs: " + objetosConsumidos;
        }

        if (imagenesCorazones != null)
        {
            for (int i = 0; i < imagenesCorazones.Length; i++)
            {
                if (imagenesCorazones[i] != null)
                {
                    imagenesCorazones[i].enabled = i < vidas;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}