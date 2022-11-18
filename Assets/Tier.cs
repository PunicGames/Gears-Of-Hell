using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Tier
{
    public string name;

    // Important! sum(Mele.probs + Ranged.probs + Special.probs) <= 1;
    [Header("Basic enemies:")]
    [Space]
    public float prob_melee;
    public List<GameObject> melee;
    [Space]
    public float prob_ranged;
    public List<GameObject> ranged;

    [Header("Special enemies:")]
    // Special.Probs is equal to: 1 - (Mele.probs + Ranged.probs)
    public List<GameObject> special;

    // Returns an enemy using the probs setted
    public GameObject GetRandomEnemy() 
    {
        var value = Random.value;

        if (value < prob_melee)
            return melee[Random.Range(0, melee.Count)];
        else if (value < (prob_melee + prob_ranged))
            return ranged[Random.Range(0, melee.Count)];
        else
            return special[Random.Range(0, melee.Count)];

    }

    public void Add(Tier tier)
    {
        this.prob_melee = tier.prob_melee;
        foreach (var enemy in tier.melee)
            this.melee.Add(enemy);

        this.prob_ranged = tier.prob_ranged;
        foreach (var enemy in tier.ranged)
            this.ranged.Add(enemy);

        foreach (var enemy in tier.special)
            this.special.Add(enemy);
    }

    public void Remove(Tier tier)
    {
        foreach (var enemy in tier.melee)
            this.melee.Remove(enemy);

        foreach (var enemy in tier.ranged)
            this.ranged.Remove(enemy);

        foreach (var enemy in tier.special)
            this.special.Remove(enemy);
    }


}