using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float mainThrust = 1000f;
    [SerializeField] float rcsThrust = 200f;
    [SerializeField] float loadLevelDelay = 1f;
    [SerializeField] AudioClip deathAudio;
    [SerializeField] AudioClip explodeAudio;
    [SerializeField] AudioClip thrustAudio;
    [SerializeField] AudioClip newLevelSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;


    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State {Alive, Dying, Trancending};
    State state = State.Alive;

    bool collisionsDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if(Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }

    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            LoadNextLevel();
        }
        else if(Input.GetKeyDown(KeyCode.C))
        {
            //toggle collisons
            collisionsDisabled = !collisionsDisabled;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive || collisionsDisabled) {return;}

        switch(collision.gameObject.tag)
        {
            case "Friendly":
                //do nothing
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.PlayOneShot(deathAudio);
        Invoke("ExplodeRocket", 2.8f);
        Invoke("LoadSameLevel", 3f);
    }

    private void ExplodeRocket()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.PlayOneShot(explodeAudio);
        deathParticles.Play();
    }

    private void StartSuccessSequence()
    {
        state = State.Trancending;
        audioSource.PlayOneShot(newLevelSound);
        successParticles.Play();
        Invoke("LoadNextLevel", loadLevelDelay);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadSameLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (currentSceneIndex == SceneManager.sceneCountInBuildSettings -1)
        {
            nextSceneIndex = 0;
        }

        SceneManager.LoadScene(nextSceneIndex);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))//Can thrust while rotating
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(thrustAudio);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; //Ignore game physics while rotating

        float rotationThisFrame = rcsThrust * Time.deltaTime;
        //Cannot rotate both ways at the same time
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-(Vector3.forward) * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; //Resume Physics
    }

}
