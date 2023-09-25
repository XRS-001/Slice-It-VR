using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class GameManager : MonoBehaviour
{
    public GameObject sceneAudio;
    public AudioClip waveAudio;
    public GameObject waveElapsedMenu;
    public GameObject waveEndMenu;
    public int score;
    public Transform[] cannons;
    public GameObject[] sliceableObjectsPrefabs;  // Prefab for the sliceable object
    public Image startFadeImage;
    public GameObject physicsRig;
    public XRInteractorLineVisual leftRay;
    public XRInteractorLineVisual rightRay;
    private float spawnInterval;

    private bool isWaveActive = false;
    private float waveDuration = 120f;  // Wave duration in seconds (1 minute)
    private float objectSpeed = 13f;   // Speed at which sliceable objects are shot out

    private AudioSource waveAudioSource; // Reference to the AudioSource component of the waveAudio

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(leftRay.reticle);
        DontDestroyOnLoad(rightRay.reticle);
        DontDestroyOnLoad(GameObject.Find("XR Interaction Manager"));
        DontDestroyOnLoad(this.gameObject);

        StartCoroutine(FadeImage());
    }

    public void SpawnItem(GameObject grabbedObject)
    {
        Instantiate(grabbedObject);
    }

    public void LoadNewScene(string sceneName)
    {
        // Load the new scene
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        physicsRig.GetComponent<PhysicsRig>().leftController.GetComponent<ControllerInteractors>().allowSelect = false;
        physicsRig.GetComponent<PhysicsRig>().rightController.GetComponent<ControllerInteractors>().allowSelect = false;
        physicsRig.GetComponent<PhysicsRig>().leftController.GetComponent<ControllerInteractors>().allowSelect = true;
        physicsRig.GetComponent<PhysicsRig>().rightController.GetComponent<ControllerInteractors>().allowSelect = true;
        if (sceneName == "Menu")
        {
            Destroy(GameObject.Find("PhysicsRig"));
            Destroy(GameObject.Find("GameManager"));
        }
        StartCoroutine(FadeImage());
    }

    IEnumerator FadeImage()
    {
        // Set the image's color to opaque
        startFadeImage.color = new Color(startFadeImage.color.r, startFadeImage.color.g, startFadeImage.color.b, 1f);

        // Wait for a short delay
        yield return new WaitForSeconds(0.5f);

        // Gradually decrease the alpha value of the image's color over time
        float fadeDuration = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            startFadeImage.color = new Color(startFadeImage.color.r, startFadeImage.color.g, startFadeImage.color.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        physicsRig.SetActive(false);
        physicsRig.SetActive(true);
    }

    public void StartWave(string selectedDifficulty, GameObject cannonParent)
    {
        if (!isWaveActive)
        {
            score = 0;
            StartCoroutine(SpawnWave(selectedDifficulty, cannonParent));
        }
    }

    IEnumerator SpawnWave(string selectedDifficulty, GameObject cannonParent)
    {
        // Get the AudioSource component of the waveAudio GameObject
        waveAudioSource = sceneAudio.GetComponent<AudioSource>();
        // Start playing the wave audio
        waveAudioSource.PlayOneShot(waveAudio, 0.2f);

        Debug.Log("Starting Wave");
        isWaveActive = true;
        float elapsedTime = 0f;

        cannons = cannonParent.transform.GetComponentsInChildren<Transform>();
        while (elapsedTime < waveDuration)
        {
            
            // Spawn a sliceable object at a random cannon
            int cannonIndex = Random.Range(1, cannons.Length);
            Transform cannon = cannons[cannonIndex];
            AudioSource cannonAudio = cannon.transform.GetComponent<AudioSource>();
            cannonAudio.Play();
            Vector3 spawnPosition = cannon.transform.position;

            // Calculate the direction towards the physicsRig
            Vector3 direction = (physicsRig.GetComponent<PhysicsRig>().playerHead.transform.position - spawnPosition).normalized + Vector3.up * 0.25f;

            GameObject newSliceableObject = Instantiate(sliceableObjectsPrefabs[Random.Range(0, sliceableObjectsPrefabs.Length)], spawnPosition, Quaternion.identity);
            Destroy(newSliceableObject, 5);

            // Apply force towards the physicsRig
            Rigidbody rb = newSliceableObject.GetComponent<Rigidbody>();
            rb.AddForce(direction * objectSpeed, ForceMode.Impulse);

            // Apply random torque
            Vector3 torque = Random.onUnitSphere * objectSpeed;
            rb.AddTorque(torque, ForceMode.Impulse);

            // Wait for a short interval before spawning the next object
            if (selectedDifficulty == "Easy")
            {
                spawnInterval = 2f;
            }
            if (selectedDifficulty == "Medium")
            {
                spawnInterval = 1.333f;
            }
            if (selectedDifficulty == "Hard")
            {
                spawnInterval = 0.666f;
            }
            yield return new WaitForSeconds(spawnInterval);

            elapsedTime += spawnInterval;
        }

        // Gradually fade out the wave audio
        StartCoroutine(FadeOutWaveAudio());

        // End the wave
        isWaveActive = false;
        waveElapsedMenu.SetActive(false);
        waveEndMenu.SetActive(true);
    }

    IEnumerator FadeOutWaveAudio()
    {
        // Gradually decrease the volume of the wave audio over time
        float fadeDuration = 1f;
        float startVolume = waveAudioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
            waveAudioSource.volume = volume;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Stop playing the wave audio
        waveAudioSource.Stop();
    }

    private void Update()
    {
        sceneAudio = GameObject.Find("SceneAudio");
        waveElapsedMenu = GameObject.Find("WaveElapsedMenu") ?? null;
        waveEndMenu = GameObject.Find("WaveOverMenu")?.GetComponentsInChildren<Transform>(true)[1].gameObject;
    }
}



