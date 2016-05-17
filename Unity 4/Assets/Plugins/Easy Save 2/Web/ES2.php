<?php
/*
ES2.php. 
Copyright© Moodkie
Should be used with Easy Save 2 from Moodkie Interactive.
*/

// *** MYSQL DB DETAILS ***
// Replace these with your own database details.

$db_host 		= 	"localhost"; 	// MySQL Host Name.
$db_user 		= 	"root"; 		// MySQL User Name.
$db_password 	= 	"myDatabasePassword"; 	// MySQL Password.
$db_name 		= 	"es2"; 			// MySQL Database Name.

// *** GENERAL DETAILS ***
// If you are not using your own login system, replace these with your Unity username and password.

$unityUsername = "ES2";				//This should match the webUsername set in Unity.
$unityPassword = "65w84e4p994z3Oq"; //This should match the webPassword set in Unity.

// *** LOGIN AUTHENTICATION ***

// If using your own login system, replace the contents of this method with your own authentication.
// Note that by default ES2Web sends the password as an MD5 hash.
// If you want to receive the password unhashed, set the hashingType variable of your ES2Web object to ES2Web.HashingType.None.
// Remember to return true if authentication was successful, otherwise return false.

function Authenticate($username, $password)
{
	global $unityUsername, $unityPassword;
	if($username != $unityUsername || $password != md5($unityPassword))
		return false; // Username or password does not match.
	else
		return true; // Username and password both match.
}





/* 
------------------------------------------------------------------------------------
			YOU SHOULD NOT NEED TO MODIFY ANYTHING BELOW THIS LINE.
------------------------------------------------------------------------------------
*/

// *** MYSQL STRUCTURE DETAILS ***

$fileTableName = "es2files";	// The name of the table used to store file names.
$dataTableName = "es2data";		// The name of the table used to store ES2 data.
$fileFieldName = "filename";	// The name of the field where we save our file names.
$filePrimaryName = "id";		// The name of the field containing the primary key for filenames.
$tagFieldName = "tag"; 			// The name of the field where you want to save the tag.
$dataFieldName = "data"; 	// The name of the field where you want to save the data.
$fileIDFieldName = "fileId";		// An index into the file table representing the file containing this tag.

// Attempt to connect to database.
$mysqli = new mysqli();
$mysqli->connect($db_host, $db_user, $db_password, $db_name);

if(mysqli_connect_error())
{
	if(!isset($_POST['mode']))
	{
		echo "Could not connect to database. ES2.php or MySQL database are incorrectly configured.";
		exit();
	}
	
	echo "01"; // Could not connect to database with these credentials.
	exit();
}

if(!isset($_POST['mode']))
{
	echo "ES2.php and MySQL database are working correctly.";
	exit();
}

// If user hasn't logged on or has forgot to enter any login details.
if(Authenticate($_POST['username'], $_POST['password']) == false)
{
	echo "02"; // Wrong Unity username or password.
	exit();
}

