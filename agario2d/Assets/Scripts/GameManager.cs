using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    public GameObject aiPrefab, foodPrefab;
    public Text gaStatistics;
    public Button speedUpButton;
    public int aiCount,foodCount;
    public float foodCheckPeriod = 5f;
    public bool torus;

    private (float xRange, float yRange) mapData = (23f,10f);
    private float camOrthSize, screenRatio,widthOrtho;
    private List<Transform> transformList;

    [Space]
    [Space]
    [Header("Genetic Algorithm ")]
    private  GeneticAlgorithm geneticAlgorithm;
    public float crossOverRatio;
    public float mutationRatio;
    public float epochTime;
    private float epochBackUp;
    private int epochCount = 0;
    void Awake()
    {
        Application.runInBackground = true;
        epochBackUp = epochTime;
        camOrthSize = Camera.main.orthographicSize;
        screenRatio = (float)Screen.width / (float)Screen.height;  
        widthOrtho  = camOrthSize * screenRatio;
        TransformList= new List<Transform>();
        mapData = (widthOrtho , camOrthSize);

        InstantiateSceneObjects();     
        geneticAlgorithm = new GeneticAlgorithm(aiCount, mutationRatio, crossOverRatio, transformList[0].GetComponent<AIBrain>().NeuralNetwork.NeuralWeightLength);
       
    }

    private void Start()
    {
        StartCoroutine(CheckFoodCount());
    }

    void InstantiateSceneObjects()
    {
        for(int aiIndex = 0; aiIndex<aiCount; aiIndex++)
        {
            GameObject aiRef = InstantiateParameter(aiPrefab);
            TransformList.Add(aiRef.transform);
            aiRef.GetComponent<AIBrain>().GmRef = this;
        }

        for(int foodIndex = 0; foodIndex<foodCount; foodIndex++)
        {
            GameObject foodRef = InstantiateParameter(foodPrefab);
            TransformList.Add(foodRef.transform);
            foodRef.GetComponent<FoodScript>().GmRef = this;
        }
        
    }

    IEnumerator CheckFoodCount()
    {
        while(true)
        {
            if((TransformList.Count - aiCount)  < foodCount)
            {
                for(int foodIndex = 0; foodIndex < foodCount + aiCount - (TransformList.Count - aiCount); foodIndex++)
                {
                    GameObject foodRef = InstantiateParameter(foodPrefab);
                    TransformList.Add(foodRef.transform);
                    foodRef.GetComponent<FoodScript>().GmRef = this;
                }
            }    
                yield return new WaitForSeconds(foodCheckPeriod);
           
        }
    }

     GameObject InstantiateParameter(GameObject parameter)
     {
         GameObject tempObject = Instantiate(parameter,
            new Vector3(Random.Range(-1* mapData.xRange,mapData.xRange), 
            Random.Range(-1 * mapData.yRange,mapData.yRange), 0),
            Quaternion.identity);
            tempObject.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f,1f), Random.Range(0f,1f),Random.Range(0f,1f));
          return tempObject;  
    }

   

    // Update is called once per frame
    void Update()
    {
        if(epochTime <= 0)
        {
            epochTime = epochBackUp;
            DeployGeneticAlgorithm();
            RefreshLevel();
            epochCount++;
        }
      epochTime -= Time.deltaTime;
      RefreshText();
    }


    void RefreshLevel()
    {
       for(int aiIndex = 0; aiIndex < aiCount; aiIndex++)
       {
            TransformList.ElementAt(aiIndex).gameObject.GetComponent<AIBrain>().RestoreComponents();
            TransformList.ElementAt(aiIndex).transform.position = new Vector3(Random.Range(-1* mapData.xRange,mapData.xRange), 
                                                                            Random.Range(-1 * mapData.yRange,mapData.yRange), 0);
       }
    }
    private void RefreshText()
    {
         gaStatistics.text = $"Epoch:  + {epochTime}\nEpoch Count: {epochCount}\nBest Fitness: {geneticAlgorithm.BestFitness}";
    }

    void DeployGeneticAlgorithm()
    {
        var currentPopulation = GetPopulation();
        var newPopulation = geneticAlgorithm.Epoch(ref currentPopulation);
        for (int individualIndex = 0; individualIndex < aiCount; individualIndex++)
        {
            var currentIndividual = TransformList[individualIndex].GetComponent<AIBrain>();
            currentIndividual.NeuralNetwork.PutWeights(newPopulation[individualIndex].weightList);
            currentIndividual.IndividualScore = newPopulation[individualIndex].genomeFitness;
            currentIndividual.IndividualScore = 0;
        }
    }

    List<(List<float> weightList, float fitness)> GetPopulation()
    {
       List<(List<float> weightList, float fitness)> currentPopulation = new List<(List<float> weightList, float fitness)>();

        for (int individualIndex = 0; individualIndex < aiCount; individualIndex++)
        {
            var currentIndividual = TransformList[individualIndex].GetComponent<AIBrain>();
            currentPopulation.Add((currentIndividual.NeuralNetwork.GetWeights(), currentIndividual.IndividualScore));
        }


       return currentPopulation;
    }

   public void SpeedUpButton()
{
    if (Time.timeScale == 1f)
    {
        Time.timeScale = 10f;
        speedUpButton.GetComponentInChildren<Text>().text = "Normal Speed";
    }
    else
    {
        Time.timeScale = 1f;
        speedUpButton.GetComponentInChildren<Text>().text = "Speed Up";
    }
}

 public List<Transform> TransformList { get => transformList; set => transformList = value; }
    public float CamOrthSize { get => camOrthSize; set => camOrthSize = value; }
    public float ScreenRatio { get => screenRatio; set => screenRatio = value; }
    public float WidthOrtho { get => widthOrtho; set => widthOrtho = value; }
}
