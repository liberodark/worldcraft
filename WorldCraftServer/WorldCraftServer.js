/***********************************

	*	Serveur WorldCraft	*
	*	Développé par Jey	*

************************************/

// Chargement des modules ================================================================
var express = require("express");
var app = express(), server = require('http').createServer(app);
var bodyParser = require('body-parser');

var world = require('./Basics/World');
var w = new world();


//Support du passage de parametres par URL et par JSON
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));

app.get("/map",function(req,res,next){
	res.status(200).send(w.getBlock(req.params.x,req.params.y,req.params.z));
});

app.get("*",function(req,res,next){
	res.status(404).send("Not found");
});

server.listen(1992);
console.log('Serveur en attente sur le port 1992... ');
