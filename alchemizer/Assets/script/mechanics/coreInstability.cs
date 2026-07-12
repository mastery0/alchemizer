using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class coreInstability : MonoBehaviour
{
    //pressure goes up as the player continue hitting enemy and taking dmg and slowly goes down when he stop,reset on heal
    public player player;
    public static coreInstability instance;
    public pressureBar pressureBar;
    public Volume volume;

    public int currentPressure = 0;
    public int pressurePlusDelta = 10;
    public int pressureMinusDelta = 10;
    public int maxPressure = 100;


    public float atkMod=1.15f;
    public float defMod=0.90f;


    public bool glassCannon = false;
    bool applied = false;
    public int lastTier = 0;
    private bool done1, done2;
    private bool isDecreasing;

    [Header("saturation")]
    public float baseSaturation = 0f;
    public float maxSaturationDelta = -25f;
    private ColorAdjustments colorAdjustments;
    private float smooth;
    public float smoothspd = 1.2f;
    private bool isFlashing = false;
    private void Start()
    {
        instance = this;
        volume.profile.TryGet(out colorAdjustments);
    }
    private void Update()
    {
        if (!player.coreInstability) return;
        int newTier=0;
        currentPressure=Mathf.Clamp(currentPressure, 0, maxPressure);
        pressureBar.setAmount(currentPressure,maxPressure);
        smooth=Mathf.MoveTowards(smooth,(float)currentPressure/(float)maxPressure,smoothspd*Time.unscaledDeltaTime);
        if (colorAdjustments != null) colorAdjustments.saturation.value = Mathf.Lerp(baseSaturation, maxSaturationDelta, smooth);
        player.timeSinceHit += Time.deltaTime;
        player.timeSinceAttack += Time.deltaTime;
        if ((player.timeSinceAttack > 3f && player.timeSinceHit > 3f)&&!isDecreasing) StartCoroutine(decreasePressure());
        //if heal pressure=0;
        if (currentPressure >= 30)
        {
            newTier = 1;
        }
        if (currentPressure >= 60)
        {
            newTier = 2;
        }
        if (newTier != lastTier)
        {
            if (isFlashing) StopCoroutine(Flash());
            StartCoroutine(Flash());
            // Remove old multiplayers
            Debug.Log(newTier);
            if (lastTier >= 1) { player.attackDamage /= atkMod; player.defense /= defMod; }
            if (lastTier >= 2) { player.attackDamage /= atkMod; player.defense /= defMod; }

            // Apply new multiplayers
            if (newTier >= 1) { player.attackDamage *= atkMod; player.defense *= defMod; }
            if (newTier >= 2) { player.attackDamage *= atkMod; player.defense *= defMod; }

            lastTier = newTier;
        }
        if (glassCannon&&!applied)
        {
            applied= true;
            atkMod = 1.30f;
            defMod = 0.75f;
        }
    }
    private IEnumerator decreasePressure()
    {
        isDecreasing = true;
        currentPressure -= pressureMinusDelta;
        yield return new WaitForSeconds(0.5f);
        isDecreasing=false;
    }
    private IEnumerator Flash()
    {
        isFlashing = true;

        Color originalColor = Color.white;

        pressureBar.bar.color = Color.black;
        yield return new WaitForSeconds(0.08f);

        pressureBar.bar.color = originalColor;

        isFlashing = false;
    }
}

