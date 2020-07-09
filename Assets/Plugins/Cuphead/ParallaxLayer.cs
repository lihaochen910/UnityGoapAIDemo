using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public enum Type
    {
        MinMax,
        Comparative,
        Centered
    }

    public Type type;

    [Range(-3f, 3f)]
    public float percentage;

    public Vector2 bottomLeft;

    public Vector2 topRight;

    private Camera _camera;

    private bool _initialized;

    private Vector3 _startPosition;

    private Vector3 _cameraStartPosition;

    private Vector2 _offset
    {
        get
        {
            return this._startPosition - this._cameraStartPosition;
        }
    }

    void Start()
    {
        this._camera = Camera.main;
        this._startPosition = base.transform.position;
        this._cameraStartPosition = this._camera.transform.position;
    }

    void LateUpdate()
    {
        switch (this.type)
        {
            default:
                this.UpdateComparative();
                break;
            case Type.MinMax:
                this.UpdateMinMax();
                break;
            case Type.Centered:
                this.UpdateCentered();
                break;
        }
    }

    private void UpdateComparative()
    {
        Vector3 position = base.transform.position;
        Vector2 offset = this._offset;
        float x = offset.x;
        Vector3 position2 = this._camera.transform.position;
        position.x = x + position2.x * this.percentage;
        Vector2 offset2 = this._offset;
        float y = offset2.y;
        Vector3 position3 = this._camera.transform.position;
        position.y = y + position3.y * this.percentage;
        base.transform.position = position;
    }

    private void UpdateMinMax()
    {
        //Vector3 position = base.transform.position;
        //Vector2 vector = this._camera.transform.position;
        //Vector2 zero = Vector2.zero;
        //float num = vector.x + Mathf.Abs(this._camera.Left);
        //float num2 = this._camera.Right + Mathf.Abs(this._camera.Left);
        //float num3 = vector.y + Mathf.Abs(this._camera.Bottom);
        //float num4 = this._camera.Top + Mathf.Abs(this._camera.Bottom);
        //zero.x = num / num2;
        //zero.y = num3 / num4;
        //if (float.IsNaN(zero.x))
        //{
        //    zero.x = 0.5f;
        //}
        //if (float.IsNaN(zero.y))
        //{
        //    zero.y = 0.5f;
        //}
        //float num5 = Mathf.Lerp(this.bottomLeft.x, this.topRight.x, zero.x);
        //Vector3 position2 = this._camera.transform.position;
        //position.x = num5 + position2.x;
        //float num6 = Mathf.Lerp(this.bottomLeft.y, this.topRight.y, zero.y);
        //Vector3 position3 = this._camera.transform.position;
        //position.y = num6 + position3.y;
        //base.transform.position = position;
    }

    private void UpdateCentered()
    {
        Vector3 position = base.transform.position;
        float x = this._startPosition.x;
        Vector3 position2 = this._camera.transform.position;
        position.x = x + (position2.x - this._startPosition.x) * this.percentage;
        float y = this._startPosition.y;
        Vector3 position3 = this._camera.transform.position;
        position.y = y + (position3.y - this._startPosition.y) * this.percentage;
        base.transform.position = position;
    }
}
