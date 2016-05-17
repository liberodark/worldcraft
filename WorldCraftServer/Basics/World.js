/*****************************************
	* Classe du monde WorldCraft	*
	*	  Développée par Jey		*
******************************************/

var Simplex = require('../Generation/Simplex');

module.exports = function(){
	
	var WATER_LEVEL = 50;
	var MAX_HEIGHT = 512;
	
	this.noiseGenerator = new Simplex();
	this.caveGenerator = new Simplex();
	
	this.World;
	this.heightMap;
	
	
	this.getBlock = function(x, y, z){
		if(y<0)return "air";
		if(!this.World)this.World = new Array();
		if(!this.World[x])this.World[x] = new Array();
		if(!this.World[x][y])this.World[x][y] = new Array();
		if(!this.World[x][y][z])
			this.World[x][y][z] = this.getBiome(x,y,z);
		return this.World[x][y][z];
	}
	
	this.getBiome = function(x,y,z){
		if(!this.heightMap)this.heightMap = new Array();
		if(!this.heightMap[x])this.heightMap[x] = new Array();
		if(!this.heightMap[x][z])this.heightMap[x][z] = Math.floor(this.noiseGenerator.noise(x,z) * MAX_HEIGHT);
		var height = this.heightMap[x][z];
		var cave = this.caveGenerator.noise(x,z)>0.8;
		if(cave && y > WATER_LEVEL)return "air";
		else if(cave && y <= WATER_LEVEL)return "water";
		if(height < y && y > WATER_LEVEL)return "air";
		else if(height < y && y <= WATER_LEVEL)return "water";
		else{
			var res = Math.abs(this.noiseGenerator.noise3d(x,y,z));
			if(res > 0 && res < 0.33)return "sand";
			else if(res >= 0.33 && res < 0.66)return "dirt";
			else return "snow";
		}
	}
	
	this.displayWorld = function(){
		var block = "air";
		var string = "";
		for(var i = 0; i < 20; i++){
			for(var j = 0; j < 20; j++){
				block = this.getBlock(i,10,j);
				if(block === "air")string += "a";
				else if(block==="dirt")string+="d";
				else if(block==="sand")string+="s";
				else if(block==="snow")string+="o";
				else if(block==="water")string+="w";
			}
			console.log(string);
			string="";
		}
	}
}