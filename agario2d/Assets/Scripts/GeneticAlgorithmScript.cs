using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GeneticAlgorithm
{

    List<(List<float> weightList, float genomeFitness)> population;
    int populationSize;
    int chromosomeLength;

    int fittestGenome;
    int generCounter;
    int NumElites = 4;
    int NumOfCopiesElite = 2;
    float maxPerturbation = 0.3f;
    float totalFitness;
    float bestFitness;
    float worstFitness;
    float averageFitness;



    float mutationRate;
    float crossOverRate;

    

    public GeneticAlgorithm(int popSize, float mutationRate, float crossOverRate, int chromoLength)
    {
        
        this.populationSize = popSize;
        this.mutationRate = mutationRate;
        this.crossOverRate = crossOverRate;
        this.chromosomeLength = chromoLength;

        totalFitness = 0;
        generCounter = 0;
        fittestGenome = 0;
        BestFitness = 0;
        AverageFitness = 0;
        worstFitness = Mathf.Infinity;

        Population = new List<(List<float> weightList, float genomeFitness)>();

    }

    void Mutate(ref List<float> chromosome)
    {
        for (int chromosomeIndex = 0; chromosomeIndex < chromosome.Count; chromosomeIndex++)
        {
            if (Random.Range(0f, 1f) < mutationRate)
            {
                chromosome[chromosomeIndex] += Random.Range(-maxPerturbation, maxPerturbation);
            }
        }
    }
    List<float> GetChromoRoulettte()
    {        
        float slice = Random.Range(0f, 1f) * totalFitness;
        List<float> chosenOne = Population[0].weightList;
        float fitnessSoFar = 0f;

        if (totalFitness > 0)
        {
            for (int currentIndividual = 0; currentIndividual < populationSize; currentIndividual++)
            {
                fitnessSoFar += Population[currentIndividual].genomeFitness;
                if (fitnessSoFar >= slice)
                {
                    chosenOne = Population[currentIndividual].weightList;
                    break;
                }
            }
        }

        else
        {
            chosenOne.Clear();

            for (int chosenOneIndex = 0; chosenOneIndex < chromosomeLength; chosenOneIndex++)
            {
                chosenOne.Add(Random.Range(-1f, 1f));
            }
        }
        return chosenOne;
    }

    void CrossOver(ref List<float> mum, ref List<float> dad, ref List<float> baby1, ref List<float> baby2)
    {
        if((Random.Range(0f,1f)) > crossOverRate || (mum.Equals(dad)))//Crossover gercekleşmezse
        {
            baby1 = mum;
            baby2 = dad;
            return;
        }
        else //Crossover gerçekleştiği zaman
        {
            int crossoverPoint = Random.Range(0, chromosomeLength - 1);

            for (int beforePointIndex = 0; beforePointIndex < crossoverPoint; beforePointIndex++)
            {
                baby1.Add(mum[beforePointIndex]);
                baby2.Add(dad[beforePointIndex]);
            }

            for (int afterPointIndex = crossoverPoint; afterPointIndex < mum.Count; afterPointIndex++)
            {
                baby1.Add(dad[afterPointIndex]);
                baby2.Add(mum[afterPointIndex]);
            }
        }
        
    }


    public List<(List<float> weightList, float genomeFitness)> Epoch(ref List<(List<float> weightList, float genomeFitness)> oldPopulation)
    {
        Population = oldPopulation;
        Reset();

        oldPopulation.Sort((x, y) => y.genomeFitness.CompareTo(x.genomeFitness));

        CalculateBestWorstAvTot();

        List<(List<float> weightList, float genomeFitness)> newPopulation = new List<(List<float> weightList, float genomeFitness)>();

        if(NumOfCopiesElite * NumElites % 2 == 0)
        {
            GrabNBest(NumElites , NumOfCopiesElite, ref newPopulation);
        }
        while(newPopulation.Count < populationSize)
        {
            List<float> mum = GetChromoRoulettte();
            List<float> dad = GetChromoRoulettte();

            List<float> baby1 = new List<float>();
            List<float> baby2 = new List<float>();

            CrossOver(ref mum, ref dad, ref baby1, ref baby2);

            Mutate(ref baby1);
            Mutate(ref baby2);

            newPopulation.Add((baby1, 0.0f));
            newPopulation.Add((baby2, 0.0f));
        }

        return newPopulation;
    }

    //Elitisim için en iyi bireyin bulunması
    void GrabNBest(int nBest, int numberOfCopies, ref List<(List<float> weightList, float genomeFitness)> populationReference)
    {
            while (nBest > 0)
            {
                for (int bestIndex = 0; bestIndex < numberOfCopies; bestIndex++)
                {
                    populationReference.Add((Population[nBest].weightList, 0.0f));
                }
                nBest--;
            }
    }

    void CalculateBestWorstAvTot()
    {
        

    }

    void Reset()
    {
        totalFitness = 0;
        BestFitness = 0;
        worstFitness = 0;
        AverageFitness = 0;
    }
  public float BestFitness { get => bestFitness; set => bestFitness = value; }
    public float AverageFitness { get => totalFitness / populationSize; set => averageFitness = value; }
    public List<(List<float> weightList, float genomeFitness)> Population { get => population; set => population = value; }

}