using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Image = UnityEngine.UI.Image;

public class PlayerHealthManager : MonoBehaviour
{
    [Header("Health Stuff")]
    public GameObject healthPrefab;
    public int heartAmount = 9;
    public int healTime = 4;
    public Sprite heart;
    public Sprite heartCracked;
    public float voidDamageStartY = -60f;
    public float voidDamageSpeed = 0.25f;

    [Header("Game Over Stuff")]
    
    public int tweenSpeed = 10;
    [Range(0, 255)]
    public int tweenToAlpha = 200;

    #region Private Variables
    int currentHeartAmount;
    int lastHeartAmount;

    GameObject[] hearts;

    Rigidbody rb;

    Vector3 vel;
    float yvel;
    int healthToLoose = 0;

    int cooldown = 0;

    [HideInInspector] public bool alive = true;

    private FirstPersonController _character;
    private GameObject _gameOverGameObject;
    private Transform _healthTransform;
    
    #endregion

    void Start()
    {
        _character = GetComponent<FirstPersonController>();
        _gameOverGameObject = GameManager.Instance.GameOverGameObject;
        _healthTransform = GameManager.Instance.HeartsParent;

        rb = _character.gameObject.GetComponent<Rigidbody>();

        _character.landEvent += OnLand;

        vel = rb.velocity;
        yvel = vel.y;

        currentHeartAmount = heartAmount;
        lastHeartAmount = currentHeartAmount;

        hearts = new GameObject[heartAmount];

        for (int i = 0; i < heartAmount; i++)
        {
            hearts[i] = Instantiate(healthPrefab, _healthTransform);
        }

        StartCoroutine(nameof(MainLoop));
    }

    void FixedUpdate()
    {
        vel = rb.velocity;
        yvel = vel.y;

        if (yvel <= -10.0f && cooldown <= 0)
        {
            cooldown = 30;
            healthToLoose++;
        }

        if (cooldown > 0)
        {
            cooldown--;
        }
    }

    void OnLand()
    {
        for (int i = 0; i < healthToLoose; i++)
        {
            currentHeartAmount--;

            RefreshHearts();
            healthToLoose--;
        }

        if (healthToLoose > 1)
		{
            PlayDamageSound();
            print("OnLand");
        }
    }

    IEnumerator GameOver()
    {
        if (alive)
        {
            alive = false;
            Cursor.lockState = CursorLockMode.None;
            _character.playerCanMove = false;
            _character.cameraCanMove = false;
            _character.enableHeadBob = false;

            yield return new WaitForSeconds(0.5f);
            Image gameOverImage = _gameOverGameObject.GetComponent<Image>();
            gameOverImage.color = new Color32(255, 255, 255, 0);
            _gameOverGameObject.SetActive(true);

            for (int i = 0; i < tweenToAlpha; i += tweenSpeed)
            {
                gameOverImage.color = new Color32(255, 255, 255, (byte)i);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    public void Respawn()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _character.playerCanMove = true;
        _character.cameraCanMove = true;
        _character.enableHeadBob = true;

        Debug.Log("Respawn");
        alive = true;
        RefillHearts();
        currentHeartAmount = heartAmount;
        _gameOverGameObject.SetActive(false);
        StartCoroutine(nameof(MainLoop));

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void RefillHearts()
    {
        foreach (GameObject heartGameObject in hearts)
        {
            heartGameObject.GetComponent<Image>().sprite = heart;
        }
    }

    public void RefreshHearts()
    {
        RefillHearts();

        for (int i = 0; i < heartAmount - currentHeartAmount && i < heartAmount; i++)
        {
            hearts[i].GetComponent<Image>().sprite = heartCracked;
        }

        lastHeartAmount = currentHeartAmount;

        if (currentHeartAmount <= 0)
        {
            StartCoroutine(nameof(GameOver));
        }
    }

	public void PlayDamageSound()
	{
		_character.GetComponent<AudioSource>().Play();
        print("Playing Sound");
	}

	IEnumerator MainLoop()
    {
        IEnumerator voidDamageCoroutine = VoidDamage();
        StartCoroutine(voidDamageCoroutine);

        while (alive)
        {
            if (!(currentHeartAmount >= heartAmount))
            {
                currentHeartAmount++;
                RefreshHearts();
            }

            yield return new WaitForSeconds(healTime);
        }

        StopCoroutine(voidDamageCoroutine);
    }

    IEnumerator VoidDamage()
    {
        while (alive)
        {
            if (_character.transform.position.y <= voidDamageStartY)
            {
                while (currentHeartAmount > 0)
                {
                    currentHeartAmount--;
                    RefreshHearts();
                    PlayDamageSound();

                    yield return new WaitForSeconds(voidDamageSpeed);
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
