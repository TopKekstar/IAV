/*
 * Copyright (c) by Junjie Chen
 * Please refer to https://unity3d.com/legal/as_terms for the terms and conditions
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Jackyjjc.Bayesianet;

public class GameManager : MonoBehaviour {

    private VariableElimination ve;
	// Use this for initialization
	void Start () {
        //string networkXml = Resources.Load<TextAsset>("enemy_logic_xml").text;
        string networkJson = (Resources.Load("enemy_logic_json") as TextAsset).text;

        //ve = new VariableElimination(new BayesianGenieParser().Parse(networkXml));
        ve = new VariableElimination(new BayesianJsonParser().Parse(networkJson));
        MakeDecision();
    }

    public Text decisionText;
    public Text probabilityText;

    public void MakeDecision()
    {
        // You can specify a list of evidence
        List<string> observations = new List<string> {
            "brave=" + GetIsBrave(),
            "enemy_amount=" + GetEnemyAmount(),
            "cover_type=" + GetCoverType()
        };

        // You can then use them to infer another variable in the network
        double[] fightDistribution = ve.Infer("fight", observations);
        bool fight = ve.PickOne(fightDistribution) == 0;

        // You can do chain interence based on previous inference results
        observations.Add("fight=" + fight);

        // The API functions are overloaded to fit your needs
        // e.g. you can use a less string-based approach if you want to do things programmatically
        BayesianNetwork network = ve.GetNetwork();
        Proposition braveProp = network.FindNode("brave").Instantiate(GetIsBrave());
        Proposition enemyAmountProp = network.FindNode("enemy_amount").Instantiate(GetEnemyAmount());
        Proposition hasCoverProp = network.FindNode("cover_type").Instantiate(GetCoverType());
        Proposition fightProp = network.FindNode("fight").Instantiate(fight.ToString());
        BayesianNode runAwayNode = ve.GetNetwork().FindNode("run_away");
        double[] runawayDistribution = ve.Infer(runAwayNode, braveProp, enemyAmountProp, hasCoverProp, fightProp);
        bool runaway = ve.PickOne(runawayDistribution) == runAwayNode.var.GetTokenIndex("True");

        // Since it is a bayesian network, you can infer any variables with partial or even no information
        ve.Infer("enemy_amount", "fight=True");
        ve.Infer("fight");

        if (enemyAmount.Equals("NoEnemy"))
        {
            decisionText.text = "Did not see any enemy.";
        } else if (fight)
        {
            decisionText.text = "The NPC decided to fight. ";
        } else if (!fight && runaway)
        {
            decisionText.text = "The NPC decided to run away.";
        } else
        {
            decisionText.text = "The NPC decided to wait for his chance.";
        }
        decisionText.text = "Decision made: " + decisionText.text;

        probabilityText.text = string.Format("true: {0}%\t\tfalse: {1}%\ntrue: {2}%\t\tfalse: {3}%", 
            fightDistribution[0] * 100, fightDistribution[1] * 100, runawayDistribution[0] * 100, runawayDistribution[1] * 100);
    }

    public Text enemyAmountSliderText;
    public void SliderValueChange(float sliderValue)
    {
        enemyAmountSliderText.text = string.Format("The number of enemies: {0}", sliderValue);
        enemyAmount = (int)sliderValue;

        MakeDecision();
    }

    // you can map continuous values into discrete ones
    private int enemyAmount;
    private string GetEnemyAmount()
    {
        string result;
        if (enemyAmount == 0) result = "NoEnemy";
        else if (enemyAmount <= 2) result = "Underwhelm";
        else if (enemyAmount == 3) result = "Level";
        else result = "Overwhelm";
        return result;
    }

    public ToggleGroup coverTypeToggleGroup;
    private string GetCoverType()
    {
        return coverTypeToggleGroup.ActiveToggles().First().GetComponentInChildren<Text>().text;
    }

    public Toggle braveToggle;
    private string GetIsBrave()
    {
        return braveToggle.isOn.ToString();
    }
}
