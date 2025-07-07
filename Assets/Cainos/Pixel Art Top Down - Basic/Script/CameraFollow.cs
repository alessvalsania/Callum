using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    //let camera follow target
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public float lerpSpeed = 1.0f;

        private Vector3 offset;

        private Vector3 targetPos;

        private void Start()
        {
            if (target == null) return;

            offset = transform.position - target.position;
        }

        private void Update()
        {
            if (target == null) return;

            targetPos = target.position + offset;

            // Suma el offset del shake si existe
            Vector3 shakeOffset = Vector3.zero;
            if (CameraShake.Instance != null)
                shakeOffset = CameraShake.Instance.ShakeOffset;

            transform.position = Vector3.Lerp(transform.position, targetPos + shakeOffset, lerpSpeed * Time.deltaTime);
        }

    }
}
