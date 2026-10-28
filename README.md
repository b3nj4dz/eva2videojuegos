## Nombre de juego: Pokerun

## Nombre estudiante: Benjamín Canales

## Decisiones Técnicas

* **Plataforma:** Desarrollado en **Unity 2D** optimizado para PC (Desktop), estructurado en una única escena para los niveles del juego con cargas mediante `SceneManager` para el menú principal.
* **Pipeline de Desarrollo:**
  * **Estructura Vertical de Niveles:** Diseño de mapas apilados verticalmente donde el paso entre niveles se gestiona mediante triggers invisibles (`ZonaTransicion`).
  * **Control de Flujo:** Requisito de recolección de 10 Pokéballs por nivel (30 en total) para habilitar el teletransporte y la condición de victoria final.
  * **UI e Interfaz:** Paneles interactivos de Menú Principal, Pausa (tecla `ESC` / `P`), Victoria y Derrota con congelamiento de tiempo mediante `Time.timeScale`.
* **Físicas:**
  * Control del jugador mediante `Rigidbody2D` para el movimiento horizontal y saltos basados en impulsos de física 2D.
  * Detección de suelo precisa utilizando **Raycast2D** (`Physics2D.Raycast`) hacia abajo con una capa de suelo dedicada (`LayerMask`).
* **Audio:**
  * Sistema de triple canal `AudioSource` independiente:
    * **Música de fondo:** Pista continua en bucle con pausa/despausa automática en menús de pausa y detención en pantallas finales.
    * **Efectos de sonido (SFX):** Reproducción puntual de saltos, daño, recolección de Pokéballs, teletransporte (`PlayOneShot`) y victoria/derrota.
    * **Pasos:** Canal en bucle exclusivo sincronizado con el movimiento horizontal y estado de contacto con el suelo (`isGrounded`).

---

## Riesgos y Mitigaciones

| Riesgo | Impacto | Mitigación Implementada |
| :--- | :--- | :--- |
| **Colisión defectuosa o traspaso de plataformas** | Alto | Implementación de detección de suelo por Raycast2D ajustando la distancia (`groundCheckDistance`) y separando la física en `FixedUpdate`. |
| **Pérdida de foco del fondo al mover la cámara** | Medio | Emparentado directo del sprite de fondo a la `Main Camera` (o configuración en *Screen Space - Camera*) fijando la distancia en el eje Z. |
| **Mezcla desordenada de audios en eventos simultáneos** | Medio | Separación de las fuentes de sonido en 3 componentes `AudioSource` independientes (Música, Efectos y Pasos). |
| **Transición no deseada entre niveles sin recolectar objetos** | Alto | Validación lógica estricta en `ZonaTransicion.cs` comprobando que `objetosConsumidos` cumpla con la cuota requerida antes de permitir el teletransporte. |
| **Bloqueo del juego por congelamiento de tiempo (`Time.timeScale = 0`)** | Alto | Reestablecimiento explícito de `Time.timeScale = 1f` en todos los métodos de inicio, reinicio y retorno al menú principal en `PlayerController.cs`. |
