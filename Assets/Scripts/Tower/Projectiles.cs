using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum proType
{
    rock, arrow, fireball
};

public class Projectiles : MonoBehaviour {

    [SerializeField]
    private int attackStrength;
    [SerializeField]
    private proType projectileType;
	
    
    public int AttackStrength
    {
        get
        {
            
            return attackStrength;
        }
    }

    public proType ProjectileType
    {
        get
        {
            return projectileType;
        }
    }
}
