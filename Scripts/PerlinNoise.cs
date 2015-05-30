using UnityEngine;
using System.Collections;
using System;

public class PerlinNoise : MonoBehaviour {

	Texture2D perlinTex;
	// Use this for initialization
	void Start () {

		float[,] noise = generateWhiteNoise (25, 25);
		float[,] perlinNoise = generatePerlinNoise (noise, 1);



	}
	
	// Update is called once per frame
	void Update () {
	
	}

	float[,] generateWhiteNoise (int width, int height) {

		UnityEngine.Random.seed = 0;
		float[,] noise = new float[width, height];

		for (int i = 0; i < width; i++ ) {
			for (int j = 0; j < height; j++ ) {
				noise[i,j] = UnityEngine.Random.value;
			}
		}

		return noise;

	}

	float[,] generateSmoothNoise (float[,] baseNoise, int octave) {

		int width = baseNoise.GetLength (0);
		int height = baseNoise.GetLength (1);

		float[,] smoothNoise = new float[width, height];

		int samplePeriod = 1 << octave;
		float sampleFrequency = 1f / samplePeriod;

		for (int i = 0; i < width; i++) {

			int sample_i0 = (i/ samplePeriod) * samplePeriod;
			int sample_i1 = (sample_i0 + samplePeriod) % width;
			float horizontal_blend = (i - sample_i0) * sampleFrequency;

			for (int j = 0; j < height; j++) {
				int sample_j0 = (j/samplePeriod*samplePeriod);
				int sample_j1 = (sample_j0 + samplePeriod) % height;
				float vertical_blend = (j - sample_j0) * sampleFrequency;

				float top = Mathf.Lerp(baseNoise[sample_i0, sample_j0], baseNoise[sample_i1, sample_j0], horizontal_blend); 
				float bottom = Mathf.Lerp(baseNoise[sample_i0, sample_j1], baseNoise[sample_i1, sample_j1], horizontal_blend);

				smoothNoise[i, j] = Mathf.Lerp(top, bottom, vertical_blend);
			}
		}
		return smoothNoise;
	}

	float[,] generatePerlinNoise (float[,] baseNoise, int octaveCount) {

		int width = baseNoise.GetLength (0);
		int height = baseNoise.GetLength (1);

		float[][,] smoothNoise = new float[octaveCount][,];

		float persistance = 0.5f;

		for (int i = 0; i < octaveCount; i++) //havnt initalized each 2d array so might not work
			smoothNoise[i] = generateSmoothNoise(baseNoise, i);

		float[,] perlinNoise = new float[width, height];
		float amplitude = 1f;
		float totalAmplitude = 0f;

		for (int octave = octaveCount - 1; octave >= 0; octave--) {

			amplitude *= persistance;
			totalAmplitude += amplitude;

			for (int i = 0; i < width; i++ ) 
				for (int j = 0; j < height; j++) 
					perlinNoise[i,j] += smoothNoise[octave][i,j] * amplitude;


			for (int i = 0; i < width; i++)
				for (int j = 0; j < height; j++)
					perlinNoise[i,j] /= totalAmplitude;
		}
		return perlinNoise;
	}

	/*
	Texture2D arrayToTexture (float[,] perlinNoise) {
		int width = perlinNoise.GetLength (0);
		int height = perlinNoise.GetLength (1);
		for (int i = 
	}
	*/

}
