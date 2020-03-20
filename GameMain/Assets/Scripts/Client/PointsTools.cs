using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsTools : MonoSingleton<PointsTools>
{
    public List<Transform> Points;
    public List<BoxCollider> Colliders;

    // Use this for initialization
    void Start()
    {
        if(Points.Count <= 1)
        {
            Debug.LogError("無法計算兩個點 請拉三個點以上");
        }
        else
        {
            Colliders = new List<BoxCollider>();

            Vector3 center = Vector3.zero;
            for(int i = 0; i < Points.Count; i++)
            {
                center = center + Points[i].position;
            }

            center = center / Points.Count;
            new GameObject("Center ").transform.position = center;
            for(int i = 0; i < Points.Count; i++)
            {
                var obj = new GameObject("Collider_" + i);
                var c = obj.AddComponent<BoxCollider>();


                Vector3 thisPos = Points[i].position;
                Vector3 nextPos = (i == Points.Count - 1) ? Points[0].position : Points[i + 1].position;
                float dis = Vector3.Distance(nextPos, thisPos);
                obj.transform.localScale = new Vector3(dis, 5, 1);
                obj.transform.position = (thisPos + nextPos) * 0.5f;
                obj.transform.Rotate(Vector3.up, 90 - GetAngle(new Vector2(thisPos.x, thisPos.z), new Vector2(nextPos.x, nextPos.z)));
                c.size = new Vector3(1, 1, 0.05f);
                Colliders.Add(c);
            }
        }
    }

    private float GetAngle(Vector2 a, Vector2 b)
    {
        if(a.x == b.x && a.y >= b.y)
            return 0;

        b -= a;
        float angle = Mathf.Acos(-b.y / b.magnitude) * (180 / Mathf.PI);

        return (b.x < 0 ? -angle : angle);
    }

    public Vector3 GetPointPos(int _index)
    {
        if(_index > Points.Count || _index < 0)
            return Points[0].position;

        _index = _index == 4 ? 0 : _index;

        return Points[_index].position;
    }
    
    public int GetMoveDir(Transform _trans, int _Nowindex, bool isClosewise)
    {
        int nextIndex = 0;

        if(isClosewise)
            nextIndex = _Nowindex == Points.Count - 1 ? 0 : _Nowindex + 1;
        else
            nextIndex = _Nowindex;

        _Nowindex = Mathf.Clamp(_Nowindex, 0, Points.Count - 1);
        nextIndex = Mathf.Clamp(nextIndex, 0, Points.Count - 1);

        if(Vector3.Distance(_trans.position, Points[nextIndex].position) < 0.1f)
        {
            _trans.eulerAngles = Colliders[nextIndex].transform.eulerAngles;
            _trans.position = Points[nextIndex].transform.position;

            if(isClosewise)
                return nextIndex;
            else if(nextIndex == 0)
                return Points.Count - 1;
            else
                return nextIndex - 1;
        }
        else
        {
            _trans.eulerAngles = Colliders[_Nowindex].transform.eulerAngles;
            return _Nowindex;
        }
    }

    public int FindMoveIndex(Collider _collider)
    {
        for(int i = 0; i < Colliders.Count; i++)
        {
            if(Colliders[i] == _collider)
                return i;
        }

        return 0;
    }

    public Vector3 GetMoveDir(int _index)
    {
        if(_index > Points.Count || _index < 0)
            return Vector3.zero;

        return Colliders[_index].transform.eulerAngles;
    }
}
