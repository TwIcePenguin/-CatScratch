using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowDieEffect : MonoSingleton<ShowDieEffect>
{
    public Transform DieEffect;
    public float ShowTimer = -999f;
    public Image MapUI;
    public Vector3 CamPos;

    void Awake()
    {
        CamPos = MapUI.transform.localPosition;
    }
    
    public void CatShowDieEffect(Transform _trans)
    {
        AudioPlayer.Instance.playByName("cat_beCatch");

        DieEffect.position = _trans.position + Vector3.up * 5;
        ShowTimer = Time.time + 5f;

        StartCoroutine(CoMCam());
    }

    void Update()
    {
        if(ShowTimer > Time.time)
        {
            DieEffect.position = Vector3.MoveTowards(DieEffect.position, DieEffect.position + DieEffect.up * 20, Time.deltaTime * 10f);
        }
    }

    IEnumerator CoMCam()
    {
        MapUI.transform.localPosition = Vector3.MoveTowards(CamPos, CamPos - MapUI.transform.up * Random.Range(1, 1000), Time.deltaTime * Random.Range(1, 1000));
        yield return new WaitForSeconds(0.05f);
        MapUI.transform.localPosition = Vector3.MoveTowards(CamPos, CamPos + MapUI.transform.up * Random.Range(8, 1100), Time.deltaTime * Random.Range(1, 1000));
        yield return new WaitForSeconds(0.05f);
        MapUI.transform.localPosition = Vector3.MoveTowards(CamPos, CamPos - MapUI.transform.up * Random.Range(5, 2103), Time.deltaTime * Random.Range(1, 1000));
        yield return new WaitForSeconds(0.05f);
        MapUI.transform.localPosition = Vector3.MoveTowards(CamPos, CamPos + MapUI.transform.up * Random.Range(5, 1107), Time.deltaTime * Random.Range(1, 1000));
        yield return new WaitForSeconds(0.05f);
        MapUI.transform.localPosition = Vector3.MoveTowards(CamPos, CamPos - MapUI.transform.up * Random.Range(5, 2100), Time.deltaTime * Random.Range(1, 1000));
        yield return new WaitForSeconds(0.05f);
        MapUI.transform.localPosition = Vector3.MoveTowards(CamPos, CamPos + MapUI.transform.up * Random.Range(8, 1100), Time.deltaTime * Random.Range(1, 1000));
        yield return new WaitForSeconds(0.05f);
        MapUI.transform.localPosition = CamPos;
    }
}
