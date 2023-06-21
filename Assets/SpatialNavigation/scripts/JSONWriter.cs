using UnityEngine;
using System.Collections;

public class JSONWriter : MonoBehaviour {

    public static JSONObject buildJSON(SpatialNavNode[] dataArray, float t_recallStart, Recall[] responseArray, OMT_CS omtComponent)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        int i, dataLength, responseLength;
        dataLength = dataArray.Length;
        responseLength = responseArray.Length;

        json.AddField("TaskType", "SpatialNavWeb");

        JSONObject locationArr = new JSONObject(JSONObject.Type.ARRAY);
        for(i = 0; i < dataLength; i++)
        {
            JSONObject arr = new JSONObject(JSONObject.Type.ARRAY);
            arr.AddField("Waypoint", dataArray[i].waypoint.transform.name);
            arr.AddField("Delivery", dataArray[i].item);
            arr.AddField("ArrivalTime", dataArray[i].arrivalTime);
            locationArr.AddField(i.ToString(), arr);
        }
        json.AddField("Locations", locationArr);

        JSONObject recallArr = new JSONObject(JSONObject.Type.ARRAY);
        for(i = 0; i < responseLength; i++)
        {
            JSONObject arr = new JSONObject(JSONObject.Type.ARRAY);
            arr.AddField("LocationResponse", responseArray[i].location);
            arr.AddField("DeliveryResponse", responseArray[i].item);
            arr.AddField("TimeResponse", responseArray[i].time);
            recallArr.AddField(i.ToString(), arr);
        }
        json.AddField("Recall", recallArr);

        JSONObject crowArr = new JSONObject(JSONObject.Type.ARRAY); //This could get pretty long if the user is putzing around
        for (i = 0; i < omtComponent.waypointGroups[0].waypointTimeStamp.Count; i++)
        {
            float x = omtComponent.waypointGroups[0].waypointPosition[i].x;
            float z = omtComponent.waypointGroups[0].waypointPosition[i].z;
            float time = omtComponent.waypointGroups[0].waypointTimeStamp[i] - t_recallStart;
            crowArr.AddField(x.ToString() + "," + z.ToString(), time);
        }
        json.AddField("CrowFlies", crowArr);

        return json;
    }
}
