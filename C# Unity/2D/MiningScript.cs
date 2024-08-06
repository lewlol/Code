using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Rendering.Universal;
using Unity.Burst.CompilerServices;
using UnityEngine.Rendering;

public class MiningScript : MonoBehaviour
{
    private GameObject lastHighlightedBlock;
    private Material originalMaterial;
    private float holdTimer;
    private bool isHolding;
    private Vector3 originalBlockPosition;


    public TextMeshProUGUI insufficientPowerText;

    private bool isInsufficientPowerTextActive = false;
    private Coroutine insufficientPowerCoroutine;

    private Vector3 originalPosition; // Store the original position of the block
    private Vector3 shakeOffset; // Store the current shake offset

    //public Texture2D pickaxecorsortexture;

    public MeshTextAppear tma;
    public HealthStamUI hs;
    int amountDropped;
    private GameObject previousHighlightedBlock; 

    public LayerMask ignoreRaycast;

    public AudioSource mine;
    private void Start()
    {
        originalPosition = Vector3.zero; // Initialize originalPosition
        shakeOffset = Vector3.zero; // Initialize shakeOffset
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, ~ignoreRaycast);

        if (lastHighlightedBlock != null && lastHighlightedBlock != hit.collider?.gameObject)
        {
            RestoreOriginalMaterial();
            isHolding = false;
            holdTimer = 0.0f;
        }
        if (lastHighlightedBlock != null && !IsInRange(lastHighlightedBlock))
        {
            RestoreOriginalMaterial();
            isHolding = false;
            holdTimer = 0.0f;
        }

