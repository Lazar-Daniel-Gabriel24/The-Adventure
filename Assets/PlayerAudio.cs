using System.Collections;
using UnityEngine;

public class PlayerAudioAdvanced : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip walkClip;
    public AudioClip jumpClip;
    public AudioClip attackClip;
    public AudioClip hurtClip;
    public AudioClip deathClip;
    public AudioClip pickupClip;
    public AudioClip healClip;
    public AudioClip hitClip;
    [Header("Footstep Settings")]
    public float stepDelay = 0.5f;
    private float stepTimer = 0f;

    [Header("Hurt Clip Segments (in seconds)")]
    public AudioSegment[] hurtSegments;

    private Rigidbody2D rb;
    private bool isGrounded = true;
    private bool isWalking = false;
    private Coroutine segmentCoroutine;
    private bool isDead = false;
    private bool isRolling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isDead) return;

        isWalking = Mathf.Abs(rb.velocity.x) > 0.1f && isGrounded && !isRolling;

        if (isWalking)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayClip(walkClip);
                stepTimer = stepDelay;
            }
        }
    }

    public void PlayJumpSound() => PlayClip(jumpClip);
    public void PlayAttackSound() => PlayClip(attackClip);
    public void PlayPickupSound() => PlayClip(pickupClip);
    public void PlayHealSound() => PlayClip(healClip);

    public void PlayHitSound() => PlayClip(hitClip);
    public void PlayHurtSound()
    {
        if (isDead) return;

        if (hurtClip == null || audioSource == null || hurtSegments.Length == 0)
        {
            PlayClip(hurtClip); // fallback
            return;
        }

        if (segmentCoroutine != null)
            StopCoroutine(segmentCoroutine);

        AudioSegment segment = hurtSegments[Random.Range(0, hurtSegments.Length)];
        segmentCoroutine = StartCoroutine(PlayClipSegment(hurtClip, segment.startTime, segment.duration));
    }

    private IEnumerator PlayClipSegment(AudioClip clip, float startTime, float duration)
    {
        audioSource.clip = clip;
        audioSource.time = startTime;
        audioSource.Play();

        yield return new WaitForSeconds(duration);

        audioSource.Stop();
        segmentCoroutine = null;
    }

    public void PlayDeathSound()
    {
        isDead = true;

        if (segmentCoroutine != null)
        {
            StopCoroutine(segmentCoroutine);
            segmentCoroutine = null;
        }

        audioSource.Stop();
        PlayClip(deathClip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void SetGrounded(bool grounded)
    {
        isGrounded = grounded;
    }

    public void StopAllAudio()
    {
        if (segmentCoroutine != null)
        {
            StopCoroutine(segmentCoroutine);
            segmentCoroutine = null;
        }
        audioSource.Stop();
    }

    public void ResetDeathState()
    {
        isDead = false;
    }

    public void SetRolling(bool rolling)
    {
        isRolling = rolling;
    }
}
