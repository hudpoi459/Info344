<?php
	$num = $_REQUEST['n'];
	for ($x=2; $x <= $num; $x++) {
		if ($x % 2 == 0) {
			echo $x." ";
		}
		
	}
?>
