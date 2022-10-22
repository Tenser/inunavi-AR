using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.Android;
using System.IO;

public class Test : MonoBehaviour
{
    public ARRaycastManager arRaycaster;
    public GameObject line;
    public GameObject sphere;
    public GameObject arrow;
    public GameObject marker;
    //public Text text;
    public Camera camera;
    private List<(float, float)> pos;
    private List<(float, float)> posMarker;
    private List<String> placeName;
    private (float, float) currentLoc;
    private double direction;
    private double check;
    private List<GameObject> objects;
    private List<GameObject> markerObjects;
    private string[] lls;
    private string[] llsMarker;
    private (float, float) currentLocUnity;
    private float currentDirectionUnity;
    private double timer;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        //this.text.text = camera.transform.rotation.eulerAngles.y.ToString();
        //JSON Data
        //string json = "{\"location\":\"37.373523,126.632177,37.37354870376667\",\"route\":\"37.373502,126.632131,37.37354870376667, 126.63209351762701,37.37365490376666, 126.631924317627,37.3738553037667, 126.63163461762701,37.37382360376667, 126.63136791762695,37.37436290376671, 126.6311533176269,37.37459070376673, 126.63078491762684,37.37470370376671, 126.63087071762692,37.37480180376674, 126.63075271762683,37.37486890376675, 126.63062801762685,37.37505270376674, 126.63044131762682,37.37521670376675, 126.63059081762684,37.375491603766804, 126.63086371762685,37.37564620376679, 126.6308356176269,37.375670703766815, 126.63078871762687,37.37581350376678, 126.63053921762685,37.3759841037668, 126.63025111762677,37.376051203766835, 126.63032361762681,37.376240903766856, 126.63038921762679,37.37662130376685, 126.62976421762673,37.37664613110675,126.62981137633322\",\"angle\":0}";
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        
        // Check if the user has location service enabled.
        if (!Input.location.isEnabledByUser)
        {
            yield break;
        }

