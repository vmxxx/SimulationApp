using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Linq;

//using static System.Math;

public class RunSimulation : MonoBehaviour
{
    public static RunSimulation instance;


    public Dictionary<(int, int), agent> agents = new Dictionary<(int, int), agent>();
    public float V = 0.4f;
    public float C = 1f;
    private int totalAmountOfIndividuals = 0;

    private List<agent> hawksArray = new List<agent>();
    private List<agent> dovesArray = new List<agent>();

    private float totalHawkFitness = 0f;
    private float totalDoveFitness = 0f;

    private Dictionary<(int, int), float> payoffResults = new Dictionary<(int, int), float>();

    public bool paused = false;
    private bool initialized = false;

    private int itr = 0;

    private void recalculateFormulas()
    {
        if (payoffResults.ContainsKey((10, 10))) { payoffResults[(10, 10)] = (V / 2) - (C / 2); }
        else { payoffResults.Add(((10, 10)), (V / 2) - (C / 2)); }
        if (payoffResults.ContainsKey((10, 9))) { payoffResults[(10, 9)] = V; }
        else { payoffResults.Add(((10, 9)), V); }
        if (payoffResults.ContainsKey((9, 10))) { payoffResults[(9, 10)] = 0; }
        else { payoffResults.Add(((9, 10)), 0); }
        if (payoffResults.ContainsKey((9, 9))) { payoffResults[(9, 9)] = V / 2; }
        else { payoffResults.Add(((9, 9)), V / 2); }
        /* payoffFormulas[entry.Key].result = PayoffFormula[entry.Key].payoffFormula*/








    }

    private void initialize()
    {

        for (int i = 0; i < 4000; i++)
        {
            agent newHawk = new agent();
            newHawk.ID = 10;
            newHawk.icon = "icon";
            newHawk.agentName = "Hawk";
            newHawk.agentDescription = "description";
            newHawk.authorID = 1;
            newHawk.fitness = 0;

            hawksArray.Add(newHawk);
            totalAmountOfIndividuals = totalAmountOfIndividuals + 1;
        }



        for (int i = 0; i < 6000; i++)
        {
            agent newDove = new agent();
            newDove.ID = 9;
            newDove.icon = "icon";
            newDove.agentName = "Dove";
            newDove.agentDescription = "description";
            newDove.authorID = 1;
            newDove.fitness = 0;

            dovesArray.Add(newDove);
            totalAmountOfIndividuals = totalAmountOfIndividuals + 1;
        }

        //compilePayoffFormulas();

    }


    // Start is called before the first frame update
    private void FixedUpdate()
    {
        //playOneRound();
    }

    public void playOneRound()
    {
        if (initialized == false) { initialize(); initialized = true; }
        if (paused == false/* && itr % 200 == 0*/)
        {
            //1st phase
            //Debug.Log("BEFORE");
            //printFitness();

            //2nd phase
            recalculateFormulas();
            assignIndividualsInPairwiseContestsAndCalculateTheirFitness();
            Debug.Log("In_BETWEEN");
            printFitness();

            //3rd phase
            killOrDuplicateEachIndividual();

            //4th phase
            //Debug.Log("AFTER");
            //printFitness();
            printAmountOfIndividuals();
        }
        itr++;
    }

    private void printAmountOfIndividuals()
    {
        Debug.Log("Hawks / Doves   (total_amount_of_individuals): " + hawksArray.Count + " / " + dovesArray.Count + "   (" + totalAmountOfIndividuals + ");    Hawk frequency = " + hawksArray.Count + " / " + totalAmountOfIndividuals + " = " + ((float)hawksArray.Count / (float)totalAmountOfIndividuals) + ";");
    }

