using System;
using UnityEngine;

public class UniversalList2D<T>{

	private List2D<T> positives;
	private List2D<T> negatives;
	private List2D<T> negativoPositive;
	private List2D<T> positivoNegative;

	public UniversalList2D (){
		positives = new List2D<T>();
		negatives = new List2D<T>();
		negativoPositive = new List2D<T>();
		positivoNegative = new List2D<T>();
	}

	public void Set(T obj, Int32 x, Int32 y){
		if (x >= 0 && y >= 0) {
			positives.AddOrReplace (obj,x,y);
		}
		else if (x < 0 && y >= 0) {
			negativoPositive.AddOrReplace (obj,1-x,y);
		}
		else if (x >= 0 && y < 0) {
			positivoNegative.AddOrReplace (obj,x,1-y);
		}
		else if (x < 0 && y < 0) {
			negatives.AddOrReplace (obj,1-x,1-y);
		}
	}

	public T Get(Int32 x, Int32 y){
		if (x >= 0 && y >= 0 && positives.IsCorrectIndex(new Vector2i (x, y))) {
			return positives.Get (x,y);
		}
		else if (x < 0 && y >= 0 && negativoPositive.IsCorrectIndex(new Vector2i (1-x,y))) {
			return negativoPositive.Get (1-x,y);
		}
		else if (x >= 0 && y < 0 && positivoNegative.IsCorrectIndex(new Vector2i (x, 1-y))) {
			return positivoNegative.Get (x,1-y);
		}
		else if (x < 0 && y < 0 && negatives.IsCorrectIndex(new Vector2i (1-x, 1-y))) {
			return negatives.Get (1-x,1-y);
		}
		return default(T);
	}
}

