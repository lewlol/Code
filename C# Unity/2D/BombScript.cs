using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Rendering.Universal;

public class BombScript : MonoBehaviour
{
    public GameObject bombPrefab;
    public float bombForce = 5.0f;
    public float bombRadius = 2.0f;
    public float bombTimer = 2.0f;

    public MeshTextAppear mta;
    public TextMeshProUGUI bombCount;

    public ParticleSystem particles;

    private float lightwaittime = 0.02f;
    private float lightIncrease = 0.5f;
    private float lightIntensityFallOff = 0.1f;

    private bool canThrowBomb = true; // Add this variable
    public float throwCooldown = 3.0f; // Add this variable

    private Vector3 originalCameraPosition;
    public float shakeMagnitude = 0.1f;
    public float shakeDuration = 0.2f;

    private void Start()
    {
        shakeMagnitude = 0.2f;
        shakeDuration = 0.5f;
    }

    private void Update()
    {

      


        if (Input.GetMouseButtonDown(1) && canThrowBomb && Inventory.bombs > 0) // Check if the player has bombs and cooldown is over
        {
            ThrowBomb();
        }
        bombCount.text = Inventory.bombs.ToString();
    }

    private void ThrowBomb()
    {
        canThrowBomb = false; // Disable bomb throwing
        Inventory.bombs--; // Decrement the bomb count

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0.0f;

        Vector3 throwDirection = (mousePosition - transform.position).normalized;

        GameObject bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();

        bombRb.velocity = throwDirection * bombForce;

        StartCoroutine(ExplodeAfterDelay(bomb));    
    }

    private IEnumerator ExplodeAfterDelay(GameObject bomb)
    {
        yield return new WaitForSeconds(bombTimer);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(bomb.transform.position, bombRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Block"))
            {
                CollectAndDestroyBlock(collider.gameObject);
                mta.GenerateText(collider.gameObject.transform.position, 2f, "+1 " + collider.gameObject.GetComponent<TileDataPlaceholder>().thisTile.name, 30);
            }

            if(collider.gameObject.tag == "Player")
            {
                collider.gameObject.GetComponent<PlayerMovement>().TakeDamage(15);
            }
        }
        originalCameraPosition = Camera.main.transform.position;
        StartCoroutine(ScreenShake());

        bomb.GetComponent<CircleCollider2D>().enabled = false;
        bomb.GetComponent<SpriteRenderer>().enabled = false;
        Light2D bLight = bomb.transform.GetChild(0).GetComponent<Light2D>();
        StartCoroutine(LightIncrease(bLight));
        bLight.transform.SetParent(null);
        bomb.GetComponentInChildren<Light2D>().enabled = false;
        particles = bomb.GetComponentInChildren<ParticleSystem>();
        particles.Play();
        particles.transform.parent = null;
        bomb.GetComponentInChildren<AudioSource>().Play();
        yield return new WaitForSeconds(throwCooldown);// Add this line
        Destroy(bomb);
        Destroy(particles);
        canThrowBomb = true; // Re-enable bomb throwing
    }

    private void CollectAndDestroyBlock(GameObject block)
    {
        TileDataPlaceholder tdp = block.GetComponent<TileDataPlaceholder>();
        int coinWorth = tdp.thisTile.coinWorth;

        int amountToDrop = 1; // Default amount dropped

        if (Random.value <= 0.05f) // 5% chance for double drop
        {
            amountToDrop = 2; // Double drop
        }

        // Determine the type of material and add it to the inventory
        switch (tdp.thisTile.tileType)
        {
            case TileTypes.Rock:
                Inventory.Rocks += amountToDrop;
                EndSCene.Totalrocks += amountToDrop;
                Inventory.Rockcoins += coinWorth * amountToDrop;
                break;
            case TileTypes.Ore:
                Inventory.Ores += amountToDrop;
                EndSCene.TotalOres += amountToDrop;
                Inventory.Orescoins += coinWorth * amountToDrop;
                break;
            case TileTypes.Gemstone:
                Inventory.Gemstones += amountToDrop;
                EndSCene.TotalGems += amountToDrop;
                Inventory.Gemstonecoins += coinWorth * amountToDrop;
                break;
                // Add more cases for other tile types if needed
        }

        Inventory.Totalcoins += coinWorth * amountToDrop; // Add coins to the total
        EndSCene.Totalcoins += coinWorth * amountToDrop;

        // Destroy the block
        Destroy(block);
    }
    private IEnumerator ScreenShake()
    {
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            Vector3 cameraShake = Random.insideUnitCircle * shakeMagnitude;

            Camera.main.transform.position = originalCameraPosition + new Vector3(cameraShake.x, cameraShake.y, 0.0f);

            elapsed += Time.deltaTime;

            yield return null;
        }

        Camera.main.transform.position = originalCameraPosition;
    }

    IEnumerator LightIncrease(Light2D explosionLight)
    {
        explosionLight.pointLightOuterRadius += lightIncrease;
        yield return new WaitForSeconds(lightwaittime);
        explosionLight.pointLightOuterRadius += lightIncrease;
        yield return new WaitForSeconds(lightwaittime);
        explosionLight.pointLightOuterRadius += lightIncrease;
        yield return new WaitForSeconds(lightwaittime);
        explosionLight.pointLightOuterRadius += lightIncrease;
        explosionLight.pointLightInnerRadius += lightIncrease;
        explosionLight.falloffIntensity += lightIntensityFallOff;
        yield return new WaitForSeconds(lightwaittime);
        explosionLight.pointLightOuterRadius += lightIncrease;
        explosionLight.pointLightInnerRadius += lightIncrease;
        explosionLight.falloffIntensity += lightIntensityFallOff;
        yield return new WaitForSeconds(lightwaittime);
        explosionLight.pointLightOuterRadius += lightIncrease;
        explosionLight.pointLightInnerRadius += lightIncrease;
        explosionLight.falloffIntensity += lightIntensityFallOff;
        yield return new WaitForSeconds(lightwaittime);
        explosionLight.pointLightOuterRadius += lightIncrease;
        explosionLight.pointLightInnerRadius += lightIncrease;
        explosionLight.falloffIntensity += lightIntensityFallOff;
        yield return new WaitForSeconds(lightwaittime);
        explosionLight.pointLightOuterRadius = 0f;
    }
}