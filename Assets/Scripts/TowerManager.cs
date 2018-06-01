using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerManager : Singleton<TowerManager> {

	public TowerBtn TowerBtnPressed{get; set;}
    private SpriteRenderer spriteRenderer;
    private List<Tower> TowerList = new List<Tower>();
    private List<Collider2D> BuildList = new List<Collider2D>();
    private Collider2D buildTile;


	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        buildTile = GetComponent<Collider2D>(); 
        
	}

    public void RegisterBuildSite(Collider2D buildTag) {
        BuildList.Add(buildTag);
    }

    public void RegisterTower(Tower tower) {
        TowerList.Add(tower);
    }

    public void RenameTagsBuildSites() {
        foreach(Collider2D buildTag in BuildList) {
            buildTag.tag = "BuildSite";
        }
        BuildList.Clear();
    }

    public void DestroyAllTower() {
        foreach(Tower tower in TowerList) {
            Destroy(tower.gameObject);
        }
        TowerList.Clear();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if (hit.collider.tag == "BuildSite")
            {
                buildTile = hit.collider;
                hit.collider.tag = "BuildSiteFull";
                RegisterBuildSite(buildTile);
                placeTower(hit);
            }
        }
        if (spriteRenderer.enabled)
        {
            followMouse();
        }
    }

    public void placeTower(RaycastHit2D hit) {
        if (!EventSystem.current.IsPointerOverGameObject() && TowerBtnPressed != null)
        {
            Tower newTower = Instantiate(TowerBtnPressed.TowerObject);
            newTower.transform.position = hit.transform.position;
            buyTower(TowerBtnPressed.TowerPrice);
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.TowerBuilt);
            RegisterTower(newTower);
            disableDragSprite();
        }
    }

    public void buyTower(int price) {
        GameManager.Instance.subtractMoney(price);
    }
	public void selectedTower(TowerBtn towerSelected) {
        if(towerSelected.TowerPrice <= GameManager.Instance.TotalMoney ){
		TowerBtnPressed = towerSelected;
        enableDragSprite(TowerBtnPressed.DragSprite);
     }
	}

    public void followMouse()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(transform.position.x, transform.position.y);
    }
    public void enableDragSprite(Sprite sprite)
    {
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite; 
    }

    public void disableDragSprite() 
    {
        spriteRenderer.enabled = false;
        
    }
}
