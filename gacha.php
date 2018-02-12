<?php
$dsn = '';
$user = '';
$password = '';
 
try {

  $dbh = new PDO($dsn, $user, $password);

} catch (PDOException $e) {
  echo 'データベースにアクセスできません' . $e->getMessage();
  exit;
}

$owner_id = $_REQUEST['owner_id'];
$name = $_REQUEST['name'];
$id_str = $_REQUEST['id_str'];
$hp = $_REQUEST['hp'];
$speed = $_REQUEST['speed'];
$attribute = $_REQUEST['attribute'];
$attack_force = $_REQUEST['attack_force'];
$defence_force = $_REQUEST['defence_force'];
$image_url = $_REQUEST['image_url'];
$is_regular = $_REQUEST['is_regular'];

$skill = array($_REQUEST['skill0'],$_REQUEST['skill1'],$_REQUEST['skill2']);


$query = "INSERT INTO  `tsuyomilog_twittergame`.`followers` (
`auto_id` ,
`owner_id` ,
`name` ,
`id_str` ,
`hp` ,
`speed` ,
`attribute` ,
`attack_force`,
`defence_force`,
`image_url` ,
`is_regular`,
`skill0`,
`skill1`,
`skill2`
)
VALUES (
NULL ,  '$owner_id',  '$name',  '$id_str',  '$hp',  '$speed',  '$attribute', '$attack_force', '$defence_force', '$image_url',  '$is_regular',
'$skill[0]', '$skill[1]', '$skill[2]'
)";


if(!$result = $mysqli->query( $query )) {
	die( 'Error data set: ' . $mysqli->connect_error() );
}
$mysqli->close();

?>