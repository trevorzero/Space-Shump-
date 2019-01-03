﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public float speed = 10f;
	public float fireRate = 0.3f;
	public float health = 10;
	public int score = 100;
	public float showDamageDuration = 0.1f;
	public float powerUpDropChance = 1f;
	[Header("Set Dynamically: Enemy")]
	public Color[] originalColors;
	public Material[] materials;
	public bool showingDamage = false;
	public float damageDoneTime;
	public bool notifiedOfDestuction = false;
	protected BoundsCheck bndCheck;


	public Vector3 pos
	{
		get{
			return(this.transform.position);
		}
		set{
			this.transform.position = value;
		}
	}

	void Awake()
	{
		bndCheck = GetComponent<BoundsCheck>();

		materials = Utils.GetAllMaterials(gameObject);
		originalColors = new Color[materials.Length];

		for (int i = 0; i<materials.Length; i++) {
			originalColors[i] = materials[i].color;
		}

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Move();
		if (showingDamage && Time.time > damageDoneTime)
		{
			UnShowDamage();
		}
		if(bndCheck != null && !bndCheck.isOnScreen)
		{
			if(pos.y < bndCheck.camHeight - bndCheck.radius)
			{
				Destroy(gameObject);
			}
		}
	}
	public virtual void Move()
	{
		Vector3 tempPos = pos;
		tempPos.y -= speed * Time.deltaTime;
		pos = tempPos;
	}
/*	void OnCollisionEnter(Collision coll) {
		GameObject otherGO = coll.gameObject;

		if (otherGO.tag == "ProjectileHero")
		{
			Destroy(otherGO);
			Destroy(gameObject);
		}
		else{
			print("Enemy hit by non-ProjectileHero: " + otherGO.name);
		}
	}*/
	void OnCollisionEnter(Collision coll)
	{
		GameObject otherGO = coll.gameObject;
		switch (otherGO.tag)
		{
			case "ProjectileHero":
			Projectile p = otherGO.GetComponent<Projectile>();
			ShowDamage();
			Debug.Log(p.type);

			if(!bndCheck.isOnScreen)
			{
				Destroy(otherGO);
				break;
			}
			//Debug.Log("health is"  + health);
			//Debug.Log("Damage is"  +  Main.GetWeaponDefinition(p.type).damageOnHit);
			health -= Main.GetWeaponDefinition(p.type).damageOnHit;
		//	Debug.Log("health is"  + health);
			if(health <= 0)
			{
				
				if (!notifiedOfDestuction) {
					Main.S.ShipDestroyed(this);
				}
				notifiedOfDestuction = true;
				Destroy(this.gameObject);
			}
			Destroy(otherGO);
		//	Destroy(gameObject);
		
			break;

			default:
			print("Enemy hit by non-ProjectileHero: " + otherGO.name);
			break;
		}
	}
	void ShowDamage()
	{
		foreach(Material m in materials)
		{
			m.color = Color.red;
		}
		showingDamage = true;
		damageDoneTime = Time.time + showDamageDuration;
	}

	void UnShowDamage()
	{
		for(int i=0; i<materials.Length; i++)
		{
			materials[i].color = originalColors[i];
		}
		showingDamage = false;
	}
}
