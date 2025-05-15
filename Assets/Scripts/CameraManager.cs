using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private CinemachineVirtualCamera[] allVirtualCameras;

    [Header("Controls for lerping the y Damping during player jump")]
    [SerializeField] private float fallPanAmount = 0.25f;
    [SerializeField] private float fallYPanTime = 0.35f;
    public float fallSpeedYDampingChangeThreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; private set; }

    private Coroutine lerpYPanCoroutine;

    private CinemachineVirtualCamera currentCamera;
    private CinemachineFramingTransposer framingTransposer;

    private float normYPanAmount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Găsim prima cameră validă cu FramingTransposer
        for (int i = 0; i < allVirtualCameras.Length; i++)
        {
            var cam = allVirtualCameras[i];
            var transposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();

            if (transposer != null)
            {
                currentCamera = cam;
                framingTransposer = transposer;
                break;
            }
        }

        if (framingTransposer == null)
        {
            Debug.LogError("CameraManager: Nu s-a găsit niciun CinemachineVirtualCamera cu un FramingTransposer!");
            return;
        }

        normYPanAmount = framingTransposer.m_YDamping;
    }

    #region Lerp the Y Damping
    public void LerpYDamping(bool isPlayerFalling)
    {
        if (framingTransposer == null) return;

        if (lerpYPanCoroutine != null)
            StopCoroutine(lerpYPanCoroutine);

        lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        float startDampAmount = framingTransposer.m_YDamping;
        float endDampAmount = isPlayerFalling ? fallPanAmount : normYPanAmount;

        LerpedFromPlayerFalling = isPlayerFalling;

        float elapsedTime = 0f;
        while (elapsedTime < fallYPanTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, elapsedTime / fallYPanTime);
            framingTransposer.m_YDamping = lerpedPanAmount;
            yield return null;
        }

        IsLerpingYDamping = false;
    }
    #endregion
}
