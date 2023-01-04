using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Random=UnityEngine.Random;
public class GameManager : MonoBehaviour
{
    GameObject cylinder;
    GameObject knife;
    Material cylinderMaterial;
    Material indicatorMaterial;
    Text scoreUI;
    Text timerUI;
    TextMeshProUGUI comboUI;
    public static float score = 0;
    public static float highestScore;
    Vector3 startingScale = new Vector3(0.35f,1,0.35f);
    Vector3 indicatorScale = new Vector3(0.35f,0.01f,0.37f);
    Vector3 knifeStartingRotation = new Vector3(0,0,0);
    Vector3 knifeEndingRotation = new Vector3(0,0,-100);
    List<GameObject> indicatorList = new List<GameObject>();
    bool isCuttingDown = false;
    float cutAnimationTime = 0;
    float lastSpawn = 0;
    float spawnWaitTime = 0;
    float indicatorSpeed = 0.01f;
    float endZ = 2.6f;
    float decayTimer = 0;
    float decayRate = 1;
    int combo = 1;
    float healthGainModifier = 1;
    float healthLossModifier = 1;
    float lastCutTime = 0;
    float gameStartTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        gameStartTime = Time.realtimeSinceStartup;
        knife = GameObject.Find("KnifePivot");
        timerUI = GameObject.Find("Stopwatch").GetComponent<Text>();
        scoreUI = GameObject.Find("Score").GetComponent<Text>();
        comboUI = GameObject.Find("ComboText").GetComponent<TextMeshProUGUI>();
        comboUI.text = combo + "x";
        timerUI.text = "Time: " + gameStartTime;
        scoreUI.text = "Score: " + score;
        cylinderMaterial = Resources.Load("Materials/CylinderMaterial") as Material;
        indicatorMaterial = Resources.Load("Materials/Background") as Material;
        cylinder = SpawnCylinder(startingScale, cylinderMaterial);
        GameObject.Find("Directional Light").transform.LookAt(cylinder.transform);
        InvokeRepeating("IncreaseSpeed", 1, 1);
        highestScore = PlayerPrefs.GetFloat("HighestScore", highestScore);
        Debug.Log(highestScore);

    }
    void Cut() {
        cutAnimationTime = 0;
        isCuttingDown = true;
        float currentSize = cylinder.transform.localScale.y;
        if(currentSize < 2.48f) {
            return;
        }
        cylinder.transform.localScale = new Vector3(0.35f, 2.48f, 0.35f);
        float newSize = (currentSize-2.48f)/2;
        GameObject temp_cylinder = SpawnCylinder(new Vector3(1,1,1), cylinderMaterial);
        GameObject pivot = new GameObject();
        temp_cylinder.transform.parent = pivot.transform;
        temp_cylinder.transform.localPosition = new Vector3(0,0,0.3f);
        pivot.transform.position = new Vector3(0,0,2.55f+(newSize/2));
        temp_cylinder.transform.localScale = new Vector3(0.35f, newSize, 0.35f);
        Destroy(temp_cylinder.GetComponent<CapsuleCollider>());
        Rigidbody tempRB = temp_cylinder.AddComponent<Rigidbody>();
        pivot.AddComponent<CutPiece>();
        bool hitIndicator = false;
        for(int i = indicatorList.Count-1; i >= 0; i--) {
            GameObject indicator = indicatorList[i];
            endZ = 2.48f + indicator.GetComponent<Indicator>().GetValue() * 0.12f;
            if(indicator.transform.position.z >= 2.48f) {
                indicator.transform.parent = temp_cylinder.transform;
                // indicator.transform.localPosition = new Vector3(0,0,0.25f);
                indicatorList.Remove(indicator);
            }
            if(indicator.transform.position.z >= 2.48f && indicator.transform.position.z <= endZ) {
                float scoreIncrease = indicator.GetComponent<Indicator>().GetValue()*combo;
                combo++;
                lastCutTime = Time.realtimeSinceStartup;
                HealthBar.SetCurrentHealth(HealthBar.GetCurrentHealth()+healthGainModifier);
                score+=Mathf.Clamp(scoreIncrease, 0, 10);
                scoreUI.text = "Score: " + score;
                Destroy(indicator);
                hitIndicator = true;

            }
        }
        if(!hitIndicator) {
            HealthBar.SetCurrentHealth(HealthBar.GetCurrentHealth()-healthLossModifier);
            scoreUI.text = "Score: " + score;
            combo = 1;
        }
        tempRB.AddTorque(new Vector3(Random.Range(-5,5), Random.Range(-5,5), Random.Range(-5,5)));
    }
    int GetRandomIndicatorSize() {
        float roll = Random.Range(0,100);
        if(roll < 5) {
            return 5;
        }
        else if(roll < 15) {
            return 4;
        }
        else if(roll < 30) {
            return 3;
        }
        else if(roll < 50) {
            return 2;
        }
        else {
            return 1;
        }
    }
    void ComboDisplay() {
        comboUI.text = combo + "x";
        comboUI.color = new Color(comboUI.color.r,comboUI.color.g,comboUI.color.b,1-Mathf.Lerp(0,1,Time.realtimeSinceStartup-lastCutTime));
        if(Time.realtimeSinceStartup-lastCutTime <= 0.05f) {
            comboUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(0,400),Random.Range(-200,200));
            comboUI.color = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),1-Mathf.Lerp(0,1,Time.realtimeSinceStartup-lastCutTime));
        }
    }
    // Update is called once per frame
    void Update()
    {
        knife.transform.rotation = Quaternion.Euler(Vector3.Lerp(knifeStartingRotation, knifeEndingRotation, cutAnimationTime));
        timerUI.text = "Time: " + (int)(Time.realtimeSinceStartup - gameStartTime);
        if(Input.GetButtonDown("Fire1")) {
            Cut();
        }
        if(Time.realtimeSinceStartup > lastSpawn + spawnWaitTime) {
            SpawnIndicator();
        }
        foreach(GameObject obj in indicatorList) {
            Indicator indicator = obj.GetComponent<Indicator>();
            if(obj.transform.position.z > endZ && !indicator.missed) {
                HealthBar.SetCurrentHealth(HealthBar.GetCurrentHealth()-healthLossModifier);
                indicator.missed = true;
                scoreUI.text = "Score: " + score;
            }
        }
        decayTimer += Time.deltaTime;
        if(decayTimer >= decayRate && score > 100) {
            scoreUI.text = "Score: " + score;
            decayTimer = 0;
        }
        healthLossModifier = Mathf.Min(((Time.realtimeSinceStartup - gameStartTime)/4.2f)+1,99);
        healthGainModifier = Mathf.Pow(0.9f,(Time.realtimeSinceStartup - gameStartTime)/4.5f);
        if(HealthBar.GetCurrentHealth() <= 0) {
            SceneManager.LoadScene("GameOver");
            highestScore = Mathf.Max(highestScore, score);
            PlayerPrefs.SetFloat("HighestScore", highestScore);
            PlayerPrefs.Save();
        }
    }
    void FixedUpdate() {
        ComboDisplay();
        if(isCuttingDown) {
            cutAnimationTime += 0.1f;
            if(cutAnimationTime >= 1) {
                isCuttingDown = false;
                cutAnimationTime = 1;
            }
        }
        else {
            cutAnimationTime -= 0.1f;
        }
        cylinder.transform.localScale = new Vector3(0.35f, cylinder.transform.localScale.y + indicatorSpeed, 0.35f);
        foreach(GameObject indicator in indicatorList) {
            indicator.transform.Translate(0,indicatorSpeed,0);
        }
    }
    void IncreaseSpeed() {
        indicatorSpeed += 0.0002f;
    }
    void SpawnIndicator() {
        GameObject indicator = SpawnCylinder(indicatorScale, indicatorMaterial);
        Indicator i = indicator.AddComponent<Indicator>();
        i.SetSize(GetRandomIndicatorSize());
        indicatorList.Add(indicator);
        lastSpawn = Time.realtimeSinceStartup;
        spawnWaitTime = Random.Range(0.4f - indicatorSpeed >= 0.06f ? 0.3f : indicatorSpeed*5 , 1);
    }
    GameObject SpawnCylinder(Vector3 scale, Material material) {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.Rotate(90,0,0);
        cylinder.transform.localScale = scale;
        cylinder.GetComponent<Renderer>().material = material;
        return cylinder;
    }
    GameObject SpawnCube() {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        return cube;
    }
    
}