    private void printFitness()
    {
        totalHawkFitness = 0f;
        totalDoveFitness = 0f;
        for (int i = 0; i < hawksArray.Count; i++)
        {
            totalHawkFitness = totalHawkFitness + hawksArray.ElementAt(i).fitness;
        }
        for (int i = 0; i < dovesArray.Count; i++)
        {
            totalDoveFitness = totalDoveFitness + dovesArray.ElementAt(i).fitness;
        }
        Debug.Log("totalHawkFitness / totalDoveFitness: " + totalHawkFitness + " / " + totalDoveFitness + ";");
        /*
        float totalW = (((float)hawksArray.Count / (float)totalAmountOfIndividuals) * totalHawkFitness) + (((float)dovesArray.Count / (float)totalAmountOfIndividuals) * totalDoveFitness);
        float a = ((((float)hawksArray.Count / (float)totalAmountOfIndividuals) * totalHawkFitness) / totalW);
        float b = ((((float)dovesArray.Count / (float)totalAmountOfIndividuals) * totalDoveFitness) / totalW);
        Debug.Log("total W = ((hawksArray.Count/totalAmountOfIndividuals) * totalHawkFitness) + ((dovesArray.Count/totalAmountOfIndividuals) * totalDoveFitness): " + (((float)hawksArray.Count / (float)totalAmountOfIndividuals) * totalHawkFitness) + " + " + (((float)dovesArray.Count / (float)totalAmountOfIndividuals) + totalDoveFitness) +" = " + totalW + ";");
        Debug.Log("((hawksArray.Count/totalAmountOfIndividuals) * totalHawkFitness) / total W = " + (((float)hawksArray.Count / (float)totalAmountOfIndividuals) * totalHawkFitness) + " / " + totalW + " = " + a);
        Debug.Log("((dovesArray.Count/totalAmountOfIndividuals) * totalDoveFitness) / total W = " + (((float)dovesArray.Count / (float)totalAmountOfIndividuals) * totalDoveFitness) + " / " + totalW + " = " + b);
        /**/
        //Debug.Log("totalHawkFitness in % / totalDoveFitness in %: " + totalHawkFitness/hawksArray.Count + " / " + totalDoveFitness/dovesArray.Count + ";");

        /*
        for (int i = 0; i < hawksArray.Count; i++)
        {
            Debug.Log(hawksArray.ElementAt(i).fitness);
        }
        Debug.Log(" ");
        for (int i = 0; i < dovesArray.Count; i++)
        {
            Debug.Log(dovesArray.ElementAt(i).fitness);
        }
        /**/
    }

    private void killOrDuplicateEachIndividual()
    {
        int ii = 0;
        int duplicatesToMake = 0;
        List<int> indexes = new List<int>();
        foreach (agent individual in hawksArray)
        {
            if (individual.fitness <= -1)
            {
                indexes.Add(ii);
            }
            else if (individual.fitness >= 1)
            {
                duplicatesToMake = duplicatesToMake + (int)System.Math.Floor(individual.fitness);
                individual.fitness = individual.fitness - (int)System.Math.Floor(individual.fitness);
            }
            //individual.fitness = 0;
            ii++;
        }
        for (int i = 0; i < indexes.Count; i++)
        {
            totalAmountOfIndividuals = totalAmountOfIndividuals - 1;
            hawksArray.RemoveAt(indexes[i] - i);
        }
        for (int i = 0; i < duplicatesToMake; i++)
        {
            agent newHawk = new agent();
            newHawk.ID = 10;
            newHawk.icon = "icon";
            newHawk.agentName = "Hawk";
            newHawk.agentDescription = "description";
            newHawk.authorID = 1;
            newHawk.fitness = 0;

            hawksArray.Add(newHawk);
            totalAmountOfIndividuals = totalAmountOfIndividuals + 1;
        }


        ii = 0;
        duplicatesToMake = 0;
        indexes = new List<int>();
        foreach (agent individual in dovesArray)
        {
            if (individual.fitness <= -1)
            {
                indexes.Add(ii);
            }
            else if (individual.fitness >= 1)
            {
                duplicatesToMake = duplicatesToMake + (int)System.Math.Floor(individual.fitness);
                individual.fitness = individual.fitness - (int)System.Math.Floor(individual.fitness);
            }
            //individual.fitness = 0;
            ii++;
        }
        for (int i = 0; i < indexes.Count; i++)
        {
            totalAmountOfIndividuals = totalAmountOfIndividuals - 1;
            dovesArray.RemoveAt(indexes[i] - i);
        }
        for (int i = 0; i < duplicatesToMake; i++)
        {
            agent newDove = new agent();
            newDove.ID = 9;
            newDove.icon = "icon";
            newDove.agentName = "Dove";
            newDove.agentDescription = "description";
            newDove.authorID = 1;
            newDove.fitness = 0;

            dovesArray.Add(newDove);
            totalAmountOfIndividuals = totalAmountOfIndividuals + 1;
        }
    }

