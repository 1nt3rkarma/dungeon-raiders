using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoDrawer : MonoBehaviour
{
    public enum CallModes { OnSelected, Always }

    public CallModes callMode;

    public enum Shapes { Cube, Sphere }

    public enum Styles { Solid, Wired }

    public bool drawForward;

    public Color color = Color.blue;

    public Shapes shape;

    public Styles style;

    private void OnDrawGizmos()
    {
        if (callMode == CallModes.Always)
            Draw();
    }

    private void OnDrawGizmosSelected()
    {
        if (callMode == CallModes.OnSelected)
            Draw();
    }

    void Draw()
    {
        Gizmos.color = color;

        if (drawForward)
        {
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }

        switch (shape)
        {
            case Shapes.Cube:
                switch (style)
                {
                    case Styles.Solid:
                        Gizmos.DrawCube(transform.position, transform.localScale);
                        break;
                    case Styles.Wired:
                        Gizmos.DrawWireCube(transform.position, transform.localScale);
                        break;
                }
                break;
            case Shapes.Sphere:
                switch (style)
                {
                    case Styles.Solid:
                        Gizmos.DrawSphere(transform.position, transform.localScale.x);
                        break;
                    case Styles.Wired:
                        Gizmos.DrawWireSphere(transform.position, transform.localScale.x);
                        break;
                }
                break;
        }
    }
}
