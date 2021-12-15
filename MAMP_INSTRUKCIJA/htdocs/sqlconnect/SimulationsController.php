<?php
namespace SimulationsController;
include_once "CoreController.php";
use CoreController\core as core;

class simulations extends core
{
    public function create($data)
    {
		$name = $data["name"];
		$image = $data["image"];
		$description = $data["description"];
		$authorID = $data["authorID"];
        $namecheckquery = "INSERT INTO simulations (name, image, description, likesCount, dislikeCount, authorID) VALUES (\"$name\", \"$image\", \"$description\", 0, 0, $authorID);";
        //$namecheckquery = "INSERT INTO simulations (name, image, description, likesCount, dislikeCount, authorID) VALUES (\"name\", \"image\", \"description\", 0, 0, 5);";
        //$namecheckquery = "INSERT INTO agents (icon, name, description, authorID) VALUES (\"icon\", \"name\", \"description\", 5);";
		$this -> con -> query ($namecheckquery) or die("sql failed!");
		
		for($i = 1; $i <= $data["agentCount"]; $i++)
		{
			for($j = 1; $j <= $data["agentCount"]; $j++)
			{
				$agent1 = $data[$i."_".$j."_payoffFormula_agent1"];
				$agent2 = $data[$i."_".$j."_payoffFormula_agent2"];
				$payoffFormula = $data[$i."_".$j."_payoffFormula_payoffFormula"];
				$simulationID = $data[$i."_".$j."_payoffFormula_authorID"];
				
				$namecheckquery2 = "INSERT INTO payoffFormulas (agent1, agent2, payoffFormula, authorID) VALUES ($agent1, $agent2, \"$payoffFormula\", $simulationID);";
				
				$this -> con -> query ($namecheckquery2);
			}
		}
		/**/
		
		echo "0;";
    }

    public function read($data)
    {		
		
        $authorID = $data["authorID"];

        $namecheckquery1 = 'SELECT * FROM simulations WHERE authorID = '.$authorID.';';
		$namecheckquery2 = 'SELECT * FROM simulations LIMIT 10;';

        $result1 = $this -> con -> query ($namecheckquery1);
		echo '0;'.$result1->num_rows.'{';
		for($i = 0; $i < $result1->num_rows; $i++)
		{
			$row = $result1 -> fetch_assoc();
			echo'{ID:'.$row["ID"].', name:"'.$row["name"].'", image:"'.$row["image"].'", description:"'.$row["description"].'", likesCount:'.$row["likesCount"].', dislikesCount:'.$row["dislikeCount"].', authorID:'.$row["authorID"].'}';
		}
        $result2 = $this -> con -> query ($namecheckquery2);
		echo '}0;'.$result2->num_rows.'{';
		for($i = 0; $i < $result2->num_rows; $i++)
		{
			$row = $result2 -> fetch_assoc();
			echo'{ID:'.$row["ID"].', name:"'.$row["name"].'", image:"'.$row["image"].'", description:"'.$row["description"].'", likesCount:'.$row["likesCount"].', dislikesCount:'.$row["dislikeCount"].', authorID:'.$row["authorID"].'}';
		}
		echo '}0;';
    }

    public function update()
    {
		/*
		$icon = $data["ID"];
		$name = $data["name"];
		$name = $data["image"];
		$description = $data["description"];
		$description = $data["likesCount"];
		$description = $data["dislikesCount"];
		$authorID = $data["authorID"];
        $namecheckquery = "INSERT INTO simulations (name, image, description, likesCount, dislikesCount, authorID) VALUES (\"$name\", \"$image\", \"$description\", 0, 0, $authorID);";
		
        $namecheckquery2 = "UPDATE simulations SET name = \"$name\", image = \"$iamge\", description = \"$description\", likesCount = 0, dislikesCount = 0 WHERE simulations.ID = $ID;";
		$this -> con -> query ($namecheckquery);
		$this -> con -> query ($namecheckquery2);
		echo "0;";
		/**/
    }

    public function delete()
    {
		/*
		$ID = $data["ID"];
		$namecheckquery = "DELETE FROM simulations WHERE simulations.ID = $ID AND simulations.authorID != 0;";
		$namecheckquery2 = "DELETE FROM payoffFormulas WHERE payoffFormulas.authorID = $ID";
		$this -> con -> query ($namecheckquery);
		$this -> con -> query ($namecheckquery2);
		echo "0;";
		/**/
    }

}
?>