/*
	*** UPLOAD FUNCTIONALITY ***
*/
if($_POST["mode"] == "upload")
{
	$filePath = $_FILES["data"]["tmp_name"];
	
	// If file doesn't exist or it contains no data, throw an error.
	if(!file_exists($filePath) || filesize($filePath) == 0)
	{
		echo "03"; // No data was received from Unity.
		exit();
	}
	
	$fileHandle = fopen($filePath, 'r');
	
	$data = '';
	
	// Get bytes from file.
	while (!feof($fileHandle)) 
	{
    	$data .= fgets($fileHandle);
	}
	fclose($fileHandle); // Remember to close the file stream.
		
	// Generate a secure prepared statement for each query.
	if(		($existsStatement = $mysqli -> prepare("SELECT $filePrimaryName FROM $fileTableName WHERE $fileFieldName = ?")) &&
			($fileStatement = $mysqli -> prepare("INSERT INTO $fileTableName ($fileFieldName) VALUES (?)")) &&
			($dataStatement = $mysqli -> prepare("INSERT INTO $dataTableName VALUES (?,?,?) 
													ON DUPLICATE KEY UPDATE $dataFieldName=VALUES($dataFieldName)")))
	{
		// Check if file already exists, 
		// and set $fileId to the id of this file if it does.
		$existsStatement -> bind_param("s", $_POST['filename']);
		$existsStatement -> execute();
		$existsStatement -> bind_result($fileId);
		$existsStatement -> fetch();
		$existsStatement -> close();
		
		// If no file with this name exists, create it in file table.
		if($fileId == "")
		{
			$fileStatement -> bind_param("s", $_POST['filename']);
			$fileStatement -> execute();
			$fileId = $mysqli -> insert_id;
			$fileStatement -> close();
		}
		
		// Add/update data and tag in data table.
		$dataStatement -> bind_param("ssi", $_POST['tag'], $data, $fileId);
		$dataStatement -> execute();
		$dataStatement -> close();
	}
	else
	{
		echo "04"; // ES2 MySQL table could not be found.
		exit();
	}
}

/*
	*** DOWNLOAD FUNCTIONALITY ***
*/
else if($_POST["mode"] == "download")
{
	// If we're looking to load a single tag of data.
	if($_POST["type"] == "tag")
		$getDataStatement = $mysqli -> prepare("SELECT $dataFieldName FROM $dataTableName
												INNER JOIN $fileTableName
												ON $filePrimaryName = $fileIDFieldName
												WHERE $tagFieldName = ?
												AND $fileFieldName = ?");
	// Else we're looking to load an entire file.
	else
		$getDataStatement = $mysqli -> prepare("SELECT $dataFieldName FROM $dataTableName
												INNER JOIN $fileTableName
												ON $filePrimaryName = $fileIDFieldName
												WHERE $fileFieldName = ?");
												
	// Check our statement is valid.
	if($getDataStatement)
	{
		if($_POST["type"] == "tag")
			$getDataStatement -> bind_param("ss", $_POST['tag'], $_POST['filename']);
		else
			$getDataStatement -> bind_param("s", $_POST['filename']);
		$getDataStatement -> execute();
		$getDataStatement -> store_result();
		$getDataStatement -> bind_result($data);
		
		$isData = false;
		
		// Fetch and output our data.
		while($getDataStatement -> fetch())
		{
			$isData = true;
			echo $data;
		}
		
		if(!$isData)
		{
			echo "05"; // Data does not exist on this server.
			exit();
		}
		
		$getDataStatement -> close();
	}
	else
	{
		echo "04"; // ES2 MySQL table could not be found.
		exit();
	}
}
/*
	*** DELETE FUNCTIONALITY ***
*/
else if($_POST["mode"] == "delete")
{
	// If we're looking to delete a single tag of data.
	if($_POST["type"] == "tag")
		$getDataStatement = $mysqli -> prepare("DELETE $dataTableName FROM $dataTableName
												INNER JOIN $fileTableName
												ON $filePrimaryName = $fileIDFieldName
												WHERE $tagFieldName = ?
												AND $fileFieldName = ?");
	// Else we're looking to delete an entire file.
	else
	{
		$getDataStatement = $mysqli -> prepare("DELETE FROM $fileTableName WHERE $fileFieldName = ?");
	}
	// Check that statement is valid.											
	if($getDataStatement)
	{
		if($_POST["type"] == "tag")
			$getDataStatement -> bind_param("ss", $_POST['tag'], $_POST['filename']);
		else
			$getDataStatement -> bind_param("s", $_POST['filename']);
			
		$getDataStatement -> execute();
		$getDataStatement -> close();
	}
	else
	{
		echo "04"; // ES2 MySQL table could not be found.
		exit();
	}
}
	// Close our connection.
	$mysqli -> close();

?>