using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBlobMovement : BlobMovement
{
    public TrailRenderer blobMeltingEffect;
    public void ActivateTrail()
    {
        blobMeltingEffect.emitting = true;
    }
}
