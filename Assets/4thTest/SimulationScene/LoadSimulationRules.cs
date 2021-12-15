using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

using UnityEditor;

using System.IO;
using System.Linq;


public class LoadSimulationRules : MonoBehaviour
{
    public bool assetDatabaseRefreshed = false;

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(loadPayoffFormulas());
    }

    public void compilePayoffFormulas()
    {
        int i = 34;
        List<string> txtLines = File.ReadAllLines("Assets/4thTest/SimulationScene/RunSimulation.txt").ToList();   //Fill a list with the lines from the txt file.
        Debug.Log("TXT_LINESSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS: ");
        foreach (KeyValuePair<(int, int), PayoffFormula> entry in Buffer.instance.payoffFormulas)
        {
            txtLines.Insert(i, "if (payoffResults.ContainsKey(" + entry.Key + ")) { payoffResults[" + entry.Key + "] = " + Buffer.instance.payoffFormulas[entry.Key].payoffFormula + "; }");
            i++;
            txtLines.Insert(i, "else { payoffResults.Add((" + entry.Key + "), " + Buffer.instance.payoffFormulas[entry.Key].payoffFormula + " ); }");
            i++;
        }
        File.WriteAllLines("Assets/4thTest/SimulationScene/RunSimulation.cs", txtLines);
    }

    public void compileANewBufferInitializer()
    {
        List<string> txtLines = File.ReadAllLines("Assets/4thTest/Buffer.txt").ToList();   //Fill a list with the lines from the txt file.
        txtLines.Insert(37, "authenticatedUser.ID = " + Buffer.instance.authenticatedUser.ID + ";");
        txtLines.Insert(38, "authenticatedUser.username = \"" + Buffer.instance.authenticatedUser.username + "\";");
        txtLines.Insert(39, "currentSimulation.ID = " + Buffer.instance.currentSimulation.ID + ";");
        File.WriteAllLines("Assets/4thTest/Buffer.cs", txtLines);
    }

    // Update is called once per frame
    IEnumerator loadPayoffFormulas()
    {
        //Get its payoffFormulas
        WWWForm form = new WWWForm();
        form.AddField("class", "PayoffFormulasController\\payoffFormulas");
        form.AddField("function", "read");
        //form.AddField("authorID", Buffer.instance.authenticatedUser.ID);
        form.AddField("simulationID", 1);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {
            Debug.Log(www.text);

            foreach (Match match in Regex.Matches(www.text, @"{(.*?)}"))
            {
                Debug.Log(match.ToString());

                string seperate_entry = match.ToString();

                Debug.Log("seperate_entry: " + seperate_entry);

                string ID = Regex.Match(seperate_entry, @"ID:(.*?),").Value;
                string agent1 = Regex.Match(seperate_entry, @"agent1:(.*?),").Value;
                string agent2 = Regex.Match(seperate_entry, @"agent2:(.*?),").Value;
                string payoffFormula = Regex.Match(seperate_entry, @"payoffFormula:(.*?),").Value;
                string authorID = Regex.Match(seperate_entry, @"authorID:(.*?)}").Value;

                //int a1 = Int32.Parse(agent1);
                //int a2 = Int32.Parse(agent1);

                Debug.Log("PAYOFF_FORMULA: " + payoffFormula);
                if (ID != "")
                {

                    Debug.Log("PAYOFF_FORMULA: " + payoffFormula);
                    Buffer.instance.newFormula = new PayoffFormula();

                    Buffer.instance.newFormula.ID = Int32.Parse(ID.Substring(3, ID.Length - 4));
                    Buffer.instance.newFormula.agent1 = Int32.Parse(agent1.Substring(7, agent1.Length - 8));
                    Buffer.instance.newFormula.agent2 = Int32.Parse(agent2.Substring(7, agent2.Length - 8));
                    Buffer.instance.newFormula.payoffFormula = payoffFormula.Substring(15, payoffFormula.Length - 17);
                    Buffer.instance.newFormula.authorID = Int32.Parse(authorID.Substring(9, authorID.Length - 10));

                    Debug.Log(payoffFormula);
                    Debug.Log(Buffer.instance.newFormula.payoffFormula);
                    /**/

                    Buffer.instance.payoffFormulas.Add((Buffer.instance.newFormula.agent1, Buffer.instance.newFormula.agent2), Buffer.instance.newFormula);
                }
            }
            
            Buffer.instance.printPayoffFormulas();

            Debug.Log("WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW1");

            compilePayoffFormulas();
            compileANewBufferInitializer();
            //Destroy(Buffer.instance.gameObject);
            Debug.Log("Buffer Destroyed!");
            Debug.Log("WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW2");
            AssetDatabase.ImportAsset("Assets/4thTest/SimulationScene/RunSimulation.cs");
            AssetDatabase.ImportAsset("Assets/4thTest/SimulationScene/Buffer.cs");
        }
        else
        {
            Debug.Log("Loading simulations failed. Error #" + www.text);
        }
        //Buffer.instance.reinitialize();

        //(later) Get its agent details
    }
}
