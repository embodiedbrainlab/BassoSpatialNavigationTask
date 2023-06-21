using UnityEngine;
using System.Collections;

public class Snippets {
	/* https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle */
	/* This is probably a bit more generic than necessary but it'll be nice to reuse someday */
	/* Provided by user `Matt Howells` at Stack Overflow */
	public static void FisherYates<T>(System.Random rng, T[] arr) {
		Debug.Log("Shuffling an array...");
		int n = arr.Length;
		while (n > 1) {
			int k = rng.Next(n--);
			T temp = arr[n];
			arr[n] = arr[k];
			arr[k] = temp;
		}
	}
}
