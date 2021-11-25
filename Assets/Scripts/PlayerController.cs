using System.Collections;
using System.Collections.Generic;
using Behaviours;
using DG.Tweening;
using UnityEngine;

public class PlayerController : CommonBehaviours
{
    public static PlayerController instance;
    
    private bool var1, var2, var3;
    private float distance,moveDuration;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    protected override void Start()
    {
        base.Start();
            
        StartCoroutine(nameof(Movement));
    }
    
    private IEnumerator Movement()
    {
        while (true)
        {
            if (LevelManager.gameState == GameState.Normal && canMove)
            {
                float verticalInput = Input.GetAxis("Vertical");
                float horizontalInput = Input.GetAxis("Horizontal");

                if (!isOnRamp)
                {
                    var1 = false;
                    var2 = false;
                    var3 = false;
                                        
                    transform.Translate(maxSpeed * Time.deltaTime * verticalInput * Vector3.forward, Space.Self);
                    transform.Rotate(0, horizontalInput * verticalInput * 2f * Time.deltaTime * 125f, 0, Space.Self);
                    yield return null;
                }
                else
                {
                    if (verticalInput > 0)
                    {
                        distance = Vector3.Distance(transform.position, finalPos);
                        moveDuration = Mathf.Clamp(distance / 20f, 1.5f, 3.5f);     
                        if (!var1)
                        {
                            transform.DOMove(finalPos,moveDuration).SetEase(Ease.InOutSine);

                            StartTrails();

                            var1 = true;
                            var2 = false;
                            var3 = false;
                            yield return null;
                        }
                        yield return null;
                    }
                    else if (verticalInput == 0)
                    {
                        distance = Vector3.Distance(transform.position, rampStartPos);
                        moveDuration = Mathf.Clamp(distance / 20f, 1.5f, 3.5f);     
                        if(rampStartPos.y < transform.position.y)
                        {
                            if (!var2)
                            {
                                transform.DOMove(rampStartPos, moveDuration * 4f).SetEase(Ease.InExpo);

                                StartTrails();

                                var1 = false;
                                var2 = true;
                                var3 = false;
                                yield return null;
                            }
                            yield return null;
                        }
                        yield return null;

                    }
                    else
                    {
                        distance = Vector3.Distance(rampStartPos + new Vector3(0,- rampHeight * 0.05f,- rampLength * 0.05f), transform.position);
                        moveDuration = Mathf.Clamp(distance / 20f, 1.5f, 3.5f);     
                        if (!var3)
                        {
                            transform.DOMove(rampStartPos + new Vector3(0,- rampHeight * 0.05f,- rampLength * 0.05f), moveDuration * 1.5f).SetEase(Ease.InOutExpo);

                            StartTrails();

                            var1 = false;
                            var2 = false;
                            var3 = true;
                            yield return null;
                        }
                        yield return null;
                    }

                }

            }
            else
            {
                yield return null;
            }
        }
    }
}
