	<!doctype html>
<html lang="en">
<head>
	<meta charset="utf-8">
	<title>NBA Player Stats</title>
	<link rel="stylesheet" href="style.css">
</head>
<body>
	<?php 
		$database = new PDO('mysql:host=info344database.cv2gmoaqx2ww.us-west-2.rds.amazonaws.com;dbname=nbaplayerstats', 'info344user', 'townsen24');
		$strs = array("%".$_POST['pname'],$_POST['pname']."%");
		$query = $database->prepare("SELECT * FROM playerstats WHERE PlayerName LIKE :first OR PlayerName LIKE :second");
		$query->bindParam(':first',$strs[0]);
		$query->bindParam(':second',$strs[1]);
		$query->execute();
		if ($query->rowCount() > 0) {
			$rows = $query->fetchAll();
			foreach ($rows as $row) {
				echo "<div id = \"player\">";
				echo "<div id = \"playerImage\">";
				$name = str_replace(".","",explode(" ", $row['PlayerName']));
				$fname = strtolower($name[0]);
				if (count($name) > 2) {
					$lname = "";
					for ($i = 1; $i < count($name) - 1; $i++) {
						$lname = strtolower($lname.$name[$i])."_";
					}
					$lname = strtolower($lname.$name[count($name) - 1]);
				} else {
					$lname = strtolower($name[1]);
				}
				echo "<img src =\"2013nba_playerphotos\\".$fname."_".$lname.".jpg\">";
				echo "</div>";
				echo "<div id = \"statsLeft\">";
				echo "<div id = \"playerName\">$row[PlayerName]</div>";
				echo "<hr>";
				echo "2012 - 2013 Season<br>";
				echo "<ul>";
				echo "<li>GPb<br>";
				echo str_replace(".","",$row['GP'])."</li>";
				echo "<li>FGP<br>";
				echo str_replace(".","",$row['FGP'])."</li>";
				echo "<li>TPP<br>";
				echo str_replace(".","",$row['TPP'])."</li>";
				echo "<li>FTP<br>";
				echo str_replace(".","",$row['FTP'])."</li>";
				echo "</div>";
				echo "<div id = \"statsRight\">";
				echo "</div>";
				echo "</div>";
			}
		} else {
			echo "<div id = \"player\">";
			echo "Sorry! That player cannot be found";
			echo "</div>";
		}
	?>
</body>
</html>