    private void assignIndividualsInPairwiseContestsAndCalculateTheirFitness()
    {
        List<agent> alreadyAssigneHawks = new List<agent>();
        List<agent> alreadyAssigneDoves = new List<agent>();
        int totalHawkHawkContests = 0;
        int totalHawkDoveContests = 0;
        int totalDoveHawkContests = 0;
        int totalDoveDoveContests = 0;

        int totalContests = 0;

        System.Random rand = new System.Random();
        int x;
        int xx = 2;
        bool hawks_deleted = false;
        bool doves_deleted = false;
        string[] agents_array = new string[] { "hawks", "doves" };

        if (!hawksArray.Any() && hawks_deleted == false) { agents_array = agents_array.Where((val, idx) => val != "hawks").ToArray(); xx = xx - 1; hawks_deleted = true; }
        if (!dovesArray.Any() && doves_deleted == false) { agents_array = agents_array.Where((val, idx) => val != "doves").ToArray(); xx = xx - 1; doves_deleted = true; }

        if (totalAmountOfIndividuals % 2 == 1)
        {
            x = rand.Next(xx);
            if (agents_array[x] == "hawks")
            {
                int ii = rand.Next(hawksArray.Count);
                alreadyAssigneHawks.Add(hawksArray[ii]);

                hawksArray.Remove(hawksArray[ii]);
            }
            else if (agents_array[x] == "doves")
            {
                int ii = rand.Next(dovesArray.Count);
                alreadyAssigneDoves.Add(dovesArray[ii]);

                dovesArray.Remove(dovesArray[ii]);
            }
        }
        /**/

        agent agent1 = new agent();
        agent agent2 = new agent();



        for (int i = 0; i < totalAmountOfIndividuals / 2; i++)
        {

            if (!hawksArray.Any() && hawks_deleted == false) { agents_array = agents_array.Where((val, idx) => val != "hawks").ToArray(); xx = xx - 1; hawks_deleted = true; }
            if (!dovesArray.Any() && doves_deleted == false) { agents_array = agents_array.Where((val, idx) => val != "doves").ToArray(); xx = xx - 1; doves_deleted = true; }

            x = rand.Next(xx);

            if (agents_array[x] == "hawks")
            {
                int ii = rand.Next(hawksArray.Count);
                agent1 = hawksArray[ii];
                alreadyAssigneHawks.Add(hawksArray[ii]);

                hawksArray.Remove(hawksArray[ii]);
            }
            /**/
            else if (agents_array[x] == "doves")
            {
                int ii = rand.Next(dovesArray.Count);
                agent1 = dovesArray[ii];
                alreadyAssigneDoves.Add(dovesArray[ii]);

                dovesArray.Remove(dovesArray[ii]);
            }

            if (!hawksArray.Any() && hawks_deleted == false) { agents_array = agents_array.Where((val, idx) => val != "hawks").ToArray(); xx = xx - 1; hawks_deleted = true; }
            if (!dovesArray.Any() && doves_deleted == false) { agents_array = agents_array.Where((val, idx) => val != "doves").ToArray(); xx = xx - 1; doves_deleted = true; }

            x = rand.Next(xx);
            if (agents_array[x] == "hawks")
            {
                int ii = rand.Next(hawksArray.Count);
                agent2 = hawksArray[ii];
                alreadyAssigneHawks.Add(hawksArray[ii]);

                hawksArray.Remove(hawksArray[ii]);
            }
            else if (agents_array[x] == "doves")
            {
                int ii = rand.Next(dovesArray.Count);
                agent2 = dovesArray[ii];
                alreadyAssigneDoves.Add(dovesArray[ii]);

                dovesArray.Remove(dovesArray[ii]);
            }


            //Debug.Log("INDEX: " + i);
            //Debug.Log("{ totalAmountOfIndividuals: " + totalAmountOfIndividuals + " } { hawksArray.Count: " + hawksArray.Count + " } { alreadyAssigneHawks.Count: " + alreadyAssigneHawks.Count + " } { dovesArray.Count: " + dovesArray.Count + "{ alreadyAssigneDoves.Count: " + alreadyAssigneDoves.Count + "}");

            calculateIndividualsFitness(agent1, agent2, i);
            if (agent1.agentName == "Hawk" && agent2.agentName == "Hawk") { totalHawkHawkContests++; totalContests++; }
            else if (agent1.agentName == "Hawk" && agent2.agentName == "Dove") { totalHawkDoveContests++; totalContests++; }
            else if (agent1.agentName == "Dove" && agent2.agentName == "Hawk") { totalDoveHawkContests++; totalContests++; }
            else if (agent1.agentName == "Dove" && agent2.agentName == "Dove") { totalDoveDoveContests++; totalContests++; }
            /**/
        }
        /*
        int b = totalAmountOfIndividuals / 2;
        float a = (((float)totalHawkHawkContests) / b);
        float c = (((float)totalHawkDoveContests) / b);
        float d = (((float)totalDoveHawkContests) / b);
        float e = (((float)totalDoveDoveContests) / b);
        Debug.Log("totalHawkHawkContests / totalContests = " + (totalHawkHawkContests) + " / " + totalContests + " = " + a);
        Debug.Log("totalHawkDoveContests / totalContests = " + (totalHawkDoveContests) + " / " + totalContests + " = " + c);
        Debug.Log("totalDoveHawkContests / totalContests = " + (totalDoveHawkContests) + " / " + totalContests + " = " + d);
        Debug.Log("totalDoveDoveContests / totalContests = " + (totalDoveDoveContests) + " / " + b + " = " + e);
        //float a = (((float)totalHawkHawkContests * (float)2) / b);
        //float c = (((float)totalHawkDoveContests * (float)2) / b);
        //float d = (((float)totalDoveHawkContests * (float)2) / b);
        //float e = (((float)totalDoveDoveContests * (float)2) / b);
        //Debug.Log("totalHawkHawkContests*2 / totalContests = " + (totalHawkHawkContests * 2) + " / " + totalContests + " = " + a);
        //Debug.Log("totalHawkDoveContests*2 / totalContests = " + (totalHawkDoveContests * 2) + " / " + totalContests + " = " + c);
        //Debug.Log("totalDoveHawkContests*2 / totalContests = " + (totalDoveHawkContests * 2) + " / " + totalContests + " = " + d);
        //Debug.Log("totalDoveDoveContests*2 / totalContests = " + (totalDoveDoveContests * 2) + " / " + b + " = " + e);
        /**/


        hawksArray = alreadyAssigneHawks;
        dovesArray = alreadyAssigneDoves;
        /**/

    }

