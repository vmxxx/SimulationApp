<?php
namespace PayoffFormulasController;
include_once "CoreController.php";
use CoreController\core as core;

class payoffFormulas extends core
{
    public function create($data)
    {
    }

    public function read($data)
    {		
		$simulationID = $data["simulationID"];
		$sql = "SELECT * FROM payoffFormulas WHERE authorID = $simulationID;";
		$result = $this -> con -> query ($sql) or die("sql failed!");
		echo '0;'.$result->num_rows;
		for($i = 0; $i < $result->num_rows; $i++)
		{
			$row = $result -> fetch_assoc();
			echo'{ID:'.$row["payoffFormulaID"].', agent1:'.$row["agent1"].', agent2:'.$row["agent2"].', payoffFormula:"'.$row["payoffFormula"].'", authorID:'.$row["authorID"].'}';
		}
    }

    public function update()
    {
		
    }

    public function delete()
    {
		
    }

}
?>