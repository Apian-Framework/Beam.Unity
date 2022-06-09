using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ToastMgr : MonoBehaviour
{
    public const int maxToasts = 4;
    public const float defDisplaySecs = 2.0f;

    public GameObject toastPrefab;

    public List<Toast> toasts;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    protected void FixupPositions()
    {
        float runningDy = 0;
        for (int idx  = toasts.Count - 1; idx >= 0; idx--)
        {
            toasts[idx].SetIndexPos(runningDy);
            runningDy += toasts[idx].height;
        }
    }

    private void _RemoveTaggedToast(string tag)
    {
        if (tag != null)
        {
            Toast dt = toasts.Where(t => t.toastTag == tag).FirstOrDefault();
            if (dt != null)
             RemoveToast(dt);
        }
    }

    public void ShowToast(string msg, Toast.ToastColor color=Toast.ToastColor.kBlue, float displaySecs=defDisplaySecs, string tag = null)
    {
         _RemoveTaggedToast(tag);
        GameObject toastGo = GameObject.Instantiate(toastPrefab, transform);
        toastGo.transform.SetParent(transform.parent);
        Toast toast= (Toast)toastGo.transform.GetComponent<Toast>();
		toast.Setup(this, msg, color, displaySecs, tag);
        if (toasts.Count >= maxToasts)
            RemoveToast(toasts[maxToasts-1]);
        toasts.Add(toast);
        toastGo.SetActive(true);
        FixupPositions();
    }

    public void RemoveToast(Toast theToast)
    {
        toasts.Remove(theToast);
        GameObject.Destroy(theToast.gameObject);
        FixupPositions();
    }

    public void ClearToasts()
    {
        List<Toast> delToasts = toasts.Where(t => true).ToList();
        delToasts.ForEach(t => RemoveToast(t));
    }
}
