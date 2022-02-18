using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Cube : SingletonGameStateObserver<Cube>
{
    COLOR m_Color;
    [SerializeField] Material m_CubeMaterial;
    [SerializeField] Color m_Color1;
    [SerializeField] Color m_Color2;
    [SerializeField] float m_ColorLerpCoef;
    [SerializeField] float m_EvolveColorCoefMin;
    [SerializeField] float m_EvolveColorCoefMax;
    [SerializeField] protected Color[] m_Colors;

    int m_GoingUpOrDown;
    Vector3 m_Direction;
    bool m_IsMoving = false;
    Transform m_Transform;
    [SerializeField] float m_EvolveColor;
    Level m_CurrentLevel;

    float m_TimeAcceptInputs;

	[SerializeField] Transform m_CameraTransform;

    public override void SubscribeEvents()
    {
        base.SubscribeEvents();
        EventManager.Instance.AddListener<LevelHasBeenInstantiatedEvent>(LevelHasBeenInstantiated);
        EventManager.Instance.AddListener<InColorTileHasBeenReachedEvent>(InColorTileHasBeenReached);
        EventManager.Instance.AddListener<OutColorTileHasBeenReachedEvent>(OutColorTileHasBeenReached);
        EventManager.Instance.AddListener<GoToNextLevelEvent>(GoToNextLevel);
    }

    public override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
        EventManager.Instance.RemoveListener<LevelHasBeenInstantiatedEvent>(LevelHasBeenInstantiated);
        EventManager.Instance.RemoveListener<InColorTileHasBeenReachedEvent>(InColorTileHasBeenReached);
        EventManager.Instance.RemoveListener<OutColorTileHasBeenReachedEvent>(OutColorTileHasBeenReached);
        EventManager.Instance.RemoveListener<GoToNextLevelEvent>(GoToNextLevel);
    }

    void SetCubeColor(COLOR color,int upOrDown)
    {
        m_GoingUpOrDown = upOrDown;
        m_CubeMaterial.SetInt("_GoingUpOrDown", m_GoingUpOrDown);
        StartCoroutine(ChangeColorCoroutine(m_Color, color, .5f));
    }

    void ResetCubeColor()
    {
        m_GoingUpOrDown = 0;
        SetCubeColor(COLOR.black,0);
    }

    void InColorTileHasBeenReached(InColorTileHasBeenReachedEvent e)
    {
        SetCubeColor(e.eColor,1);
    }
    void OutColorTileHasBeenReached(OutColorTileHasBeenReachedEvent e)
    {
        if (e.eColor == m_Color)
        {
            EventManager.Instance.Raise(new CubeHasCleanedAColorEvent() { eColor = m_Color });
            ResetCubeColor();
        }
    }

    void GoToNextLevel(GoToNextLevelEvent e)
    {
        ResetCubeColor();
    }

    void LevelHasBeenInstantiated(LevelHasBeenInstantiatedEvent e)
    {
        StopAllCoroutines();
        m_CurrentLevel = e.eLevel;
        m_Transform.position = m_CurrentLevel.CubeSpawnPosition;
        m_Transform.rotation = Quaternion.identity;
        m_IsMoving = false;
        ResetCubeColor();
        m_TimeAcceptInputs = Time.time + .5f;
    }

    protected override void Awake()
    {
        base.Awake();
        m_Transform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsPlaying || !m_CurrentLevel || Time.time < m_TimeAcceptInputs) return;

        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");

        if (!m_IsMoving)
        {
            Vector3 dir = Vector3.zero;

            if (Mathf.Abs(hInput)>0)
            {
                // BUGGED CODE

                dir = Vector3.right * Mathf.Sign(hInput);
                dir = Quaternion.Euler(0, m_CameraTransform.eulerAngles.y, 0) * dir;
                dir = GetClosestDirection(dir);

                // END OF BUGGED CODE
            }
            else if(Mathf.Abs(vInput)>0)
            {
                // BUGGED CODE
                dir = Vector3.forward * Mathf.Sign(vInput);
                dir = Quaternion.Euler(0, m_CameraTransform.eulerAngles.y, 0) * dir;
                dir = GetClosestDirection(dir);
                // END OF BUGGED CODE
            }

            if (dir.magnitude != 0)
            {
                dir.Normalize();
                if (m_CurrentLevel.IsPathWalkable(m_Transform.position + dir))
                {
                    RoundRotation90();
                    RoundPosition();
                    EventManager.Instance.Raise(new CubeHasLeftPositionEvent() { ePos = m_Transform.position });
                    StartCoroutine(Rotation90Coroutine(dir, .5f));
                }
                else
                {
                    StartCoroutine(QuickAndShortNoGoRotationCoroutine(dir, .5f, 10f));
                }
            }
        }

        //Cube Material Update
        m_CubeMaterial.SetColor("_Color1", m_Color1);
        m_CubeMaterial.SetColor("_Color2", m_Color2);
        m_CubeMaterial.SetFloat("_ColorLerpCoef", m_ColorLerpCoef);
        m_CubeMaterial.SetVector("_CubeWorldPosition", m_Transform.position);
    }

    Vector3 GetRotAxis(Vector3 direction)
    {
        return Vector3.Cross(Vector3.up, direction);
    }

    Vector3 GetRotCenter(Vector3 direction)
    {
        return m_Transform.position + (direction - Vector3.up) * .5f;
    }

    void RoundRotation90()
    {
        m_Transform.rotation = Quaternion.Euler(Mathf.RoundToInt(m_Transform.rotation.eulerAngles.x / 90f) * 90f,
            Mathf.RoundToInt(m_Transform.rotation.eulerAngles.y / 90f) * 90f,
            Mathf.RoundToInt(transform.rotation.eulerAngles.z / 90f) * 90f);
    }

    void RoundPosition()
    {
        m_Transform.position = new Vector3(Mathf.RoundToInt(m_Transform.position.x), Mathf.RoundToInt(m_Transform.position.y), Mathf.RoundToInt(m_Transform.position.z));
    }

    public IEnumerator QuickAndShortNoGoRotationCoroutine(Vector3 direction, float duration, float angle)
    {
        Debug.Log("QuickAndShortNoGoRotationCoroutine");
        m_IsMoving = true;
        direction.Normalize(); // on ne sait jamais
        Vector3 rotAxis = GetRotAxis(direction);
        Vector3 rotCenter = GetRotCenter(direction);

        yield return StartCoroutine(RotationAroundAxisCoroutine(rotAxis, rotCenter, duration / 2f, angle));
        yield return StartCoroutine(RotationAroundAxisCoroutine(rotAxis, rotCenter, duration / 2f, -angle));
        m_IsMoving = false;
    }

    public IEnumerator RotationAroundAxisCoroutine(Vector3 rotAxis, Vector3 rotCenter, float duration, float angle)
    {

        // BUGGED CODE
        Quaternion wantedRotation = Quaternion.AngleAxis(angle, rotAxis).normalized;
        Quaternion originalRotation = m_Transform.rotation;
        Vector3 originalPosition = m_Transform.position;
        Vector3 wantedPosition = originalPosition + Vector3.ProjectOnPlane(rotCenter - m_Transform.position, Vector3.up).normalized * 2f;
        float _ActualTime = 0f;
        while (_ActualTime < duration)
        {
            _ActualTime += Time.deltaTime / duration;
            m_Transform.position = Vector3.Lerp(originalPosition, wantedPosition, _ActualTime);
            m_Transform.rotation = Quaternion.Lerp(originalRotation, wantedRotation * m_Transform.rotation, _ActualTime);
            yield return null;
        }
        yield return null;
        // END OF BUGGED CODE
    }

    public IEnumerator Rotation90Coroutine(Vector3 direction, float duration)
    {
        m_IsMoving = true;
        direction.Normalize(); // on ne sait jamais
        Vector3 rotAxis = GetRotAxis(direction);
        Vector3 rotCenter = GetRotCenter(direction);
        yield return StartCoroutine(RotationAroundAxisCoroutine(rotAxis, rotCenter, duration, 90f));
        EventManager.Instance.Raise(new CubeHasReachedPositionEvent() { ePos = m_Transform.position });
        m_IsMoving = false;
    }

    public IEnumerator ChangeColorCoroutine(COLOR startColor, COLOR endColor, float duration)
    {
        m_Color = endColor;

        m_Color1 = m_Colors[(int)startColor];
        m_Color2 = m_Colors[(int)endColor];
        m_ColorLerpCoef = 0;
        m_EvolveColor = 0.7f;
        StartCoroutine(ColorEvolve());
        yield return new WaitForEndOfFrame();

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            float k = elapsedTime / duration;
            k = Mathf.Sin(k * Mathf.PI / 2f);
            m_ColorLerpCoef = k;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        m_ColorLerpCoef = 1f;
    }
	public Vector3 GetClosestDirection(Vector3 direction)
    {
        float angleOffSetToRight = Vector3.Angle(direction, Vector3.right);
        float angleOffSetToLeft = Vector3.Angle(direction, -Vector3.right);
        float angleOffSetToForward = Vector3.Angle(direction, Vector3.forward);
        float angleOffSetToBackward = Vector3.Angle(direction, -Vector3.forward);

        Dictionary<Vector3, float> allAngles = new Dictionary<Vector3, float> {
            {Vector3.right, angleOffSetToRight },
            {-Vector3.right, angleOffSetToLeft },
            {Vector3.forward, angleOffSetToForward },
            {-Vector3.forward, angleOffSetToBackward }, 
        };

        return allAngles.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;

    }

    IEnumerator ColorEvolve()
    {
        if (m_GoingUpOrDown == 1)
        {
            m_EvolveColor = -0.5f;
            while (m_EvolveColor <= 0.7)
            {
                m_EvolveColor += Random.Range(m_EvolveColorCoefMin, m_EvolveColorCoefMax);

                //float test = Mathf.Sin(Time.time) * Time.deltaTime + m_EvolveColor;
                m_CubeMaterial.SetFloat("_EvolveColor", m_EvolveColor);

                yield return null;
            }
            if (m_EvolveColor > 0.7)
            {
                m_EvolveColor = 0.7f;
                m_CubeMaterial.SetFloat("_EvolveColor", m_EvolveColor);

            }
        }
        else
        {
            m_EvolveColor = 0.7f;
            while (m_EvolveColor >= -0.5)
            {
                m_EvolveColor -= Random.Range(m_EvolveColorCoefMin, m_EvolveColorCoefMax);

                //float test = Mathf.Sin(Time.time) * Time.deltaTime + m_EvolveColor;
                m_CubeMaterial.SetFloat("_EvolveColor", m_EvolveColor);

                yield return null;
            }
            if (m_EvolveColor < -0.5)
            {
                m_EvolveColor = -0.5f;
                m_CubeMaterial.SetFloat("_EvolveColor", m_EvolveColor);

            }
        }




    }
}
