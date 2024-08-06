using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class PressurePlateTrap : MonoBehaviour
{
    public GameObject particles;
    public Light2D explosionLight;
    private float lightwaittime = 0.1f;
    private float lightIncrease = 2f;
    public AudioSource explosion;

    public float shakeMagnitude = 0.2f;
    public float shakeDuration = 0.5f;

    private Vector3 originalCameraPosition;

    private void Start()
    {
        shakeMagnitude = 0.2f;
        shakeDuration = 0.5f;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            StartCoroutine(ExplodeAfterDelay(gameObject));

            explosion.Play();
            collision.gameObject.GetComponent<PlayerMovement>().TakeDamage(30);
        }
    }

    private IEnumerator ExplodeAfterDelay(GameObject pressurePlate)
    {
        yield return new WaitForSeconds(0);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(pressurePlate.transform.position, 3);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Block"))
            {
                CollectAndDestroyBlock(collider.gameObject);
            }
        }

        StartCoroutine(LightIncrease());
        explosionLight.pointLightOuterRadius += 0.5f;
       
        BoxCollider2D bx = GetComponent<BoxCollider2D>();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        originalCameraPosition = Camera.main.transform.position;
        StartCoroutine(ScreenShake());
        bx.enabled = false;
        sr.enabled = false;
        particles.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        Destroy(pressurePlate);
    }

    private void CollectAndDestroyBlock(GameObject block)
    {
        TileDataPlaceholder tdp = block.GetComponent<TileDataPlaceholder>();
        int coinWorth = tdp.thisTile.coinWorth;

        // Destroy the block
        Destroy(block);
    }

    IEnumerator LightIncrease()
    {
        explosionLight.pointLightOuterRadius += lightIncrease;
        yield return new WaitForSeconds(lightwaittime);
        explosionLight.pointLightOuterRadius += lightIncrease;
        yield return new WaitForSeconds(lightwaittime);
        explosionLight.pointLightOuterRadius += lightIncrease;
        yield return new WaitForSeconds(lightwaittime);
        explosionLight.pointLightOuterRadius += lightIncrease;
        yield return new WaitForSeconds(lightwaittime);
        explosionLight.pointLightOuterRadius += lightIncrease;
        yield return new WaitForSeconds(lightwaittime);
        explosionLight.pointLightOuterRadius = 0f;
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

}
