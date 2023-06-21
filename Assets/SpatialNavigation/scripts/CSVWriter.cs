﻿using UnityEngine;
using System.Collections;
using System.IO;

public class CSVWriter : MonoBehaviour {

	public static void writeCSV(SpatialNavNode[] dataArray, float t_recallStart, Recall[] responseArray, OMT_CS omtComponent)
	{
		int i, dataLength, responseLength;
		dataLength = dataArray.Length;
		responseLength = responseArray.Length;

		using (StreamWriter writer = new StreamWriter ("Results.csv")) {
			writer.WriteLine ("TaskType,SpatialNav");
			writer.WriteLine ("Revisit Phase");
			writer.WriteLine ("Order,Waypoint,Delivery");
			for (i = 0; i < dataLength; ++i) {
				writer.WriteLine (dataArray [i].waypoint.transform.name + "," + dataArray [i].item + "," + dataArray [i].arrivalTime);
			}
			writer.WriteLine ("Recall Phase");
			writer.WriteLine ("Order,LocationResponse,DeliveryResponse");
			for (i = 0; i < dataLength; ++i) {
				writer.WriteLine(responseArray[i].location + "," + responseArray[i].item + "," + responseArray[i].time);
			}
		}
	}
}