        Input.location.Start(1, 1);
        Input.compass.enabled = true;

        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(0.5f);
            maxWait--;
        }
        /*
        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            //yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            //yield break;
        }
        else
        {
            // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }
        */

        updateState();
        //this.text.text = this.currentLoc.ToString() + this.direction.ToString() + " " + Input.location.status + Input.location.isEnabledByUser;

        
        Debug.Log(this.currentLoc);
        Debug.Log(this.direction);

        arrow.SetActive(false);
        sphere.SetActive(false);
        marker.SetActive(false);
        this.check = 0;
        pos = new List<(float, float)>();
        posMarker = new List<(float, float)>();
        objects = new List<GameObject>();
        markerObjects = new List<GameObject>();
        placeName = new List<string>();
        lls = new string[0];
        llsMarker = new string[0];
        timer = 0;
        //this.currentLocUnity = (0, 0);
        //this.currentDirectionUnity = 0;

        //this.dataRecept(json);
        //Debug.Log(gameObject.transform.parent);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        */
        timer += Time.deltaTime;
        //float dx = camera.transform.position.x - currentLocUnity.Item1;
        //float dz = camera.transform.position.z - currentLocUnity.Item2;
        //float da = camera.transform.rotation.eulerAngles.y - currentDirectionUnity;
        if (timer >= 5)
        {
            timer = 0;
            deleteAllObjects();
            updateState();
            //this.text.text = this.currentLoc.ToString() + this.direction.ToString(); 
            float da = (float)(camera.transform.rotation.eulerAngles.y * Math.PI / 180);
            //this.text.text = lls.Length.ToString();
            for (int i = 0; i < lls.Length; i += 2)
            {
                this.pos.Add(translate(this.currentLoc.Item1, this.currentLoc.Item2, float.Parse(lls[i]), float.Parse(lls[i + 1]), camera.transform.position.x, camera.transform.position.z, da));
            }
            
            for (int i = 0; i < llsMarker.Length; i += 3)
            {
                this.placeName.Add(llsMarker[i]);
                this.posMarker.Add(translate(this.currentLoc.Item1, this.currentLoc.Item2, float.Parse(llsMarker[i + 1]), float.Parse(llsMarker[i + 2]), camera.transform.position.x, camera.transform.position.z, da));
            }
            
            //this.text.text += "\n" + dx.ToString() + ", " + dz.ToString() + ", " + camera.transform.rotation.eulerAngles.y.ToString() + "\n";
            if (pos.Count > 1) 
                setObjects();
            if (posMarker.Count > 0)
                setMarkerObjects();
            //this.currentLocUnity = (camera.transform.position.x, camera.transform.position.z);
            //this.currentDirectionUnity = camera.transform.rotation.eulerAngles.y;
        }
        
        
    }

    void updateState()
    {
        LocationInfo location = Input.location.lastData;
        this.currentLoc = (location.latitude, location.longitude);
        //this.direction = Input.compass.trueHeading * Math.PI / 180;
        this.direction = Math.Atan2(-Input.compass.rawVector.x, -Input.compass.rawVector.z);
    }

    void drawLine(float x1, float z1, float x2, float z2)
    {
        Vector3 rotation = new Vector3(0, (float)(Math.Atan2(x2 - x1, z2 - z1) * 180 / Math.PI), 0);
        GameObject line_copy = Instantiate(line, new Vector3(x1, -2, z1), Quaternion.Euler(rotation));
        line_copy.transform.localScale = new Vector3(0, 0, (float)Math.Sqrt(Math.Pow(z2 - z1, 2) + Math.Pow(x2 - x1, 2)));
        //this.check += line_copy.transform.localScale.z;
        GameObject arrow_copy = Instantiate(arrow, new Vector3(x1, 0, z1), Quaternion.Euler(rotation));
        arrow_copy.SetActive(true);
        objects.Add(line_copy);
        objects.Add(arrow_copy);
    }

    (float, float) translate(double lat1, double lon1, double lat2, double lon2, float dx, float dz, float da)
    {
        lat1 = lat1 * Math.PI / 180;
        //lon1 = lon1 * Math.PI / 180;
        lat2 = lat2 * Math.PI / 180;
        //lon2 = lon2 * Math.PI / 180;
        double lonDelta = (lon2 - lon1) * Math.PI / 180;
        double distance = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lonDelta)) * 6371e3;
        double degree = Math.Atan2(Math.Sin(lonDelta) * Math.Cos(lat1) , Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(lonDelta));
        //double degree = Math.Atan(Math.Sin((lon2 - lon1) * Math.PI / 180) / Math.Cos(lat1 * Math.PI / 180) * Math.Tan(lat2 * Math.PI / 180) - Math.Sin(lat1 * Math.PI / 180) * Math.Cos((lon2 - lon1) * Math.PI / 180));
        //degree = -degree;
        degree += da - this.direction;
        float x = (float)(distance * Math.Sin(degree)) + dx;
        float z = (float)(distance * Math.Cos(degree)) + dz;

        //float x2 = (float)(x * Math.Cos(da) + z * Math.Sin(da)) + dx;
        //float z2 = (float)(z * Math.Cos(da) - x * Math.Sin(da)) + dz;
        
        Debug.Log("distance "+distance.ToString());
        Debug.Log("degree "+degree);
        Debug.Log("x " + x);
        Debug.Log("x " + z);
        return (x, z);
    }

    void setObjects()
    {
        Debug.Log(direction);
        for (int i = 0; i < pos.Count - 1; i++)
        {
            GameObject sphere_copy;
            sphere_copy = Instantiate(sphere, new Vector3(pos[i].Item1, -2, pos[i].Item2), Quaternion.identity);
            sphere_copy.SetActive(true);
            objects.Add(sphere_copy);
            drawLine(pos[i].Item1, pos[i].Item2, pos[i + 1].Item1, pos[i + 1].Item2);

        }
        GameObject sphere_last;
        sphere_last = Instantiate(sphere, new Vector3(pos[pos.Count - 1].Item1, -2, pos[pos.Count - 1].Item2), Quaternion.identity);
        sphere_last.SetActive(true);
        objects.Add(sphere_last);
        //Debug.Log(this.check);
        
    }

    void setMarkerObjects()
    {
        for (int i = 0; i < placeName.Count; i++)
        {
            GameObject marker_copy;
            Vector3 rotation = new Vector3(0, 90 - (float)(Math.Atan2(posMarker[i].Item2 - camera.transform.position.z, posMarker[i].Item1 - camera.transform.position.x) * 180 / Math.PI), 0);
            marker_copy = Instantiate(marker, new Vector3(posMarker[i].Item1, 0, posMarker[i].Item2), Quaternion.Euler(rotation));
            marker_copy.SetActive(true);
            marker_copy.transform.Find("PlaceName").GetComponent<TextMesh>().text = addEnter(this.placeName[i], 8);
            markerObjects.Add(marker_copy);
            
        }
    }

    string addEnter(string placeName, int limit)
    {
        string res = "";
        int i = 0;
        int count = 0;
        while (i < placeName.Length)
        {
            if (placeName[i] == ' ' || placeName[i] == '¡¤')
            {
                int j = i + 1;
                int count2 = count + 1;
                while (j < placeName.Length && placeName[j] != ' ' && placeName[j] != '¡¤')
                {
                    j++;
                    count2++;
                }
                Debug.Log(count2);
                if (count2 > limit)
                {
                    res += "\n";
                    count = 0;
                }else
                {
                    res += placeName[i];
                    count++;
                }
            }else if (placeName[i] == '(')
            {
                int j = i + 1;
                int count2 = count + 1;
                while (j < placeName.Length && placeName[j] != ')')
                {
                    j++;
                    count2++;
                }
                if (count2 + 1 > limit)
                {
                    res += "\n" + placeName[i];
                    count = 1;
                }
                else
                {
                    res += "(";
                    count++;
                }
            }
            else
            {
                if (count >= limit)
                {
                    
                    res += "\n" + placeName[i];
                    count = 1;
                }
                else
                {
                    res += placeName[i];
                    count++;
                }
            }
            
            i++;
        }
        return res;
    }

    void deleteAllObjects()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            Destroy(objects[i]);
        }
        for (int i = 0; i < markerObjects.Count; i++)
        {
            Destroy(markerObjects[i]);
        }
        objects.Clear();
        pos.Clear();
        markerObjects.Clear();
        posMarker.Clear();
        placeName.Clear();
    }

    void dataRecept(String json)
    {
        NaviInfo ni = JsonUtility.FromJson<NaviInfo>(json);
        this.lls = ni.route.Split(',');
        
        
        for (int i = 0; i < lls.Length; i += 2)
        {
            this.pos.Add(translate(this.currentLoc.Item1, this.currentLoc.Item2, float.Parse(lls[i]), float.Parse(lls[i + 1]), 0, 0, 0));
        }

        setObjects();
        
    }

    void dataRecept2(String str)
    {
        this.llsMarker = str.Split(',');

        for (int i = 0; i < llsMarker.Length; i += 3)
        {
            this.placeName.Add(llsMarker[i]);
            this.posMarker.Add(translate(this.currentLoc.Item1, this.currentLoc.Item2, float.Parse(llsMarker[i + 1]), float.Parse(llsMarker[i + 2]), 0, 0, 0));
        }
        setMarkerObjects();
        //this.text.text += posMarker[0].ToString();
    }

    public class NaviInfo
    {
        //public String location;
        public String route;
        //public double angle;
    }
}