        if (hit.collider != null && hit.collider.CompareTag("Block") && IsInRange(hit.collider.gameObject))
        {
            HighlightBlock(hit.collider.gameObject);

            if (Input.GetMouseButton(0))
            {
                if (!isHolding)
                {
                    isHolding = true;
                    originalBlockPosition = hit.collider.transform.position;
                }

                if (PlayerMovement.stamina > 0)
                {
                    TileDataPlaceholder tdp = hit.collider.gameObject.GetComponent<TileDataPlaceholder>();
                    int requiredBreakingPower = tdp.thisTile.breakingpower;
                    int playerBreakingPower = PlayerMovement.breakingpower;

                    if (playerBreakingPower >= requiredBreakingPower)
                    {
                        holdTimer += Time.deltaTime;

                        if (!hit.collider.gameObject.GetComponentInChildren<ParticleSystem>().isPlaying)
                        {
                            //Activate Particles
                            hit.collider.gameObject.GetComponentInChildren<ParticleSystem>().Play();
                        }

                        if (!mine.isPlaying)
                            PlayMiningSound(hit.collider.gameObject.GetComponent<TileDataPlaceholder>().thisTile.miningSound);
                        // Apply shaking effect while mining
                        float shakeMagnitude = 0.05f; // Adjust the magnitude as needed
                        shakeOffset = Random.insideUnitCircle * shakeMagnitude;
                        hit.collider.transform.position = originalPosition + shakeOffset;

                        if (holdTimer >= Artefacts.Holdduration)
                        {
                            BlockStats(hit);
                            Vector2 blockpos = hit.collider.transform.position;
                            string text = tdp.thisTile.name;
                            RestoreOriginalMaterial();
                            isHolding = false;
                            holdTimer = 0.0f;

                            //Text 
                            if (hit.collider.gameObject.GetComponent<TileDataPlaceholder>().thisTile.tileType != TileTypes.Loot)
                            {
                                tma.GenerateText(blockpos, 1, "+" + amountDropped + " " + text, 30);
                            }

                            StartCoroutine(DestroyDelay(hit));
                        }
                    }
                    else
                    {
                        Debug.Log("Pickaxe not strong enough to break this block!");
                        tma.gameObject.GetComponent<Indicator>().EnableIndicator(false, 5f, "Pickaxe not strong enough to break this block!");
                        StopParticles();
                    }
                }
                else
                {
                    Debug.Log("Not enough stamina to break this block!");
                    tma.gameObject.GetComponent<Indicator>().EnableIndicator(false, 5f, "Not enough stamina to break this block!");
                    StopParticles();

                }
            }
            else
            {
                 StopParticles();
                isHolding = false;
                holdTimer = 0.0f;
            }
        }
        else
        {
            StopParticles();
            isHolding = false;
            holdTimer = 0.0f;
        }
    }

    private bool IsInRange(GameObject block)
    {
        Vector3 playerPosition = transform.position;
        Vector3 targetPosition = block.transform.position;
        Vector3 direction = targetPosition - playerPosition;
        float distance = Vector3.Distance(playerPosition, targetPosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(playerPosition, direction, distance, ~ignoreRaycast);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Block") && hit.collider.gameObject != block)
            {
                return false;
            }
        }

        return distance <= Artefacts.miningrange;
    }

    private void HighlightBlock(GameObject block)
    {
        if (block != previousHighlightedBlock)
        {
            if (previousHighlightedBlock != null)
            {
                StopParticles(previousHighlightedBlock);
            }

            RestoreOriginalMaterial();
            lastHighlightedBlock = block;

            originalMaterial = block.GetComponent<Renderer>().material;
            originalPosition = block.transform.position;

            // Get the child object and enable it
            GameObject childObject = block.transform.GetChild(0).gameObject;
            SpriteRenderer childSpriteRenderer = childObject.GetComponent<SpriteRenderer>();
            childSpriteRenderer.enabled = true;

            // Change the cursor to the custom texture
           // Cursor.SetCursor(pickaxecorsortexture, Vector2.zero, CursorMode.Auto);

            previousHighlightedBlock = block; // Update the previously highlighted block
        }
    }

    private void RestoreOriginalMaterial()
    {
        if (lastHighlightedBlock != null && originalMaterial != null)
        {
            StopParticles(lastHighlightedBlock); // Stop particles of the previous highlighted block

            lastHighlightedBlock.GetComponent<Renderer>().material = originalMaterial;
            lastHighlightedBlock.transform.position = originalPosition;

            // Get the child object and disable it
            GameObject childObject = lastHighlightedBlock.transform.GetChild(0).gameObject;
            SpriteRenderer childSpriteRenderer = childObject.GetComponent<SpriteRenderer>();
            childSpriteRenderer.enabled = false;

            // Change the cursor back to the default
           // Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            lastHighlightedBlock = null;
            previousHighlightedBlock = null; // Reset the previous highlighted block
        }
    }

    private void StopParticles()
    {
        if (lastHighlightedBlock != null)
        {
            lastHighlightedBlock.GetComponentInChildren<ParticleSystem>().Stop();
        }
    }
    private void BlockStats(RaycastHit2D hit)
    {
        float currentStaminaChance = Mathf.Clamp(Artefacts.staminachance, 0.0f, 1.0f);
        if (Random.value <= currentStaminaChance)
        {
            PlayerMovement.stamina--;
            hs.UpdateStaminaUI();
        }

        int fortuneLevel = PlayerMovement.fortune;

        TileDataPlaceholder tdp = hit.collider.gameObject.GetComponent<TileDataPlaceholder>();
        int coinWorth = tdp.thisTile.coinWorth;

        //Check Chest First
        if (tdp.thisTile.tileType == TileTypes.Loot)
        {
            hit.collider.gameObject.GetComponent<LootBag>().RollLoot();
            return;
        }

        //5% chance every level
        float chanceForDoubleDrop = 0.05f * fortuneLevel;

        if (Random.value <= chanceForDoubleDrop) // Check if the player gets double drops
        {
            Inventory.Totalcoins += coinWorth * 2;
            amountDropped = 2;
            if (tdp.thisTile.tileType == TileTypes.Rock)
            {
                Inventory.Rocks += 2;
                EndSCene.Totalrocks += 2;
                Inventory.Rockcoins += coinWorth * 2;
                EndSCene.Totalcoins += coinWorth * 2;
            }
            else if (tdp.thisTile.tileType == TileTypes.Ore)
            {
                Inventory.Ores += 2;
                EndSCene.TotalOres += 2;
                Inventory.Orescoins += coinWorth * 2;
                EndSCene.Totalcoins += coinWorth * 2;
            }
            else if (tdp.thisTile.tileType == TileTypes.Gemstone)
            {
                Inventory.Gemstones += 2;
                EndSCene.TotalGems += 2;
                Inventory.Gemstonecoins += coinWorth * 2;
                EndSCene.Totalcoins += coinWorth * 2;
            }
        }
        else // Regular drop
        {
            Inventory.Totalcoins += coinWorth;
            amountDropped = 1;
            if (tdp.thisTile.tileType == TileTypes.Rock)
            {
                EndSCene.Totalrocks++;
                Inventory.Rocks++;
                Inventory.Rockcoins += coinWorth;
                EndSCene.Totalcoins += coinWorth * 2;
            }
            else if (tdp.thisTile.tileType == TileTypes.Ore)
            {
                Inventory.Ores++;
                EndSCene.TotalOres++;
                Inventory.Orescoins += coinWorth;
                EndSCene.Totalcoins += coinWorth * 2;
            }
            else if (tdp.thisTile.tileType == TileTypes.Gemstone)
            {
                Inventory.Gemstones++;
                EndSCene.TotalGems ++ ;
                Inventory.Gemstonecoins += coinWorth;
                EndSCene.Totalcoins += coinWorth * 2;
            }
        }
    }

    public IEnumerator DestroyDelay(RaycastHit2D hit)
    {
        hit.collider.gameObject.GetComponentInChildren<ParticleSystem>().Stop();
        hit.collider.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        hit.collider.gameObject.GetComponent<SpriteRenderer>().enabled = false;

        if (hit.collider.gameObject.GetComponentInChildren<Light2D>() != null)
        {
            hit.collider.gameObject.GetComponentInChildren<Light2D>().enabled = false;
        }
        yield return new WaitForSeconds(2f);
        Destroy(hit.collider.gameObject);
    }

    private void StopParticles(GameObject block)
    {
        if (block != null)
        {
            block.GetComponentInChildren<ParticleSystem>().Stop();
        }
    }

    public void PlayMiningSound(AudioClip miningSound)
    {
        mine.clip = miningSound;
        mine.Play();
    }
}