using UnityEngine;

public class RunningCloud : MonoBehaviour
{

  public float speed = 10f;

  private bool isChangingSkybox = false;

  float levelChangeTime = 3f;
  float elapsed = 0.0f;

  float currentSkyboxValue = 0f;
  float nextSkyboxValue = 0f;

  Rigidbody m_Rigidbody;
  GameObject player;
  Rigidbody player_Rigidbody;

  void checkLevelChange()
  {
    if (isChangingSkybox)
    {
      if (elapsed < levelChangeTime)
      {
        float v_middle = Mathf.Lerp(currentSkyboxValue, nextSkyboxValue, elapsed / levelChangeTime);
        RenderSettings.skybox.SetFloat("_Height", v_middle);
        elapsed += Time.deltaTime;
      }
      else
      {
        elapsed = 0f;
        isChangingSkybox = false;
        Object.Destroy(this.gameObject);
      }
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.name.Contains("wind_zone"))
    {
      speed -= other.GetComponent<WindScript>().windForce;
    }
    if (other.name.Contains("kinematic_bag"))
    {
      int absorbedClouds = player.GetComponent<FlyBag>().absorbedClouds;
      isChangingSkybox = true;
      currentSkyboxValue = RenderSettings.skybox.GetFloat("_Height");
      nextSkyboxValue = currentSkyboxValue + 0.2f;
      player.GetComponent<FlyBag>().absorbedClouds = absorbedClouds + 1;
      player.GetComponent<FlyBag>().triggerLevelChange();
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.name == "wind_zone")
    {
      speed += other.GetComponent<WindScript>().windForce;
    }
  }

  void Start()
  {
    m_Rigidbody = GetComponent<Rigidbody>();
    player = GameObject.Find("kinematic_bag");
    player_Rigidbody = player.GetComponent<Rigidbody>();


    RenderSettings.skybox.SetFloat("_Height", 0f);
  }

  void FixedUpdate()
  {
    checkLevelChange();
    Vector3 heading = m_Rigidbody.position - player_Rigidbody.position;
    float distance = heading.magnitude;
    Vector3 direction = heading / distance;
    if (distance < 5f)
    {
      m_Rigidbody.MovePosition(transform.position + direction * Time.deltaTime * speed);
    }
  }
}