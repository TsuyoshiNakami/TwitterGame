<?php

// データベースへのアクセスに必要な各種パラメータ（アップするので一時的に削除しました）
$dsn = '';
$user = '';
$password = '';
 
try {

  $dbh = new PDO($dsn, $user, $password);

} catch (PDOException $e) {
  echo 'データベースにアクセスできません' . $e->getMessage();
  exit;
}

// C#からパラメータを受け取る
$sql = $_REQUEST['sql'];
$stmt = $dbh -> query($sql);

// 実行結果を全取得
$user = array();
$user = $stmt->fetchAll();

// 出力結果が空の時は、nullを出し、JSON形式で変換.
if(empty($user)) {
	$user = null;
	echo json_encode( $user );
}else{
	echo json_encode( $user );
}

?>