using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KompasObject : MonoBehaviour {

	public virtual void OnClick() { }
    public virtual void OnHover() { }
    public virtual void OnDrag(Vector3 mousePos) { }
    public virtual void OnDragEnd(Vector3 mousePos) { }
}
