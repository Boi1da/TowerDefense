using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum gameStatus {
        next, play, gameover, win
}



public class GameManager : Singleton<GameManager> {


    [SerializeField]
    private int totalWaves = 10; 
    [SerializeField]
    private Text totalMoneyLbl; 
    [SerializeField]
    private Text currentWaveLbl; //current Wave label (top)
    [SerializeField]
    private Text TotalEscapedLbl; //current escaped enemies

	[SerializeField]
	private GameObject spawnPoint;
    [SerializeField]
    private Enemy[] enemies; //Array to hold 3 enemies 
    [SerializeField]
    private int totalEnemies = 3;
    [SerializeField]
    private int enemiesPerSpawn;

    [SerializeField]
    private Text playBtnLbl;
    [SerializeField]
    private Button playBtn;

    private int waveNumber = 0;
    private int totalMoney = 10;
    private int totalEscaped = 0;
    private int roundEscaped = 0;
    private int totalKilled = 0; 
    private int whichEnemiesToSpawn = 0; 
    private int enemiesToSpawn = 0;
    private gameStatus currentState = gameStatus.play; 
    private AudioSource audioSource;
    public List<Enemy> EnemyList = new List<Enemy>(); 

	const float spawnDelay = 0.5f; //Every half second!

    public int TotalEscaped {
        get {
            return totalEscaped;
        }
        set {
            totalEscaped = value;
        }
    }

    public int RoundEscaped {
        get {
            return roundEscaped;
        }
        set {
            roundEscaped = value; 
        }
    }

    public int TotalKilled {
        get {
            return totalKilled;
        }
        set {
            totalKilled = value; 
        }
    }
    public int TotalMoney {
        get {
            return totalMoney;
        }
        set {
            totalMoney = value; 
            totalMoneyLbl.text = totalMoney.ToString();
        }
    }

    public AudioSource AudioSource {
        get {
            return audioSource;
        }
    }


	// Use this for initialization
	void Start () {
		playBtn.gameObject.SetActive(false); //Hides the next wave button
        audioSource = GetComponent<AudioSource>();
        showMenu();
	}

    void Update() {
        handleEscape();
    }
	

	IEnumerator spawn() {
		if (enemiesPerSpawn > 0 && EnemyList.Count < totalEnemies) {
			for(int i = 0; i< enemiesPerSpawn; i++) {
				if(EnemyList.Count < totalEnemies){
					Enemy newEnemy = Instantiate(enemies[Random.Range(0, enemiesToSpawn)]) as Enemy; //Cast as gameObject since instantiate returns normal object
					newEnemy.transform.position = spawnPoint.transform.position;
				}
			}
			yield return new WaitForSeconds(spawnDelay);
			StartCoroutine(spawn());
		}
	}

    public void RegisterEnemy(Enemy enemy)
    {
        EnemyList.Add(enemy);
    }

    public void UnRegisterEnemy(Enemy enemy)
    {
        EnemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    public void DestroyAllEnemies()
    {
        foreach(Enemy enemy in EnemyList)
        {
            Destroy(enemy.gameObject); 
        }

        EnemyList.Clear();
    }

    public void addMoney(int amount) {
        TotalMoney += amount; 
    }

    public void subtractMoney(int amount) {
        TotalMoney -= amount;
    }

    public void isWaveOver() {
        TotalEscapedLbl.text = "Escaped " + totalEscaped + "/10";
        if ((roundEscaped + TotalKilled)== totalEnemies) {
            if (waveNumber <= enemies.Length) {
                enemiesToSpawn = waveNumber; 
            }
            setCurrentGameState(); 
            showMenu();
        }
    }

    public void setCurrentGameState(){
        if (TotalEscaped >= 10) {
            currentState = gameStatus.gameover; 
        } else if (waveNumber == 0 && (TotalKilled + RoundEscaped) == 0) {
            currentState = gameStatus.play;
        } else if (waveNumber >= totalWaves) {
            currentState = gameStatus.win;
        } else {
            currentState = gameStatus.next;
        }
    }
    public void showMenu() {
        switch (currentState) {
            case gameStatus.gameover: 
                playBtnLbl.text = "Play Again!";
                GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Gameover);
                break; 
            case gameStatus.next:
                playBtnLbl.text = "Next Wave!";
                break; 
            case gameStatus.play: 
                playBtnLbl.text = "Play" ;
                break;
            case gameStatus.win: 
                playBtnLbl.text = "Play";
                break;
        }
        playBtn.gameObject.SetActive(true);
    }

    public void playBtnPressed() {
        switch(currentState) {
            case gameStatus.next:
                waveNumber += 1; //Increment the wave number 
                totalEnemies += waveNumber;
            break;
        default: //Play for first time
            totalEnemies = 3; 
            TotalEscaped = 0; 
            totalMoney = 10;
            enemiesToSpawn = 0;
            TowerManager.Instance.DestroyAllTower(); 
            TowerManager.Instance.RenameTagsBuildSites(); 
            totalMoneyLbl.text = TotalMoney.ToString(); 
            TotalEscapedLbl.text = "Escaped " + TotalEscaped + "/10";
            audioSource.PlayOneShot(SoundManager.Instance.Newgame);          
            break;
        }
        DestroyAllEnemies();
        TotalKilled = 0;
        RoundEscaped = 0; 
        currentWaveLbl.text = "Wave " + (waveNumber + 1);
        StartCoroutine(spawn());
        playBtn.gameObject.SetActive(false);
        }

    private void handleEscape() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TowerManager.Instance.disableDragSprite(); 
            TowerManager.Instance.TowerBtnPressed = null; 
        }
    }
}
