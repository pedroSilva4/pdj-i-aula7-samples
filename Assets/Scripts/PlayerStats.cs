using System.Collections;
using UnityEngine;
 
/// <summary>
/// Estatísticas do player: velocidade base e power-ups temporários.
/// Usa Coroutine para efectos com duração definida.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("Velocidade")]
    [SerializeField] public float baseSpeed = 5f;
    [SerializeField] private float baseJumpForce  = 4f;
    [SerializeField] private float baseRotationSpeed  = 10f;


    
    public float Speed => currentSpeed;
    public float JumpForce => baseJumpForce;
    public float RotationSpeed => baseRotationSpeed;

    public float currentSpeed { get; private set; }
 
     void Start()
     {
         currentSpeed = baseSpeed;
     }
 
    // ── Estado interno ────────────────────────────────────────────
    // Guardar a referência permite cancelar a coroutine se o player
    // apanhar um novo power-up antes do anterior terminar.
    private Coroutine _activeBoost;
 
    // ── API pública ───────────────────────────────────────────────
    /// <summary>
    /// Aplica speed boost temporário. Cancela boost anterior se activo.
    /// Chamado por PowerUpPickup.OnCollected().
    /// </summary>
    public void ApplySpeedBoost(float multiplier, float duration)
    {
        // Cancela boost anterior para evitar dois boosts simultâneos.
        // Sem isto: dois power-ups → duas coroutines → velocidade
        // nunca reposta correctamente.
        if (_activeBoost != null)
            StopCoroutine(_activeBoost);
 
        // Inicia nova coroutine e guarda referência.
        _activeBoost = StartCoroutine(
            SpeedBoostRoutine(multiplier, duration));
 }
 
    // ── Coroutine ────────────────────────────────────────────────
    // IEnumerator: tipo de retorno obrigatório para coroutines.
    // yield return: pausa a coroutine sem bloquear o resto do jogo.
    private IEnumerator SpeedBoostRoutine(float mult, float dur)
    {
        
 
        // Aplica velocidade multiplicada.
        currentSpeed = baseSpeed * mult;
 
        // Pausa a coroutine 'dur' segundos.
        // O resto do jogo continua a correr normalmente.
        yield return new WaitForSeconds(dur);
 
        // Repõe velocidade base após o tempo expirar.
        currentSpeed = baseSpeed;
        _activeBoost = null;
    }
}
