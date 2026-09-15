using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Lightweight runtime-rigged actor. Models in Resources/Models can replace these
// procedural prototypes later without changing the combat-facing API.
public sealed class Digimon3DActor : MonoBehaviour
{
    public string UnitId { get; private set; }
    public bool Enemy { get; private set; }

    Transform visualRoot, leftLimb, rightLimb, tail;
    Vector3 destination;
    float phase, hitPulse, attackPulse;
    Renderer[] renderers;
    readonly List<Color> baseColors = new List<Color>();

    public void Initialize(string id, bool enemy)
    {
        UnitId = id; Enemy = enemy; destination = transform.position;
        visualRoot = new GameObject("Visual Rig").transform;
        visualRoot.SetParent(transform, false);
        Digimon3DModelFactory.Build(id, visualRoot, out leftLimb, out rightLimb, out tail);
        renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer item in renderers) baseColors.Add(item.material.color);
        if (enemy) transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    public void MoveTo(Vector3 worldPosition) { destination = worldPosition; }

    public void PlayAttack()
    {
        attackPulse = 1f;
        StopCoroutine(nameof(AttackRoutine));
        StartCoroutine(nameof(AttackRoutine));
    }

    public void PlayHit() { hitPulse = 1f; }

    public void PlayDeath()
    {
        StopAllCoroutines();
        StartCoroutine(DeathRoutine());
    }

    void Update()
    {
        phase += Time.deltaTime;
        Vector3 delta = destination - transform.position;
        bool walking = delta.sqrMagnitude > .0025f;
        if (walking)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * 2.4f);
            Vector3 facing = new Vector3(delta.x, 0, delta.z);
            if (facing.sqrMagnitude > .001f) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(facing), Time.deltaTime * 10f);
        }
        float bob = Mathf.Sin(phase * (walking ? 11f : 3.5f)) * (walking ? .075f : .025f);
        visualRoot.localPosition = new Vector3(0, bob, attackPulse * .12f);
        if (leftLimb) leftLimb.localRotation = Quaternion.Euler(walking ? Mathf.Sin(phase * 11f) * 28f : 0, 0, attackPulse * 38f);
        if (rightLimb) rightLimb.localRotation = Quaternion.Euler(walking ? -Mathf.Sin(phase * 11f) * 28f : 0, 0, -attackPulse * 38f);
        if (tail) tail.localRotation = Quaternion.Euler(0, Mathf.Sin(phase * 5f) * 18f, 0);
        attackPulse = Mathf.MoveTowards(attackPulse, 0, Time.deltaTime * 5f);
        hitPulse = Mathf.MoveTowards(hitPulse, 0, Time.deltaTime * 7f);
        for (int i = 0; i < renderers.Length; i++) renderers[i].material.color = Color.Lerp(baseColors[i], Color.white, hitPulse);
    }

    IEnumerator AttackRoutine()
    {
        Vector3 start = visualRoot.localPosition;
        for (float t = 0; t < 1; t += Time.deltaTime * 9f)
        {
            visualRoot.localPosition = start + Vector3.forward * Mathf.Sin(t * Mathf.PI) * .45f;
            yield return null;
        }
        visualRoot.localPosition = start;
    }

    IEnumerator DeathRoutine()
    {
        Quaternion start = visualRoot.localRotation;
        for (float t = 0; t < 1; t += Time.deltaTime * 2.7f)
        {
            visualRoot.localRotation = Quaternion.Slerp(start, Quaternion.Euler(0, 0, Enemy ? 88 : -88), t);
            visualRoot.localScale = Vector3.one * Mathf.Lerp(1, .7f, t);
            yield return null;
        }
        yield return new WaitForSeconds(.25f);
        gameObject.SetActive(false);
    }
}
