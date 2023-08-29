using UnityEngine;

public class FlyBag : MonoBehaviour
{
    Rigidbody m_Kinematic;
    Transform hold_bag;
    Rigidbody m_Rigidbody;
    CharacterController controller;
    Material m_Material1;
    Material m_Material2;
    Material white_screen;

    GameObject walls;
    GameObject star;

    float wallsTransparency = 0f;

    public float m_Thrust = 20f;
    public float m_Speed = 5f;

    public float horizontalSpeed = 2.0F;
    public float verticalSpeed = 2.0F;

    public int absorbedClouds = 0;

    public bool isDisabled = false;

    public float shine = 0f;

    public float endTimer = 0f;

    public bool isGoingNextLevel = false;

    float white_screen_transparency = 0f;
    float levelChangeTime = 3f;

    private float levelDistance = 0f;
    private float nextPosition = 0f;
    float currentDamage = 0f;
    float nextDamage = 0f;
    float elapsed = 0.0f;

    AudioSource startSong;
    AudioSource endSong;
    AudioSource bagSound;


    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        m_Kinematic = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        hold_bag = this.gameObject.transform.GetChild(1);
        m_Rigidbody = hold_bag.GetComponent<Rigidbody>();
        m_Material1 = transform.GetChild(1).transform.GetChild(0).GetComponent<Renderer>().material;
        m_Material2 = transform.GetChild(1).transform.GetChild(1).GetComponent<Renderer>().material;
        walls = GameObject.Find("Walls");
        star = GameObject.Find("star");
        white_screen = transform.GetChild(0).transform.GetChild(0).GetComponent<Renderer>().material;
        bagSound = GameObject.Find("bag_sound").GetComponent<AudioSource>();
        startSong = GameObject.Find("start_song").GetComponent<AudioSource>();
        endSong = GameObject.Find("end_song").GetComponent<AudioSource>();
    }

    public void triggerLevelChange()
    {
        isGoingNextLevel = true;
        levelDistance = absorbedClouds * 200f - transform.position.y;
        nextPosition = transform.position.y + levelDistance;
        currentDamage = m_Material1.GetFloat("_damage");
        nextDamage = currentDamage + 0.6f;
    }

    public void handleNextLevel()
    {
        GetComponent<CharacterController>().Move(Vector3.up * Time.deltaTime * levelDistance / levelChangeTime);

        elapsed += Time.deltaTime;
        float damage_middle = Mathf.Lerp(currentDamage, nextDamage, elapsed / levelChangeTime);
        m_Material1.SetFloat("_damage", damage_middle);
        m_Material2.SetFloat("_damage", damage_middle);

        if ((levelDistance > 0 && transform.position.y >= nextPosition) || (levelDistance < 0 && transform.position.y <= nextPosition))
        {
            isGoingNextLevel = false;
            elapsed = 0f;
        }
    }

    void FixedUpdate()
    {
        m_Rigidbody.AddTorque(Random.insideUnitSphere * 20f);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (isGoingNextLevel)
        {
            handleNextLevel();
        }
        if (absorbedClouds < 5 && !isGoingNextLevel)
        {
            Vector3 moveDirection = Vector3.zero;
            if (Input.GetButton("Jump"))
            {
                moveDirection += transform.up;
            }
            if (Input.GetKey("w"))
            {
                moveDirection += transform.forward;
            }
            if (Input.GetKey("a"))
            {
                moveDirection += -transform.right;
            }
            if (Input.GetKey("d"))
            {
                moveDirection += transform.right;
            }
            if (Input.GetKey("s"))
            {
                moveDirection += -transform.forward;
            }
            controller.Move(moveDirection * Time.deltaTime * m_Speed);
        }
        else if (!isGoingNextLevel)
        {
            endTimer += Time.deltaTime;
            controller.Move(Vector3.up * Time.deltaTime * m_Speed);
            currentDamage = m_Material1.GetFloat("_damage");
            if (startSong.volume > 0f)
            {
                startSong.volume -= Time.deltaTime * 0.1f;
                bagSound.volume -= Time.deltaTime * 0.01f;
            }
            if (currentDamage < 20f)
            {
                m_Material1.SetFloat("_damage", currentDamage + 1f * Time.deltaTime);
                m_Material2.SetFloat("_damage", currentDamage + 1f * Time.deltaTime);
            }
            else
            {
                if (endSong.volume < 0.8f)
                {
                    endSong.volume += Time.deltaTime * 0.1f;
                }
                if (wallsTransparency < 1f)
                {
                    wallsTransparency += Time.deltaTime * 0.01f;
                    Transform w_Transform = walls.transform;
                    for (int i = 0; i < w_Transform.childCount; i++)
                    {
                        w_Transform.GetChild(i).GetComponent<Renderer>().material.SetFloat("_transparecy", wallsTransparency);
                    }
                    Transform s_Transform = star.transform;
                    for (int i = 0; i < s_Transform.childCount; i++)
                    {
                        ParticleSystem.MainModule star_main = s_Transform.GetChild(i).GetComponent<ParticleSystem>().main;
                        star_main.startColor = new Color(1f, 1f, 1f, wallsTransparency);
                    }
                }
                if (shine < 1f)
                {
                    shine += Time.deltaTime * 0.01f;
                    m_Material1.SetFloat("_shine", shine);
                    m_Material2.SetFloat("_shine", shine);
                }
                if (white_screen_transparency < 1f && endTimer > 40f)
                {
                    white_screen_transparency = white_screen_transparency + Time.deltaTime * 0.035f;
                    white_screen.SetFloat("_transparency", white_screen_transparency);
                }
                star.transform.position = transform.position + 100f * Vector3.up;
                if (endTimer > 66f)
                {
                    Application.Quit();
                }
            }
        }


        float h = horizontalSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
        float v = verticalSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime;

        transform.Rotate(-v, h, 0);
    }
}