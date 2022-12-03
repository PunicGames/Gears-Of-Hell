using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerksManager : MonoBehaviour
{
    int totalPerks;
    private int maxPerksLevel;

    ////////////////////////Perks index////////////////////////
    /*
    rapid fire          -> 0
    laser shot          -> 1
    big shot            -> 2
    tactic vest         -> 3
    tactical boots      -> 4
    medic               -> 5
    electrical barrier  -> 6
    gunsmith            -> 7
    */

    private RapidFire sc_rapidFire;
    private LaserShot sc_laserShot;
    private BigShot sc_bigShot;
    private TacticVest sc_tacticVest;
    private TacticalBoots sc_tacticalBoots;
    private Medic sc_medic;
    private ElectricBarrier sc_electricBarrier;
    private Gunsmith sc_gunsmith;

    

    ////////////////////////Array active perks////////////////////////
    [HideInInspector] public bool[] availablePerks;

    ////////////////////////Array perk levels////////////////////////
    //los valores van del 0 al 4, siendo 4 el nivel 1 y 4 el maximo nivel(5)
    //las ventajas que no estan activas aparecen tambien como 0
    //[0] -> nivel 1
    //[1] -> nivel 2
    //[2] -> nivel 3
    //[3] -> nivel 4
    //[4] -> nivel 5
    [HideInInspector] public int[] perkLevels;

    //Arrays perks factor per level

    //Rapid fire
    private float[] rapidFireFactor;

    //Laser shot
    private float[] laserShotFactor;

    //Big shot
    private float[] bigShotFactor;

    //Tactic vest
    private float[] tacticVestFactor;

    //Tactical boots
    private float[] tacticalBootsFactor;

    //Medic
    private float[] medicFactor;

    //Electrical barrier
    private float[] electricBarrierFactor;

    //Gunsmith
    private float[] gunsmithFactor;

    private void Start()
    {
        totalPerks = 8;
        maxPerksLevel = 5;

        sc_rapidFire = GetComponentInChildren<RapidFire>();
        sc_laserShot = GetComponentInChildren<LaserShot>();
        sc_bigShot = GetComponentInChildren<BigShot>();
        sc_tacticVest = GetComponentInChildren<TacticVest>();
        sc_tacticalBoots = GetComponentInChildren<TacticalBoots>();
        sc_medic = GetComponentInChildren<Medic>();
        sc_electricBarrier = GetComponentInChildren<ElectricBarrier>();
        sc_gunsmith = GetComponentInChildren<Gunsmith>();


        //Initialize active perks array
        availablePerks = new bool[totalPerks];

        for (int i = 0; i < availablePerks.Length; i++)
        {
            availablePerks[i] = false;
        }

        perkLevels = new int[totalPerks];
        for (int i = 0; i < perkLevels.Length; i++)
        {
            perkLevels[i] = 0;
        }

        InitializeRapidFireFactors();
        InitializeLaserShotFactors();
        InitializeBigShotFactors();
        InitializeTacticVestFactors();
        InitializeTacticalBootsFactors();
        InitializeMedicFactors();
        InitializeElectricBarrierFactors();
        InitializeGunsmithFactors();
    }

    ////////////////////////Initialize perks factors////////////////////////

    private void InitializeRapidFireFactors()
    {
        ////////////////////////Initialize rapid fire factors////////////////////////
        rapidFireFactor = new float[maxPerksLevel];

        //level 1
        rapidFireFactor[0] = 1.2f;
        //level 2
        rapidFireFactor[1] = 1.4f;
        //level 3
        rapidFireFactor[2] = 1.6f;
        //level 4
        rapidFireFactor[3] = 1.8f;
        //level 5
        rapidFireFactor[4] = 2f;
    }

    private void InitializeLaserShotFactors()
    {
        ////////////////////////Initialize laser shot factors////////////////////////
        laserShotFactor = new float[maxPerksLevel];

        //level 1
        laserShotFactor[0] = 1.2f;
        //level 2
        laserShotFactor[1] = 1.4f;
        //level 3
        laserShotFactor[2] = 1.6f;
        //level 4
        laserShotFactor[3] = 1.8f;
        //level 5
        laserShotFactor[4] = 2f;
    }

    private void InitializeBigShotFactors()
    {
        ////////////////////////Initialize big shot factors////////////////////////
        bigShotFactor = new float[maxPerksLevel];

        //level 1
        bigShotFactor[0] = 1.2f;
        //level 2
        bigShotFactor[1] = 1.4f;
        //level 3
        bigShotFactor[2] = 1.6f;
        //level 4
        bigShotFactor[3] = 1.8f;
        //level 5
        bigShotFactor[4] = 2f;
    }

    private void InitializeTacticVestFactors()
    {
        ////////////////////////Initialize tactic vest factors////////////////////////
        tacticVestFactor = new float[maxPerksLevel];

        //level 1
        tacticVestFactor[0] = 1.2f;
        //level 2
        tacticVestFactor[1] = 1.4f;
        //level 3
        tacticVestFactor[2] = 1.6f;
        //level 4
        tacticVestFactor[3] = 1.8f;
        //level 5
        tacticVestFactor[4] = 2f;
    }

    private void InitializeTacticalBootsFactors()
    {
        ////////////////////////Initialize tactical boots factors////////////////////////
        tacticalBootsFactor = new float[maxPerksLevel];

        //level 1
        tacticalBootsFactor[0] = 1.1f;
        //level 2
        tacticalBootsFactor[1] = 1.2f;
        //level 3
        tacticalBootsFactor[2] = 1.3f;
        //level 4
        tacticalBootsFactor[3] = 1.4f;
        //level 5
        tacticalBootsFactor[4] = 1.5f;
    }

    private void InitializeMedicFactors()
    {
        ////////////////////////Initialize medic factors////////////////////////
        medicFactor = new float[maxPerksLevel];
        //el factor representa el intervalo de tiempo en segundos para regenerar un 5% de la vida maxima

        //level 1
        medicFactor[0] = 30f;
        //level 2
        medicFactor[1] = 20f;
        //level 3
        medicFactor[2] = 10f;
        //level 4
        medicFactor[3] = 5f;
        //level 5
        medicFactor[4] = 2f;
    }

    private void InitializeElectricBarrierFactors()
    {
        ////////////////////////Initialize electric barrier factors////////////////////////
        electricBarrierFactor = new float[maxPerksLevel];
        //el factor representa el tiempo de espera en segundos para reactivar una nueva barrera

        //level 1
        electricBarrierFactor[0] = 45f;
        //level 2
        electricBarrierFactor[1] = 30f;
        //level 3
        electricBarrierFactor[2] = 15f;
        //level 4
        electricBarrierFactor[3] = 10f;
        //level 5
        electricBarrierFactor[4] = 5f;
    }

    private void InitializeGunsmithFactors()
    {
        ////////////////////////Initialize gunsmith factors////////////////////////
        gunsmithFactor = new float[maxPerksLevel];
        //el factor representa el intervalo de tiempo en segundos para regenerar una bala

        //level 1
        gunsmithFactor[0] = 8f;
        //level 2
        gunsmithFactor[1] = 4f;
        //level 3
        gunsmithFactor[2] = 2f;
        //level 4
        gunsmithFactor[3] = 1f;
        //level 5
        gunsmithFactor[4] = 0.5f;
    }

    ////////////////////////Activate perks////////////////////////

    //Rapid fire
    public void ActivateRapidFire()
    {
        sc_rapidFire.bufAttackSpeed = rapidFireFactor[perkLevels[0]];
        sc_rapidFire.gameObject.SetActive(true);
        availablePerks[0] = true;
    }

    //Laser shot
    public void ActivateLaserShot()
    {
        sc_laserShot.bufBulletForce = laserShotFactor[perkLevels[1]];
        sc_laserShot.gameObject.SetActive(true);
        availablePerks[1] = true;
    }

    //Big shot
    public void ActivateBigShot()
    {
        sc_bigShot.bufScaleFactor = bigShotFactor[perkLevels[2]];
        sc_bigShot.gameObject.SetActive(true);
        availablePerks[2] = true;
    }

    //Tactic vest
    public void ActivateTacticVest()
    {
        sc_tacticVest.bufHealth = tacticVestFactor[perkLevels[3]];
        sc_tacticVest.gameObject.SetActive(true);
        availablePerks[3] = true;
    }

    //Tactical boots
    public void ActivateTacticalBoots()
    {
        sc_tacticalBoots.bufSpeed = tacticalBootsFactor[perkLevels[4]];
        sc_tacticalBoots.gameObject.SetActive(true);
        availablePerks[4] = true;
    }

    //Medic
    public void ActivateMedic()
    {
        sc_medic.healRate = medicFactor[perkLevels[5]];
        sc_medic.gameObject.SetActive(true);
        availablePerks[5] = true;
    }

    //Electrical barrier
    public void ActivateElectricalBarrier()
    {
        sc_electricBarrier.cooldown = electricBarrierFactor[perkLevels[6]];
        sc_electricBarrier.gameObject.SetActive(true);
        availablePerks[6] = true;
    }

    //Gunsmith
    public void ActivateGunsmith()
    {
        sc_gunsmith.ammoRegenRate = gunsmithFactor[perkLevels[7]];
        sc_gunsmith.gameObject.SetActive(true);
        availablePerks[7] = true;
    }












    ////////////////////////Deactivate perks////////////////////////

    //Rapid fire
    public void DeactivateRapidFire()
    {
        sc_rapidFire.gameObject.SetActive(false);
    }

    //Laser shot
    public void DeactivateLaserShot()
    {
        sc_laserShot.gameObject.SetActive(false);
    }

    //Big shot
    public void DeactivateBigShot()
    {
        sc_bigShot.gameObject.SetActive(false);
    }

    //Tactic vest
    public void DeactivateTacticVest()
    {
        sc_tacticVest.gameObject.SetActive(false);
    }

    //Tactical boots
    public void DeactivateTacticalBoots()
    {
        sc_tacticalBoots.gameObject.SetActive(false);
    }

    //Medic
    public void DeactivateMedic()
    {
        sc_medic.gameObject.SetActive(false);
    }

    //Electrical barrier
    public void DeactivateElectricalBarrier()
    {
        sc_electricBarrier.gameObject.SetActive(false);
    }

    //Gunsmith
    public void DeactivateGunsmith()
    {
        sc_gunsmith.gameObject.SetActive(false);
    }



    ////////////////////////Upgrade perks////////////////////////
    
    //Rapid fire
    public void UpgradeRapidFire()
    {
        if(perkLevels[0] < maxPerksLevel - 1)
        {
            DeactivateRapidFire();
            perkLevels[0]++;
            ActivateRapidFire();
        }
    }

    //Laser shot
    public void UpgradeLaserShot()
    {
        if (perkLevels[1] < maxPerksLevel - 1)
        {
            DeactivateLaserShot();
            perkLevels[1]++;
            ActivateLaserShot();
        }
    }

    //Big shot
    public void UpgradeBigShot()
    {
        if (perkLevels[2] < maxPerksLevel - 1)
        {
            DeactivateBigShot();
            perkLevels[2]++;
            ActivateBigShot();
        }
    }

    //Tactic vest
    public void UpgradeTacticVest()
    {
        if (perkLevels[3] < maxPerksLevel - 1)
        {
            DeactivateTacticVest();
            perkLevels[3]++;
            ActivateTacticVest();
        }
    }

    //Tactical boots
    public void UpgradeTacticalBoots()
    {
        if (perkLevels[4] < maxPerksLevel - 1)
        {
            DeactivateTacticalBoots();
            perkLevels[4]++;
            ActivateTacticalBoots();
        }
    }

    //Medic
    public void UpgradeMedic()
    {
        if (perkLevels[5] < maxPerksLevel - 1)
        {
            DeactivateMedic();
            perkLevels[5]++;
            ActivateMedic();
        }
    }

    //Electrical barrier
    public void UpgradeElectricalBarrier()
    {
        if (perkLevels[6] < maxPerksLevel - 1)
        {
            DeactivateElectricalBarrier();
            perkLevels[6]++;
            ActivateElectricalBarrier();
        }
    }

    //Gunsmith
    public void UpgradeGunsmith()
    {
        if (perkLevels[7] < maxPerksLevel - 1)
        {
            DeactivateGunsmith();
            perkLevels[7]++;
            ActivateGunsmith();
        }
    }

    ////////////////////////Check max level perks////////////////////////

    
    
}
