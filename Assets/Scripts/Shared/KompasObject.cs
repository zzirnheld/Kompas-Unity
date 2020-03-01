using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface KompasObject {
    void OnClick();
    void OnHover();
    void OnDrag(Vector3 mousePos);
    void OnDragEnd(Vector3 mousePos);
}