    private void calculateIndividualsFitness(agent agent1, agent agent2, int i)
    {
        //Debug.Log("V: " + V + "C: " + C);
        //Debug.Log("i: " + i + ", BEFORE: (" + agent1.agentName + ", " + agent2.agentName + "): " + (agent1.fitness, agent2.fitness));
        //Debug.Log("(agent1.agentName, agent2.agentName): " + (agent1.agentName, agent2.agentName) + " payoffResults[(agent1, agent2)]: " + payoffResults[(agent1.ID, agent2.ID)]);
        //Debug.Log("BEFORE (agent1.fitness, agent2.fitness): " + (agent1.fitness, agent2.fitness));
        agent1.fitness = agent1.fitness + payoffResults[(agent1.ID, agent2.ID)];
        agent2.fitness = agent2.fitness + payoffResults[(agent2.ID, agent1.ID)];

        //Debug.Log("i: " + i + ", AFTER: (" + agent1.agentName + ", " + agent2.agentName + "): " + (agent1.fitness, agent2.fitness));
        /*
        if (agent1.fitness == agent2.fitness && (agent1.fitness <= -1 && agent2.fitness <= -1))
        {

        }
        else if (agent1.fitness <= -1 && agent1.fitness < agent2.fitness)
        {
            float x = agent1.fitness - 1;
            agent1.fitness = agent1.fitness - x;
            agent2.fitness = agent2.fitness - x;
            Debug.Log("float x: " + x);
        }
        else if (agent2.fitness <= -1 && agent2.fitness < agent1.fitness)
        {
            float x = agent2.fitness - 1;
            agent1.fitness = agent1.fitness - x;
            agent2.fitness = agent2.fitness - x;
            Debug.Log("float x: " + x);
        }
        /**/
    }
}

public class agent
{
    public int ID;
    public string icon;
    public string agentName;
    public string agentDescription;
    public int authorID;

    public float fitness;
}

