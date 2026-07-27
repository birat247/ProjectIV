using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace TravisGameAssets
{
    public class HitImpactEffectsPreview : MonoBehaviour
    {
        [Header("Hit Effect Setup")]
        public Collider floorCollider;
        public Transform particlesPool;

        [Header("UI")]
        public Text hitNameLabel;
        public Text hitIndexLabel;

        [Header("Camera")]
        public Transform cameraPivot;
        public float cameraRotationSpeed = 10f;

        [Header("Floor")]
        public MeshRenderer floor;

        [Header("Icons")]
        public Image rotationIcon;
        public Image floorIcon;
        public Image slowMotionIcon;

        [Header("Lighting")]
        public GameObject sceneLight;

        [Header("Camera Zoom")]
        public float minFov = 15f;
        public float maxFov = 90f;
        public float sensitivity = 10f;

        private GameObject[] hitEffects;
        private int hitIndex;

        private Vector3 initialCameraPosition;
        private Quaternion initialCameraRotation;
        private float initialFov;

        private bool cameraRotating;
        private bool floorVisible;
        private bool slowMotion;
        private bool lighting;

        private Camera mainCamera;

        // ============================================================
        // START
        // ============================================================

        private void Start()
        {
            hitIndex = 0;

            cameraRotating = false;
            floorVisible = true;
            slowMotion = false;
            lighting = true;

            mainCamera = Camera.main;

            if (mainCamera == null)
            {
                Debug.LogError(
                    "HitImpactEffectsPreview: Main Camera was not found."
                );

                return;
            }

            initialCameraPosition =
                mainCamera.transform.position;

            initialCameraRotation =
                mainCamera.transform.rotation;

            initialFov =
                mainCamera.fieldOfView;

            // --------------------------------------------------------
            // PARTICLE POOL
            // --------------------------------------------------------

            if (particlesPool == null)
            {
                Debug.LogError(
                    "HitImpactEffectsPreview: Particles Pool is not assigned."
                );

                hitEffects = new GameObject[0];
                return;
            }

            hitEffects =
                new GameObject[particlesPool.childCount];

            for (int i = 0;
                 i < particlesPool.childCount;
                 i++)
            {
                hitEffects[i] =
                    particlesPool.GetChild(i).gameObject;
            }

            RefreshHitUI();
        }

        // ============================================================
        // UPDATE
        // ============================================================

        private void Update()
        {
            if (mainCamera == null)
                return;

            HandleKeyboardInput();
            HandleMouseInput();
            HandleCameraRotation();
        }

        // ============================================================
        // KEYBOARD INPUT - NEW INPUT SYSTEM
        // ============================================================

        private void HandleKeyboardInput()
        {
            Keyboard keyboard = Keyboard.current;

            if (keyboard == null)
                return;

            // A = Previous
            if (keyboard.aKey.wasPressedThisFrame)
            {
                PreviousHit();
            }

            // Left Arrow = Previous
            if (keyboard.leftArrowKey.wasPressedThisFrame)
            {
                PreviousHit();
            }

            // D = Next
            if (keyboard.dKey.wasPressedThisFrame)
            {
                NextHit();
            }

            // Right Arrow = Next
            if (keyboard.rightArrowKey.wasPressedThisFrame)
            {
                NextHit();
            }

            // 1 = Rotation
            if (keyboard.digit1Key.wasPressedThisFrame)
            {
                ToggleRotation();
            }

            // 2 = Floor
            if (keyboard.digit2Key.wasPressedThisFrame)
            {
                ToggleFloor();
            }

            // 3 = Slow motion
            if (keyboard.digit3Key.wasPressedThisFrame)
            {
                ToggleSlowMotion();
            }

            // Space = Lighting
            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                ToggleLighting();
            }
        }

        // ============================================================
        // MOUSE INPUT - NEW INPUT SYSTEM
        // ============================================================

        private void HandleMouseInput()
        {
            Mouse mouse = Mouse.current;

            if (mouse == null)
                return;

            // Left mouse = spawn hit
            if (mouse.leftButton.wasPressedThisFrame)
            {
                SpawnHitFromMouse(mouse);
            }

            // Right mouse = reset camera
            if (mouse.rightButton.wasPressedThisFrame)
            {
                ResetCamera();
            }

            // Middle mouse = reset FOV
            if (mouse.middleButton.wasPressedThisFrame)
            {
                ResetFOV();
            }

            // Mouse wheel
            Vector2 scroll =
                mouse.scroll.ReadValue();

            if (Mathf.Abs(scroll.y) > 0.01f)
            {
                float fov =
                    mainCamera.fieldOfView;

                fov -=
                    (scroll.y / 120f) *
                    sensitivity;

                fov =
                    Mathf.Clamp(
                        fov,
                        minFov,
                        maxFov
                    );

                mainCamera.fieldOfView =
                    fov;
            }
        }

        // ============================================================
        // SPAWN HIT USING MOUSE
        // ============================================================

        private void SpawnHitFromMouse(Mouse mouse)
        {
            if (floorCollider == null)
                return;

            if (hitEffects == null ||
                hitEffects.Length == 0)
                return;

            // Don't spawn if clicking UI
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Vector2 mousePosition =
                mouse.position.ReadValue();

            Ray ray =
                mainCamera.ScreenPointToRay(
                    mousePosition
                );

            RaycastHit hit;

            if (floorCollider.Raycast(
                    ray,
                    out hit,
                    1000f))
            {
                GameObject newHit =
                    SpawnHit();

                if (newHit != null)
                {
                    newHit.transform.position =
                        hit.point;
                }
            }
        }

        // ============================================================
        // CAMERA ROTATION
        // ============================================================

        private void HandleCameraRotation()
        {
            if (!cameraRotating)
                return;

            if (cameraPivot == null)
                return;

            cameraPivot.Rotate(
                Vector3.up *
                cameraRotationSpeed *
                Time.deltaTime
            );
        }

        // ============================================================
        // RESET CAMERA
        // ============================================================

        private void ResetCamera()
        {
            if (mainCamera == null)
                return;

            mainCamera.transform.position =
                initialCameraPosition;

            mainCamera.transform.rotation =
                initialCameraRotation;

            if (cameraRotating)
            {
                ToggleRotation();
            }
        }

        // ============================================================
        // RESET FOV
        // ============================================================

        private void ResetFOV()
        {
            if (mainCamera == null)
                return;

            mainCamera.fieldOfView =
                initialFov;
        }

        // ============================================================
        // TOGGLE ROTATION
        // ============================================================

        public void ToggleRotation()
        {
            cameraRotating =
                !cameraRotating;

            if (rotationIcon != null)
            {
                Color color =
                    rotationIcon.color;

                color.a =
                    cameraRotating
                        ? 1f
                        : 0.33f;

                rotationIcon.color =
                    color;
            }
        }

        // ============================================================
        // TOGGLE FLOOR
        // ============================================================

        public void ToggleFloor()
        {
            floorVisible =
                !floorVisible;

            if (floor != null)
            {
                floor.enabled =
                    floorVisible;
            }

            if (floorIcon != null)
            {
                Color color =
                    floorIcon.color;

                color.a =
                    floorVisible
                        ? 1f
                        : 0.33f;

                floorIcon.color =
                    color;
            }
        }

        // ============================================================
        // TOGGLE SLOW MOTION
        // ============================================================

        public void ToggleSlowMotion()
        {
            slowMotion =
                !slowMotion;

            Time.timeScale =
                slowMotion
                    ? 0.5f
                    : 1f;

            if (slowMotionIcon != null)
            {
                Color color =
                    slowMotionIcon.color;

                color.a =
                    slowMotion
                        ? 1f
                        : 0.33f;

                slowMotionIcon.color =
                    color;
            }
        }

        // ============================================================
        // TOGGLE LIGHTING
        // ============================================================

        public void ToggleLighting()
        {
            lighting =
                !lighting;

            if (sceneLight != null)
            {
                sceneLight.SetActive(
                    lighting
                );
            }
        }

        // ============================================================
        // NEXT HIT
        // ============================================================

        public void NextHit()
        {
            if (hitEffects == null ||
                hitEffects.Length == 0)
                return;

            hitIndex++;

            if (hitIndex >= hitEffects.Length)
            {
                hitIndex = 0;
            }

            RefreshHitUI();
        }

        // ============================================================
        // PREVIOUS HIT
        // ============================================================

        public void PreviousHit()
        {
            if (hitEffects == null ||
                hitEffects.Length == 0)
                return;

            hitIndex--;

            if (hitIndex < 0)
            {
                hitIndex =
                    hitEffects.Length - 1;
            }

            RefreshHitUI();
        }

        // ============================================================
        // REFRESH UI
        // ============================================================

        private void RefreshHitUI()
        {
            if (hitEffects == null ||
                hitEffects.Length == 0)
            {
                if (hitNameLabel != null)
                {
                    hitNameLabel.text =
                        "No Hit Effects";
                }

                if (hitIndexLabel != null)
                {
                    hitIndexLabel.text =
                        "00/00";
                }

                return;
            }

            if (hitIndex < 0 ||
                hitIndex >= hitEffects.Length)
            {
                hitIndex = 0;
            }

            if (hitEffects[hitIndex] != null)
            {
                if (hitNameLabel != null)
                {
                    hitNameLabel.text =
                        hitEffects[hitIndex].name;
                }
            }

            if (hitIndexLabel != null)
            {
                hitIndexLabel.text =
                    string.Format(
                        "{0}/{1}",
                        (hitIndex + 1).ToString("00"),
                        hitEffects.Length.ToString("00")
                    );
            }
        }

        // ============================================================
        // SPAWN HIT
        // ============================================================

        private GameObject SpawnHit()
        {
            if (hitEffects == null ||
                hitEffects.Length == 0)
                return null;

            if (hitIndex < 0 ||
                hitIndex >= hitEffects.Length)
                return null;

            if (hitEffects[hitIndex] == null)
                return null;

            GameObject spawnedHit =
                Instantiate(
                    hitEffects[hitIndex]
                );

            if (mainCamera != null)
            {
                spawnedHit.transform.LookAt(
                    mainCamera.transform
                );
            }

            return spawnedHit;
        }

        // ============================================================
        // CLEANUP
        // ============================================================

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}