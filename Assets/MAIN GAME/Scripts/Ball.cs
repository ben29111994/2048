using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using JellyCube;

public class Ball : MonoBehaviour
{
    public bool isChoose = false;
    public List<Mesh> listMesh = new List<Mesh>();
    public Texture Ktexture;
  
    // Start is called before the first frame update
    public void OnChangeColor()
    {
        transform.DOKill();
        transform.DORotate(new Vector3(0, 180, 0), 0.3f).OnComplete(() =>
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        });
        var number = int.Parse(name);
        if (number == 2)
            GetComponent<MeshFilter>().mesh = listMesh[0];
        if (number == 4)
            GetComponent<MeshFilter>().mesh = listMesh[1];
        if (number == 8)
            GetComponent<MeshFilter>().mesh = listMesh[2];
        if (number == 16)
            GetComponent<MeshFilter>().mesh = listMesh[3];
        if (number == 32)
            GetComponent<MeshFilter>().mesh = listMesh[4];
        if (number == 64)
            GetComponent<MeshFilter>().mesh = listMesh[5];
        if (number == 128)
            GetComponent<MeshFilter>().mesh = listMesh[6];
        if (number == 256)
            GetComponent<MeshFilter>().mesh = listMesh[7];
        if (number == 512)
            GetComponent<MeshFilter>().mesh = listMesh[8];
        if (number == 1024)
            GetComponent<MeshFilter>().mesh = listMesh[9];
        if (number == 2048)
            GetComponent<MeshFilter>().mesh = listMesh[10];
        if (number == 4000)
            GetComponent<MeshFilter>().mesh = listMesh[11];
        if (number == 8000)
            GetComponent<MeshFilter>().mesh = listMesh[12];
        if (number == 16000)
            GetComponent<MeshFilter>().mesh = listMesh[13];
        if (number == 32000)
            GetComponent<MeshFilter>().mesh = listMesh[14];
        if (number == 64000)
            GetComponent<MeshFilter>().mesh = listMesh[15];
        if (number == 128000)
            GetComponent<MeshFilter>().mesh = listMesh[16];
        if (number == 256000)
            GetComponent<MeshFilter>().mesh = listMesh[17];
        if (number == 512000)
            GetComponent<MeshFilter>().mesh = listMesh[18];
        GetComponent<RubberEffect>().OnEnable();
        if (number >= 4000)
        {
            GetComponent<MeshRenderer>().material.mainTexture = Ktexture;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == name && !GameController.isSpawn && (!transform.parent.CompareTag("Pre") || !other.transform.parent.CompareTag("Pre")))
        {
            var total = int.Parse(other.name) + int.Parse(name);
            if(total == 4096)
            {
                total = 4000;
            }
            GameController.instance.SpawnBall(gameObject, total, other.gameObject);
        }
    }
}
